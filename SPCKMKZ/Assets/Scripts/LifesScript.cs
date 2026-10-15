using UnityEngine;
using UnityEngine.UI;

public class BarraVidaBoss : MonoBehaviour
{
    [SerializeField] private Image liquido;          // imagem Filled
    [SerializeField] private float velocidade = 2f;  // velocidade da descida
    [SerializeField] private int larguraEmPixels = 196; // largura real do liquido.png

    private float alvo = 1f;

    // chame isso toda vez que a vida do boss mudar
    public void AtualizarBarra(float vidaAtual, float vidaMax)
    {
        alvo = Mathf.Clamp01(vidaAtual / vidaMax);
    }

    private void Update()
    {
        float f = Mathf.MoveTowards(liquido.fillAmount, alvo, velocidade * Time.deltaTime);

        // trava em pixels inteiros: nunca corta "meio pixel"
        liquido.fillAmount = Mathf.Round(f * larguraEmPixels) / larguraEmPixels;
    }
}