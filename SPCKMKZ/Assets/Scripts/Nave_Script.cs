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


    // ==========================================
    // DROPS (VIDA E HABILIDADE)
    // ==========================================

    [Header("Drops")]

    // Máximo de vidas que a nave pode ter
    public int vidaMaxima = 8;

    // Quanto tempo dura a habilidade de tiro rápido (segundos)
    public float duracaoTiroRapido = 8f;

    // 0.5 = o cooldown cai pela metade (atira 2x mais rápido)
    [Range(0.1f, 1f)]
    public float multiplicadorTiroRapido = 0.5f;

    // Tempo restante da habilidade
    private float tiroRapidoTimer = 0f;


    // ==========================================
    // SOM DO TIRO
    // ==========================================

    // Componente AudioSource que vai tocar o som
    public AudioSource audioSource;

    // Som do tiro
    public AudioClip somTiro;


    // Start é chamado quando o jogo começa
    void Start()
    {
        // Mostra as vidas iniciais no HUD
        if (gameManager != null)
            gameManager.AtualizarVidas(life);
    }


    // Update é chamado uma vez por frame
    void Update()
    {
        // ==========================================
        // CONTROLE DA HABILIDADE DE TIRO RÁPIDO
        // ==========================================

        if (tiroRapidoTimer > 0f)
        {
            tiroRapidoTimer -= Time.deltaTime;
        }


        // Se apertar X, dispara
        if (Input.GetKey(KeyCode.X))
        {
            Shoot();
        }

        // Movimentação da nave
        Move();


        // ==========================================
        // CONTROLE DO COOLDOWN DO TIRO
        // ==========================================

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }


        // ==========================================
        // CONTROLE DA VULNERABILIDADE
        // ==========================================

        if (!vulnerable)
        {
            vulnerable_timer += Time.deltaTime;

            if (vulnerable_timer >= vulnerable_cooldown)
            {
                vulnerable = true;
            }
        }
        else
        {
            sprite.GetComponent<SpriteRenderer>().enabled = true;
        }


        // ==========================================
        // EFEITO DE PISCAR QUANDO LEVA DANO
        // ==========================================

        if (flash_timer > .1 && !vulnerable)
        {
            sprite.GetComponent<SpriteRenderer>().enabled =
                !sprite.GetComponent<SpriteRenderer>().enabled;

            flash_timer = 0;
        }
        else
        {
            flash_timer += Time.deltaTime;
        }
    }


    // ==========================================
    // MOVIMENTAÇÃO DA NAVE
    // ==========================================

    void Move()
    {
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


    // ==========================================
    // DISPARO
    // ==========================================

    void Shoot()
    {
        // Verifica se o cooldown terminou
        if (timer <= 0)
        {

            // ==========================================
            // TOCA O SOM DO TIRO
            // ==========================================

            if (audioSource != null && somTiro != null)
            {
                audioSource.PlayOneShot(somTiro);
            }


            // ==========================================
            // DISPARO INTERCALADO
            // ==========================================

            if (Intercalate)
            {
                GameObject spawn = spawn_tiro[cur_spawn];

                Instantiate(
                    shoot_prefab,
                    spawn.transform.position,
                    spawn.transform.rotation
                );

                Instantiate(
                    spawn_efx,
                    spawn.transform.position,
                    spawn.transform.rotation,
                    spawn.transform
                );


                cur_spawn++;


                if (cur_spawn >= spawn_tiro.Count)
                {
                    cur_spawn = 0;
                }
            }


            // ==========================================
            // DISPARO DOS DOIS TIROS
            // ==========================================

            else
            {
                foreach (var spawn in spawn_tiro)
                {
                    Instantiate(
                        shoot_prefab,
                        spawn.transform.position,
                        spawn.transform.rotation
                    );

                    Instantiate(
                        spawn_efx,
                        spawn.transform.position,
                        spawn.transform.rotation,
                        spawn.transform
                    );
                }
            }


            // Reinicia o cooldown (mais curto se o tiro rápido estiver ativo)
            timer = tiroRapidoTimer > 0f
                ? shoot_cooldown * multiplicadorTiroRapido
                : shoot_cooldown;
        }
    }

<<<<<<< HEAD

    // ==========================================
    // COLISÃO COM INIMIGO
    // ==========================================

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Inimigo") && vulnerable == true)
        {
=======
    private void OnTriggerEnter2D(Collider2D col){
        
        if ((col.gameObject.CompareTag("Inimigo") || col.gameObject.CompareTag("Boss")) && vulnerable == true){
>>>>>>> origin/Tales_Branch
            Debug.Log("Memes");


            vulnerable = false;

            vulnerable_timer = 0;

            life--;
<<<<<<< HEAD

            // Atualiza as vidas no HUD
            if (gameManager != null)
                gameManager.AtualizarVidas(life);
=======
            SistemaVidas.Instance.PerderVida();
            if (life <= 0){
>>>>>>> origin/Tales_Branch


            // ==========================================
            // GAME OVER
            // ==========================================

            if (life <= 0)
            {
                gameManager.GameOver();
            }
        }
    }


    // ==========================================
    // DROPS: CHAMADOS PELO SCRIPT Drop
    // ==========================================

    // Ganha 1 vida (até o máximo)
    public void GanharVida()
    {
        life = Mathf.Min(life + 1, vidaMaxima);

        if (gameManager != null)
            gameManager.AtualizarVidas(life);
    }

    // Ativa o tiro rápido (pegar outro renova o tempo)
    public void GanharHabilidade()
    {
        tiroRapidoTimer = duracaoTiroRapido;
    }
}