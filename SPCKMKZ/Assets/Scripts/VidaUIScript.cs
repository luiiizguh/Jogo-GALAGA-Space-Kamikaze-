using UnityEngine;
using UnityEngine.Events;

// Vai no EMPTY OBJECT. O sprite (Draw Mode = Tiled) fica dentro dele, como filho.
public class SistemaVidas : MonoBehaviour
{
    // Permite chamar de qualquer script da cena: SistemaVidas.Instance.PerderVida();
    public static SistemaVidas Instance { get; private set; }

    [Header("Referência")]
    [Tooltip("O sprite em Tiled mode que mostra as vidas")]
    [SerializeField] private SpriteRenderer spriteVidas;

    [Header("Vidas")]
    [SerializeField] private int vidasMaximas = 3;

    [Header("Eventos")]
    [Tooltip("Chamado toda vez que a vida muda, com o valor atual")]
    public UnityEvent<int> aoMudarVida;
    [Tooltip("Chamado quando chega a 0 vidas (ex: recarregar a fase, tela de game over)")]
    public UnityEvent aoMorrer;

    public int VidaAtual { get; private set; }
    public int VidasMaximas => vidasMaximas;

    private float larguraTile;       // largura de UMA vida (1 repetição do sprite)
    private float larguraCheia;      // largura com todas as vidas
    private float pivotNormalizadoX; // 0 = pivot na esquerda, 0.5 = centro, 1 = direita
    private Vector3 posicaoInicial;

    private void Awake()
    {
        Instance = this;

        if (spriteVidas == null || spriteVidas.sprite == null)
        {
            Debug.LogError("SistemaVidas: arraste o sprite no campo Sprite Vidas (e ele precisa ter um Sprite).");
            enabled = false;
            return;
        }

        if (spriteVidas.drawMode != SpriteDrawMode.Tiled)
            spriteVidas.drawMode = SpriteDrawMode.Tiled;

        Sprite sprite = spriteVidas.sprite;
        larguraTile = sprite.bounds.size.x;
        larguraCheia = larguraTile * vidasMaximas;
        pivotNormalizadoX = sprite.pivot.x / sprite.rect.width;
        posicaoInicial = spriteVidas.transform.localPosition;

        VidaAtual = vidasMaximas;
        AtualizarVisual();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void PerderVida(int quantidade = 1)
    {
        if (VidaAtual <= 0) return; // já morreu, não dispara de novo

        VidaAtual = Mathf.Max(0, VidaAtual - quantidade);
        AtualizarVisual();
        aoMudarVida.Invoke(VidaAtual);

        if (VidaAtual == 0)
            aoMorrer.Invoke();
    }

    public void GanharVida(int quantidade = 1)
    {
        if (VidaAtual <= 0) return;

        VidaAtual = Mathf.Min(vidasMaximas, VidaAtual + quantidade);
        AtualizarVisual();
        aoMudarVida.Invoke(VidaAtual);
    }

    public void ResetarVidas()
    {
        VidaAtual = vidasMaximas;
        AtualizarVisual();
        aoMudarVida.Invoke(VidaAtual);
    }

    private void AtualizarVisual()
    {
        float novaLargura = larguraTile * VidaAtual;

        // muda só a largura; a altura continua a que você configurou
        Vector2 tamanho = spriteVidas.size;
        tamanho.x = novaLargura;
        spriteVidas.size = tamanho;

        // O Tiled encolhe a partir do pivot. Esse ajuste mantém a borda ESQUERDA parada,
        // então as vidas somem sempre da direita pra esquerda, qualquer que seja o pivot.
        float deslocamento = pivotNormalizadoX * (novaLargura - larguraCheia);
        spriteVidas.transform.localPosition =
            posicaoInicial + new Vector3(deslocamento * spriteVidas.transform.localScale.x, 0f, 0f);

        // com 0 vidas o tamanho vira 0, então só esconde o sprite
        spriteVidas.enabled = VidaAtual > 0;
    }

    // Pra testar sem player: clique nos três pontinhos do componente no Inspector (em Play)
    [ContextMenu("Testar: Perder Vida")]
    private void TestarPerder() => PerderVida();

    [ContextMenu("Testar: Ganhar Vida")]
    private void TestarGanhar() => GanharVida();
}