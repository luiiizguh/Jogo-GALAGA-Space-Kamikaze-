using UnityEngine;

public class TesteGameOver : MonoBehaviour
{
    [Header("Arraste o painel de Game Over")]
    public GameObject painelGameOver;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            painelGameOver.SetActive(true);
        }
    }
}