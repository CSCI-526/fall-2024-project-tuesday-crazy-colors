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

        // Gradually decrease spawn interval
        currentSpawnInterval = Mathf.Max(initialSpawnInterval * 0.5f, initialSpawnInterval - (scoreManager.score * 0.01f));
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
        int additionalEnemies = Mathf.FloorToInt(currentScore / enemyIncreaseInterval);
        return Mathf.Min(baseEnemiesPerSpawn + additionalEnemies, maxEnemiesPerSpawn);
    }

    void SpawnEnemyOnPlatform(GameObject platform)
    {
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

    int GetNextSpawnIndex()
    {
        int playerPlatformIndex = GetPlayerPlatformIndex();
        if (playerPlatformIndex == -1 || platforms.Count <= playerPlatformIndex + 2)
        {
            return -1;
        }

        int startIndex = Mathf.Max(playerPlatformIndex + 2, lastSpawnedPlatformIndex + 1);
        for (int i = startIndex; i < platforms.Count; i++)
        {
            if (platforms[i] != null)
            {
                PlatformMover platformMover = platforms[i].GetComponent<PlatformMover>();
                if (platformMover == null || platformMover.GetBehavior() != PlatformMover.PlatformBehavior.SeeSaw)
                {
                    return i;
                }
            }
        }

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
                return i - 1;
            }
        }

        return platforms.Count - 1;
    }

    public void AddPlatform(GameObject platform)
    {
        platforms.Add(platform);
    }
}