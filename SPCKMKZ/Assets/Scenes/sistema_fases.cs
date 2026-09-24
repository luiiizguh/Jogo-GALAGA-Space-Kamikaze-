using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Pontuação")]
    public int score = 0;
    public int highScore = 0;
    [SerializeField] private int pointsPerKill = 10;

    [Header("UI (opcional - arraste no Inspector)")]
    public UnityEngine.UI.Text scoreText;
    public UnityEngine.UI.Text highScoreText;

    private const string HIGHSCORE_KEY = "SpaceKamikaze_HighScore";

    void Awake()
    {
        // Singleton simples
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        score = 0;
        highScore = PlayerPrefs.GetInt(HIGHSCORE_KEY, 0);
        UpdateUI();
    }

    // Chame este método quando um inimigo morrer
    public void AddScore(int amount = -1)
    {
        int pointsToAdd = amount > 0 ? amount : pointsPerKill;
        score += pointsToAdd;

        if (score > highScore)
        {
            highScore = score;
            SaveHighScore();
        }

        UpdateUI();
    }

    void SaveHighScore()
    {
        PlayerPrefs.SetInt(HIGHSCORE_KEY, highScore);
        PlayerPrefs.Save();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Pontos: " + score;

        if (highScoreText != null)
            highScoreText.text = "Recorde: " + highScore;
    }

    public void ResetGame()
    {
        score = 0;
        UpdateUI();
    }
}