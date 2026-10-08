using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShipSelector : MonoBehaviour
{
    [Header("Sprites das naves, na ordem que devem aparecer")]
    public Sprite[] naves;

    [Header("Onde a imagem da nave selecionada é exibida")]
    public Image imagemNaveSelecionada; // Arraste aqui o componente Image (UI) que mostra a nave

    [Header("Cena do jogo")]
    public string nomeCenaJogo = "SampleScene";

    [Header("Sons")]
    public AudioSource audioSource;       // AudioSource só para efeitos (NÃO o da música)
    public AudioClip somTrocar;           // som ao passar de uma nave para outra
    public AudioClip somConfirmar;        // som ao confirmar a escolha
    public float tempoAntesDeTrocarCena = 0.4f; // tempo para o som de confirmar tocar

    // Guarda qual nave foi escolhida (acessível de qualquer script/cena)
    public static int NaveEscolhida = 0;

    private int indiceAtual = 0;
    private bool confirmado = false;

    void Start()
    {
        indiceAtual = 0;
        confirmado = false;
        AtualizarImagem();
    }

    void Update()
    {
        if (confirmado) return; // evita apertar várias vezes durante a troca de cena

        // Setas ou A/D para trocar de nave
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            ProximaNave();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            NaveAnterior();
        }

        // Enter ou Espaço para confirmar
        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmarEscolha();
        }
    }

    public void ProximaNave()
    {
        indiceAtual++;
        if (indiceAtual >= naves.Length)
            indiceAtual = 0; // volta pra primeira

        TocarSom(somTrocar);
        AtualizarImagem();
    }

    public void NaveAnterior()
    {
        indiceAtual--;
        if (indiceAtual < 0)
            indiceAtual = naves.Length - 1; // vai pra última

        TocarSom(somTrocar);
        AtualizarImagem();
    }

    void AtualizarImagem()
    {
        if (imagemNaveSelecionada != null && naves.Length > 0)
        {
            imagemNaveSelecionada.sprite = naves[indiceAtual];
        }
    }

    void TocarSom(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Chamado ao apertar Enter/Espaço (ou por botão, se quiser)
    public void ConfirmarEscolha()
    {
        if (confirmado) return;
        confirmado = true;

        NaveEscolhida = indiceAtual;
        SelecaoNave.Escolher(indiceAtual); // <-- NOVA LINHA: grava a escolha para o GerenciadorFases
        Debug.Log("Nave escolhida: " + indiceAtual);

        TocarSom(somConfirmar);
        Invoke(nameof(CarregarCena), tempoAntesDeTrocarCena);
    }

    void CarregarCena()
    {
        SceneManager.LoadScene(nomeCenaJogo);
    }
}