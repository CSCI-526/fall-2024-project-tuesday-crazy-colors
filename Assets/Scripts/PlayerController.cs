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
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.IsTouchingLayers(playerCollider, whatIsGroundLayer);

        playerRigidbody.velocity = new Vector2(moveSpeed, playerRigidbody.velocity.y);

        if (currentPlatform != null)
        {
            Color platformColor = currentPlatform.GetComponent<Renderer>().material.color;
            if (spriteRenderer.color != platformColor)
            {
                EndGame(); 
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isGrounded) {
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
            }
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeColorAscending(); 
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeColorDescending(); 
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

    void ChangeColorDescending()
    {
        currentColorIndex = (currentColorIndex - 1 + colorOrder.Length) % colorOrder.Length;
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
            } else {
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
        }
    }

    // End game logic
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
}
