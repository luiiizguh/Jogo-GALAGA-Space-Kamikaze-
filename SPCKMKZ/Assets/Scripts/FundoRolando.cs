
using UnityEngine;

// Faz uma imagem de fundo rolar para baixo em loop infinito
public class FundoRolando : MonoBehaviour
{
    [Header("Velocidade")]
    [Tooltip("Velocidade normal do fundo.")]
    public float velocidade = 2f;

    [Header("Transição do boss")]
    [Tooltip("Marcado: acelera durante a transição do boss.")]
    public bool acelerarNaTransicao = true;

    private float alturaImagem;
    private float yInicial;
    private Transform copia;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            Debug.LogError("FundoRolando: falta um SpriteRenderer neste objeto.", this);
            enabled = false;
            return;
        }

        alturaImagem = sr.bounds.size.y;
        yInicial = transform.position.y;

        GameObject obj = new GameObject("FundoCopia");
        obj.transform.SetParent(transform.parent);
        obj.transform.localScale = transform.localScale;

        SpriteRenderer srCopia = obj.AddComponent<SpriteRenderer>();
        srCopia.sprite = sr.sprite;
        srCopia.sortingOrder = sr.sortingOrder;
        srCopia.sortingLayerID = sr.sortingLayerID;
        srCopia.color = sr.color;
        srCopia.sharedMaterial = sr.sharedMaterial;

        copia = obj.transform;
        copia.position = transform.position + Vector3.up * alturaImagem;
    }

    void Update()
    {
        // Usa o multiplicador controlado pelo TransicaoBoss
        float multiplicador = acelerarNaTransicao
            ? TransicaoBoss.MultiplicadorEstrelas
            : 1f;

        // Calcula a velocidade efetiva sem alterar o valor do Inspector
        float velocidadeAtual = velocidade * multiplicador;
        float movimento = velocidadeAtual * Time.deltaTime;

        // Move as duas imagens para baixo
        transform.position += Vector3.down * movimento;
        copia.position += Vector3.down * movimento;

        // Reposiciona a imagem original quando ela sai da tela
        if (transform.position.y <= yInicial - alturaImagem)
        {
            transform.position += Vector3.up * alturaImagem * 2f;
        }

        // Reposiciona a cópia quando ela sai da tela
        if (copia.position.y <= yInicial - alturaImagem)
        {
            copia.position += Vector3.up * alturaImagem * 2f;
        }
    }

    void OnDestroy()
    {
        // Remove a cópia criada pelo script
        if (copia != null)
        {
            Destroy(copia.gameObject);
        }
    }
}
