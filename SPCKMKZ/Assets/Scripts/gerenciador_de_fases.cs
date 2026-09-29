using UnityEngine;
using TMPro;

// Controla as fases do jogo
public class GerenciadorFases : MonoBehaviour
{
    public int faseAtual = 1;
    public int quantidadeInimigos;
    public float tempoEntreSpawns;

    // Referência para o spawner
    public SpawnerInimigos spawner;

    // Textos do HUD (arraste do Canvas)
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

        bool temSpawn = ConfigurarFase();

        if (temSpawn)
        {
            spawner.IniciarFase();
        }

        AtualizarHUD();
    }

    // Chamado pelo Enemy quando o jogador o mata
    public void InimigoDerrotado(int pontosGanhos)
    {
        inimigosDerrotados++;
        pontos += pontosGanhos;

        InimigoFinalizado();
    }

    // Chamado pelo Enemy quando ele passa da tela
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
            textoFase.text = "FASE " + faseAtual;

        if (textoPontos != null)
            textoPontos.text = "PONTOS: " + pontos;

        if (textoRestantes != null)
        {
            int restantes = quantidadeInimigos - inimigosFinalizados;
            textoRestantes.text = "INIMIGOS: " + restantes;
        }
    }

    // Retorna true se a fase tem inimigos normais para spawnar
    bool ConfigurarFase()
    {
        if (faseAtual == 1)
        {
            quantidadeInimigos = 100;
            tempoEntreSpawns = 1f;
            return true;
        }
        else if (faseAtual == 2)
        {
            quantidadeInimigos = 200;
            tempoEntreSpawns = 0.8f;
            return true;
        }
        else if (faseAtual == 3)
        {
            quantidadeInimigos = 300;
            tempoEntreSpawns = 0.7f;
            return true;
        }

        // Fase do boss
        quantidadeInimigos = 0;
        if (textoRestantes != null) textoRestantes.text = "BOSS!";
        return false;
    }

    public void ProximaFase()
    {
        faseAtual++;
        IniciarFase();
    }
}