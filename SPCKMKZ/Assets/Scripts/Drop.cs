using UnityEngine;

public class Drop : MonoBehaviour
{
    public enum Tipo { Vida, Habilidade }
    public Tipo tipo;

    public float velocidadeQueda = 0.8f;
    public float limiteInferiorY = -1.8f; // ajuste ao fundo da sua tela

    void Update()
    {
        transform.Translate(Vector3.down * velocidadeQueda * Time.deltaTime);

        if (transform.position.y < limiteInferiorY)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Não usa tag: procura o Nave_Script na nave (ou no pai do collider)
        Nave_Script nave = other.GetComponentInParent<Nave_Script>();
        if (nave == null) return;

        if (tipo == Tipo.Vida) nave.GanharVida();
        else nave.GanharHabilidade();

        Destroy(gameObject);
    }
}