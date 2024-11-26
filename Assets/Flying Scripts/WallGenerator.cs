using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    public GameObject wallPrefab;  // Prefab for the normal wall section (with collider)
    public GameObject coloredSectionPrefab;  // Prefab for the colored section (with no collider)
    public int numberOfWalls = 15;  // Number of walls to generate
    public float wallSpacing = 30f;  // Space between the walls (along the X-axis)
    public float totalWallHeight = 50f;  // Total height of each wall (500 units)
    public float coloredSectionHeight = 7f;  // Fixed height of the colored section
    public GameObject player;  // Reference to the player GameObject

    void Start()
    {
        // Check if player reference is assigned
        if (player == null)
        {
            Debug.LogError("Player reference not assigned!");
            return;
        }

        GenerateWalls();
    }

    Color GetRandomColor()
    {
        Color[] colors = { Color.red, Color.green, Color.yellow };
        return colors[Random.Range(0, colors.Length)];
    }

    void GenerateWalls()
    {
        // Get the player's Y position and calculate wall start positions
        float playerYPosition = player.transform.position.y;

        // Define the range where colored sections will be placed (-25 to +25 from player's Y position)
        float lowerBoundY = playerYPosition - 20f;
        float upperBoundY = playerYPosition + 20f;

        Debug.Log("Generating walls between Y positions: " + lowerBoundY + " and " + upperBoundY);

        float xPosition = 0f;  // Start position of the first wall

        for (int i = 0; i < numberOfWalls; i++)
        {
            // Set wall start position (25 units above the player's Y position)
            float wallStartY = playerYPosition -25f;

            // Create wall pieces
            float currentHeight = 0f;  // Start from the bottom of the wall

            // Ensure at least two colored sections are added within the range (-25 to +25 from player)
            int coloredSectionCount = 0;  // Counter for colored sections

            // Loop to generate alternating wall sections and colored sections
            while (currentHeight < totalWallHeight)
            {
                if (currentHeight + coloredSectionHeight <= totalWallHeight && coloredSectionCount < 2)
                {
                    if (currentHeight-25 >= lowerBoundY && currentHeight-25 <= upperBoundY)
                    {
                        GameObject coloredSection = Instantiate(coloredSectionPrefab, new Vector3(xPosition, wallStartY + currentHeight + coloredSectionHeight / 2, 0), Quaternion.identity);
                        coloredSection.transform.SetParent(transform);
                        coloredSection.transform.localScale = new Vector3(1, coloredSectionHeight, 1);
                        currentHeight += coloredSectionHeight;  // Add the height of the colored section
                        coloredSectionCount++;  // Increment the colored section count
                    }
                }
                // Add wall section first
                if (currentHeight < totalWallHeight)
                {
                    float wallSectionHeight = Random.Range(1f, 10);  // Random height for wall section
                    GameObject wall = Instantiate(wallPrefab, new Vector3(xPosition, wallStartY + currentHeight + wallSectionHeight / 2, 0), Quaternion.identity);
                    wall.transform.SetParent(transform);
                    wall.transform.localScale = new Vector3(1, wallSectionHeight, 1);
                    currentHeight += wallSectionHeight;  // Add the height of the wall section
                }

                // Add colored section if within the Y range (-25 to +25 from player) and we have not added two colored sections yet
                

                // If two colored sections are already placed, fill remaining space with wall sections
                if (coloredSectionCount >= 2)
                {
                    if (currentHeight < totalWallHeight)
                    {
                        float wallSectionHeight = Random.Range(1f, totalWallHeight - currentHeight);  // Random height for wall section
                        GameObject wall = Instantiate(wallPrefab, new Vector3(xPosition, wallStartY + currentHeight + wallSectionHeight / 2, 0), Quaternion.identity);
                        wall.transform.SetParent(transform);
                        wall.transform.localScale = new Vector3(1, wallSectionHeight, 1);
                        currentHeight += wallSectionHeight;  // Add the height of the wall section
                    }
                }
            }

            // Move the position for the next wall generation
            xPosition += wallSpacing;
        }
    }
}
