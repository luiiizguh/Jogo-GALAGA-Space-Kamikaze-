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

    // Área onde os inimigos nascem (fixa no código, o Inspector não altera)
    private const float X_MIN = -1.525f;
    private const float X_MAX = 1.525f;
    private const float Y_INICIAL = 1.400f;

    // Chamado pelo GerenciadorFases no começo de cada fase
    public void IniciarFase()
    {
        StopAllCoroutines();
        inimigosCriados = 0;
        StartCoroutine(CriarInimigos());
    }

    IEnumerator CriarInimigos()
    {
        // Enquanto ainda faltar criar inimigos
        while (inimigosCriados < gerenciadorFases.quantidadeInimigos)
        {
            // Escolhe uma posição aleatória no eixo X
            float posicaoX = Random.Range(X_MIN, X_MAX);

            // Cria o inimigo
            GameObject novo = Instantiate(
                inimigoPrefab,
                new Vector3(posicaoX, Y_INICIAL, 0f),
                Quaternion.identity
            );

            // Passa a referência do gerenciador para o inimigo
            VidaInimigos inimigo = novo.GetComponentInChildren<VidaInimigos>();

            if (inimigo != null)
            {
                inimigo.gerenciador = gerenciadorFases;
            }
            else
            {
                Debug.LogError("O prefab do inimigo não tem o script Enemy!", novo);
            }

            // Soma 1 ao contador
            inimigosCriados++;

            // Espera o tempo definido pela fase
            yield return new WaitForSeconds(gerenciadorFases.tempoEntreSpawns);
        }
    }

    // Mostra a área de spawn (linha vermelha) na aba Scene
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            new Vector3(X_MIN, Y_INICIAL, 0f),
            new Vector3(X_MAX, Y_INICIAL, 0f)
        );
    }
}