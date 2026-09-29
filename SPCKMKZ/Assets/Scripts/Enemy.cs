using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int vida = 1;

    // Pontos que o jogador ganha ao derrotar este inimigo
    public int pontos = 10;

    // Y abaixo do qual o inimigo é destruído (ajuste no prefab)
    public float limiteY = -6f;

    // Preenchido pelo spawner
    [HideInInspector] public GerenciadorFases gerenciadorFases;

    // Evita contar o mesmo inimigo duas vezes
    private bool finalizado = false;

    void Update()
    {
        // Teste: tecla K mata o inimigo (remova depois)
        if (Input.GetKeyDown(KeyCode.K))
        {
            Morrer();
        }

        if (transform.position.y < limiteY)
        {
            Escapar();
        }
    }

    public void ReceberDano(int dano)
    {
        vida -= dano;

        if (vida <= 0)
        {
            Morrer();
        }
    }

    void Morrer()
    {
        if (finalizado) return;
        finalizado = true;

        gerenciadorFases.InimigoDerrotado(pontos);
        Destroy(gameObject);
    }

    void Escapar()
    {
        if (finalizado) return;
        finalizado = true;

        gerenciadorFases.InimigoEscapou();
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        VidaJogador jogador = outro.GetComponentInParent<VidaJogador>();

        if (jogador != null)
        {
            Bater(jogador);
        }
    }

    void Bater(VidaJogador jogador)
    {
        if (finalizado) return;
        finalizado = true;

        jogador.LevarDano(1);

        // Conta como inimigo resolvido (sem dar pontos), para a fase poder terminar
        gerenciadorFases.InimigoEscapou();
        Destroy(gameObject);
    }
}