using UnityEngine;

/// A cada "intervalo" segundos, SE estiver com a broca, cria a broca (Instantiate),
/// solta uma rajada em leque do prefab escolhido no Spawn e troca pro sprite SEM broca.
/// O sprite COM broca só volta quando a broca colide com o boss (Recuperar).
public class BossLancaBroca : MonoBehaviour
{
    [Header("Broca")]
    [Tooltip("Prefab da broca (arraste da aba Project, não da Hierarchy).")]
    public GameObject prefabBroca;

    [Tooltip("Empty filho do boss que marca de onde a broca sai e pra onde volta.")]
    public Transform spawn;

    [Tooltip("A cada quantos segundos ele lança (contando só enquanto está com a broca).")]
    public float intervalo = 5f;

    [Tooltip("Posição Z em que a broca e os projéteis são criados.")]
    public float zBroca = -1f;

    [Header("Rajada em leque (soltada junto com a broca)")]
    [Tooltip("Prefab que o boss solta (arraste da aba Project). Deixe vazio pra não soltar nada.")]
    public GameObject prefabRajada;

    [Tooltip("Quantas cópias do prefab saem por lançamento.")]
    public int quantidade = 5;

    [Tooltip("Abertura total do leque, em graus. As cópias são distribuídas igualmente dentro dela.")]
    public float arco = 120f;

    [Tooltip("Direção central do leque, em graus. -90 = pra baixo, 0 = direita, 90 = pra cima, 180 = esquerda.")]
    public float anguloCentral = -90f;

    [Tooltip("Velocidade das cópias (usada se o prefab não tiver o script ProjetilBoss).")]
    public float velocidadeRajada = 6f;

    [Tooltip("Tempo de vida das cópias em segundos.")]
    public float tempoVidaRajada = 5f;

    [Tooltip("Gira cada cópia para apontar na direção em que ela voa (sprite original apontando pra BAIXO).")]
    public bool rotacionarRajada = true;

    [Tooltip("Ajuste fino da rotação do sprite. Teste 0, 90, 180 e -90.")]
    public float offsetAnguloRajada = 0f;

    [Header("Sprites (objetos filhos do boss)")]
    public GameObject spriteComBroca;
    public GameObject spriteSemBroca;

    private bool temBroca = true;
    private float cooldown;

    void Start()
    {
        cooldown = intervalo;

        if (prefabBroca == null)
        {
            Debug.LogWarning("BossLancaBroca: 'Prefab Broca' está vazio no Inspector.");
            enabled = false;
            return;
        }

        MostrarComBroca(true);
    }

    void Update()
    {
        if (!temBroca) return;

        cooldown -= Time.deltaTime;
        if (cooldown <= 0f)
            LancarBroca();
    }

    public void LancarBroca()
    {
        if (!temBroca || prefabBroca == null) return;

        Transform pontoSpawn = spawn != null ? spawn : transform;

        Vector3 posicao = pontoSpawn.position;
        posicao.z = zBroca;

        GameObject obj = Instantiate(prefabBroca, posicao, Quaternion.identity);
        BossBrocaScript broca = obj.GetComponent<BossBrocaScript>();

        if (broca == null)
        {
            Debug.LogWarning("BossLancaBroca: o prefab precisa ter o script BossBrocaScript.");
            Destroy(obj);
            return;
        }

        broca.Iniciar(pontoSpawn, this);

        SoltarRajada(posicao);

        temBroca = false;
        MostrarComBroca(false);                       // boss perde a broca
    }

    // Cria as cópias do prefab em leque, com os ângulos distribuídos igualmente.
    // Cria as furadeirinhas em leque, com os ângulos distribuídos igualmente.
    // Cada cópia nasce já girada na direção dela; o FuradeirinhaScript cuida do movimento.
    void SoltarRajada(Vector3 posicao)
    {
        if (prefabRajada == null || quantidade <= 0) return;

        for (int i = 0; i < quantidade; i++)
        {
            // 0 = borda esquerda do leque, 1 = borda direita (com 1 cópia, vai no centro)
            float t = quantidade == 1 ? 0.5f : (float)i / (quantidade - 1);
            float angulo = anguloCentral - arco * 0.5f + arco * t;

            Quaternion rotacao = Quaternion.identity;
            if (rotacionarRajada)
                rotacao = Quaternion.Euler(0f, 0f, angulo + 90f + offsetAnguloRajada);

            GameObject copia = Instantiate(prefabRajada, posicao, rotacao);

            if (tempoVidaRajada > 0f)
                Destroy(copia, tempoVidaRajada);   // segurança: some mesmo se o script dela não se destruir
        }
    }

    // Chamado pela broca quando ela colide com o boss.
    public void Recuperar()
    {
        temBroca = true;
        cooldown = intervalo;
        MostrarComBroca(true);                        // boss recupera a broca
    }

    void MostrarComBroca(bool comBroca)
    {
        if (spriteComBroca != null) spriteComBroca.SetActive(comBroca);
        if (spriteSemBroca != null) spriteSemBroca.SetActive(!comBroca);
    }
}