using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;
    public int maxEnemiesPerPlatform = 3;

    private float spawnTimer;
    private List<GameObject> platforms = new List<GameObject>();
    private ScoreManager scoreManager;
    private int lastSpawnedPlatformIndex = -1;

    void Start()
    {
        spawnTimer = spawnInterval;
        scoreManager = FindObjectOfType<ScoreManager>();
        platforms.AddRange(GameObject.FindGameObjectsWithTag("Platform"));
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            SpawnEnemies();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnEnemies()
    {
        int spawnIndex = GetNextSpawnIndex();
        if (spawnIndex != -1)
        {
            GameObject spawnPlatform = platforms[spawnIndex];
            int enemiesToSpawn = CalculateEnemiesToSpawn();
            
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemyOnPlatform(spawnPlatform);
            }
            
            lastSpawnedPlatformIndex = spawnIndex;
        }
    }

    int CalculateEnemiesToSpawn()
    {
        int currentScore = scoreManager.score;
        if (currentScore >= 20)
        {
            return 3;
        }
        else if (currentScore >= 10)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    void SpawnEnemyOnPlatform(GameObject platform)
    {
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

    int GetNextSpawnIndex()
    {
        int playerPlatformIndex = GetPlayerPlatformIndex();
        if (playerPlatformIndex == -1 || platforms.Count <= playerPlatformIndex + 2)
        {
            return -1; // Not enough platforms ahead of the player
        }

        int startIndex = Mathf.Max(playerPlatformIndex + 2, lastSpawnedPlatformIndex + 1);
        for (int i = startIndex; i < platforms.Count; i++)
        {
            if (platforms[i] != null)
            {
                return i;
            }
        }

        return -1; // No suitable platform found
    }

    int GetPlayerPlatformIndex()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return -1;

        for (int i = 0; i < platforms.Count; i++)
        {
            if (platforms[i] != null && player.transform.position.x < platforms[i].transform.position.x)
            {
                return i - 1; // Return the index of the platform the player is on or has just passed
            }
        }

        return platforms.Count - 1; // Player is beyond all platforms
    }

    public void AddPlatform(GameObject platform)
    {
        platforms.Add(platform);
    }
}