using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Controla as fases do jogo
public class GerenciadorFases : MonoBehaviour
{
    // Configuração de uma fase (aparece no Inspector)
    [System.Serializable]
    public class ConfigFase
    {
        public string nome = "Fase";
        public GameObject inimigoPrefab;
        public int quantidadeInimigos = 10;
        public float tempoEntreSpawns = 1f;

        [Tooltip("Arraste da Hierarchy o objeto (imagem) que só aparece nesta fase.")]
        public GameObject objetoDaFase;

        public AudioClip musica;
    }

    [Header("Fases (Element 0 = fase 1, Element 1 = fase 2...)")]
    public List<ConfigFase> fases = new List<ConfigFase>();

    [Header("Estado atual")]
    public int faseAtual = 1;

    // Valores da fase atual (o Spawner lê estes campos)
    [HideInInspector] public int quantidadeInimigos;
    [HideInInspector] public float tempoEntreSpawns;
    [HideInInspector] public GameObject inimigoAtual;

    [Header("Referências")]
    public SpawnerInimigos spawner;

    [Header("Troca do planeta")]
    [Tooltip("Câmera usada para saber se o planeta saiu da tela. Se vazio, usa a Main Camera.")]
    public Camera cameraJogo;
    [Tooltip("Segurança: se o planeta antigo não sair da tela depois desse tempo (segundos), troca mesmo assim.")]
    public float esperaMaximaPlaneta = 60f;

    [Header("Música")]
    [Tooltip("AudioSource que toca a música. Se ficar vazio, o gerenciador cria um sozinho.")]
    public AudioSource fonteMusica;

    [Header("HUD")]
    public TMP_Text textoFase;
    public TMP_Text textoPontos;
    public TMP_Text textoRestantes;

    [Header("Vitória")]
    public GameObject painelVitoria;
    public TMP_Text textoPontosVitoria;   // opcional
    public AudioClip musicaVitoria;       // opcional

    // Pontuação total do jogador
    private int pontos = 0;

    // Inimigos da fase atual que ainda existem na cena
    private List<GameObject> inimigosVivos = new List<GameObject>();

    private int inimigosCriados = 0;
    private int inimigosFinalizados = 0;
    private bool jogoCompleto = false;

    // Planeta que está aparecendo agora e a rotina que espera ele sair
    private GameObject objetoAtual;
    private Coroutine rotinaTrocaObjeto;

    // Espera 1 frame para os outros scripts da cena se prepararem
    IEnumerator Start()
    {
        ProcurarReferencias();

        Time.timeScale = 1f;

        if (painelVitoria != null)
            painelVitoria.SetActive(false);

        // Começa com todos os objetos de fase desligados
        DesligarObjetosDasFases();

        yield return null;

        IniciarFase();
    }

    // Se alguma referência não foi arrastada no Inspector, procura na cena
    void ProcurarReferencias()
    {
        if (spawner == null)
            spawner = FindObjectOfType<SpawnerInimigos>();

        if (spawner != null && spawner.gerenciadorFases == null)
            spawner.gerenciadorFases = this;

        if (cameraJogo == null)
            cameraJogo = Camera.main;

        // Usa o AudioSource do próprio objeto; se não existir, cria um
        if (fonteMusica == null)
        {
            fonteMusica = GetComponent<AudioSource>();

            if (fonteMusica == null)
                fonteMusica = gameObject.AddComponent<AudioSource>();
        }

        fonteMusica.playOnAwake = false;
        fonteMusica.mute = false;
        fonteMusica.volume = 1f;
        fonteMusica.spatialBlend = 0f;   // som 2D, sem depender da posição
    }

