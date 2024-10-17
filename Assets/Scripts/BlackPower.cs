
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackPower : MonoBehaviour
{
    public float duration = 10f; 
    private PlayerController playerController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ActivateShadowImmunity(duration); 
                Destroy(gameObject); 
            }
        }
    }
}


