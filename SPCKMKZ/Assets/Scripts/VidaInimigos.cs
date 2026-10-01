using UnityEngine;

public class VidaInimigos : MonoBehaviour
{
    public int vida = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        
    }
   
public void ReceberDano(int dano)
    {
        vida -= dano;
        if (vida <= 0)
        {
            Destroy(gameObject);
        }
    }
}
