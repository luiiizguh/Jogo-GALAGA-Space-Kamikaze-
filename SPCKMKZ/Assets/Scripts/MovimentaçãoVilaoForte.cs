using System.Collections;
using UnityEngine;

/// <summary>
/// Vilão que desce a tela em zig-zag (oscilação lateral suave), mantendo o
/// sprite sempre "reto" — nunca giramos o transform, só a posição muda.
/// Atira teias periodicamente. Anexar diretamente no GameObject do vilão.
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

    void Start()
    {
        xInicial = transform.position.x;
        StartCoroutine(RotinaDeTiro());
    }

    void Update()
    {
        MoverEmZigZag();
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
        // seno — cria o zig-zag fluido. Nunca mexemos em transform.rotation,
        // então a "cabeça" do sprite fica sempre reta.
        float deslocamentoX = Mathf.Sin(Time.time * velocidadeZigZag) * amplitudeZigZag;

        Vector3 novaPosicao = transform.position;
        novaPosicao.y -= velocidadeDescida * Time.deltaTime;
        novaPosicao.x = xInicial + deslocamentoX;
        transform.position = novaPosicao;
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