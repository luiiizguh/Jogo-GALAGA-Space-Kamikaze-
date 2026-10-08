using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla o menu de Game Over usando somente o teclado.
///
/// SETA PARA CIMA / BAIXO = muda a opção (toca o som de selecionar)
/// ENTER = confirma
///
/// Opções:
/// 0 = RESTART (volta para a cena do menu)
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

    [Header("Cena do menu")]
    [Tooltip("Nome exato da cena do menu (precisa estar em File > Build Settings).")]
    [SerializeField] private string nomeCenaMenu = "Menu";

    [Header("Tamanho dos botões")]
    [SerializeField] private float tamanhoNormal = 1f;
    [SerializeField] private float tamanhoSelecionado = 1.2f;

    [Header("Sons")]
    [Tooltip("AudioSource só para efeitos (pode ser um AudioSource neste mesmo objeto).")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip somSelecionar;
    [Tooltip("Opcional: som ao apertar Enter.")]
    [SerializeField] private AudioClip somConfirmar;
    [SerializeField] private float tempoAntesDeConfirmar = 0.3f;

    // 0 = Restart (menu)
    // 1 = Sair
    private int opcaoSelecionada = 0;
    private bool confirmado = false;

    private void OnEnable()
    {
        // Começa selecionando RESTART (sem tocar som ao abrir o painel)
        opcaoSelecionada = 0;
        confirmado = false;

        AtualizarSelecao(false);
    }

    private void Update()
    {
        if (confirmado) return;

        // =========================
        // SETA PARA CIMA
        // =========================
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            opcaoSelecionada--;

            if (opcaoSelecionada < 0)
            {
                opcaoSelecionada = 1;
            }

            AtualizarSelecao(true);
        }

        // =========================
        // SETA PARA BAIXO
        // =========================
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            opcaoSelecionada++;

            if (opcaoSelecionada > 1)
            {
                opcaoSelecionada = 0;
            }

            AtualizarSelecao(true);
        }

        // =========================
        // ENTER
        // =========================
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            StartCoroutine(ConfirmarOpcao());
        }
    }

    // Atualiza o visual da opção selecionada
    private void AtualizarSelecao(bool tocarSom)
    {
        botaoRestart.transform.localScale = Vector3.one * tamanhoNormal;
        botaoSair.transform.localScale = Vector3.one * tamanhoNormal;

        if (opcaoSelecionada == 0)
        {
            botaoRestart.transform.localScale = Vector3.one * tamanhoSelecionado;
            botaoRestart.Select();
        }
        else
        {
            botaoSair.transform.localScale = Vector3.one * tamanhoSelecionado;
            botaoSair.Select();
        }

        if (tocarSom)
        {
            TocarSom(somSelecionar);
        }
    }

    private void TocarSom(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Executa a opção escolhida
    private IEnumerator ConfirmarOpcao()
    {
        confirmado = true;

        if (somConfirmar != null)
        {
            TocarSom(somConfirmar);

            // Realtime porque o jogo está com Time.timeScale = 0 no Game Over
            yield return new WaitForSecondsRealtime(tempoAntesDeConfirmar);
        }

        if (opcaoSelecionada == 0)
        {
            VoltarParaMenu();
        }
        else
        {
            SairDoJogo();
        }
    }

    // Volta para a cena do menu
    private void VoltarParaMenu()
    {
        // Volta o tempo e o áudio ao normal antes de trocar de cena
        Time.timeScale = 1f;
        AudioListener.pause = false;

        SceneManager.LoadScene(nomeCenaMenu);
    }

    // Fecha o jogo
    private void SairDoJogo()
    {
        Debug.Log("Saindo do jogo...");

        Application.Quit();
    }
}