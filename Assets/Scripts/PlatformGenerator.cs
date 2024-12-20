using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Make sure this is included

public class PlatformGenerator : MonoBehaviour
{
    public GameObject jumpPadPrefab; // Assign this in the Inspector
    public float jumpPadThreshold = 11f; // Distance threshold to add jump pad
    private GameObject lastPlatform;
    private int platformCount = 0;
    private int platformPhase = 0;
    public GameObject seesawIndicatorPrefab;
    public bool enableCoins = false;
    public GameObject platform;
    public Transform generationPoint;
    public float distanceBetween;
    public float distanceBetweenMin;
    public float distanceBetweenMax;
    private float platformWidth;

    public GameObject whitePowerUpPrefab;
    public int powerUpInterval; 

     // Coin
    public GameObject coinPrefab;

    public TMP_Text powerUpLabel; // Ensure this is a TMP_Text

    public Color[] platformColors;
    public float verticalOffsetRange = 10.0f;

    // Flags to check if power-ups have been shown
    private bool whitePowerUpShown = false;
    private EnemySpawner enemySpawner;

    // BG color change
    
    public Color[] darkBackgroundColors;  
    
    private int platformsGenerated = 0; 

    // Start is called before the first frame update

    void Start()
    {
        platformWidth = platform.transform.localScale.x;
        platformColors = new Color[] { Color.red, Color.yellow, Color.green };
        enemySpawner = FindObjectOfType<EnemySpawner>();
    
         darkBackgroundColors = new Color[] { 
            new Color(0.1f, 0.1f, 0.2f), // Very dark navy
            new Color(0.15f, 0.1f, 0.2f), // Dark purple
            new Color(0.2f, 0.1f, 0.15f), // Dark burgundy
            new Color(0.1f, 0.2f, 0.1f), // Dark forest green
            new Color(0.2f, 0.15f, 0.1f), // Dark brownish-gray
            new Color(0.15f, 0.2f, 0.25f), // Dark teal
            new Color(0.1f, 0.1f, 0.1f), // Very dark gray
            new Color(0.2f, 0.15f, 0.2f), // Dark slate gray
            new Color(0.2f, 0.2f, 0.1f), // Dark olive green
            new Color(0.2f, 0.1f, 0.1f), // Dark red
            new Color(0.1f, 0.2f, 0.2f), // Dark cyan
            new Color(0.15f, 0.1f, 0.15f), // Dark magenta
            new Color(0.1f, 0.1f, 0.15f), // Dark charcoal
            new Color(0.15f, 0.2f, 0.15f), // Dark emerald
            new Color(0.1f, 0.1f, 0.2f), // Deep indigo
            new Color(0.2f, 0.2f, 0.2f)
        };
   
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
            
            PlatformMover platformMover = newPlatform.AddComponent<PlatformMover>();

            // Assign behavior based on the current phase
            AssignPlatformBehavior(platformMover);

            platformCount++;
            platformsGenerated++;

            if (lastPlatform != null && distanceBetween >= jumpPadThreshold)
            {
                AddJumpPad(lastPlatform);
            }

            lastPlatform = newPlatform;

            // Background color change logic
            if (platformsGenerated == 10)
            {
                Color newBackgroundColor = darkBackgroundColors[Random.Range(0, darkBackgroundColors.Length)];
                Camera.main.backgroundColor = newBackgroundColor;
                Debug.Log("Background color changed to: " + newBackgroundColor);
                platformsGenerated = 0;
            }

            // Power-up generation logic
            if (platformCount >= powerUpInterval)
            {
                platformCount = 0;
                GameObject powerUpToSpawn = whitePowerUpPrefab;
                Instantiate(powerUpToSpawn, newPlatform.transform.position + Vector3.up, Quaternion.identity);

                // Display power-up text
                if (powerUpToSpawn == whitePowerUpPrefab && !whitePowerUpShown)
                {
                    // powerUpLabel.text = "White Power-Up Ahead!\nCollect to Land freely without changing color!";
                    whitePowerUpShown = true;
                    StartCoroutine(ResetLabel());
                }
            }

            if (enemySpawner != null)
            {
                enemySpawner.AddPlatform(newPlatform);
            }

            // Add seesaw indicator if applicable
            if (platformMover.GetBehavior() == PlatformMover.PlatformBehavior.SeeSaw)
            {
                AddSeesawIndicator(newPlatform);
            }

            // Update the platform phase
            UpdatePlatformPhase();
        }
    }

    void AddJumpPad(GameObject platform)
    {
        // Calculate position for jump pad (at the end of the platform)
        Vector3 jumpPadPosition = platform.transform.position;
        jumpPadPosition.x += platform.transform.localScale.x / 2; // Move to the end of the platform
        jumpPadPosition.y += 0.0f; // Adjust height as needed

        // Instantiate jump pad
        GameObject jumpPad = Instantiate(jumpPadPrefab, jumpPadPosition, Quaternion.identity);
        jumpPad.transform.SetParent(platform.transform); // Parent to platform for easier management
    }

private void AssignPlatformBehavior(PlatformMover platformMover)
{
    switch (platformPhase)
    {
        case 0: // Static phase
            platformMover.SetBehavior(PlatformMover.PlatformBehavior.Static);
            break;
        case 1: // Static + Vertical movement + ShrinkAndGrowHorizontally phase
            int randomBehavior = Random.Range(0, 3);
            switch (randomBehavior)
            {
                case 0:
                    platformMover.SetBehavior(PlatformMover.PlatformBehavior.Static);
                    break;
                case 1:
                    platformMover.SetBehavior(PlatformMover.PlatformBehavior.MoveVertically);
                    break;
                case 2:
                    platformMover.SetBehavior(PlatformMover.PlatformBehavior.ShrinkAndGrowHorizontally);
                    break;
            }
            break;
        case 2: // All types combined (including SeeSaw)
            randomBehavior = Random.Range(0, 4);
            platformMover.SetBehavior((PlatformMover.PlatformBehavior)randomBehavior);
            break;
        default: // All types combined (including SeeSaw)
            randomBehavior = Random.Range(0, 4);
            platformMover.SetBehavior((PlatformMover.PlatformBehavior)randomBehavior);
            break;
    }
}

private void UpdatePlatformPhase()
{
    if (platformCount % 4 == 0)
    {
        platformPhase++;
        if (platformPhase > 2)
        {
            platformPhase = 3; // Stay in the "all types" phase
        }
    }
}

    private void AddSeesawIndicator(GameObject platform)
    {
        GameObject indicator = Instantiate(seesawIndicatorPrefab, platform.transform);
        indicator.transform.localPosition = new Vector3(0f, -1.39f, 0.1f);
        indicator.transform.localScale = new Vector3(0.00667f, 0.1f, 0.1f) * platform.transform.localScale.x;
    }

    private IEnumerator ResetLabel()
    {
        yield return new WaitForSeconds(5f);
        powerUpLabel.text = "";
    }
}