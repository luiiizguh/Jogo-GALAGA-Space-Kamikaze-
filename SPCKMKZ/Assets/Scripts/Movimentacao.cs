using UnityEngine;

/// <summary>
/// Move o objeto em linha reta de cima para baixo, em velocidade constante.
/// Anexar no GameObject pai "InimigoEscudo".
/// </summary>
public class MovimentoParaBaixo : MonoBehaviour
{
    [Tooltip("Velocidade de descida (unidades por segundo).")]
    public float velocidade = 2f;

    [Header("Fora da tela")]
    [Tooltip("Se marcado, destrói o inimigo automaticamente quando ele sai da tela por baixo.")]
    public bool destruirForaDaTela = true;

    [Tooltip("Posição Y considerada 'fora da tela' por baixo (ajuste conforme sua câmera).")]
    public float limiteInferior = -6f;

    void Update()
    {
        transform.position += Vector3.down * velocidade * Time.deltaTime;

        if (destruirForaDaTela && transform.position.y < limiteInferior)
        {
            Destroy(gameObject);
        }
    }
}