    void Update()
    {
        if (jogoCompleto) return;

        // Remove da lista os inimigos já destruídos (por qualquer motivo)
        int removidos = inimigosVivos.RemoveAll(inimigo => inimigo == null);

        if (removidos > 0)
        {
            inimigosFinalizados += removidos;
            AtualizarHUD();
        }

        // Fase termina quando todos foram criados e nenhum está vivo
        if (quantidadeInimigos > 0 &&
            inimigosCriados >= quantidadeInimigos &&
            inimigosVivos.Count == 0)
        {
            Debug.Log("FASE COMPLETA!");
            ProximaFase();
        }
    }

    void IniciarFase()
    {
        inimigosVivos.Clear();
        inimigosCriados = 0;
        inimigosFinalizados = 0;

        int indice = faseAtual - 1;

        // Acabaram as fases da lista: vitória
        if (indice < 0 || indice >= fases.Count)
        {
            Vitoria();
            return;
        }

        // Carrega a configuração desta fase
        ConfigFase config = fases[indice];
        inimigoAtual = config.inimigoPrefab;
        quantidadeInimigos = config.quantidadeInimigos;
        tempoEntreSpawns = config.tempoEntreSpawns;

        AtivarObjetoDaFase(config.objetoDaFase);
        TocarMusica(config.musica, true);

        if (spawner != null)
        {
            spawner.IniciarFase();
        }
        else
        {
            Debug.LogError("Não existe nenhum SpawnerInimigos ativo na cena!", this);
        }

        AtualizarHUD();

        Debug.Log("FASE " + faseAtual + " INICIADA (" + config.nome + ")");
        Debug.Log("Quantidade de inimigos: " + quantidadeInimigos);
    }

    // Desliga os objetos de todas as fases da lista
    void DesligarObjetosDasFases()
    {
        foreach (ConfigFase f in fases)
        {
            if (f.objetoDaFase != null)
                f.objetoDaFase.SetActive(false);
        }
    }

    // Pede para mostrar o objeto (planeta) da fase.
    // O planeta anterior NÃO é desligado na hora: ele termina o caminho e sai da tela primeiro.
    void AtivarObjetoDaFase(GameObject objeto)
    {
        if (objeto == null)
        {
            Debug.LogWarning("A fase " + faseAtual + " não tem 'Objeto Da Fase' na lista.");
            return;
        }

        // Se já havia uma troca esperando (fase passou rápido), cancela e usa a mais nova
        if (rotinaTrocaObjeto != null)
            StopCoroutine(rotinaTrocaObjeto);

        rotinaTrocaObjeto = StartCoroutine(TrocarObjetoDaFase(objeto));
    }

    IEnumerator TrocarObjetoDaFase(GameObject novo)
    {
        // Se existe um planeta antigo ainda na cena, espera ele sair da tela
        if (objetoAtual != null && objetoAtual != novo && objetoAtual.activeInHierarchy)
        {
            GameObject antigo = objetoAtual;

            bool jaApareceu = false;
            float tempo = 0f;

            while (antigo != null && antigo.activeInHierarchy)
            {
                bool fora = ForaDaTela(antigo);

                if (!fora)
                    jaApareceu = true;

                // Só libera depois de ter aparecido e saído da tela
                if (jaApareceu && fora)
                    break;

                // Segurança para nunca travar a troca
                tempo += Time.deltaTime;
                if (tempo >= esperaMaximaPlaneta)
                {
                    Debug.LogWarning("O planeta antigo não saiu da tela a tempo; trocando mesmo assim.");
                    break;
                }

                yield return null;
            }

            if (antigo != null)
                antigo.SetActive(false);
        }

        // Agora sim aparece o planeta da nova fase
        objetoAtual = novo;
        novo.SetActive(true);

        rotinaTrocaObjeto = null;

        Debug.Log("Objeto da fase ativado: " + novo.name);
    }

