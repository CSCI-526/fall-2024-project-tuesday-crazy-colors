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

    // Power up
    public GameObject powerUpPrefab;
    public int powerUpInterval;
    private int platformCount = 0;

    // Colors
    public Color[] platformColors;

    // Select vertical offset range
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
            // Vertical offset
            float verticalOffset = Random.Range(-verticalOffsetRange, verticalOffsetRange);

            // New platform vertically positioned
            transform.position = new Vector3(transform.position.x + platformWidth + distanceBetween, transform.position.y + verticalOffset, transform.position.z);

            // Instantiate the platform
            GameObject newPlatform = Instantiate(platform, transform.position, transform.rotation);

            newPlatform.tag = "Platform";

            // Random colors for platform
            Renderer platformRenderer = newPlatform.GetComponent<Renderer>();
            platformRenderer.material.color = platformColors[Random.Range(0, platformColors.Length)];

            // Moving platform script
            newPlatform.AddComponent<PlatformMover>(); 

            // Spawning white power-up
            platformCount++;
            if (platformCount >= powerUpInterval)
            {
                Instantiate(powerUpPrefab, newPlatform.transform.position + Vector3.up, Quaternion.identity);
                platformCount = 0;
            }
        }
    }
}
