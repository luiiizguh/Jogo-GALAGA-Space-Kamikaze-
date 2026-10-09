using UnityEngine;

/// Furadeirinha lançada em leque pelo boss. Vai em linha reta para onde está
/// apontada, no estilo minhoca: a velocidade oscila em onda senoidal entre
/// uma mínima (devagar, sem parar) e uma máxima.
/// Anexar no PREFAB da furadeirinha.
public class FuradeirinhaScript : MonoBehaviour
{
    [Header("Velocidade (oscila entre as duas)")]
    [Tooltip("Velocidade no fundo da onda. Maior que 0 = nunca para, só fica mais devagar.")]
    public float velocidadeMinima = 1.5f;

    [Tooltip("Velocidade no pico da onda.")]
    public float velocidadeMaxima = 10f;

    [Tooltip("Quantos ciclos por segundo. 1 = um avanço por segundo, 2 = dois, 0.5 = um a cada 2s.")]
    public float frequencia = 1.5f;

    [Tooltip("Marcado: cada furadeirinha começa num ponto aleatório da onda (não pulsam juntas).")]
    public bool faseAleatoria = false;

    [Tooltip("Direção para onde a ponta aponta, no espaço LOCAL do sprite. (0,-1) = ponta pra BAIXO.")]
    public Vector2 direcaoLocal = new Vector2(0f, -1f);

    [Header("Vida")]
    [Tooltip("Tempo (segundos) até se autodestruir, mesmo se não acertar nada.")]
    public float tempoDeVida = 5f;

    [Header("Dano")]
    public string tagJogador = "Jogador";
    public bool destruirAoAcertar = true;

    private Vector3 direcao;
    private float tempo;

    void Start()
    {
        // direção fixada na hora que nasce, a partir da rotação que o boss deu
        Vector3 d = transform.TransformDirection(direcaoLocal);
        d.z = 0f;
        direcao = d.normalized;

        if (faseAleatoria)
            tempo = Random.value / Mathf.Max(frequencia, 0.01f);

        if (tempoDeVida > 0f)
            Destroy(gameObject, tempoDeVida);
    }

    void Update()
    {
        tempo += Time.deltaTime;

        // onda de 0 a 1: começa no fundo (devagar), vai ao pico e volta
        float onda = (1f - Mathf.Cos(2f * Mathf.PI * frequencia * tempo)) * 0.5f;
        float velocidade = Mathf.Lerp(velocidadeMinima, velocidadeMaxima, onda);

        transform.position += direcao * velocidade * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagJogador))
        {
            Debug.Log("Furadeirinha acertou o jogador!");
            // other.GetComponent<VidaJogador>()?.TomarDano(1);

            if (destruirAoAcertar)
                Destroy(gameObject);
        }
    }
}