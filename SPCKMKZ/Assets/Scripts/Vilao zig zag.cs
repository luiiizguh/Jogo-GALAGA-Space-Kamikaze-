using System.Collections;
using UnityEngine;

/// <summary>
/// Vilão que desce a tela em zig-zag (oscilação lateral suave), mantendo o
/// sprite sempre "reto" — nunca giramos o transform, só a posição muda.
/// Atira teias periodicamente e se destrói ao sair da tela por baixo.
/// Anexar diretamente no GameObject do vilão.
/// </summary>
public class VilaoZigZag : MonoBehaviour
{
    [Header("Vida")]
    public int vida = 8;

    [Header("Movimento em zig-zag")]
    [Tooltip("Velocidade de descida constante (eixo Y).")]
    public float velocidadeDescida = 1.5f;
    [Tooltip("Até onde ele se afasta do centro do zig-zag, pra cada lado.")]
    public float amplitudeZigZag = 2f;
    [Tooltip("Velocidade da oscilação lateral. Quanto maior, mais rápido o zig-zag.")]
    public float velocidadeZigZag = 2f;

    [Header("Limites da tela")]
    [Tooltip("Margem (em unidades) entre o vilão e a borda lateral. Use a metade da largura do sprite.")]
    public float margemLateral = 0.5f;
    [Tooltip("Quanto ele precisa passar da borda de baixo para ser destruído (use a altura do sprite).")]
    public float margemDestruir = 1f;

    [Header("Variação (evita todos virem iguais)")]
    [Tooltip("Se marcado, cada vilão escolhe uma posição X aleatória ao nascer, dentro da tela.")]
    public bool posicaoInicialAleatoria = true;
    [Tooltip("Se marcado, cada vilão começa num ponto diferente da onda e pode ir para esquerda ou direita primeiro.")]
    public bool faseAleatoria = true;

    [Header("Tiro (Teia)")]
    [Tooltip("Prefab da teia que este vilão atira.")]
    public GameObject prefabTeia;
    [Tooltip("De onde a teia nasce. Se vazio, usa a própria posição do vilão.")]
    public Transform spawnTiro;
    [Tooltip("Tempo (segundos) entre um tiro e outro.")]
    public float intervaloTiro = 2f;

    [Header("Colisão / Dano recebido")]
    public string tagProjetilJogador = "Tiro";
    public int danoPorAcerto = 1;

    private float xInicial;
    private float limiteEsquerdo;
    private float limiteDireito;
    private float limiteInferior;
    private float amplitudeReal;

    private float tempoLocal;
    private float faseInicial;
    private float direcao = 1f;

    void Start()
    {
        CalcularLimites();

        float minX = limiteEsquerdo + amplitudeReal;
        float maxX = limiteDireito - amplitudeReal;

        if (posicaoInicialAleatoria)
            xInicial = Random.Range(minX, maxX);
        else
            xInicial = Mathf.Clamp(transform.position.x, minX, maxX);

        if (faseAleatoria)
        {
            faseInicial = Random.Range(0f, Mathf.PI * 2f);
            direcao = Random.value < 0.5f ? -1f : 1f;
        }

        // Já nasce na posição certa da onda, sem "pulo" no primeiro frame
        Vector3 pos = transform.position;
        pos.x = xInicial + Mathf.Sin(faseInicial) * direcao * amplitudeReal;
        transform.position = pos;

        StartCoroutine(RotinaDeTiro());
    }

    void Update()
    {
        MoverEmZigZag();
        VerificarSaidaDaTela();
    }

    // Descobre onde ficam as bordas da câmera no mundo
    private void CalcularLimites()
    {
        Camera cam = Camera.main;

        if (cam == null)
        {
            // Sem câmera: não limita nada
            limiteEsquerdo = -1000f;
            limiteDireito = 1000f;
            limiteInferior = -1000f;
            amplitudeReal = amplitudeZigZag;
            return;
        }

        float z = Mathf.Abs(cam.transform.position.z - transform.position.z);
        limiteEsquerdo = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, z)).x + margemLateral;
        limiteDireito  = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, z)).x - margemLateral;
        limiteInferior = cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, z)).y - margemDestruir;

        // Se a amplitude for maior que a metade da tela útil, reduz para caber
        float metadeLargura = (limiteDireito - limiteEsquerdo) * 0.5f;
        amplitudeReal = Mathf.Min(amplitudeZigZag, metadeLargura);
    }

    private IEnumerator RotinaDeTiro()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervaloTiro);
            Atirar();
        }
    }

    private void MoverEmZigZag()
    {
        // Desce em linha reta constante no Y, enquanto oscila em X usando
        // seno — cria o zig-zag fluido. Cada vilão tem seu próprio relógio
        // e fase, então não oscilam em sincronia. Nunca mexemos em
        // transform.rotation, então o sprite fica sempre reto.
        tempoLocal += Time.deltaTime;

        float deslocamentoX = Mathf.Sin(tempoLocal * velocidadeZigZag + faseInicial) * direcao * amplitudeReal;

        Vector3 novaPosicao = transform.position;
        novaPosicao.y -= velocidadeDescida * Time.deltaTime;
        novaPosicao.x = Mathf.Clamp(xInicial + deslocamentoX, limiteEsquerdo, limiteDireito);
        transform.position = novaPosicao;
    }

    // Destrói o vilão quando ele passa da borda de baixo da tela
    private void VerificarSaidaDaTela()
    {
        if (transform.position.y < limiteInferior)
        {
            Destroy(gameObject);
        }
    }

    private void Atirar()
    {
        Debug.Log("VilaoZigZag: tentando atirar...");

        if (prefabTeia == null)
        {
            Debug.LogWarning("VilaoZigZag: 'Prefab Teia' está vazio no Inspector — nada foi instanciado.");
            return;
        }

        Vector3 posicaoDisparo = spawnTiro != null ? spawnTiro.position : transform.position;
        Instantiate(prefabTeia, posicaoDisparo, Quaternion.identity);
    }
}