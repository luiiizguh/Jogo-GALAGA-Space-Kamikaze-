using UnityEngine;

/// <summary>
/// Controla o inimigo "dois em um": núcleo + escudo.
/// Enquanto o escudo estiver ativo, o núcleo não recebe dano.
/// Anexar este script no GameObject pai "InimigoEscudo".
/// </summary>
public class InimigoEscudo : MonoBehaviour
{
    [Header("Vida")]
    public int vidaNucleo = 3;
    public int vidaEscudo = 5;

    [Header("Referências")]
    [Tooltip("Arraste aqui o GameObject filho do escudo.")]
    public GameObject escudoObjeto;

    [Header("Eventos (opcional)")]
    public GameObject efeitoDestruirEscudo;
    public GameObject efeitoDestruirNucleo;

    private bool escudoAtivo = true;

    void Start()
    {
        escudoAtivo = escudoObjeto != null && escudoObjeto.activeSelf;
    }

    public bool EscudoEstaAtivo()
    {
        return escudoAtivo;
    }

    public void ReceberDanoEscudo(int dano)
    {
        if (!escudoAtivo) return;

        vidaEscudo -= dano;
        if (vidaEscudo <= 0)
        {
            escudoAtivo = false;

            if (efeitoDestruirEscudo != null)
                Instantiate(efeitoDestruirEscudo, escudoObjeto.transform.position, Quaternion.identity);

            if (escudoObjeto != null)
                escudoObjeto.SetActive(false);
        }
    }

}