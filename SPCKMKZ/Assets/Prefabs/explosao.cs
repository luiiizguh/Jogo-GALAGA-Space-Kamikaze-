using UnityEngine;

/// <summary>
/// Autodestrói este objeto depois de "duracao" segundos.
/// Usar num efeito com Animator (como a explosão), ajustando "duracao"
/// pra bater com o tempo total da animação, evitando que o objeto
/// fique parado na tela depois de terminar.
/// </summary>
public class DestruirAposTempo : MonoBehaviour
{
    [Tooltip("Tempo (segundos) até este objeto se autodestruir. Ajuste pra bater com a duração da sua animação.")]
    public float duracao = 1f;

    void Start()
    {
        Destroy(gameObject, duracao);
    }
}