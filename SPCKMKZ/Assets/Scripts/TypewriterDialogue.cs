using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TypewriterDialogue : MonoBehaviour
{
    public enum ModoImagem
    {
        TrocaACadaFala,   // alterna toda vez que começa uma nova fala
        TrocaPorTempo     // alterna sozinha a cada X segundos
    }

    [Header("UI")]
    [SerializeField] private TMP_Text textoUI;
    [SerializeField] private Image imagemUI;

    [Header("Diálogo")]
    [Tooltip("Usado só se nenhum diálogo for enviado pelo DialogueLoader (útil pra testar a cena)")]
    [SerializeField] private DialogueData dialogoPadrao;

    [Header("Typewriter")]
    [SerializeField] private float tempoPorLetra = 0.04f;
    [SerializeField] private KeyCode teclaAvancar = KeyCode.X;

    [Header("Imagem")]
    [SerializeField] private ModoImagem modoImagem = ModoImagem.TrocaACadaFala;
    [SerializeField] private float intervaloImagem = 0.5f;

    private DialogueData dados;
    private int indiceFala = -1;
    private bool digitando;
    private bool usandoImagemA;
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

        usandoImagemA = true;
        imagemUI.sprite = dados.imagemA;

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
            SceneManager.LoadScene(cena);
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
            yield return new WaitForSeconds(tempoPorLetra);
        }

        digitando = false;
        rotinaDigitar = null;
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
        usandoImagemA = !usandoImagemA;
        imagemUI.sprite = usandoImagemA ? dados.imagemA : dados.imagemB;
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
