using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhitePowerUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Debug.Log("Power Up!");
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            Debug.Log(player);
            if (player != null)
            {
                player.TemporaryColorChange(Color.white, 10f);
                Destroy(gameObject);
            }
        }
    }
}
