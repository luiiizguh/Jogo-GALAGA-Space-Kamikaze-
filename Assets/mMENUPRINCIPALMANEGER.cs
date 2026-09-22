using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Configurações de Telas e Painéis")]
    // Mudado para GameObject para que você possa ativar/desativar a tela de naves na interface
    [SerializeField] private GameObject menuNave;
    [SerializeField] private GameObject painelMenuInicial;

    [Header("Configurações de Transição de Cena")]
    // Mantido como string porque armazena o nome da fase do jogo que vai carregar
    [SerializeField] private string nomeDoLevelDeJogo;

    // Função para iniciar o jogo carregando a cena da fase
    public void Jogar()
    {
        SceneManager.LoadScene(nomeDoLevelDeJogo);
    }
    
    // Função para abrir o menu de naves e esconder o menu principal
    public void Naves()
    {
        painelMenuInicial.SetActive(false);
        menuNave.SetActive(true);
    }

    // Função para fechar o menu de naves e voltar para o menu principal
    public void FecharNaves()
    {
        painelMenuInicial.SetActive(true);
        menuNave.SetActive(false);
    }

    // Função para fechar o jogo
    public void Sair()
    {
        Debug.Log("Sair do Jogo");
        Application.Quit(); // Adicionado os parênteses () que faltavam
    }
}
