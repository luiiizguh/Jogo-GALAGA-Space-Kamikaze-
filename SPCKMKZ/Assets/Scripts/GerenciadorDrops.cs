using UnityEngine;

public class GerenciadorDrops : MonoBehaviour
{
    public static GerenciadorDrops Instance;

    [Header("Prefabs dos drops")]
    public GameObject prefabVida;
    public GameObject prefabHabilidade;

    [Header("A cada quantos inimigos mortos cai um drop (aleatório)")]
    public int minimoMortes = 5;
    public int maximoMortes = 10;

    [Range(0f, 1f)]
    public float chanceDeVida = 0.4f; // o resto é habilidade

    int mortesAtuais;
    int mortesParaProximoDrop;

    void Awake()
    {
        Instance = this;
        SortearProximoDrop();
    }

    void SortearProximoDrop()
    {
        mortesAtuais = 0;
        // o limite máximo do Random.Range com int é exclusivo, por isso o +1
        mortesParaProximoDrop = Random.Range(minimoMortes, maximoMortes + 1);
    }

    // Chame isso quando um inimigo morrer
    public void RegistrarMorte(Vector3 posicao)
    {
        mortesAtuais++;

        if (mortesAtuais >= mortesParaProximoDrop)
        {
            GameObject prefab = Random.value < chanceDeVida ? prefabVida : prefabHabilidade;
            if (prefab != null)
                Instantiate(prefab, posicao, Quaternion.identity);

            SortearProximoDrop();
        }
    }
}