using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    private float moveSpeed;
    private float horizontalMoveSpeed;
    private float initialYPosition;
    private float initialXPosition;
    private Vector3 initialScale;
    public float movementRange = 2.0f; // Height
    public float horizontalMovementRange = 2.0f; // Width

    public float rotationSpeed = 30.0f; // Rotation speed for rotating platforms
    public float shrinkGrowSpeed = 2.0f; // Speed for shrinking and growing

    private enum PlatformBehavior { MoveVertically, Rotate, MoveHorizontally, ShrinkAndGrowHorizontally }
    private PlatformBehavior platformBehavior;

    void Start()
    {
        moveSpeed = Random.Range(0f, 5f);
        horizontalMoveSpeed = Random.Range(0f, 5f);

        // Randomly choose a platform behavior: Move Vertically, Rotate, Move Horizontally, or Shrink and Grow Horizontally
        int randomBehavior = Random.Range(0, 4);  // 0 for MoveVertically, 1 for Rotate, 2 for MoveHorizontally, 3 for ShrinkAndGrowHorizontally
        platformBehavior = (PlatformBehavior)randomBehavior;

        // Store the initial positions and scale for movement
        initialYPosition = transform.position.y;
        initialXPosition = transform.position.x;
        initialScale = transform.localScale;
    }

    void Update()
    {
        // Determine platform behavior
        if (platformBehavior == PlatformBehavior.MoveVertically)
        {
            // Move the platform up and down
            float newY = initialYPosition + Mathf.Sin(Time.time * moveSpeed) * movementRange;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        else if (platformBehavior == PlatformBehavior.Rotate)
        {
            // Rotate the platform slowly around the Z axis
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
        else if (platformBehavior == PlatformBehavior.MoveHorizontally)
        {
            // Move the platform left and right
            float newX = initialXPosition + Mathf.Sin(Time.time * horizontalMoveSpeed) * horizontalMovementRange;
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
        else if (platformBehavior == PlatformBehavior.ShrinkAndGrowHorizontally)
        {
            // Shrink and grow the platform horizontally
            float scaleX = initialScale.x + Mathf.Sin(Time.time * shrinkGrowSpeed) * (initialScale.x - 2.0f) / 2.0f;
            transform.localScale = new Vector3(Mathf.Clamp(scaleX, 2.0f, initialScale.x), initialScale.y, initialScale.z);
        }
    }
}