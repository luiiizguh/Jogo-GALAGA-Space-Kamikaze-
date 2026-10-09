using UnityEngine;

/// Broca do boss. Fica sempre no mesmo X do boss e só se move no eixo Y:
/// desce acelerando até a altura onde o jogador estava e sobe acelerando
/// de volta. É destruída SOMENTE ao colidir com o objeto de tag "Boss".
/// Anexar no PREFAB da broca.
public class BossBrocaScript : MonoBehaviour
{
    [Header("Ida (descida)")]
    public float velocidadeInicialIda = 2f;
    public float velocidadeMaximaIda = 14f;
    public float aceleracaoIda = 30f;

    [Header("Volta (subida)")]
    public float velocidadeInicialVolta = 2f;
    public float velocidadeMaximaVolta = 14f;
    public float aceleracaoVolta = 30f;

    [Header("Tags")]
    public string tagJogador = "Jogador";
    public string tagBoss = "Boss";

    [Header("Alvo")]
    [Tooltip("Distância extra abaixo da altura do jogador antes de voltar (0 = volta na altura dele).")]
    public float distanciaExtra = 0f;

    private Transform origem;
    private BossLancaBroca boss;
    private float alvoY;
    private float velocidadeAtual;
    private bool voltando;

    // Chamado pelo boss logo depois do Instantiate.
    public void Iniciar(Transform pontoOrigem, BossLancaBroca donoBoss)
    {
        origem = pontoOrigem;
        boss = donoBoss;
        voltando = false;
        velocidadeAtual = velocidadeInicialIda;

        alvoY = transform.position.y - 5f;   // se não achar o jogador, desce 5 unidades

        GameObject alvo = GameObject.FindGameObjectWithTag(tagJogador);
        if (alvo != null)
            alvoY = alvo.transform.position.y - distanciaExtra;
    }

    void Update()
    {
        if (origem == null) return;

        Vector3 pos = transform.position;
        pos.x = origem.position.x;   // sempre acompanha o boss no eixo X

        if (!voltando)
        {
            velocidadeAtual = Mathf.Min(velocidadeAtual + aceleracaoIda * Time.deltaTime, velocidadeMaximaIda);
            pos.y = Mathf.MoveTowards(pos.y, alvoY, velocidadeAtual * Time.deltaTime);

            if (Mathf.Abs(pos.y - alvoY) <= 0.01f)
            {
                voltando = true;
                velocidadeAtual = velocidadeInicialVolta;   // recomeça devagar e acelera na subida
            }
        }
        else
        {
            velocidadeAtual = Mathf.Min(velocidadeAtual + aceleracaoVolta * Time.deltaTime, velocidadeMaximaVolta);
            pos.y = Mathf.MoveTowards(pos.y, origem.position.y, velocidadeAtual * Time.deltaTime);
        }

        transform.position = pos;
    }

    void OnTriggerEnter2D(Collider2D other) { Tratar(other); }
    void OnTriggerStay2D(Collider2D other)  { Tratar(other); }

    void Tratar(Collider2D other)
    {
        // só colide com o boss (pela tag) depois de ter chegado ao alvo
        if (voltando && other.CompareTag(tagBoss))
        {
            if (boss != null) boss.Recuperar();
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag(tagJogador))
        {
            Debug.Log("A broca acertou o jogador!");
            // other.GetComponent<VidaJogador>()?.TomarDano(1);
        }
    }
}