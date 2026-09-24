using UnityEngine;

public class Nave_Script : MonoBehaviour
{
    public int life = 5;
    public float shoot_damage = 5f;
    public float velocity = .5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move(){

        float move_x = Input.GetAxisRaw("Horizontal");
        float move_y = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(move_x, move_y).normalized;

        transform.Translate(move * velocity * Time.deltaTime);
    }
}
