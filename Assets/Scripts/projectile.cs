using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    private PlayerController player;
    private Vector2 direction;

    void Start()
    {
        Destroy(gameObject, lifetime);
        player = FindObjectOfType<PlayerController>();
        direction = transform.right;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (player != null)
            {
                player.OnEnemyKilled();
            }
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Platform"))
        {
            Destroy(gameObject);
        }
    }
}