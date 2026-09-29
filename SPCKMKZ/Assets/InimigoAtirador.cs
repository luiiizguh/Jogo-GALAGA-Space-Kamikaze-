using UnityEngine;

/// <summary>
/// Inimigo fixo na parte de cima da tela: não desce, só se move de um lado
/// pro outro (movimento senoidal, bem suave) e atira periodicamente.
/// Anexar diretamente no GameObject do inimigo.
/// </summary>
public class InimigoAtirador : MonoBehaviour
{
    [Header("Vida")]
    public int vida = 5;

    [Header("Movimento lateral")]
    [Tooltip("Até onde ele se afasta do ponto inicial pra cada lado (em unidades).")]
    public float amplitudeMovimento = 3f;
    [Tooltip("Velocidade do vai-e-vem. Quanto maior, mais rápido ele oscila.")]
    public float velocidadeMovimento = 2f;

    [Header("Tiro")]
    [Tooltip("Prefab do projétil que este inimigo atira.")]
    public GameObject prefabProjetil;
    [Tooltip("De onde o projétil nasce. Se vazio, usa a própria posição do inimigo.")]
    public Transform pontoDeTiro;
    [Tooltip("Tempo (segundos) entre um tiro e outro.")]
    public float intervaloTiro = 1.5f;
    [Tooltip("Velocidade com que o projétil desce a tela.")]
    public float velocidadeProjetil = 5f;

    [Header("Colisão / Dano recebido")]
    [Tooltip("Tag usada pelos projéteis do jogador.")]
    public string tagProjetilJogador = "Projetil";
    [Tooltip("Dano recebido por acerto de projétil.")]
    public int danoPorAcerto = 1;

    private Vector3 posicaoInicial;

    void Start()
    {
        posicaoInicial = transform.position;
        InvokeRepeating(nameof(Atirar), intervaloTiro, intervaloTiro);
    }

    void Update()
    {
        MoverLateralmente();
    }

    private void MoverLateralmente()
    {
        // Mathf.Sin gera uma oscilação contínua e suave, sem "trocar de
        // direção" bruscamente como um vai-e-volta linear faria.
        float deslocamentoX = Mathf.Sin(Time.time * velocidadeMovimento) * amplitudeMovimento;
        transform.position = new Vector3(posicaoInicial.x + deslocamentoX, posicaoInicial.y, posicaoInicial.z);
    }

    private void Atirar()
    {
        if (prefabProjetil == null) return;

        Vector3 posicaoDisparo = pontoDeTiro != null ? pontoDeTiro.position : transform.position;
        GameObject projetil = Instantiate(prefabProjetil, posicaoDisparo, Quaternion.identity);

        ProjetilInimigo scriptProjetil = projetil.GetComponent<ProjetilInimigo>();
        if (scriptProjetil != null)
            scriptProjetil.velocidade = velocidadeProjetil;
    }

    public void ReceberDano(int dano)
    {
        vida -= dano;
        if (vida <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagProjetilJogador))
        {
            ReceberDano(danoPorAcerto);
            Destroy(other.gameObject);
        }
    }
}