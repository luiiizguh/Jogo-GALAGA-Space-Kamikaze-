using UnityEngine;

/// <summary>
/// Inimigo com cooldown de tiro (decresce a cada frame) e movimento lateral
/// lento: anda pro lado onde o jogador está, espelhando o sprite conforme
/// a direção. Anexar diretamente no GameObject do inimigo.
/// </summary>
public class InimigoLateral : MonoBehaviour
{
    [Header("Tiro")]
    [Tooltip("Tempo (segundos) entre um tiro e outro. Também é o valor pro qual o cooldown reseta.")]
    public float cooldownMaximo = 1.5f;
    private float cooldown;

    [Tooltip("Prefab do tiro deste inimigo.")]
    public GameObject prefabTiro;

    [Tooltip("Objeto que marca de onde o tiro nasce (\"spawn\"). Se vazio, usa a própria posição do inimigo.")]
    public Transform spawn;

    [Tooltip("Velocidade com que o tiro desce a tela.")]
    public float velocidadeTiro = 5f;

    [Header("Movimento lateral")]
    [Tooltip("Se vazio, procura automaticamente um objeto com a tag abaixo.")]
    public Transform jogador;
    public string tagJogador = "Jogador";

    [Tooltip("Velocidade do movimento lateral (é devagar por padrão).")]
    public float velocidadeMovimento = 1f;

    [Tooltip("Tempo (segundos) entre uma checagem da posição da nave e outra. Maior = reage mais devagar, menos 'roubado'.")]
    public float tempoDeReacao = 0.6f;

    private SpriteRenderer spriteRenderer;
    private bool jogadorEstaEsquerda;
    private float proximaReacao;

    void Start()
    {
        cooldown = cooldownMaximo;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (jogador == null)
        {
            GameObject alvo = GameObject.FindGameObjectWithTag(tagJogador);
            if (alvo != null) jogador = alvo.transform;
        }

        if (jogador != null)
            jogadorEstaEsquerda = jogador.position.x < transform.position.x;

        proximaReacao = Time.time + tempoDeReacao;
    }

    void Update()
    {
        AtualizarCooldown();
        MoverLateralmente();
    }

    private void AtualizarCooldown()
    {
        cooldown -= Time.deltaTime;

        if (cooldown <= 0f)
        {
            Atirar();
            cooldown = cooldownMaximo;
        }
    }

    private void Atirar()
    {
        if (prefabTiro == null) return;

        Vector3 posicaoDisparo = spawn != null ? spawn.position : transform.position;
        GameObject tiro = Instantiate(prefabTiro, posicaoDisparo, Quaternion.identity);

        ProjetilInimigo scriptTiro = tiro.GetComponent<ProjetilInimigo>();
        if (scriptTiro != null)
        {
            scriptTiro.velocidade = velocidadeTiro;
        }
        else
        {
            Debug.LogWarning("InimigoLateral: o prefab do tiro não tem o script ProjetilInimigo — ele vai nascer mas não vai se mover sozinho.");
        }
    }

    private void MoverLateralmente()
    {
        if (jogador == null) return;

        if (Time.time >= proximaReacao)
        {
            jogadorEstaEsquerda = jogador.position.x < transform.position.x;
            proximaReacao = Time.time + tempoDeReacao;
        }

        if (jogadorEstaEsquerda)
        {
            transform.position += Vector3.left * velocidadeMovimento * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.right * velocidadeMovimento * Time.deltaTime;
        }

        // Espelha o sprite (flip horizontal) conforme a direção do movimento.
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !jogadorEstaEsquerda;
        }
    }
}