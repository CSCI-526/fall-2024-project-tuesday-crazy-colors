using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    private PlayerController player;

    void Start()
    {
        Destroy(gameObject, lifetime);
        player = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            player.OnEnemyKilled();
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Platform"))
        {
            Destroy(gameObject);
        }
    }
}