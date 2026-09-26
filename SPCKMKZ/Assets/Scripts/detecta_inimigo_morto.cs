using UnityEngine;

// Script que controla cada inimigo
public class Enemy : MonoBehaviour
{
    // Vida do inimigo
    public int vida = 1;

    // Referência para o Gerenciador de Fases
    public GerenciadorFases gerenciadorFases;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Morrer();
        }
    }

    // Função para receber dano
    public void ReceberDano(int dano)
    {
        // Diminui a vida
        vida -= dano;

        // Verifica se morreu
        if (vida <= 0)
        {
            Morrer();
        }
    }

    // Executada quando o inimigo morre
    void Morrer()
    {
        // Informa ao Gerenciador que um inimigo foi derrotado
        gerenciadorFases.InimigoDerrotado();

        // Remove o inimigo da cena
        Destroy(gameObject);
    }
}