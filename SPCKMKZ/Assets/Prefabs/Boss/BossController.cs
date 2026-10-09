using UnityEngine;

/// Move o boss de um lado pro outro da tela, de forma suave (seno no eixo X),
/// e faz ele flutuar um pouco pra cima e pra baixo (seno no eixo Y, só de efeito).
/// Anexar no objeto do boss.
public class BossController : MonoBehaviour
{
    [Header("Eixo X (vai e volta)")]
    [Tooltip("Distância que ele vai pra cada lado, a partir do ponto onde foi colocado na cena.")]
    public float amplitudeX = 3f;

    [Tooltip("Velocidade do vai e volta. Maior = mais rápido.")]
    public float velocidadeX = 1.5f;

    [Header("Eixo Y (flutuação)")]
    [Tooltip("Quanto ele sobe e desce a partir do ponto original. 0 desliga o efeito.")]
    public float amplitudeY = 0.3f;

    [Tooltip("Velocidade da flutuação. Use um valor diferente do X pra ficar mais orgânico.")]
    public float velocidadeY = 3f;

    [Tooltip("Atraso da onda Y em relação ao X (em radianos). Muda o desenho do trajeto.")]
    public float faseY = 0f;

    private Vector3 posicaoInicial;
    private float tempo;

    void Start()
    {
        posicaoInicial = transform.position;
    }

    void Update()
    {
        tempo += Time.deltaTime;

        float deslocamentoX = Mathf.Sin(tempo * velocidadeX) * amplitudeX;
        float deslocamentoY = Mathf.Sin(tempo * velocidadeY + faseY) * amplitudeY;

        transform.position = new Vector3(
            posicaoInicial.x + deslocamentoX,
            posicaoInicial.y + deslocamentoY,
            posicaoInicial.z);
    }
}