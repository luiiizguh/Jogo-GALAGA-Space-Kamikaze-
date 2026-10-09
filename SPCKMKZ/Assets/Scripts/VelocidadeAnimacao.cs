
using UnityEngine;

public class VelocidadeAnimacao : MonoBehaviour
{
    [Range(0f, 3f)]
    public float velocidade = 1f;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator != null)
        {
            animator.speed = velocidade;
        }
    }
}
