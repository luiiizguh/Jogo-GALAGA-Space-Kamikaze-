using UnityEngine;

// Controla as fases do jogo
public class GerenciadorFases : MonoBehaviour
{
    // Fase atual do jogador
    public int faseAtual = 1;

    // Quantidade de inimigos que devem aparecer na fase
    public int quantidadeInimigos;


   // Quantidade de inimigos derrotados
    private int inimigosDerrotados = 0;

    // Tempo entre o surgimento dos inimigos
    public float tempoEntreSpawns;

    // Configura os valores da fase quando o jogo começa
    void Start()
    {
        ConfigurarFase();
    }

    public void InimigoDerrotado()
    {
        inimigosDerrotados++;

        Debug.Log("Inimigos derrotados: " + inimigosDerrotados);

        // Verifica se todos foram derrotados
        if (inimigosDerrotados >= quantidadeInimigos)
        {
            Debug.Log("FASE COMPLETA!");

            ProximaFase();
        }
    }

    // Define as regras de cada fase
    void ConfigurarFase()
    {
        // FASE 1
        if (faseAtual == 1)
        {
            quantidadeInimigos = 100;
            tempoEntreSpawns = 1f;

            Debug.Log("FASE 1 INICIADA");
        }

        // FASE 2
        else if (faseAtual == 2)
        {
            quantidadeInimigos = 200;
            tempoEntreSpawns = 0.8f;

            Debug.Log("FASE 2 INICIADA");
        }

        // FASE 3
        else if (faseAtual == 3)
        {
            quantidadeInimigos = 300;
            tempoEntreSpawns = 0.7f;

            Debug.Log("FASE 3 INICIADA");
        }

        // FASE FINAL
        else
        {
            Debug.Log("FASE FINAL - BOSS");
        }
    }

    // Chamada quando o jogador completa uma fase
    public void ProximaFase()
    {
        // Avança para a próxima fase
        faseAtual++;

        // Atualiza as configurações da nova fase
        ConfigurarFase();
    }
}