using UnityEngine.SceneManagement;

public static class DialogueLoader
{
    public static DialogueData Atual { get; private set; }

    // Chame isso no fim da fase: DialogueLoader.Iniciar(meuDialogo);
    public static void Iniciar(DialogueData dados, string cenaDialogo = "Dialogo")
    {
        Atual = dados;
        SceneManager.LoadScene(cenaDialogo);
    }

    public static void Limpar()
    {
        Atual = null;
    }
}
