using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Controla o menu inicial pelo teclado:
// seta para cima / baixo muda o botão selecionado, Enter ativa o botão.
public class MenuController : MonoBehaviour
{
    [Header("Botões (na ordem: Naves, Jogar, Sair)")]
    [SerializeField] private Button[] botoes;

    [Header("Nomes das cenas (troque quando elas existirem)")]
    [SerializeField] private string cenaNaves = "SelecaoNaves";
    [SerializeField] private string cenaJogo = "Jogo";

    [Header("Aparência do botão selecionado")]
    [SerializeField] private Color corSelecionado = new Color(1f, 0.85f, 0.2f);
    [SerializeField] private Color corNormal = Color.white;
    [SerializeField] private float escalaSelecionado = 1.1f;

    [Header("Sons")]
    [SerializeField] private AudioSource fonteEfeitos;
    [SerializeField] private AudioClip somNavegar;

    // Guarda qual botão está selecionado (0 = Naves, 1 = Jogar, 2 = Sair)
    private int indiceSelecionado = 0;

    // Guarda o tamanho (escala) original de cada botão, para o script não alterar o formato deles
    private Vector3[] escalasOriginais;

    private void Start()
    {
        if (botoes == null || botoes.Length < 3)
        {
            Debug.LogError("MenuController: arraste os 3 botões no Inspector (Naves, Jogar, Sair).");
            enabled = false;
            return;
        }

        escalasOriginais = new Vector3[botoes.Length];

        // Desliga a navegação automática da Unity para ela não brigar com a nossa
        for (int i = 0; i < botoes.Length; i++)
        {
            escalasOriginais[i] = botoes[i].transform.localScale;

            Navigation nav = botoes[i].navigation;
            nav.mode = Navigation.Mode.None;
            botoes[i].navigation = nav;
        }

        // Liga cada botão à sua ação (funciona com o mouse também)
        botoes[0].onClick.AddListener(AbrirNaves);
        botoes[1].onClick.AddListener(Jogar);
        botoes[2].onClick.AddListener(Sair);

        AtualizarVisual();
    }

    private void Update()
    {
        if (CimaApertado())
        {
            MoverSelecao(-1);
        }
        else if (BaixoApertado())
        {
            MoverSelecao(1);
        }
        else if (EnterApertado())
        {
            // Faz o mesmo que clicar no botão selecionado
            botoes[indiceSelecionado].onClick.Invoke();
        }
    }

    private void MoverSelecao(int direcao)
    {
        // O cálculo faz a seleção "dar a volta": depois do último vai para o primeiro
        indiceSelecionado = (indiceSelecionado + direcao + botoes.Length) % botoes.Length;
        AtualizarVisual();

        // Toca o som de navegação (se os campos estiverem preenchidos no Inspector)
        if (fonteEfeitos != null && somNavegar != null)
        {
            fonteEfeitos.PlayOneShot(somNavegar);
        }
    }

    private void AtualizarVisual()
    {
        for (int i = 0; i < botoes.Length; i++)
        {
            bool selecionado = (i == indiceSelecionado);

            if (botoes[i].targetGraphic != null)
            {
                botoes[i].targetGraphic.color = selecionado ? corSelecionado : corNormal;
            }

            botoes[i].transform.localScale = escalasOriginais[i] * (selecionado ? escalaSelecionado : 1f);
        }
    }

    // ---------- Ações dos botões ----------

    private void AbrirNaves()
    {
        CarregarCena(cenaNaves);
    }

    private void Jogar()
    {
        CarregarCena(cenaJogo);
    }

    private void Sair()
    {
#if UNITY_EDITOR
        // Dentro do editor, "sair" apenas para o modo Play
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void CarregarCena(string nomeDaCena)
    {
        if (Application.CanStreamedLevelBeLoaded(nomeDaCena))
        {
            SceneManager.LoadScene(nomeDaCena);
        }
        else
        {
            Debug.LogWarning("A cena '" + nomeDaCena + "' ainda não existe ou não está no Build Profiles.");
        }
    }

    // ---------- Leitura do teclado (funciona nos dois sistemas de input da Unity) ----------

    private bool CimaApertado()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.upArrowKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.UpArrow);
#endif
    }

    private bool BaixoApertado()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.downArrowKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.DownArrow);
#endif
    }

    private bool EnterApertado()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null &&
               (Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.numpadEnterKey.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
#endif
    }
}