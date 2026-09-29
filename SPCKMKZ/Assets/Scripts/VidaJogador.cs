using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VidaJogador : MonoBehaviour
{
    public int vidas = 3;

    // Arraste do Canvas
    public TMP_Text textoVidas;
    public GameObject painelGameOver;

    void Start()
    {
        Time.timeScale = 1f;

        if (painelGameOver != null)
            painelGameOver.SetActive(false);

        AtualizarHUD();
    }

    public void LevarDano(int dano)
    {
        vidas -= dano;
        if (vidas < 0) vidas = 0;

        AtualizarHUD();

        if (vidas <= 0)
        {
            GameOver();
        }
    }

    void AtualizarHUD()
    {
        if (textoVidas != null)
            textoVidas.text = "VIDAS: " + vidas;
    }

    void GameOver()
    {
        Debug.Log("GAME OVER");
        Time.timeScale = 0f; // congela o jogo

        if (painelGameOver != null)
            painelGameOver.SetActive(true);
    }

    // Chamado pelo botão "Reiniciar"
    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
