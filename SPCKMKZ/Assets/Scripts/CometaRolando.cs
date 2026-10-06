using UnityEngine;

public class CometaRolando : MonoBehaviour
{
    public float velocidade = 3f;

    // Inclinação opcional (valor positivo = desce um pouco enquanto vai para a esquerda)
    public float velocidadeVertical = 0f;

    // Posição X (à esquerda) onde o cometa já saiu da tela e reinicia
    public float limiteEsquerda = -12f;

    // Se desmarcar, o cometa passa uma vez só e não volta
    public bool repetir = true;

    private Vector3 posicaoInicial;

    void Awake()
    {
        // Posição inicial = onde você deixou o cometa no editor (fora da tela, à direita)
        posicaoInicial = transform.position;
    }

    void OnEnable()
    {
        // Toda vez que for ativado (nova fase), começa de novo pela direita
        transform.position = posicaoInicial;
    }

    void Update()
    {
        Vector3 direcao = new Vector3(-velocidade, -velocidadeVertical, 0f);
        transform.position += direcao * Time.deltaTime;

        // Quando sair bastante da tela pela esquerda
        if (transform.position.x < limiteEsquerda)
        {
            if (repetir)
            {
                // Volta para a direita
                transform.position = posicaoInicial;
            }
            else
            {
                enabled = false;
            }
        }
    }
}