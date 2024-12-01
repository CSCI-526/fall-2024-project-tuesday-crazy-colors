

using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float initialSpawnInterval = 3f;
    public int baseEnemiesPerSpawn = 1;
    public float enemyIncreaseInterval = 10f; // Increase enemy count every 10 points
    public int maxEnemiesPerSpawn = 5;

    private float spawnTimer;
    private List<GameObject> platforms = new List<GameObject>();
    private ScoreManager scoreManager;
    private int lastSpawnedPlatformIndex = -1;
    private float currentSpawnInterval;

    void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        spawnTimer = currentSpawnInterval;
        scoreManager = FindObjectOfType<ScoreManager>();
        platforms.AddRange(GameObject.FindGameObjectsWithTag("Platform"));
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnEnemies();
            spawnTimer = currentSpawnInterval;
        }

        // Gradually decrease spawn interval with a limit
        currentSpawnInterval = Mathf.Max(initialSpawnInterval * 0.75f, initialSpawnInterval - (scoreManager.score * 0.005f));
    }

    void SpawnEnemies()
    {
        int spawnIndex = GetNextSpawnIndex();
        if (spawnIndex != -1)
        {
            GameObject spawnPlatform = platforms[spawnIndex];
            SpawnEnemyOnPlatform(spawnPlatform);
            lastSpawnedPlatformIndex = spawnIndex;
        }
        else
        {
            Debug.Log("No valid platform for spawning enemies.");
        }
        // int spawnIndex = GetNextSpawnIndex();
        // int spawnIndex = GetNextSpawnIndex();
        // if (spawnIndex != -1)
        // {
        //     GameObject spawnPlatform = platforms[spawnIndex];
        //     int enemiesToSpawn = CalculateEnemiesToSpawn();
        //     for (int i = 0; i < enemiesToSpawn; i++)
        //     {
        //         SpawnEnemyOnPlatform(spawnPlatform);
        //     }
        //     lastSpawnedPlatformIndex = spawnIndex;
        // }
        // else
        // {
        //     Debug.Log("No valid platform for spawning enemies.");
        // }

    }

    int CalculateEnemiesToSpawn()
    {
        int currentScore = scoreManager.score;
        int additionalEnemies = Mathf.FloorToInt(currentScore / enemyIncreaseInterval);
        return Mathf.Min(baseEnemiesPerSpawn + additionalEnemies, maxEnemiesPerSpawn);
    }

    void SpawnEnemyOnPlatform(GameObject platform)
    {
        // Check if there's already an enemy on the platform
        if (platform.transform.childCount > 0)
        {
            return; // Skip spawning if there's already an enemy
        }
        PlatformMover platformMover = platform.GetComponent<PlatformMover>();
        if (platformMover != null && platformMover.GetBehavior() == PlatformMover.PlatformBehavior.SeeSaw)
        {
            return;
        }

        Vector3 platformSize = platform.GetComponent<Renderer>().bounds.size;
        float randomX = Random.Range(-platformSize.x / 2 + 0.5f, platformSize.x / 2 - 0.5f);
        Vector3 spawnPosition = platform.transform.position + new Vector3(randomX, platformSize.y / 2 + 0.5f, 0);
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemy.transform.SetParent(platform.transform);

        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.SetMovementBounds(-platformSize.x / 2 + 0.5f, platformSize.x / 2 - 0.5f);
            enemyMovement.speed = Random.Range(1f, 4f);
        }
    }
    float spawnDistance = 10f;

    int GetNextSpawnIndex()
    {
        // Predefined platform indices for spawning
        List<int> predefinedPlatforms = new List<int> { 10, 15, 20, 23, 25, 27, 30 };

        // Get the player's current platform index
        int playerPlatformIndex = GetPlayerPlatformIndex();

        // Start from the first predefined platform after the player's current platform
        int startIndex = 0;
        for (int i = 0; i < predefinedPlatforms.Count; i++)
        {
            if (predefinedPlatforms[i] > playerPlatformIndex)
            {
                startIndex = i;
                break;
            }
        }
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return -1;

        for (int i = startIndex; i < predefinedPlatforms.Count; i++)
        {
            int platformIndex = predefinedPlatforms[i];

            // Validate platform existence, validity, and if it's after the player's current platform
            if (platformIndex >= platforms.Count || platforms[platformIndex] == null || platforms[platformIndex].transform.position.x < player.transform.position.x + spawnDistance)
            {
                continue;
            }

            // PlatformMover platformMover = platforms[platformIndex].GetComponent<PlatformMover>();
            // if (platformMover != null && platformMover.GetBehavior() == PlatformMover.PlatformBehavior.SeeSaw)
            // {
            //     // If the current platform is a See-Saw, skip it and try the next one
            //     i++;
            // }
            // else
            // {
            //     return platformIndex;
            // }
            PlatformMover platformMover = platforms[platformIndex].GetComponent<PlatformMover>();
            if (platformMover == null || platformMover.GetBehavior() != PlatformMover.PlatformBehavior.SeeSaw)
            {
                Debug.Log("Spawning enemy on platform: " + platformIndex);
                return platformIndex;
            }
        }

        // After the 30th platform, spawn on every other platform

        for (int i = 31; i < platforms.Count; i += 2)
        {
            if (i > playerPlatformIndex)
            {
                if (platforms[i] != null)
                {
                    // Check if the platform is a See-Saw
                    PlatformMover platformMover = platforms[i].GetComponent<PlatformMover>();
                    if (platformMover == null || platformMover.GetBehavior() != PlatformMover.PlatformBehavior.SeeSaw)
                    {
                        // If the current platform is not a See-Saw, return its index
                        return i;
                    }
                    else
                    {
                        // If the current platform is a See-Saw, check the next one
                        int nextPlatformIndex = i + 2;
                        if (nextPlatformIndex < platforms.Count && platforms[nextPlatformIndex] != null)
                        {
                            PlatformMover nextPlatformMover = platforms[nextPlatformIndex].GetComponent<PlatformMover>();
                            if (nextPlatformMover == null || nextPlatformMover.GetBehavior() != PlatformMover.PlatformBehavior.SeeSaw)
                            {
                                return nextPlatformIndex;
                            }
                        }
                    }
                }
            }
        }
        // for (int i = 31; i < platforms.Count; i += 2)
        // {
        //     if (i > playerPlatformIndex)
        //     {
        //         if (platforms[i] != null)
        //         {
        //             PlatformMover platformMover = platforms[i].GetComponent<PlatformMover>();
        //             if (platformMover == null || platformMover.GetBehavior() != PlatformMover.PlatformBehavior.SeeSaw)
        //             {
        //                 Debug.Log("Spawning enemy on platform: " + i);
        //                 return i;
        //             }
        //         }
        //     }
        // }

        Debug.Log("No valid platform found for spawning.");
        return -1;
    }

    int GetPlayerPlatformIndex()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return -1;

        for (int i = 0; i < platforms.Count; i++)
        {
            if (platforms[i] != null && player.transform.position.x < platforms[i].transform.position.x)
            {
                return Mathf.Max(0, i - 1); // Ensure valid index
            }
        }

        return platforms.Count - 1;
    }

    public void AddPlatform(GameObject platform)
    {
        platforms.Add(platform);
    }
}
