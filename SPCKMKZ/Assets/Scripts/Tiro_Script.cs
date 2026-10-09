using UnityEngine;

public class Tiro_Script : MonoBehaviour
{
    public float damage;
    public float velocity = 0.2f;
    public GameObject destroy_prefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector2(0, velocity) * Time.deltaTime);
        if (transform.position.y > 1.5){

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col){


        if (col.gameObject.CompareTag("Inimigo") || col.gameObject.CompareTag("Boss")){

            Vector2 col_point = col.ClosestPoint(transform.position);
            Enemie_Test_Script inimigo = col.GetComponent<Enemie_Test_Script>();
            Instantiate(destroy_prefab, col_point, Quaternion.identity);
            inimigo.ReceberDano(damage);
            Destroy(gameObject);
        }
    }
}
