using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private Color[] colorOrder = { Color.red, Color.blue, Color.green, Color.yellow };
    private int currentColorIndex = 0;

    public float fallThreshold = -12f;
    public float fallCheckDelay = 0.5f; 
    private float fallCheckTimer = 0f;

    private GameObject currentPlatform;

    public GameObject startGameUI;
    private bool gameStarted = false;

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

        if (currentPlatform != null)
        {
            Color platformColor = currentPlatform.GetComponent<Renderer>().material.color;
            if (spriteRenderer.color != platformColor)
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

            if (spriteRenderer.color != platformColor)
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
