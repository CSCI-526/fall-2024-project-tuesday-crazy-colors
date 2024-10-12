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
    public SendtoGoogle sendtoGoogle;
    private bool dataSent = false;

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

    // white power up
    private bool canJumpOnAnyPlatform = false;
    public Text powerUpTimerText;
    private Color originalColor;


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

            // Start shadow after delay time has passed
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
            if (spriteRenderer.color != platformColor && canJumpOnAnyPlatform == false)
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

    public async Task TemporaryColorChange(Color newColor, float duration)
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = newColor;

        canJumpOnAnyPlatform = true;
        Debug.Log(canJumpOnAnyPlatform);

        float remainingTime = duration;

        while (remainingTime > 0)
        {
            powerUpTimerText.text = "Turning back to " + GetColorName(originalColor) + " in " + remainingTime.ToString("F1") + " secs";

            await Task.Delay(100);
            
            remainingTime -= 0.1f;
        }

        // await Task.Delay((int)(duration * 1000));

        spriteRenderer.color = originalColor;
        canJumpOnAnyPlatform = false;

        Debug.Log("Player color reverted to original after power-up.");
        powerUpTimerText.text = "";
    }

    private string GetColorName(Color color)
    {
        if (color == Color.red) return "red";
        if (color == Color.blue) return "blue";
        if (color == Color.green) return "green";
        if (color == Color.yellow) return "yellow";
        return "unknown color";
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
        spriteRenderer.color = colorOrder[currentColorIndex];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            currentPlatform = collision.gameObject;
            Color platformColor = collision.gameObject.GetComponent<Renderer>().material.color;

            if (spriteRenderer.color != platformColor && canJumpOnAnyPlatform == false)
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
            // remove platofrm as a parent
            transform.SetParent(null);
        }
    }

    void EndGame()
    {
        if (endGameUI != null)
        {
            endGameUI.SetActive(true); 
        }
        //Destroy(gameObject);
        spriteRenderer.enabled = false;
        playerRigidbody.simulated = false;
        playerCollider.enabled = false;

        if (shadow != null)
        {
            shadow.SetActive(false);
        }

        if (dataSent) return; // Prevent multiple calls
        dataSent = true;

        SendtoGoogle sendToGoogle = FindObjectOfType<SendtoGoogle>();
        if (sendToGoogle != null)
        {
            sendToGoogle.Send();  
            Debug.Log("Send method called successfully in EndGame.");
        }
        else
        {
            Debug.LogError("SendtoGoogle instance not found.");
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
