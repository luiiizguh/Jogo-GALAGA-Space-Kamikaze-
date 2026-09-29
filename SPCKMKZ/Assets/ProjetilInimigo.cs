using UnityEngine;

/// <summary>
/// Projétil disparado pelo InimigoAtirador. Se autodestrói ao sair da tela
/// por baixo, ou depois de um tempo máximo (segurança), pra não acumular
/// objetos esquecidos na cena.
/// Anexar no prefab do tiro.
/// </summary>
public class ProjetilInimigo : MonoBehaviour
{
    [Tooltip("Tempo de vida máximo, mesmo que nunca saia da tela.")]
    public float tempoDeVida = 5f;

    [Tooltip("Posição Y considerada 'fora da tela' por baixo.")]
    public float limiteInferior = -6f;

    void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    void Update()
    {
        if (transform.position.y < limiteInferior)
        {
            Destroy(gameObject);
        }
    }
}