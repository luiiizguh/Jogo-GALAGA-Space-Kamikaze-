using UnityEngine;

/// <summary>
/// Dados enviados pra nave quando ela é atingida pela teia.
/// </summary>
[System.Serializable]
public class DadosLentidao
{
    public float multiplicador;
    public float duracao;
}

/// <summary>
/// Teia disparada pelo VilaoZigZag: desce a tela (a animação já pronta toca
/// sozinha via Animator) e desaparece ao encostar na nave ou sair da tela.
/// Ao acertar a nave, manda ela ficar mais lenta por um tempo.
/// Anexar no prefab que já tem o Animator com a animação da teia.
/// </summary>
public class TeiaProjetil : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 4f;
    [Tooltip("Posição Y considerada 'fora da tela' por baixo.")]
    public float limiteInferior = -6f;

    [Header("Efeito na nave")]
    public string tagNave = "Nave";
    [Tooltip("Multiplicador de velocidade aplicado na nave (ex: 0.5 = metade da velocidade normal).")]
    public float multiplicadorLentidao = 0.5f;
    [Tooltip("Por quantos segundos a lentidão dura.")]
    public float duracaoLentidao = 2f;

    void Update()
    {
        transform.position += Vector3.down * velocidade * Time.deltaTime;

        if (transform.position.y < limiteInferior)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(tagNave)) return;

        // Avisa a nave pra ficar mais lenta por um tempo. O script da nave
        // precisa ter um método público "AplicarLentidao(DadosLentidao)"
        // pra isso funcionar de verdade — combine com quem estiver fazendo
        // o script da nave. DontRequireReceiver evita erro caso esse
        // método ainda não exista.
        DadosLentidao dados = new DadosLentidao
        {
            multiplicador = multiplicadorLentidao,
            duracao = duracaoLentidao
        };

        other.gameObject.SendMessage("AplicarLentidao", dados, SendMessageOptions.DontRequireReceiver);

        Destroy(gameObject);
    }
}