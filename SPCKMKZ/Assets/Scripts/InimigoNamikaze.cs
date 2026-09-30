using UnityEngine;

/// <summary>
/// Inimigo Namikaze: entra devagar, depois MIRA no jogador girando de forma
/// suave, e quando estiver bem alinhado TRAVA a direção e mergulha acelerando.
///
/// A direção de MOVIMENTO é calculada separadamente da rotação VISUAL do
/// sprite — assim, ajustar "offsetAnguloSprite" corrige a aparência sem
/// nunca desalinhar a mira ou o movimento.
/// Anexar diretamente no GameObject do inimigo.
/// </summary>
public class InimigoNamikaze : MonoBehaviour
{
    private enum Estado { Entrando, Mirando, Mergulhando }

    [Header("Alvo")]
    [Tooltip("Se ficar vazio, o script procura automaticamente um objeto com a tag abaixo.")]
    public Transform jogador;
    public string tagJogador = "Jogador";

    [Header("Entrada (fase lenta)")]
    public float velocidadeInicial = 1.5f;
    [Tooltip("Tempo (segundos) descendo reto antes de começar a mirar.")]
    public float tempoDeEntrada = 1f;

    [Header("Mira (antes do mergulho)")]
    [Tooltip("Tempo de suavização do giro. MAIOR = curva mais larga e suave. MENOR = vira mais rápido.")]
    public float tempoSuavizacaoGiro = 0.35f;
    [Tooltip("Quando o ângulo até o jogador for menor que isso, ele trava a direção e mergulha.")]
    public float anguloParaMergulhar = 3f;

    [Header("Mergulho (depois de mirar)")]
    public float velocidadeMaxima = 9f;
    public float aceleracao = 4f;

    [Header("Sprite (só afeta a aparência, nunca o movimento)")]
    [Tooltip("Gire este valor em passos de 90 até a ponta do sprite ficar visualmente correta: teste 0, 90, 180 e -90.")]
    public float offsetAnguloSprite = 0f;

    [Header("Colisão")]
    [Tooltip("Tag usada pelos projéteis do jogador.")]
    public string tagProjetil = "Projetil";
    [Tooltip("Tag usada pelas naves. Colidir com uma nave também destrói este inimigo (ele se sacrifica no impacto).")]
    public string tagNave = "Nave";

    private Estado estado = Estado.Entrando;
    private float velocidadeAtual;
    private float tempoDecorrido;

    // anguloMovimento é sempre o ângulo "puro" (sem offset de sprite) da
    // direção real pra onde o inimigo está se deslocando.
    private float anguloMovimento;
    private float anguloVelocidadeRef;
    private Vector2 direcaoMergulho;

    void Start()
    {
        velocidadeAtual = velocidadeInicial;

        // Começa apontando puramente pra baixo (-90° no sistema atan2).
        anguloMovimento = -90f;
        AtualizarRotacaoVisual();

        if (jogador == null)
        {
            GameObject alvo = GameObject.FindGameObjectWithTag(tagJogador);
            if (alvo != null) jogador = alvo.transform;
        }
    }

    void Update()
    {
        tempoDecorrido += Time.deltaTime;

        switch (estado)
        {
            case Estado.Entrando:
                AtualizarEntrada();
                break;
            case Estado.Mirando:
                AtualizarMira();
                break;
            case Estado.Mergulhando:
                AtualizarMergulho();
                break;
        }
    }

    private Vector2 DirecaoDoAngulo(float anguloGraus)
    {
        float rad = anguloGraus * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    private void AtualizarRotacaoVisual()
    {
        // Só isso aqui usa o offset do sprite — nunca o movimento.
        transform.rotation = Quaternion.Euler(0f, 0f, anguloMovimento + offsetAnguloSprite);
    }

    private void AtualizarEntrada()
    {
        Vector2 direcao = DirecaoDoAngulo(anguloMovimento);
        transform.position += (Vector3)direcao * velocidadeAtual * Time.deltaTime;

        if (tempoDecorrido >= tempoDeEntrada)
            estado = Estado.Mirando;
    }

    private void AtualizarMira()
    {
        Vector2 direcaoAtual = DirecaoDoAngulo(anguloMovimento);
        transform.position += (Vector3)direcaoAtual * velocidadeInicial * Time.deltaTime;

        if (jogador == null)
        {
            direcaoMergulho = direcaoAtual;
            estado = Estado.Mergulhando;
            return;
        }

        Vector2 paraJogador = (Vector2)jogador.position - (Vector2)transform.position;
        float anguloAlvo = Mathf.Atan2(paraJogador.y, paraJogador.x) * Mathf.Rad2Deg;

        // Curva suave com aceleração/desaceleração, não robótica.
        anguloMovimento = Mathf.SmoothDampAngle(anguloMovimento, anguloAlvo, ref anguloVelocidadeRef, tempoSuavizacaoGiro);
        AtualizarRotacaoVisual();

        float diferenca = Mathf.Abs(Mathf.DeltaAngle(anguloMovimento, anguloAlvo));
        if (diferenca <= anguloParaMergulhar)
        {
            direcaoMergulho = DirecaoDoAngulo(anguloMovimento);
            estado = Estado.Mergulhando;
        }
    }

    private void AtualizarMergulho()
    {
        velocidadeAtual = Mathf.Min(velocidadeAtual + aceleracao * Time.deltaTime, velocidadeMaxima);
        transform.position += (Vector3)direcaoMergulho * velocidadeAtual * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagProjetil))
        {
            Destroy(other.gameObject); // remove o projétil também
            Destroy(gameObject);
        }
        else if (other.CompareTag(tagNave))
        {
            // A nave não é destruída aqui — isso fica a cargo do script dela mesma
            // (ex: perder vida). Este inimigo só desaparece ao colidir.
            Destroy(gameObject);
        }
    }
}