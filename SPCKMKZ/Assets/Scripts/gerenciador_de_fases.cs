
using UnityEngine;
using TMPro;

// Controla as fases do jogo
public class GerenciadorFases : MonoBehaviour
{
    [Header("Configuração da Fase")]
    public int faseAtual = 1;
    public int quantidadeInimigos = 10;
    public float tempoEntreSpawns = 1f;

    [Header("Referências")]
    public SpawnerInimigos spawner;

    [Header("HUD")]
    public TMP_Text textoFase;
    public TMP_Text textoPontos;
    public TMP_Text textoRestantes;

    // Pontuação total do jogador
    private int pontos = 0;

    private int inimigosDerrotados = 0;
    private int inimigosFinalizados = 0;

    void Start()
    {
        IniciarFase();
    }

    void IniciarFase()
    {
        inimigosDerrotados = 0;
        inimigosFinalizados = 0;

        if (spawner != null)
        {
            spawner.IniciarFase();
        }
        else
        {
            Debug.LogError("Spawner não foi atribuído no Inspector!");
        }

        AtualizarHUD();

        Debug.Log("FASE " + faseAtual + " INICIADA");
        Debug.Log("Quantidade de inimigos: " + quantidadeInimigos);
    }

    // Chamado quando o jogador destrói um inimigo
    public void InimigoDerrotado(int pontosGanhos)
    {
        inimigosDerrotados++;

        pontos += pontosGanhos;

        Debug.Log("Inimigo derrotado!");
        Debug.Log("Pontos atuais: " + pontos);

        InimigoFinalizado();
    }

    // Chamado quando o inimigo sai da tela
    public void InimigoEscapou()
    {
        InimigoFinalizado();
    }

    void InimigoFinalizado()
    {
        inimigosFinalizados++;

        AtualizarHUD();

        if (inimigosFinalizados >= quantidadeInimigos)
        {
            Debug.Log("FASE COMPLETA!");

            ProximaFase();
        }
    }

    void AtualizarHUD()
    {
        if (textoFase != null)
        {
            textoFase.text = "FASE " + faseAtual;
        }

        if (textoPontos != null)
        {
            textoPontos.text = "PONTOS: " + pontos;
        }

        if (textoRestantes != null)
        {
            int restantes = quantidadeInimigos - inimigosFinalizados;

            if (restantes < 0)
                restantes = 0;

            textoRestantes.text = "INIMIGOS: " + restantes;
        }
    }

    public void ProximaFase()
    {
        faseAtual++;

        Debug.Log("INDO PARA A FASE " + faseAtual);

        IniciarFase();
    }

    // Permite outros scripts consultarem a pontuação
    public int ObterPontos()
    {
        return pontos;
    }
}
