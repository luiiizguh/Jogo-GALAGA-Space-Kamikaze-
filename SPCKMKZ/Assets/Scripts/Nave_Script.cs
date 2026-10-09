using System.Collections.Generic;
using UnityEngine;

public class Nave_Script : MonoBehaviour
{
    public int life = 5;
    public float shoot_damage = 5f;
    public float shoot_cooldown = 30f;
    public float velocity = .5f;
    private float timer = 0f;
    public GameObject shoot_prefab; 
    private int current_spawn = 0;
    public List<GameObject> spawn_tiro;
    public GameObject spawn_efx;
    public bool Intercalate = false;
    private int cur_spawn = 0;
    private bool vulnerable = true;
    private int vulnerable_cooldown = 1;
    private float vulnerable_timer = 0f;
    private float flash_timer = 0f;
    public GameObject sprite;
    public GerenciadorFases gameManager;

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

            timer-=Time.deltaTime;
        }
        if (!vulnerable){

            vulnerable_timer+=Time.deltaTime;
            if (vulnerable_timer >= vulnerable_cooldown){

                vulnerable = true;
            }
        }else{

            sprite.GetComponent<SpriteRenderer>().enabled = true;
        }
        if (flash_timer > .1 && !vulnerable){
            
            sprite.GetComponent<SpriteRenderer>().enabled = !sprite.GetComponent<SpriteRenderer>().enabled;
            flash_timer = 0;
        }else {flash_timer+=Time.deltaTime;}
    }

    void Move(){

        float move_x = Input.GetAxisRaw("Horizontal");
        float move_y = Input.GetAxisRaw("Vertical");
        float xlimit = 1.4f;
        float ylimit = 0.9f;

        Vector2 move = new Vector2(move_x, move_y);

        transform.Translate(move * velocity * Time.deltaTime);
        transform.position = new Vector3(
            
            Mathf.Clamp(transform.position.x, -xlimit, xlimit),
            Mathf.Clamp(transform.position.y, -ylimit, ylimit),
            transform.position.z
        );
    }
    void Shoot(){

        if (timer <= 0){
            
            if (Intercalate){

                GameObject spawn = spawn_tiro[cur_spawn];
                Instantiate(shoot_prefab, spawn.transform.position, spawn.transform.rotation);
                Instantiate(spawn_efx, spawn.transform.position, spawn.transform.rotation, spawn.transform);
                cur_spawn++;
                if (cur_spawn >= spawn_tiro.Count){

                    cur_spawn = 0;
                }
            }else{

                foreach(var spawn in spawn_tiro){

                    Instantiate(shoot_prefab, spawn.transform.position, spawn.transform.rotation);
                    Instantiate(spawn_efx, spawn.transform.position, spawn.transform.rotation, spawn.transform);
                }
            }
            timer = shoot_cooldown;
        }
    }

    private void OnTriggerEnter2D(Collider2D col){
        
        if (col.gameObject.CompareTag("Inimigo") && vulnerable == true){
            Debug.Log("Memes");
            vulnerable = false;
            vulnerable_timer = 0;
            life--;
            SistemaVidas.Instance.PerderVida();
            if (life <= 0){

                gameManager.GameOver();
            }
        }
    }
}
