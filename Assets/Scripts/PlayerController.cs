using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    private Rigidbody2D playerRigidbody;
    public bool isGrounded;
    public LayerMask whatIsGroundLayer;
    private Collider2D playerCollider;
    public GameObject endGameUI;
    public ScoreManager scoreManager;

    private SpriteRenderer spriteRenderer;
    private Color[] colorOrder = { Color.red, Color.green, Color.yellow };
    private int currentColorIndex = 0;

    public float fallThreshold = -12f;
    public float fallCheckDelay = 0.5f; 
    private float fallCheckTimer = 0f;

    private GameObject currentPlatform;

    public GameObject startGameUI;
    private bool gameStarted = false;

    // tk shadow
    public GameObject shadow; 
    public float shadowDelay = 1f; 
    private List<Vector3> recordedPositions = new List<Vector3>(); 
    private bool shadowStarted = false;
    private float shadowStartTimer = 0f;
    private int delayFrames;

    // power-up invincibility
    private bool canJumpOnAnyPlatform = false;
    public Text powerUpTimerText;

    // Track if power-up is active
    private bool powerUpActive = false;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = colorOrder[currentColorIndex];
        if (endGameUI != null)
        {
            endGameUI.SetActive(false);
        }

        playerRigidbody.simulated = false;

        if (startGameUI != null)
        {
            startGameUI.SetActive(true);
        }

        // tk 
        delayFrames = Mathf.RoundToInt(shadowDelay / Time.deltaTime);

    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStarted)
        {
            return; 
        }

        isGrounded = Physics2D.IsTouchingLayers(playerCollider, whatIsGroundLayer);

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        playerRigidbody.velocity = new Vector2(horizontalInput * moveSpeed, playerRigidbody.velocity.y);

        if (gameStarted)
        {
            recordedPositions.Add(transform.position);

            if (!shadowStarted)
            {
                shadowStartTimer += Time.deltaTime;
                if (shadowStartTimer >= shadowDelay)
                {
                    shadowStarted = true;
                    shadow.SetActive(true); // Activate shadow after delay
                }
            }
        }

        if (currentPlatform != null)
        {
            Color platformColor = currentPlatform.GetComponent<Renderer>().material.color;
            if (spriteRenderer.color != platformColor && !canJumpOnAnyPlatform)
            {
                EndGame(); 
            }
        }

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeColorAscending(); 
        }

        fallCheckTimer += Time.deltaTime;

        if (transform.position.y < fallThreshold && fallCheckTimer > fallCheckDelay)
        {
            EndGame(); 
            Debug.Log("Game Over! Player missed the next Platform.");
        }

        ShadowControl();
    }

    // public async Task TemporaryPowerUpEffect(float duration)
    // {
    //     // Power-up active
    //     powerUpActive = true;

    //     // Reduce opacity and allow player to jump on any platform
    //     SetPlayerOpacity(0.5f);
    //     canJumpOnAnyPlatform = true;

    //     float remainingTime = duration;

    //     while (remainingTime > 0)
    //     {
    //         powerUpTimerText.text = "Invincible for " + remainingTime.ToString("F1") + " secs";

    //         await Task.Delay(100);  // 0.1 second delay
    //         remainingTime -= 0.1f;
    //     }

    //     // Restore full opacity and end power-up effect
    //     SetPlayerOpacity(1f);
    //     canJumpOnAnyPlatform = false;
    //     powerUpActive = false;

    //     Debug.Log("Player returned to original state after power-up.");
    //     powerUpTimerText.text = "";
    // }

    public void TemporaryPowerUpEffect(float duration)
    {
        StartCoroutine(PowerUpEffectCoroutine(duration));
    }

    private IEnumerator PowerUpEffectCoroutine(float duration)
    {
        powerUpActive = true;

        SetPlayerOpacity(0.5f);
        canJumpOnAnyPlatform = true;

        float remainingTime = duration;

        while (remainingTime > 0)
        {
            powerUpTimerText.text = "Invincible for " + remainingTime.ToString("F1") + " secs";

            yield return new WaitForSeconds(0.1f);  // 0.1 second delay
            remainingTime -= 0.1f;
        }

        SetPlayerOpacity(1f);
        canJumpOnAnyPlatform = false;
        powerUpActive = false;

        Debug.Log("Player returned to original state after power-up.");
        powerUpTimerText.text = "";
    }

    void SetPlayerOpacity(float opacity)
    {
        Color currentColor = spriteRenderer.color;
        currentColor.a = opacity; // Set opacity
        spriteRenderer.color = currentColor; // Apply color change
    }

    // Override EndGame temporarily for invincibility
    void EndGame()
    {
        if (endGameUI != null)
        {
            endGameUI.SetActive(true); 
        }
        spriteRenderer.enabled = false;
        playerRigidbody.simulated = false;
        playerCollider.enabled = false;

        if (shadow != null)
        {
            shadow.SetActive(false);
        }
    }

    private void ShadowControl()
    {
        if (shadowStarted && recordedPositions.Count > delayFrames)
        {
            shadow.transform.position = recordedPositions[0];
            recordedPositions.RemoveAt(0); 
        }
    }

    void ChangeColorAscending()
    {
        currentColorIndex = (currentColorIndex + 1) % colorOrder.Length;

        Color newColor = colorOrder[currentColorIndex];

        // If power-up is active, maintain reduced opacity when changing color
        if (powerUpActive)
        {
            newColor.a = 0.5f; // Maintain semi-transparency
        }

        spriteRenderer.color = newColor; // Apply color change
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            currentPlatform = collision.gameObject;
            Color platformColor = collision.gameObject.GetComponent<Renderer>().material.color;

            if (spriteRenderer.color != platformColor && !canJumpOnAnyPlatform)
            {
                EndGame(); 
                Debug.Log("Game Over! Player landed on a different color platform.");
            } 
            else
            {
                // Makes the platform parent
                transform.SetParent(collision.transform);

                // Score logic
                if (scoreManager != null)
                {
                    scoreManager.score++;
                    scoreManager.UpdateScoreText();
                }
            }
        }

        if (collision.gameObject.CompareTag("shadow"))
        {
            EndGame(); // End the game if the shadow collides with the player
            Debug.Log("Game Over! Shadow collided with the player.");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            currentPlatform = null; 
            // remove platform as a parent
            transform.SetParent(null);
        }
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame()
    {
        gameStarted = true;
        if (startGameUI != null)
        {
            startGameUI.SetActive(false); 
        }
        playerRigidbody.simulated = true; 
    }
}
