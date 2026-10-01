using UnityEngine;

/// <summary>
/// Anexar tanto no GameObject "Nucleo" quanto no "Escudo".
/// Detecta colisão com o projétil do jogador e avisa o InimigoEscudo pai.
/// </summary>
public class ParteInimigo : MonoBehaviour
{
    public enum TipoParte { Nucleo, Escudo }

    [Tooltip("Defina se este objeto é o Núcleo ou o Escudo do inimigo.")]
    public TipoParte tipo;

    [Tooltip("Tag que causa dano ao encostar. Pode ser o projétil ('Projetil') ou a própria nave ('Nave').")]
    public string tagProjetil = "Tiro";

    [Tooltip("Quantidade de dano causada por colisão.")]
    public int dano = 1;

    [Tooltip("Marque só se o objeto colisor deve ser destruído ao causar dano (ex: um projétil). Deixe desmarcado para a nave do jogador.")]
    public bool destruirObjetoColisor = false;

    private InimigoEscudo inimigo;

    void Start()
    {
        inimigo = GetComponentInParent<InimigoEscudo>();

        if (inimigo == null)
            Debug.LogWarning("ParteInimigo: não encontrou InimigoEscudo no objeto pai.");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(tagProjetil)) return;
        if (inimigo == null) return;

          

        if (destruirObjetoColisor)
            Destroy(other.gameObject);
    }
}