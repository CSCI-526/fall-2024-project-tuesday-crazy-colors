using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    public GameObject platform;
    public Transform generationPoint;
    public float distanceBetween;
    public float distanceBetweenMin;
    public float distanceBetweenMax;
    private float platformWidth;

   
    public GameObject whitePowerUpPrefab;
    public GameObject blackPowerUpPrefab;
    public int powerUpInterval; 
    private int platformCount = 0;

    // Coin
    public GameObject coinPrefab;

    
    public Color[] platformColors;

   
    public float verticalOffsetRange = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        platformWidth = platform.transform.localScale.x;

        platformColors = new Color[] {
            Color.red,
            Color.yellow,
            Color.green,
        };
    }

    // Update is called once per frame
    void Update()
    {
        distanceBetween = Random.Range(distanceBetweenMin, distanceBetweenMax);

        if (transform.position.x < generationPoint.position.x)
        {
            
            float verticalOffset = Random.Range(-verticalOffsetRange, verticalOffsetRange);
            transform.position = new Vector3(transform.position.x + platformWidth + distanceBetween, transform.position.y + verticalOffset, transform.position.z);

            
            GameObject newPlatform = Instantiate(platform, transform.position, transform.rotation);
            newPlatform.tag = "Platform";

           
            Renderer platformRenderer = newPlatform.GetComponent<Renderer>();
            platformRenderer.material.color = platformColors[Random.Range(0, platformColors.Length)];

            
            newPlatform.AddComponent<PlatformMover>();

            // Power-up spawning logic
            platformCount++;
            if (platformCount >= powerUpInterval)
            {
                platformCount = 0; 

                
                GameObject powerUpToSpawn = Random.value < 0.5f ? whitePowerUpPrefab : blackPowerUpPrefab;
                Instantiate(powerUpToSpawn, newPlatform.transform.position + Vector3.up, Quaternion.identity);

                Debug.Log(powerUpToSpawn == whitePowerUpPrefab ? "Spawned White Power-Up" : "Spawned Black Power-Up");
            } else
            {
                // Coin spawning logic
                Instantiate(coinPrefab, newPlatform.transform.position + Vector3.up * 1.5f, Quaternion.identity);
            }

        }
    }
}

