using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhitePowerUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Debug.Log("Power Up!");
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TemporaryPowerUpEffect(10f);  // 10 seconds of invincibility and semi-transparent effect
                Destroy(gameObject);
            }
        }
    }
}
