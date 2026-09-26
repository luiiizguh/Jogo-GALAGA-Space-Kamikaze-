using System.Collections.Generic;
using UnityEngine;

public class Nave_Script : MonoBehaviour
{
    public int life = 5;
    public float shoot_damage = 5f;
    public int shoot_cooldown = 30;
    public float velocity = .5f;
    private int timer = 0;
    public GameObject shoot_prefab; 
    private int current_spawn = 0;
    public List<GameObject> spawn_tiro;
    public GameObject spawn_efx;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.X)){

            Shoot();
        }
        Move();

        if (timer > 0){

            timer--;
        }
    }

    void Move(){

        float move_x = Input.GetAxisRaw("Horizontal");

        Vector2 move = new Vector2(move_x, 0);

        transform.Translate(move * velocity * Time.deltaTime);
    }
    void Shoot(){

        if (timer <= 0){

            foreach(var spawn in spawn_tiro){

                Instantiate(shoot_prefab, spawn.transform.position, spawn.transform.rotation);
                Instantiate(spawn_efx, spawn.transform.position, spawn.transform.rotation, spawn.transform);
            }
            
            timer = shoot_cooldown;
        }
    }
}
