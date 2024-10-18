using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    private float moveSpeed;
    private float initialYPosition;
    public float movementRange = 2.0f; //Height

    public float rotationSpeed = 20.0f; // Rotation speed for rotating platforms

    private enum PlatformBehavior { MoveVertically, Rotate, BreakOnCollision }
    private PlatformBehavior platformBehavior;

    private bool isBroken = false;

    void Start()
    {
        moveSpeed = Random.Range(0f, 5f);

        // Randomly choose a platform behavior: Move Vertically, Rotate, or Break On Collision
        int randomBehavior = Random.Range(0, 3);  // 0 for MoveVertically, 1 for Rotate, 2 for BreakOnCollision
        platformBehavior = (PlatformBehavior)randomBehavior;

        // Store the initial Y position for vertical movement
        initialYPosition = transform.position.y;
    }

    void Update()
    {
       // If the platform is broken, do nothing
        if (isBroken) return;

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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (platformBehavior == PlatformBehavior.BreakOnCollision && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collision detected with Player. Breaking the platform.");
            StartCoroutine(BreakPlatform());
        }
    }

    // Coroutine to break the platform
    IEnumerator BreakPlatform()
    {
        // Optional: Add some visual effect or delay before breaking
        yield return new WaitForSeconds(0.2f); // Delay before breaking (optional)

        // Disable the platform
        isBroken = true;
        // gameObject.SetActive(false); // Deactivate platform (or you can destroy it)
        Destroy(gameObject);

        // Optionally, you can trigger any break effect or particle system here
        Debug.Log("Platform broke on collision!");
    }
}
