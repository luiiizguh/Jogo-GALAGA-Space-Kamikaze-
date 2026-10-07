using UnityEngine;

// Clique direito no Project > Create > Dialogo > Dados do Dialogo
[CreateAssetMenu(fileName = "NovoDialogo", menuName = "Dialogo/Dados do Dialogo")]
public class DialogueData : ScriptableObject
{
    [TextArea(2, 5)]
    public string[] falas;

    public Sprite imagemA;
    public Sprite imagemB;

    [Tooltip("Cena carregada quando o diálogo terminar (precisa estar no Build Settings)")]
    public string proximaCena;
}
