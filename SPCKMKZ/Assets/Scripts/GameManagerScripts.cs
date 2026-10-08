using Unity.VisualScripting;
using UnityEngine;

public class GameManagerScripts : MonoBehaviour
{
    public static GameManagerScripts Instance; 
    
    public int naveSelecionada;

    void Awake()
    {
        if(Instance == null )
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
