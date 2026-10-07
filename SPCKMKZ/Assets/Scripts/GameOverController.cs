
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla o menu de Game Over usando somente o teclado.
///
/// SETA PARA CIMA / BAIXO = muda a opção
/// ENTER = confirma
///
/// Opções:
/// 0 = RESTART
/// 1 = SAIR
///
/// A opção selecionada fica maior para o jogador
/// conseguir enxergar qual opção está escolhida.
/// </summary>
public class GameOverController : MonoBehaviour
{
    [Header("Botões")]
    [SerializeField] private Button botaoRestart;
    [SerializeField] private Button botaoSair;

    [Header("Tamanho dos botões")]
    [SerializeField] private float tamanhoNormal = 1f;
    [SerializeField] private float tamanhoSelecionado = 1.2f;

    // 0 = Restart
    // 1 = Sair
    private int opcaoSelecionada = 0;

    private void OnEnable()
    {
        // Começa selecionando RESTART
        opcaoSelecionada = 0;

        AtualizarSelecao();
    }

    private void Update()
    {
        // =========================
        // SETA PARA CIMA
        // =========================
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            opcaoSelecionada--;

            // Se passar do primeiro botão,
            // vai para o último
            if (opcaoSelecionada < 0)
            {
                opcaoSelecionada = 1;
            }

            AtualizarSelecao();
        }

        // =========================
        // SETA PARA BAIXO
        // =========================
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            opcaoSelecionada++;

            // Se passar do último botão,
            // volta para o primeiro
            if (opcaoSelecionada > 1)
            {
                opcaoSelecionada = 0;
            }

            AtualizarSelecao();
        }

        // =========================
        // ENTER
        // =========================
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ConfirmarOpcao();
        }
    }

    // Atualiza o visual da opção selecionada
    private void AtualizarSelecao()
    {
        // Primeiro deixa os dois botões
        // com tamanho normal
        botaoRestart.transform.localScale =
            Vector3.one * tamanhoNormal;

        botaoSair.transform.localScale =
            Vector3.one * tamanhoNormal;

        // Depois aumenta somente o selecionado
        if (opcaoSelecionada == 0)
        {
            botaoRestart.transform.localScale =
                Vector3.one * tamanhoSelecionado;

            botaoRestart.Select();
        }
        else
        {
            botaoSair.transform.localScale =
                Vector3.one * tamanhoSelecionado;

            botaoSair.Select();
        }
    }

    // Executa a opção escolhida
    private void ConfirmarOpcao()
    {
        if (opcaoSelecionada == 0)
        {
            // RESTART
            ReiniciarJogo();
        }
        else
        {
            // SAIR
            SairDoJogo();
        }
    }

    // Reinicia o jogo
    private void ReiniciarJogo()
    {
        // Volta o tempo do jogo ao normal
        Time.timeScale = 1f;

        // Recarrega a cena atual
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    // Fecha o jogo
    private void SairDoJogo()
    {
        Debug.Log("Saindo do jogo...");

        Application.Quit();
    }
}
