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

    // Guarda qual nave foi escolhida (acessível de qualquer script/cena)
    public static int NaveEscolhida = 0;

    private int indiceAtual = 0;

    void Start()
    {
        indiceAtual = 0;
        AtualizarImagem();
    }

    public void ProximaNave()
    {
        indiceAtual++;
        if (indiceAtual >= naves.Length)
            indiceAtual = 0; // volta pra primeira

        AtualizarImagem();
    }

    public void NaveAnterior()
    {
        indiceAtual--;
        if (indiceAtual < 0)
            indiceAtual = naves.Length - 1; // vai pra última

        AtualizarImagem();
    }

    void AtualizarImagem()
    {
        if (imagemNaveSelecionada != null && naves.Length > 0)
        {
            imagemNaveSelecionada.sprite = naves[indiceAtual];
        }
    }

    // Chame esse método no botão "Confirmar" / "Jogar"
    public void ConfirmarEscolha()
    {
        NaveEscolhida = indiceAtual;
        Debug.Log("Nave escolhida: " + indiceAtual);
        SceneManager.LoadScene(nomeCenaJogo);
    }
}