    // Retorna true se o objeto está totalmente fora da área visível da câmera
    bool ForaDaTela(GameObject obj)
    {
        if (cameraJogo == null)
            cameraJogo = Camera.main;

        if (cameraJogo == null) return true;

        // Imagem de UI (Canvas)
        RectTransform rect = obj.GetComponent<RectTransform>();

        if (rect != null)
        {
            Canvas canvas = obj.GetComponentInParent<Canvas>();
            Vector3[] cantos = new Vector3[4];
            rect.GetWorldCorners(cantos);

            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;

            foreach (Vector3 c in cantos)
            {
                Vector3 p;

                if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    p = new Vector3(c.x / Screen.width, c.y / Screen.height, 0f);
                else
                    p = cameraJogo.WorldToViewportPoint(c);

                minX = Mathf.Min(minX, p.x);
                maxX = Mathf.Max(maxX, p.x);
                minY = Mathf.Min(minY, p.y);
                maxY = Mathf.Max(maxY, p.y);
            }

            return maxX < 0f || minX > 1f || maxY < 0f || minY > 1f;
        }

        // Sprite normal (SpriteRenderer etc.)
        Renderer rend = obj.GetComponentInChildren<Renderer>();

        if (rend == null) return true;

        Vector3 pMin = cameraJogo.WorldToViewportPoint(rend.bounds.min);
        Vector3 pMax = cameraJogo.WorldToViewportPoint(rend.bounds.max);

        return pMax.x < 0f || pMin.x > 1f || pMax.y < 0f || pMin.y > 1f;
    }

    void TocarMusica(AudioClip musica, bool repetir)
    {
        if (musica == null)
        {
            Debug.LogWarning("Esta fase não tem música escolhida na lista de Fases.");
            return;
        }

        if (fonteMusica == null)
        {
            Debug.LogWarning("Não há AudioSource para tocar a música.");
            return;
        }

        // Se já está tocando essa mesma música, não reinicia
        if (fonteMusica.clip == musica && fonteMusica.isPlaying) return;

        fonteMusica.clip = musica;
        fonteMusica.loop = repetir;
        fonteMusica.Play();

        Debug.Log("Tocando música: " + musica.name);
    }

    void Vitoria()
    {
        jogoCompleto = true;

        Debug.Log("VITÓRIA!");

        if (textoPontosVitoria != null)
            textoPontosVitoria.text = "PONTOS: " + pontos;

        if (painelVitoria != null)
            painelVitoria.SetActive(true);

        TocarMusica(musicaVitoria, false);

        Time.timeScale = 0f;
    }

    // Ligar no botão "Jogar de novo" do painel de vitória
    public void ReiniciarJogo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Chamado pelo Spawner logo depois de criar cada inimigo
    public void RegistrarInimigo(GameObject inimigo)
    {
        inimigosVivos.Add(inimigo);
        inimigosCriados++;
    }

    // Chamado quando o jogador destrói um inimigo (só soma pontos)
    public void InimigoDerrotado(int pontosGanhos)
    {
        pontos += pontosGanhos;

        Debug.Log("Inimigo derrotado!");
        Debug.Log("Pontos atuais: " + pontos);

        AtualizarHUD();
    }

    // Mantido para os colegas que já chamam este método.
    // Não precisa fazer nada: o Update percebe quando o inimigo some.
    public void InimigoEscapou()
    {
    }

    void AtualizarHUD()
    {
        if (textoFase != null)
        {
            textoFase.text = "FASE " + faseAtual;
        }

        if (textoPontos != null)
        {
            textoPontos.text = "PONTOS: " + pontos;
        }

        if (textoRestantes != null)
        {
            int restantes = quantidadeInimigos - inimigosFinalizados;

            if (restantes < 0)
                restantes = 0;

            textoRestantes.text = "INIMIGOS: " + restantes;
        }
    }

    public void ProximaFase()
    {
        faseAtual++;

        Debug.Log("INDO PARA A FASE " + faseAtual);

        IniciarFase();
    }

    // Permite outros scripts consultarem a pontuação
    public int ObterPontos()
    {
        return pontos;
    }
}