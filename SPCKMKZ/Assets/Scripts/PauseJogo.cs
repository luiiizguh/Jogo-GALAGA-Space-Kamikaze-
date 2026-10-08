using UnityEngine;

/// <summary>
/// Pausa o jogo com ESC e volta ao jogo com ESC de novo.
/// Anexar em um GameObject que fique SEMPRE ativo na cena (ex: um objeto vazio "Pause").
/// </summary>
public class PauseJogo : MonoBehaviour
{
    [Header("Tecla")]
    public KeyCode teclaPause = KeyCode.Escape;

    [Header("Painel de pause (opcional)")]
    [Tooltip("Painel/texto mostrado enquanto pausado, ex: 'PAUSADO - ESC para continuar'. Pode ficar vazio.")]
    public GameObject painelPause;

    [Header("Áudio")]
    [Tooltip("Se marcado, a música e os sons também pausam.")]
    public bool pausarAudio = true;

    private bool pausado = false;

    void Start()
    {
        if (painelPause != null)
            painelPause.SetActive(false);
    }

    void Update()
    {
        if (!Input.GetKeyDown(teclaPause)) return;

        if (pausado)
        {
            Continuar();
        }
        else
        {
            // Se o tempo já está parado (vitória ou game over), não deixa pausar por cima
            if (Time.timeScale == 0f) return;

            Pausar();
        }
    }

    public void Pausar()
    {
        pausado = true;
        Time.timeScale = 0f;

        if (pausarAudio)
            AudioListener.pause = true;

        if (painelPause != null)
            painelPause.SetActive(true);
    }

    public void Continuar()
    {
        pausado = false;
        Time.timeScale = 1f;

        if (pausarAudio)
            AudioListener.pause = false;

        if (painelPause != null)
            painelPause.SetActive(false);
    }

    // Segurança: ao sair da cena (reiniciar, voltar ao menu), nunca deixa o jogo travado
    void OnDestroy()
    {
        if (pausado)
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }
}