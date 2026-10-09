using UnityEngine;

public class VidaInimigos : MonoBehaviour
{
    public float vida = 3;
    public int pontos = 10;
    public GerenciadorFases gerenciador; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        
    }
   
    public void ReceberDano(float dano)
    {
        vida -= dano;
        if (vida <= 0)
        {
            Morrer();
        }
    }
    void Morrer()
    {

        if (gerenciador != null)
        gerenciador.InimigoDerrotado(pontos);

        // Conta a morte e, de vez em quando, solta vida ou habilidade
        if (GerenciadorDrops.Instance != null)
            GerenciadorDrops.Instance.RegistrarMorte(transform.position);

        Destroy(gameObject);
    }
}
