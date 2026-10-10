using UnityEngine;

public class Drop : MonoBehaviour
{
    public enum Tipo { Vida, Habilidade }

    public Tipo tipo;
    public float velocidadeQueda = 1f;   // sua cena é pequena (limites ~1.4 x 0.9), então use valores baixos
    public float limiteY = -1.5f;        // destrói ao sair da tela por baixo

    void Update()
    {
        transform.Translate(Vector2.down * velocidadeQueda * Time.deltaTime, Space.World);

        if (transform.position.y < limiteY)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Nave_Script nave = col.GetComponentInParent<Nave_Script>();
        if (nave == null) return;

        if (tipo == Tipo.Vida)
            nave.GanharVida();
        else
            nave.GanharHabilidade();

        Destroy(gameObject);
    }
}