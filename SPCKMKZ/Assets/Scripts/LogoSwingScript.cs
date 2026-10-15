using UnityEngine;

public class LogoSwingScript : MonoBehaviour
{
    [Header("Balanço (ângulo)")]
    public float anguloMaximo = 5f;      // graus pra cada lado
    public float velocidadeAngulo = 1.5f;

    [Header("Pulsar (escala)")]
    public float variacaoEscala = 0.05f; // 0.05 = 5% maior/menor
    public float velocidadeEscala = 2f;

    [Header("Opções")]
    public bool ignorarTimeScale = true; // continua animando mesmo com o jogo pausado

    private Vector3 escalaInicial;
    private Quaternion rotacaoInicial;

    void Start()
    {
        escalaInicial = transform.localScale;
        rotacaoInicial = transform.localRotation;
    }

    void Update()
    {
        float t = ignorarTimeScale ? Time.unscaledTime : Time.time;

        // Balança de um lado pro outro
        float angulo = Mathf.Sin(t * velocidadeAngulo) * anguloMaximo;
        transform.localRotation = rotacaoInicial * Quaternion.Euler(0f, 0f, angulo);

        // Pulsa a escala
        float fator = 1f + Mathf.Sin(t * velocidadeEscala) * variacaoEscala;
        transform.localScale = escalaInicial * fator;
    }
}