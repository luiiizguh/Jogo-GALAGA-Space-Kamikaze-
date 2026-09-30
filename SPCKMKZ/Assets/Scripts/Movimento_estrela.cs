using UnityEngine;

// Faz uma imagem de fundo rolar para baixo em loop infinito
public class FundoRolando : MonoBehaviour
{
    // Velocidade da rolagem (maior = mais rápido)
    public float velocidade = 2f;

    // Altura da imagem no mundo do jogo
    private float alturaImagem;
    // Posição Y onde a imagem original começou
    private float yInicial;
    // A segunda cópia da imagem, que fica logo acima da original
    private Transform copia;

    void Start()
    {
        // Pega o componente que desenha a imagem neste objeto
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        // Mede a altura da imagem já considerando a escala (bounds = tamanho no mundo)
        alturaImagem = sr.bounds.size.y;
        // Guarda onde a imagem começou
        yInicial = transform.position.y;

        // Cria um objeto novo, que será a cópia da imagem
        GameObject obj = new GameObject("FundoCopia");
        // Coloca a cópia no mesmo "pai" da original, para manter a Hierarchy organizada
        obj.transform.SetParent(transform.parent);
        // Dá à cópia a mesma escala da original
        obj.transform.localScale = transform.localScale;

        // Adiciona à cópia o componente que desenha imagens
        SpriteRenderer srCopia = obj.AddComponent<SpriteRenderer>();
        // Usa a mesma imagem
        srCopia.sprite = sr.sprite;
        // Usa a mesma ordem de desenho (atrás dos outros objetos)
        srCopia.sortingOrder = sr.sortingOrder;
        // Usa a mesma cor
        srCopia.color = sr.color;

        // Guarda o Transform da cópia para mexer nele no Update
        copia = obj.transform;
        // Posiciona a cópia exatamente em cima da original (uma altura acima)
        copia.position = transform.position + Vector3.up * alturaImagem;
    }

    void Update()
    {
        // Quanto as imagens andam neste frame (Time.deltaTime deixa igual em qualquer PC)
        float movimento = velocidade * Time.deltaTime;

        // Move a original para baixo
        transform.position += Vector3.down * movimento;
        // Move a cópia para baixo junto
        copia.position += Vector3.down * movimento;

        // Se a original já desceu uma altura inteira, ela saiu da tela
        if (transform.position.y <= yInicial - alturaImagem)
        {
            // Manda ela para cima da cópia (duas alturas acima)
            transform.position += Vector3.up * alturaImagem * 2f;
        }

        // Mesma verificação para a cópia
        if (copia.position.y <= yInicial - alturaImagem)
        {
            // Manda ela para cima da original
            copia.position += Vector3.up * alturaImagem * 2f;
        }
    }
}