using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class FlyingMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Constant horizontal speed
    public float jumpForce = 10f; // Jump force
    private Rigidbody2D rb;
    private SpriteRenderer playerRenderer;
    private Color[] colors = { Color.red, Color.green, Color.yellow };  // Available player colors
    private int currentColorIndex = 0;  // Index to track the current color

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  // Get the Rigidbody2D component attached to the player
        playerRenderer = GetComponent<SpriteRenderer>();
        playerRenderer.color = colors[currentColorIndex];  // Set initial player color
        Debug.Log("Player color initialized: " + playerRenderer.color);  // Debug log for initial color

        // Ignore collision between Player and ColoredSection layers
        // Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("ColoredSection"), true);  // Ignore collision between Player and ColoredSection
    }

    void Update()
    {
        // Apply constant horizontal movement to the right
        Vector2 movement = new Vector2(moveSpeed, 0) * Time.deltaTime;
        transform.Translate(movement);  // Move the player right

        // Handle jumping (Up Arrow Key or Spacebar)
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // Change player color based on left and right arrow keys
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeColor(false);  // Cycle color backward (left arrow)
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeColor(true);   // Cycle color forward (right arrow)
        }
    }

    void Jump()
    {
        // Apply a force upwards for jumping
        rb.velocity = new Vector2(rb.velocity.x, 0);  // Reset any existing vertical velocity before applying a new force
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);  // Apply the jump force
    }

    // Function to change the player's color (forward or backward)
    void ChangeColor(bool forward)
    {
        if (forward)
        {
            currentColorIndex = (currentColorIndex + 1) % colors.Length;  // Move forward in the color array
        }
        else
        {
            currentColorIndex = (currentColorIndex - 1 + colors.Length) % colors.Length;  // Move backward in the color array
        }

        playerRenderer.color = colors[currentColorIndex];  // Update the player color
        Debug.Log("Player color changed to: " + playerRenderer.color);  // Debug log for color change
    }

    

    // Trigger method to handle when the player enters the colored section
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ColoredSection"))
        {
            ColoredSection coloredSection = collision.gameObject.GetComponent<ColoredSection>();

            // Ensure the colored section exists and check if the player's color matches the colored section's color
            if (coloredSection != null)
            {
                Debug.Log("Player color: " + playerRenderer.color + ", Colored section color: " + coloredSection.sectionColor);  // Debug log for color comparison

                // If the player's color matches the colored section's color, ignore collision
                if (coloredSection.sectionColor == playerRenderer.color)
                {
                    Debug.Log("Player can pass through this colored section trigger.");  // Debug log when the player can pass through
                    Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("ColoredSection"), true);
                }
                else
                {
                    Debug.Log("Player hit a mismatched colored section. Game Over!");  // Debug log when the player hits a different color

                    // End the game
                    EndGame();
                }
            }
            
        }
        else
            {
                Debug.Log("null coloredSection");
            }
        // if (collision.CompareTag("ColoredSection"))
        // {
        //     Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("ColoredSection"), true);
        //     Debug.Log("Player entered the colored section. Passing through.");
        // }
    }

    void EndGame()
    {
        // Reload the current scene to restart the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Trigger method to handle when the player exits the colored section
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ColoredSection"))
        {
            // Re-enable collisions when the player exits the colored section
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("ColoredSection"), false);
            Debug.Log("Player exited the colored section.");
        }
    }
}
