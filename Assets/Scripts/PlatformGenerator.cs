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
            Color.blue,
            Color.green,
            Color.yellow,
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
        }
    }
}
