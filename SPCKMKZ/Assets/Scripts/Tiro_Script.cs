using UnityEngine;

public class Tiro_Script : MonoBehaviour
{
    public float memes;
    public float velocity = 0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector2(0, velocity) * Time.deltaTime);
    }
}
