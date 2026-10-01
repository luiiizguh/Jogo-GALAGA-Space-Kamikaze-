using UnityEngine;

public class Enemie_Test_Script : MonoBehaviour
{

    public int vida = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceberDano(int dano){

        vida-=dano;
        if (vida <= 0){

            Destroy(gameObject);
        }
        Debug.Log(vida);
    }
}
