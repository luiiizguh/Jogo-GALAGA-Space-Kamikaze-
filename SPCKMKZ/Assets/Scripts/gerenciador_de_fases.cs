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

    // Liga só o objeto da fase atual. O fundo fixo não está na lista, então não é tocado.
    void AtivarObjetoDaFase(GameObject objeto)
    {
        DesligarObjetosDasFases();

        if (objeto == null)
        {
            Debug.LogWarning("A fase " + faseAtual + " não tem 'Objeto Da Fase' na lista.");
            return;
        }

        objeto.SetActive(true);

        Debug.Log("Objeto da fase ativado: " + objeto.name);
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