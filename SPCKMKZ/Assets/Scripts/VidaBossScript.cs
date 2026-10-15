using UnityEngine;

public class BarraVidaSquare : MonoBehaviour
{
    [Header("Referências")]
    public VidaInimigos alvo;      // se deixar vazio, procura no objeto pai
    public Transform square;       // o Square da barra (se vazio, usa este objeto)

    [Header("Opções")]
    public bool ancorarEsquerda = true;  // true = encolhe da direita pra esquerda
    public float suavidade = 10f;        // 0 = instantâneo
    public bool esconderCheia = false;   // esconde a barra enquanto a vida está cheia

    private float vidaMaxima;
    private float escalaXInicial;
    private float posXInicial;
    private float larguraSprite = 1f;
    private float fillAtual = 1f;
    private SpriteRenderer sr;

    void Start()
    {
        if (alvo == null) alvo = GetComponentInParent<VidaInimigos>();
        if (square == null) square = transform;

        sr = square.GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
            larguraSprite = sr.sprite.bounds.size.x;

        vidaMaxima = alvo != null ? alvo.vida : 1f;
        escalaXInicial = square.localScale.x;
        posXInicial = square.localPosition.x;
    }

    void LateUpdate()
    {
        if (alvo == null) return;

        float fillAlvo = Mathf.Clamp01(alvo.vida / vidaMaxima);
        fillAtual = suavidade > 0f
            ? Mathf.Lerp(fillAtual, fillAlvo, Time.deltaTime * suavidade)
            : fillAlvo;

        // Encolhe no eixo X
        Vector3 escala = square.localScale;
        escala.x = escalaXInicial * fillAtual;
        square.localScale = escala;

        // Mantém a borda esquerda parada (senão encolhe pelo centro)
        if (ancorarEsquerda)
        {
            Vector3 pos = square.localPosition;
            float perda = (escalaXInicial - escala.x) * larguraSprite * 0.5f;
            pos.x = posXInicial - perda;
            square.localPosition = pos;
        }

        if (sr != null && esconderCheia)
            sr.enabled = fillAlvo < 0.999f;
    }
}