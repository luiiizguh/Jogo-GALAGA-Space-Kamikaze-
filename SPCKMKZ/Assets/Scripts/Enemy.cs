
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Vida")]
    public int vida = 1;

    [Header("Pontuação")]
    public int pontos = 10;

    [Header("Limite da tela")]
    public float limiteY = -6f;

    // Recebe o GerenciadorFases através do Spawner
    [HideInInspector]
    public GerenciadorFases gerenciadorFases;

    // Impede que o mesmo inimigo seja contado duas vezes
    private bool finalizado = false;

    void Update()
    {
        // TESTE:
        // Aperte K para matar o inimigo
        // Remova depois que terminar os testes
        if (Input.GetKeyDown(KeyCode.K))
        {
            Morrer();
        }

        // Se o inimigo passar do limite da tela
        if (transform.position.y < limiteY)
        {
            Escapar();
        }
    }

    // Recebe dano do tiro
    public void ReceberDano(int dano)
    {
        vida -= dano;

        Debug.Log("Inimigo recebeu dano: " + dano);

        if (vida <= 0)
        {
            Morrer();
        }
    }

    // Quando o inimigo é destruído pelo jogador
    void Morrer()
    {
        // Impede contar duas vezes
        if (finalizado)
            return;

        finalizado = true;

        // Verifica se o GerenciadorFases existe
        if (gerenciadorFases != null)
        {
            // Adiciona os pontos
            gerenciadorFases.InimigoDerrotado(pontos);

            Debug.Log("INIMIGO DERROTADO!");
            Debug.Log("PONTOS GANHOS: " + pontos);
        }
        else
        {
            Debug.LogError(
                "ERRO: O inimigo não possui referência para o GerenciadorFases!"
            );
        }

        Destroy(gameObject);
    }

    // Quando o inimigo passa pela parte de baixo da tela
    void Escapar()
    {
        if (finalizado)
            return;

        finalizado = true;

        if (gerenciadorFases != null)
        {
            gerenciadorFases.InimigoEscapou();

            Debug.Log("INIMIGO ESCAPOU!");
        }
        else
        {
            Debug.LogError(
                "ERRO: O inimigo não possui referência para o GerenciadorFases!"
            );
        }

        Destroy(gameObject);
    }

    // Detecta colisão com o jogador
    void OnTriggerEnter2D(Collider2D outro)
    {
        VidaJogador jogador = outro.GetComponentInParent<VidaJogador>();

        if (jogador != null)
        {
            Bater(jogador);
        }
    }

    // Quando o inimigo bate no jogador
    void Bater(VidaJogador jogador)
    {
        if (finalizado)
            return;

        finalizado = true;

        // Tira vida do jogador
        jogador.LevarDano(1);

        // Conta o inimigo como finalizado,
        // mas NÃO dá pontos
        if (gerenciadorFases != null)
        {
            gerenciadorFases.InimigoEscapou();
        }
        else
        {
            Debug.LogError(
                "ERRO: O inimigo não possui referência para o GerenciadorFases!"
            );
        }

        Destroy(gameObject);
    }
}
