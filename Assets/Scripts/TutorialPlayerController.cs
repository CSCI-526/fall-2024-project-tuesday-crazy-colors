using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialPlayerController : MonoBehaviour
{
    public float moveSpeed;

    public float fallThreshold = -12f;
    public float fallCheckDelay = 0.5f;
    private float fallCheckTimer = 0f;

    private Vector3 platformLastPosition;
    private bool isOnRotatingPlatform = false;

    public float jumpForce;
    private Rigidbody2D playerRigidbody;
    public bool isGrounded;
    public LayerMask whatIsGroundLayer;
    private Collider2D playerCollider;
    private Color[] colorOrder = { Color.red, Color.green, Color.yellow };
    private int currentColorIndex = 0;
    private GameObject currentPlatform;

    private SpriteRenderer spriteRenderer;
    public float shootCooldown = 0.1f;
    private float lastShootTime;
    public float fireRate = 0.1f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public int bulletsPerShot = 3; // Number of bullets in each spread
    public float spreadAngle = 15f; // Angle of the spread in degrees
    public GameObject crosshairPrefab;
    private GameObject crosshair;
    public float crosshairDistance = 1f;
    private Quaternion initialRotation;

    // Animations
    private Animator animator;

    // Power-up
    private bool powerUpActive = false;
    private bool canJumpOnAnyPlatform = false;
    public Text powerUpTimerText;

    // Start is called before the first frame update
    void Start()
    {
        crosshair = Instantiate(crosshairPrefab, transform.position, Quaternion.identity);
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // to fix the player shape change or rotation bug
        initialRotation = transform.rotation;
        playerRigidbody.freezeRotation = true;

        // Animations
        animator = GetComponent<Animator>();
    }

    void UpdateCrosshairPosition()
    {
        if (crosshair == null)
        {
            // Crosshair has been destroyed, so recreate it or handle the situation
            // For example:
            // crosshair = Instantiate(crosshairPrefab, transform.position, Quaternion.identity);
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        crosshair.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (powerUpActive)
            {
                Destroy(other.gameObject);
            }
            else
            {
                RestartGame();
            }
        }
    }
    void LateUpdate()
    {
        UpdateCrosshairPosition();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.IsTouchingLayers(playerCollider, whatIsGroundLayer);
        
        // float horizontalInput = Input.GetAxisRaw("Horizontal");
        float horizontalInput = 1f;
        playerRigidbody.velocity = new Vector2(horizontalInput * moveSpeed, playerRigidbody.velocity.y);

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeColorAscending();
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeColorDescending();
        }

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
        }

        if (Input.GetMouseButton(0))
        {
            if (Time.time >= lastShootTime + fireRate)
            {
                Shoot();
                lastShootTime = Time.time;
            }
        }

        UpdateCrosshairPosition();
        if (crosshair != null)
        {
            UpdateCrosshairPosition();
        }
        UpdateShootCooldown();

        if (currentPlatform != null)
        {
            Color platformColor = currentPlatform.GetComponent<SpriteRenderer>().color;
            if (spriteRenderer.color != platformColor && platformColor != Color.white && !canJumpOnAnyPlatform)
            {
                RestartGame();
                return;
            }
        }

        fallCheckTimer += Time.deltaTime;

        if (transform.position.y < fallThreshold && fallCheckTimer > fallCheckDelay)
        {
            // EndGame("fall");
            RestartGame();
            Debug.Log("Game Over! Player missed the next Platform.");
            fallCheckTimer = 0f;
            return;
        }

        if (isOnRotatingPlatform && currentPlatform != null)
        {
            Vector3 platformMovement = currentPlatform.transform.position - platformLastPosition;
            transform.position += platformMovement;  // Move the player along with the platform's movement
            platformLastPosition = currentPlatform.transform.position;  // Update platform's last position
        }

        // to fix the player shape change or rotation bug
        transform.rotation = initialRotation;

        // Animations
        animator.SetFloat("Speed", playerRigidbody.velocity.x);
        animator.SetBool("Grounded", isGrounded);
    }
    void UpdateShootCooldown()
    {
        fireRate = Mathf.Max(0.05f, 0.2f + (20 * 0.007f)); // Ensure cooldown doesn't go below 0.1 seconds
    }

    void Shoot()
    {
        if (Camera.main == null)
        {
            Debug.LogError("Main camera not found!");
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector2 direction = (mousePosition - transform.position).normalized;

        float centerAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            float currentSpread = spreadAngle * ((float)i / (bulletsPerShot - 1) - 0.5f);
            float angle = centerAngle + currentSpread;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 bulletDirection = rotation * Vector2.right;
                rb.velocity = bulletDirection * bulletSpeed;
            }
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

    void ChangeColorDescending()
    {
        currentColorIndex = (currentColorIndex - 1);
        if (currentColorIndex < 0)
        {
            currentColorIndex = colorOrder.Length - 1;
        }

        Color newColor = colorOrder[currentColorIndex];

        // If power-up is active, maintain reduced opacity when changing color
        if (powerUpActive)
        {
            newColor.a = 0.5f; // Maintain semi-transparency
        }

        spriteRenderer.color = newColor; // Apply color change
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected with " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Platform"))
        {
            currentPlatform = collision.gameObject;
            platformLastPosition = currentPlatform.transform.position;
            Color platformColor = collision.gameObject.GetComponent<SpriteRenderer>().color;
            Debug.Log("Platform color: " + platformColor);
            Debug.Log("Player color: " + spriteRenderer.color);
            if (spriteRenderer.color != platformColor && platformColor != Color.white && !canJumpOnAnyPlatform)
            {
                RestartGame();
                Debug.Log("Game Over! Player landed on a different color platform.");
            }
            else
            {
                isOnRotatingPlatform = currentPlatform.GetComponent<PlatformMover>() != null;
                if (!isOnRotatingPlatform)
                {
                    transform.SetParent(collision.transform);  // Only parent if the platform is not rotating
                }
            }
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            RestartGame(); 
            Debug.Log("Game Over! Enemy collided with the player.");
        }
       

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            currentPlatform = null;
            isOnRotatingPlatform = false; 
            transform.SetParent(null);
        }
    }

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
        bool isBlinking = false;

        while (remainingTime > 0)
        {
            powerUpTimerText.text = "Player Invincible for " + remainingTime.ToString("F1") + " secs";

            // Start blinking effect when there are 3 seconds left
            if (remainingTime <= 3f && !isBlinking)
            {
                isBlinking = true;
            }

            // If blinking, alternate between red and white
            if (isBlinking)
            {
                powerUpTimerText.color = (Mathf.FloorToInt(remainingTime * 10) % 2 == 0) ? Color.red : Color.white;
            }
            else
            {
                powerUpTimerText.color = Color.white; // Normal color
            }

            yield return new WaitForSeconds(0.1f);
            remainingTime -= 0.1f;
        }

        SetPlayerOpacity(1f);
        canJumpOnAnyPlatform = false;
        powerUpActive = false;

        Debug.Log("Player returned to original state after power-up.");
        powerUpTimerText.text = "";
        powerUpTimerText.color = Color.white;
    }

    void SetPlayerOpacity(float opacity)
    {
        Color currentColor = spriteRenderer.color;
        currentColor.a = opacity; // Set opacity
        spriteRenderer.color = currentColor; // Apply color change
    }
}
