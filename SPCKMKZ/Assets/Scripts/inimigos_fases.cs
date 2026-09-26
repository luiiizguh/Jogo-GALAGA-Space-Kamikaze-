using System.Collections;
using UnityEngine;

// Responsável por criar os inimigos da fase
public class SpawnerInimigos : MonoBehaviour
{
    // Prefab do inimigo
    public GameObject inimigoPrefab;

    // Referência para o Gerenciador de Fases
    public GerenciadorFases gerenciadorFases;

    // Quantidade de inimigos já criados
    private int inimigosCriados = 0;

    // Posição mínima e máxima onde os inimigos podem aparecer
    public float xMin = -8f;
    public float xMax = 8f;

    void Start()
    {
        Debug.Log("Spawner iniciado");
        // Inicia a criação dos inimigos
        StartCoroutine(CriarInimigos());
    }

    IEnumerator CriarInimigos()
    {
        // Enquanto ainda faltar criar inimigos
        while (inimigosCriados < gerenciadorFases.quantidadeInimigos)
        {
            Debug.Log("Criando inimigo");
            // Escolhe uma posição aleatória no eixo X
            float posicaoX = Random.Range(-1.525f, 1.525f);

            // Cria o inimigo
            Instantiate(
                inimigoPrefab,
                new Vector3(posicaoX, 1.400f, 0f),
                Quaternion.identity
            );

            // Soma 1 ao contador
            inimigosCriados++;

            // Espera o tempo definido pela fase
            yield return new WaitForSeconds(
                gerenciadorFases.tempoEntreSpawns
            );
        }
    }
}