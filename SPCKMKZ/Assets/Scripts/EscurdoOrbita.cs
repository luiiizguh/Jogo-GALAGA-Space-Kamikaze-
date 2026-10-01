using UnityEngine;

/// <summary>
/// Faz o objeto (escudo) girar suavemente ao redor de um centro (o núcleo do inimigo).
/// Anexar este script no GameObject "Escudo".
/// </summary>
public class EscudoOrbita : MonoBehaviour
{
    [Header("Configuração da Órbita")]
    [Tooltip("Transform ao redor do qual o escudo vai girar. Se vazio, usa o pai automaticamente.")]
    public Transform centro;
    public int vida = 5;

    [Tooltip("Distância do escudo até o centro (raio da órbita).")]
    public float raio = 1f;

    [Tooltip("Velocidade da rotação em graus por segundo. Quanto maior, mais rápido.")]
    public float velocidadeGraus = 90f;

    public bool sentidoHorario = true;

    [Tooltip("Se marcado, o escudo também gira sobre si mesmo enquanto orbita (efeito visual extra).")]
    public bool girarSobreSiMesmo = true;

    public string tagProjetil = "Tiro";

    private float anguloAtual;

    void Start()
    {
        if (centro == null && transform.parent != null)
            centro = transform.parent;

        if (centro == null)
        {
            Debug.LogWarning("EscudoOrbita: nenhum 'centro' definido e o objeto não tem pai.");
            return;
        }

        // Calcula o ângulo inicial a partir da posição atual do escudo,
        // assim ele não "pula" para outro lugar no primeiro frame.
        Vector3 direcao = transform.position - centro.position;
        anguloAtual = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
    }

    void Update()
    {
        if (centro == null) return;

        float direcaoRotacao = sentidoHorario ? -1f : 1f;
        anguloAtual += velocidadeGraus * direcaoRotacao * Time.deltaTime;

        float anguloRad = anguloAtual * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(anguloRad), Mathf.Sin(anguloRad), 0f) * raio;

        transform.position = centro.position + offset;

        if (girarSobreSiMesmo)
            transform.rotation = Quaternion.Euler(0f, 0f, anguloAtual);
    }


    public void ReceberDano(int dano)
    {
        vida -= dano;
        if (vida <= 0)
        {
            Destroy(gameObject);
        }
    }
}