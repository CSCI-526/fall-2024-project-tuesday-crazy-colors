using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public int bulletsPerShot = 3; // Number of bullets in each spread
    public float spreadAngle = 15f; // Angle of the spread in degrees
    private bool isOnJumpPad = false;
    private GameObject lastPlatform;  // Field to store the last platform
    public float moveSpeed;
    public float jumpForce;
    private Rigidbody2D playerRigidbody;
    public bool isGrounded;
    public LayerMask whatIsGroundLayer;
    private Collider2D playerCollider;
    public GameObject endGameUI;
    public ScoreManager scoreManager;

    public SendToGoogle sendToGoogle;
    private bool dataSent = false;
    private int finalScoreToSend = 0;

    private SpriteRenderer spriteRenderer;
    private Color[] colorOrder = { Color.red, Color.green, Color.yellow };
    private int currentColorIndex = 0;

    public float fallThreshold = -12f;
    public float fallCheckDelay = 0.5f;
    private float fallCheckTimer = 0f;

    private GameObject currentPlatform;
    private Vector3 initialPlayerScale;

    public GameObject startGameUI;
    private bool gameStarted = false;

    // power-up invincibility
    private bool canJumpOnAnyPlatform = false;
    public Text powerUpTimerText;

    // Track if power-up is active
    private bool powerUpActive = false;

    public GameObject mergeEffectPrefab;

    private Vector3 platformLastPosition;
    private bool isOnSeeSaw = false;

    // Shooting
    public GameObject projectilePrefab;
    private float lastShootTime;

    public float shootCooldown = 0.5f; 
    private Quaternion initialRotation;
    private bool isOnSeesaw = false;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float fireRate = 0.1f;
    public GameObject crosshairPrefab;
    private GameObject crosshair;
    public float crosshairDistance = 1f;


    // Analytics
    private string deathReason;
    private bool fellOffPlatform = false;
    private bool collidedWithEnemy = false;
    private bool platformColorMismatch = false;
    private bool deathDataSent = false;
    private int enemyKillCount = 0;
    private float sessionStartTime;
    private string lastPlatformType = "Neutral";
    private string secondLastPlatformType = "Neutral";
    // private float colorSwitchStartTime;
    // private float colorSwitchEndTime;
    // private float colorSwitchDurationSum = 0f;
    // private int colorSwitchDurationCount = 0;
    // private float colorSwitchDurationAverage = 0f;

    //lives
    public int lives = 3;
    public Text livesText;
    private Vector3 startPosition;
    private Vector3 respawnPosition;
    public List<Image> heartImages; // List to hold references to heart images
    public float blinkDuration = 3f; // Duration for blinking effect
    public float blinkInterval = 0.2f; // Interval for blinking

    // public GameObject deathMessageUI;

    //lives pause

    private GameObject killerEnemy;
    public TextMeshProUGUI deathMessageUI;

    // For the flying scene
    private bool isFlying = false;

    // Animations
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        crosshair = Instantiate(crosshairPrefab, transform.position, Quaternion.identity);
        initialPlayerScale = transform.localScale;
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

        if (deathMessageUI != null)
        {
            deathMessageUI.gameObject.SetActive(false);
        }

        //lives
        startPosition = transform.position;
        respawnPosition = startPosition;
        UpdateLivesText();
        initialRotation = transform.rotation;
        playerRigidbody.freezeRotation = true;

        // Change player jumping mechanics based on scene
        if (SceneManager.GetActiveScene().name == "Flying")
        {
            isFlying = true;
            gameStarted = true;
            playerRigidbody.simulated = true;
        }

        // Animations
        animator = GetComponent<Animator>();
        StartCoroutine(BlinkRemainingHearts());
    }

    public void OnEnemyKilled()
    {
        enemyKillCount++;
    }

    void UpdateLivesText()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
        }


    }

    void UpdateLivesUI()
    {
        Debug.Log("Updating Lives UI: Current Lives = " + lives);
        for (int i = 0; i < heartImages.Count; i++)
        {
            heartImages[i].enabled = i < lives;
            Debug.Log($"Heart {i} is " + (heartImages[i].enabled ? "Visible" : "Hidden"));
        }
    }

    IEnumerator BlinkRemainingHearts()
    {
        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            // Toggle visibility only for remaining hearts
            for (int i = 0; i < lives; i++)
            {
                heartImages[i].enabled = !heartImages[i].enabled;
            }

            elapsedTime += blinkInterval;
            yield return new WaitForSeconds(blinkInterval);
        }

        // Ensure all remaining hearts are visible after blinking ends
        for (int i = 0; i < lives; i++)
        {
            heartImages[i].enabled = true;
        }
    }









    // Update is called once per frame

    void LateUpdate()
    {
        UpdateCrosshairPosition();
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
    void Update()
    {

        // Animations
        animator.SetFloat("Speed", playerRigidbody.velocity.x);
        animator.SetBool("Grounded", isGrounded);
        Debug.Log("Grounded: " + isGrounded);
        Debug.Log("Speed: " + playerRigidbody.velocity.x);

        if (!gameStarted)
        {
            return;
        }

        isGrounded = Physics2D.IsTouchingLayers(playerCollider, whatIsGroundLayer);

        // float horizontalInput = Input.GetAxisRaw("Horizontal");
        float horizontalInput = 1;
        playerRigidbody.velocity = new Vector2(horizontalInput * moveSpeed, playerRigidbody.velocity.y);

        if (currentPlatform != null)
        {
            Color platformColor = currentPlatform.GetComponent<Renderer>().material.color;
            if (spriteRenderer.color != platformColor && !canJumpOnAnyPlatform)
            {
                EndGame("color");

                // EndGame();
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeColorAscending();
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeColorDescending();
        }

        if (isFlying)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
            }
        }
        else
        {
            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
            {
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
            }
        }

        fallCheckTimer += Time.deltaTime;

        if (transform.position.y < fallThreshold && fallCheckTimer > fallCheckDelay)
        {
            fellOffPlatform = true;
            Debug.Log(fellOffPlatform);
            EndGame("fall");
            // EndGame();
            Debug.Log("Game Over! Player missed the next Platform.");
            fallCheckTimer = 0f;
            return;
        }

        if (isOnSeeSaw && currentPlatform != null)
        {
            Vector3 platformMovement = currentPlatform.transform.position - platformLastPosition;
            transform.position += platformMovement;  // Move the player along with the platform's movement
            platformLastPosition = currentPlatform.transform.position;  // Update platform's last position
        }

        // if (Input.GetMouseButtonDown(0) && Time.time >= lastShootTime + shootCooldown) // Left mouse button
        // {
        //     Shoot();
        //     lastShootTime = Time.time; // Update the last shoot time
        // }

        if (Input.GetMouseButton(0))
        {
            if (Time.time >= lastShootTime + fireRate)
            {
                Shoot();
                lastShootTime = Time.time;
            }
        }

        transform.rotation = initialRotation;
        RotateWithSeesaw();
        UpdateCrosshairPosition();
        if (crosshair != null)
        {
            UpdateCrosshairPosition();
        }
        UpdateShootCooldown();
    }
    void UpdateShootCooldown()
    {
        if (scoreManager != null)
        {
            fireRate = Mathf.Max(0.05f, 0.2f + (scoreManager.score * 0.007f)); // Ensure cooldown doesn't go below 0.1 seconds
        }
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!powerUpActive)
            {
                collidedWithEnemy = true;
                killerEnemy = other.gameObject;
                //StartCoroutine(PauseAndRespawn(transform.position, "enemy"));
                Debug.Log(collidedWithEnemy);
                EndGame("enemy");
                // EndGame();
                Debug.Log("Game Over! Player collided with an enemy.");
            }
            else
            {
                // If the player has a power-up, destroy the enemy instead
                Destroy(other.gameObject);
                Debug.Log("Enemy destroyed by powered-up player!");
            }
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

    // Override EndGame temporarily for invincibility
    void EndGame(string deathReason)
    // void EndGame()
    {

        if (lives > 0)
        {
            lives--;
            UpdateLivesText();
            UpdateLivesUI();
            // StartCoroutine(BlinkRemainingHearts());
        }

        if (lives > 0 && deathReason != "fall")
        {
            Vector3 respawnPosition = transform.position;
            if (currentPlatform != null)
            {
                switch (deathReason)
                {
                    case "enemy":
                    case "color":
                        respawnPosition = currentPlatform.transform.position;
                        respawnPosition.y += 1f; // Adjust this value to spawn slightly above the platform
                        break;
                }

                if (deathReason == "color" || deathReason == "enemy")
                {
                    spriteRenderer.color = currentPlatform.GetComponent<Renderer>().material.color;
                }

                if (killerEnemy != null)
                {
                    Destroy(killerEnemy);
                    killerEnemy = null;
                }
            }
            else
            {
                Debug.LogWarning("Current platform is null. Respawning at current position.");
            }

            ResetPlayerPosition(respawnPosition);
            StartCoroutine(PauseAndRespawn(respawnPosition, deathReason));
            return;
        }



        float sessionTime = Time.time - sessionStartTime;
        string sessionTimeString = sessionTime.ToString("F2");

        if (powerUpTimerText != null)
        {
            powerUpTimerText.gameObject.SetActive(false);
        }

        // DeathAnalytics.instance.DeathLog(collidedWithEnemy, platformColorMismatch, fellOffPlatform);

        if (!deathDataSent)
        {
            deathDataSent = true;
            DeathAnalytics.instance.DeathLog(collidedWithEnemy, platformColorMismatch, fellOffPlatform, enemyKillCount, sessionTimeString, lastPlatformType, secondLastPlatformType);
            Debug.Log("Last Platform Type: " + lastPlatformType);
            Debug.Log("Second Last Platform Type: " + secondLastPlatformType);
            Debug.Log($"Death Reason in Player Control  - Enemy: {collidedWithEnemy}, Color: {platformColorMismatch}, Platform: {fellOffPlatform}");
            Debug.Log("Passed bool values to the DeathAnalytics");

        }

        if (endGameUI != null)
        {
            endGameUI.SetActive(true);
        }
        spriteRenderer.enabled = false;
        playerRigidbody.simulated = false;
        playerCollider.enabled = false;

        if (scoreManager != null)
        {
            finalScoreToSend = scoreManager.score; // Store the current score to send
            scoreManager.SaveScore();
        }

        if (!dataSent)
        {
            dataSent = true;
            SendToGoogle googleInstance = SendToGoogle.Instance; // Get the singleton instance
            if (googleInstance != null)
            {
                googleInstance.Send(finalScoreToSend); // Pass the stored score to the Send method
                Debug.Log("Send method called successfully in EndGame with score: " + finalScoreToSend);
            }
            else
            {
                Debug.LogError("SendToGoogle instance not found.");
            }
        }
        if (crosshair != null)
        {
            Destroy(crosshair);
            crosshair = null;
        }

        if (platformColorMismatch)
        {
            Debug.Log("Death reason: Platform color mismatch");
        }
        // SceneManager.LoadScene("Main Menu");
    }



    private IEnumerator PauseAndRespawn(Vector3 respawnPosition, string deathReason)
    {
        Time.timeScale = 0f; // Pause the game
        playerRigidbody.simulated = false; // Disable physics simulation

        // Show death message and countdown
        if (deathMessageUI != null)
        {
            deathMessageUI.gameObject.SetActive(true);
            TextMeshProUGUI messageText = deathMessageUI.GetComponent<TextMeshProUGUI>();
            if (messageText != null)
            {

                // Countdown timer
                for (int i = 3; i > 0; i--)
                {
                    messageText.text = $"Player respawning in {i}...";
                    yield return new WaitForSecondsRealtime(1f);
                }
                // UpdateLivesUI();
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found on deathMessageUI");
            }
        }
        else
        {
            Debug.LogError("deathMessageUI is not assigned in the Inspector");
        }



        // Hide death message
        if (deathMessageUI != null)
        {
            deathMessageUI.gameObject.SetActive(false);
        }

        Time.timeScale = 1f; // Resume the game
        playerRigidbody.simulated = true; // Re-enable physics simulation

        ResetPlayerPosition(respawnPosition);
        if (lives > 0) // Ensure there are lives remaining
        {
            yield return StartCoroutine(BlinkRemainingHearts());
        }
    }


    void ResetPlayerPosition(Vector3 resetPosition)
    {

        transform.position = resetPosition;

        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.angularVelocity = 0f;

        currentPlatform = null;
        isOnSeeSaw = false;
        isOnSeesaw = false;

        transform.SetParent(null);
        transform.rotation = initialRotation;

        killerEnemy = null;

        StopAllCoroutines();

        canJumpOnAnyPlatform = false;
        powerUpActive = false;

        SetPlayerOpacity(1f);


        if (powerUpTimerText != null)
            powerUpTimerText.text = "";

        spriteRenderer.enabled = true;
        playerRigidbody.simulated = true;
        playerCollider.enabled = true;

        if (crosshair == null)
        {
            crosshair = Instantiate(crosshairPrefab, transform.position, Quaternion.identity);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("JumpPad"))
        {
            isOnJumpPad = true;
        }

        if (collision.gameObject.CompareTag("Platform"))
        {
            transform.localScale = initialPlayerScale;
            currentPlatform = collision.gameObject;
            platformLastPosition = currentPlatform.transform.position;
            Color platformColor = collision.gameObject.GetComponent<Renderer>().material.color;

            PlatformMover platformMover = currentPlatform.GetComponent<PlatformMover>();

            if (platformMover != null)
            {
                secondLastPlatformType = lastPlatformType;
                if (platformMover.GetBehavior() == PlatformMover.PlatformBehavior.SeeSaw)
                {
                    lastPlatformType = "See Saw";
                }
                else if (platformMover.GetBehavior() == PlatformMover.PlatformBehavior.Static)
                {
                    lastPlatformType = "Static";
                }
                else if (platformMover.GetBehavior() == PlatformMover.PlatformBehavior.MoveVertically)
                {
                    lastPlatformType = "Vertically Moving";
                }
                else if (platformMover.GetBehavior() == PlatformMover.PlatformBehavior.ShrinkAndGrowHorizontally)
                {
                    lastPlatformType = "Shrinking";
                }
                else
                {
                    lastPlatformType = "Neutral";
                }
            }

            if (spriteRenderer.color != platformColor && !canJumpOnAnyPlatform)
            {
                platformColorMismatch = true;
                Debug.Log(platformColorMismatch);
                EndGame("color");
                // EndGame();
                Debug.Log("Game Over! Player landed on a different color platform.");
            }
            else
            {
                // Check if the platform has a rotating behavior (SeeSaw)
                if (platformMover != null && platformMover.GetBehavior() == PlatformMover.PlatformBehavior.SeeSaw)
                {
                    // Determine if the player landed on the left or right side of the platform
                    bool isLandingLeft = collision.contacts[0].point.x < collision.transform.position.x;
                    platformMover.AdjustSeeSawRotation(isLandingLeft);
                    isOnSeesaw = true;
                    transform.SetParent(collision.transform);
                }
                else
                {
                    isOnSeesaw = false;
                    transform.SetParent(null);
                }


                isOnSeeSaw = platformMover != null && platformMover.GetBehavior() != PlatformMover.PlatformBehavior.Static;
                if (!isOnSeeSaw)
                {
                    transform.SetParent(collision.transform);  // Only parent if the platform is not rotating
                }

                // Score logic
                if (scoreManager != null && currentPlatform != lastPlatform)
                {
                    scoreManager.score++;
                    scoreManager.UpdateScoreText();
                    lastPlatform = currentPlatform;  // Update lastPlatform to the current one
                }
            }
        }

        // if (collision.gameObject.CompareTag("Enemy"))
        // {
        //     Debug.Log("Game Over! Enemy collided with the player.");
        //     EndGame(); 
        // }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("JumpPad"))
        {
            isOnJumpPad = false;
        }

        if (collision.gameObject.CompareTag("Platform"))
        {
            currentPlatform = null;
            isOnSeeSaw = false;  // Reset when the player exits the platform
            transform.SetParent(null);  // Remove any parenting when the player exits the platform
        }
    }
    private void RotateWithSeesaw()
    {
        if (isOnSeesaw && currentPlatform != null)
        {
            float seesawRotation = currentPlatform.transform.rotation.eulerAngles.z;
            transform.rotation = Quaternion.Euler(0, 0, seesawRotation);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
    }

    public void NavigateToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void RetryGame()
    {
        scoreManager.score = 0;
        scoreManager.UpdateScoreText();
        SendToGoogle googleInstance = SendToGoogle.Instance;
        if (googleInstance != null)
        {
            googleInstance.ResetDataSent();
        }

        dataSent = false;


        fellOffPlatform = false;
        collidedWithEnemy = false;
        platformColorMismatch = false;
        deathDataSent = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        StartCoroutine(BlinkRemainingHearts());

        // if (lives <= 0)
        // {
        //     // Reset everything for a new game
        //     lives = 3;
        //     scoreManager.ResetScore();
        //     ResetPlayerPosition(startPosition);
        // }
        // else
        // {
        //     // Continue from current position if lives remain
        //     ResetPlayerPosition(respawnPosition);
        // }

        // UpdateLivesText();
        // endGameUI.SetActive(false);
        // spriteRenderer.enabled = true;
        // playerRigidbody.simulated = true;
        // playerCollider.enabled = true;
    }

    // Trigger the game start
    public void StartGame()
    {
        sessionStartTime = Time.time;
        gameStarted = true;
        playerRigidbody.simulated = true;
        startGameUI.SetActive(false);
    }
}