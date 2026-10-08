using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicaPersistente : MonoBehaviour
{
    private static MusicaPersistente instancia;

    [Header("Cenas onde essa música deve parar")]
    public string[] cenasQueParam = { "Fase_1" };

    void Awake()
    {
        if (instancia != null)
        {
            Destroy(gameObject); // já existe uma, destrói a duplicada
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += AoCarregarCena;
    }

    void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        foreach (string nome in cenasQueParam)
        {
            if (cena.name == nome)
            {
                Destroy(gameObject); // para a música do menu
                return;
            }
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= AoCarregarCena;
        if (instancia == this) instancia = null;
    }
}