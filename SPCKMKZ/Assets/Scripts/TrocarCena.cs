
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrocarCena : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Time.timeScale = 1f;

            int proximaCena = SceneManager.GetActiveScene().buildIndex + 1;

            if (proximaCena < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(proximaCena);
            }
            else
            {
                Debug.Log("Não existe próxima cena na Build Settings.");
            }
        }
    }
}
