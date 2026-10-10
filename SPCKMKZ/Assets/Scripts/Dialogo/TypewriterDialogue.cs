using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TypewriterDialogue : MonoBehaviour
{
    public enum ModoImagem
    {
        TrocaACadaFala,   // alterna toda vez que começa uma nova fala
        TrocaPorTempo     // alterna sozinha a cada X segundos
    }

    [Header("UI")]
    [SerializeField] private TMP_Text textoUI;

    [Tooltip("O objeto que fica POR CIMA. Quando ele é desligado, aparece o de baixo.")]
    [SerializeField] private GameObject imagemDeCima;

    [Header("Diálogo")]
    [Tooltip("Usado só se nenhum diálogo for enviado pelo DialogueLoader (útil pra testar a cena)")]
    [SerializeField] private DialogueData dialogoPadrao;

    [Header("Typewriter")]
    [SerializeField] private float tempoPorLetra = 0.04f;
    [SerializeField] private KeyCode teclaAvancar = KeyCode.X;

    [Header("Imagem")]
    [SerializeField] private ModoImagem modoImagem = ModoImagem.TrocaACadaFala;
    [SerializeField] private float intervaloImagem = 0.5f;

    [Header("Áudio")]
    [SerializeField] private AudioSource fonteAudio;
    [Tooltip("Os 3 sons. A cada letra é sorteado um deles.")]
    [SerializeField] private AudioClip[] sons = new AudioClip[3];
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [Tooltip("Variação aleatória de pitch pra ficar menos repetitivo (0 = sem variação)")]
    [SerializeField, Range(0f, 0.5f)] private float variacaoPitch = 0f;
    [Tooltip("Não toca som em espaços e quebras de linha")]
    [SerializeField] private bool ignorarEspacos = true;

    private DialogueData dados;
    private int indiceFala = -1;
    private bool digitando;
    private int ultimoSom = -1;
    private Coroutine rotinaDigitar;

    private void Start()
    {
        dados = DialogueLoader.Atual != null ? DialogueLoader.Atual : dialogoPadrao;

        if (dados == null)
        {
            Debug.LogError("Nenhum DialogueData definido!");
            enabled = false;
            return;
        }

        imagemDeCima.SetActive(true);

        if (modoImagem == ModoImagem.TrocaPorTempo)
            StartCoroutine(AlternarImagemPorTempo());

        ProximaFala();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(teclaAvancar)) return;

        if (digitando)
            CompletarFala();
        else
            ProximaFala();
    }

    private void ProximaFala()
    {
        indiceFala++;

        if (indiceFala >= dados.falas.Length)
        {
            string cena = dados.proximaCena;
            DialogueLoader.Limpar();
            SceneManager.LoadScene("Fase_1");
            return;
        }

        if (modoImagem == ModoImagem.TrocaACadaFala && indiceFala > 0)
            AlternarImagem();

        rotinaDigitar = StartCoroutine(DigitarFala(dados.falas[indiceFala]));
    }

    private IEnumerator DigitarFala(string fala)
    {
        digitando = true;
        textoUI.text = fala;
        textoUI.maxVisibleCharacters = 0;
        textoUI.ForceMeshUpdate();

        int total = textoUI.textInfo.characterCount;

        for (int i = 1; i <= total; i++)
        {
            textoUI.maxVisibleCharacters = i;

            char letra = textoUI.textInfo.characterInfo[i - 1].character;
            if (!(ignorarEspacos && char.IsWhiteSpace(letra)))
                TocarSom();

            yield return new WaitForSeconds(tempoPorLetra);
        }

        digitando = false;
        rotinaDigitar = null;
    }

    private void TocarSom()
    {
        if (fonteAudio == null || sons == null || sons.Length == 0) return;

        // sorteia um dos sons, evitando repetir o mesmo duas vezes seguidas
        int indice = Random.Range(0, sons.Length);
        if (sons.Length > 1 && indice == ultimoSom)
            indice = (indice + 1 + Random.Range(0, sons.Length - 1)) % sons.Length;
        ultimoSom = indice;

        if (sons[indice] == null) return;

        fonteAudio.pitch = 1f + Random.Range(-variacaoPitch, variacaoPitch);
        fonteAudio.PlayOneShot(sons[indice], volume);
    }

    private void CompletarFala()
    {
        if (rotinaDigitar != null)
            StopCoroutine(rotinaDigitar);

        textoUI.maxVisibleCharacters = int.MaxValue;
        digitando = false;
        rotinaDigitar = null;
    }

    private void AlternarImagem()
    {
        // liga/desliga o de cima: desligado mostra o de baixo, ligado esconde o de baixo
        imagemDeCima.SetActive(!imagemDeCima.activeSelf);
    }

    private IEnumerator AlternarImagemPorTempo()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervaloImagem);
            AlternarImagem();
        }
    }
}