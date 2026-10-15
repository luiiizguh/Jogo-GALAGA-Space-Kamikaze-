using UnityEngine;

public class Drop : MonoBehaviour
{
    public enum Tipo { Vida, Habilidade }

    public Tipo tipo;
    public float velocidadeQueda = 1f;
    public float limiteY = -1.5f;
    public GameObject prefab_efeito;

    [Header("Sons")]
    public AudioClip somVida;
    public AudioClip somHabilidade;
    [Range(1, 6)] public int camadasVolume = 3; // 1 = normal, 3 = bem mais alto

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

        if (prefab_efeito != null)
        {
            GameObject efeito = Instantiate(prefab_efeito, transform.position, Quaternion.identity);
            Destroy(efeito, 2f);
        }

        AudioClip som = (tipo == Tipo.Vida) ? somVida : somHabilidade;
        TocarSom2D(som);

        if (tipo == Tipo.Vida)
            nave.GanharVida();
        else
            nave.GanharHabilidade();

        Destroy(gameObject);
    }

    void TocarSom2D(AudioClip clip)
    {
        if (clip == null) return;

        GameObject go = new GameObject("SomColeta");

        // Várias fontes tocando o mesmo som somam o volume
        for (int i = 0; i < camadasVolume; i++)
        {
            AudioSource src = go.AddComponent<AudioSource>();
            src.clip = clip;
            src.volume = 1f;
            src.spatialBlend = 0f;  // 2D total, sem distância
            src.spatialize = false;
            src.Play();
        }

        Destroy(go, clip.length);
    }
}