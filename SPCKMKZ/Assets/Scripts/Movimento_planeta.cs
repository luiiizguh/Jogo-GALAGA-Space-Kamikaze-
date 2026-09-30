using UnityEngine;

public class PlanetaRolando : MonoBehaviour
{
    public float velocidade = 0.05f;

    // Altura acima da posição inicial onde o planeta reaparece
    public float distanciaReinicio = 6f;

    private float yInicial;

    void Start()
    {
        yInicial = transform.position.y;
    }

    void Update()
    {
        transform.position += Vector3.down * velocidade * Time.deltaTime;

        // Quando sair bastante da tela
        if (transform.position.y < -distanciaReinicio)
        {
            // Volta para cima
            transform.position = new Vector3(
                transform.position.x,
                yInicial,
                transform.position.z
            );
        }
    }
}