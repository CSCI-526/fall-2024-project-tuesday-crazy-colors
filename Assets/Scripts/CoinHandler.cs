using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinHandler : MonoBehaviour
{
    public float rotationSpeed = 100f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Rotating the coin along the Y-axis
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.CollectCoin();
            }
            Destroy(gameObject);
        }
    }
}

public class DisableCoins : MonoBehaviour
{
    void Start()
    {
        // Finds all objects with the CoinHandler component and disables them
        CoinHandler[] coins = FindObjectsOfType<CoinHandler>();
        foreach (CoinHandler coin in coins)
        {
            coin.gameObject.SetActive(false); // Disables the entire coin GameObject
        }
    }
}