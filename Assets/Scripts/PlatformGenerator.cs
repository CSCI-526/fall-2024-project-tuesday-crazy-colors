using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Make sure this is included

public class PlatformGenerator : MonoBehaviour
{
    public bool enableCoins = false;
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

    public TMP_Text powerUpLabel; // Ensure this is a TMP_Text

    public Color[] platformColors;
    public float verticalOffsetRange = 10.0f;

    // Flags to check if power-ups have been shown
    private bool whitePowerUpShown = false;
    private bool blackPowerUpShown = false;

    // Start is called before the first frame update

    void Start()
    {
        platformWidth = platform.transform.localScale.x;
        platformColors = new Color[] { Color.red, Color.yellow, Color.green };

        // Initialize label as empty
        powerUpLabel.text = "";
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

            platformCount++;
            if (PlayerPrefs.GetInt("coins", 0) > 20 && platformCount >= powerUpInterval)
            {
                platformCount = 0; 
                GameObject powerUpToSpawn = Random.value < 0.5f ? whitePowerUpPrefab : blackPowerUpPrefab;
                Instantiate(powerUpToSpawn, newPlatform.transform.position + Vector3.up, Quaternion.identity);

                // Display the text for white power-up only if it's the first occurrence
                if (powerUpToSpawn == whitePowerUpPrefab && !whitePowerUpShown)
                {
                    powerUpLabel.text = "White Power-Up Ahead!\nCollect to Land freely without changing color!";
                    whitePowerUpShown = true; // Mark as shown
                    StartCoroutine(ResetLabel());
                }
                // Display the text for black power-up only if it's the first occurrence
                else if (powerUpToSpawn == blackPowerUpPrefab && !blackPowerUpShown)
                {
                    powerUpLabel.text = "Black Power-Up Ahead!\nCollect to get protection from the shadow's grasp!";
                    blackPowerUpShown = true; // Mark as shown
                    StartCoroutine(ResetLabel());
                }
            }
            else
            {
                if (enableCoins)
                {
                    Instantiate(coinPrefab, newPlatform.transform.position + Vector3.up * 1.5f, Quaternion.identity);
                }
            }
        }
    }

    private IEnumerator ResetLabel()
    {
        yield return new WaitForSeconds(5f); 
        powerUpLabel.text = ""; // Clear the label after the delay
    }
}
