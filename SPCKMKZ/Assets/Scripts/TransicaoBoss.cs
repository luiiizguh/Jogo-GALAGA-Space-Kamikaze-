using System.Collections;
using UnityEngine;

// Transição para o boss: a tela (câmera) treme e o fundo acelera.
// A nave NÃO é mexida nem bloqueada.
public class TransicaoBoss : MonoBehaviour
{
    [Header("Tempos (segundos)")]
    [Tooltip("Duração total (aceleração + velocidade máxima + freada) até o chefe chegar.")]
    public float tempoViagem = 8f;
    public float tempoAcelerar = 3f;        // tempo para o fundo chegar na velocidade máxima
    public float tempoDesacelerar = 1.5f;   // freada do fundo quando o chefe chega
    public float tempoFade = 1.5f;          // fade da música da fase

    [Header("Fundo")]
    [Tooltip("Quantas vezes mais rápido o fundo fica no pico da transição.")]
    public float multiplicadorMaximoEstrelas = 5f;

    // Os scripts de fundo (FundoRolando) multiplicam a velocidade por este valor (1 = normal)
    public static float MultiplicadorEstrelas = 1f;

    [Header("Tremor da tela")]
    [Tooltip("Câmera que treme. Se vazio, usa a Main Camera.")]
    public Camera cameraJogo;
    [Tooltip("Força do tremor no pico. 0 = a tela não treme.")]
    public float intensidadeTremor = 0.04f;

    [Header("Som (opcional)")]
    public AudioSource somAcelerar;

    bool fading = false;

    void Awake()
    {
        MultiplicadorEstrelas = 1f;
    }

    // Chamado pelo GerenciadorFases
    public IEnumerator Executar(AudioSource fonteMusica, AudioClip musicaBoss, float volumeMusica)
    {
        if (cameraJogo == null)
            cameraJogo = Camera.main;

        Transform cam = cameraJogo != null ? cameraJogo.transform : null;
        Vector3 posCamera = cam != null ? cam.position : Vector3.zero;

        // a música da fase some com fade
        if (fonteMusica != null)
            StartCoroutine(Fade(fonteMusica, fonteMusica.volume, 0f));

        if (somAcelerar != null) somAcelerar.Play();

        float acelerar = Mathf.Max(0.01f, tempoAcelerar);
        float desacelerar = Mathf.Max(0.01f, tempoDesacelerar);
        float total = Mathf.Max(tempoViagem, acelerar + desacelerar);
        float inicioFreada = total - desacelerar;

        bool musicaBossIniciada = false;
        float tempo = 0f;

        while (tempo < total)
        {
            tempo += Time.deltaTime;
            float mult;

            if (tempo < acelerar)
            {
                // fundo acelera aos poucos
                float p = tempo / acelerar;
                mult = Mathf.Lerp(1f, multiplicadorMaximoEstrelas, p * p);
            }
            else if (tempo < inicioFreada)
            {
                // velocidade máxima
                mult = multiplicadorMaximoEstrelas;
            }
            else
            {
                // chefe chegando: música do boss entra e o fundo freia
                if (!musicaBossIniciada && !fading)
                {
                    musicaBossIniciada = true;
                    IniciarMusicaBoss(fonteMusica, musicaBoss, volumeMusica);
                }

                float p = Mathf.Clamp01((tempo - inicioFreada) / desacelerar);
                float k = 1f - Mathf.Pow(1f - p, 3f);
                mult = Mathf.Lerp(multiplicadorMaximoEstrelas, 1f, k);
            }

            MultiplicadorEstrelas = mult;

            // a tela treme mais forte quando o fundo está mais rápido
            if (cam != null)
            {
                float intensidade = Mathf.InverseLerp(1f, multiplicadorMaximoEstrelas, mult);
                cam.position = posCamera + (Vector3)(Random.insideUnitCircle * intensidadeTremor * intensidade);
            }

            yield return null;
        }

        // garante a música do boss mesmo se o tempo foi muito curto
        if (!musicaBossIniciada)
        {
            while (fading) yield return null;
            IniciarMusicaBoss(fonteMusica, musicaBoss, volumeMusica);
        }

        // tudo volta ao normal
        if (cam != null) cam.position = posCamera;
        MultiplicadorEstrelas = 1f;
    }

    void IniciarMusicaBoss(AudioSource fonteMusica, AudioClip musicaBoss, float volumeMusica)
    {
        if (fonteMusica == null || musicaBoss == null) return;

        fonteMusica.clip = musicaBoss;
        fonteMusica.loop = true;
        fonteMusica.volume = 0f;
        fonteMusica.Play();
        StartCoroutine(Fade(fonteMusica, 0f, volumeMusica));
    }

    IEnumerator Fade(AudioSource fonte, float de, float para)
    {
        fading = true;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, tempoFade);
            fonte.volume = Mathf.Lerp(de, para, Mathf.Clamp01(t));
            yield return null;
        }
        fonte.volume = para;
        fading = false;
    }
}