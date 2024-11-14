using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class PlayerController : MonoBehaviour
{
    private Collider2D shadowCollider;
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
    private bool gameEnded = false;
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

    // Track if shadow immunity is active
    private bool isShadowImmune = false;
    public Text shadowImmunityTimerText; // Text for shadow immunity countdown
    private bool shadowImmunityActive = false;

    public GameObject mergeEffectPrefab;

    private Vector3 platformLastPosition;
    private bool isOnSeeSaw = false;

    // coins
    public int coins = 0;
    public Text coinText;
    public GameObject projectilePrefab;
    public float shootCooldown = 0.1f;
    private float lastShootTime;
    private Quaternion initialRotation;
    private bool isOnSeesaw = false;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0.3f;
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
    public GameObject deathMessageUI;

    // Start is called before the first frame update
    void Start()
    {
        crosshair = Instantiate(crosshairPrefab, transform.position, Quaternion.identity);
        initialPlayerScale = transform.localScale;
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = colorOrder[currentColorIndex];

        if (shadow != null)
        {
            shadowCollider = shadow.GetComponent<Collider2D>();
        }
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

        // Change the color of the shadow to grey
        if (shadow != null)
        {
            SpriteRenderer shadowSpriteRenderer = shadow.GetComponent<SpriteRenderer>();
            if (shadowSpriteRenderer != null)
            {
                shadowSpriteRenderer.color = Color.grey;
            }
        }

        // coins 
        coins = PlayerPrefs.GetInt("coins", 0);
        UpdateCoinText();

        //lives
        startPosition = transform.position;
        respawnPosition = startPosition;
        UpdateLivesText();
        initialRotation = transform.rotation;
        playerRigidbody.freezeRotation = true;
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

        if (!gameStarted)
        {
            return;
        }

        isGrounded = Physics2D.IsTouchingLayers(playerCollider, whatIsGroundLayer);

        // float horizontalInput = Input.GetAxisRaw("Horizontal");
        float horizontalInput = 1;
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

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
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

        // ShadowControl();

        // if (isGrounded)
        // {
        //     respawnPosition = transform.position;
        // }
        if (Input.GetMouseButtonDown(0) && Time.time >= lastShootTime + shootCooldown) // Left mouse button
        {
            Shoot();
            lastShootTime = Time.time; // Update the last shoot time
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
            shootCooldown = Mathf.Max(0.1f, 0.1f + (scoreManager.score * 0.007f)); // Ensure cooldown doesn't go below 0.1 seconds
        }
    }

    public void ActivateShadowImmunity(float duration)
    {
        if (!shadowImmunityActive)
        {
            shadowImmunityActive = true;
            isShadowImmune = true;

            if (shadow != null)
            {
                SpriteRenderer shadowSpriteRenderer = shadow.GetComponent<SpriteRenderer>();
                if (shadowSpriteRenderer != null)
                {
                    Color shadowColor = shadowSpriteRenderer.color;
                    shadowColor.a = 0.5f;
                    shadowSpriteRenderer.color = shadowColor;
                }

                if (shadowCollider != null)
                {
                    shadowCollider.enabled = false;
                }
            }

            StartCoroutine(ShadowImmunityCoroutine(duration));
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
        Vector2 direction = (mousePosition - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab is not assigned!");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation);

        if (bullet == null)
        {
            Debug.LogError("Failed to instantiate bullet!");
            return;
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }
        else
        {
            Debug.LogError("Bullet prefab is missing Rigidbody2D component!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!powerUpActive)
            {
                collidedWithEnemy = true;
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


    private IEnumerator ShadowImmunityCoroutine(float duration)
    {
        float remainingTime = duration;
        bool isBlinking = false;

        while (remainingTime > 0)
        {
            shadowImmunityTimerText.text = "Shadow Invincible for " + remainingTime.ToString("F1") + " secs";

            if (remainingTime <= 3f && !isBlinking)
            {
                isBlinking = true;
            }

            // If blinking, alternate between red and white
            if (isBlinking)
            {
                shadowImmunityTimerText.color = (Mathf.FloorToInt(remainingTime * 10) % 2 == 0) ? Color.red : Color.white;
            }
            else
            {
                shadowImmunityTimerText.color = Color.white; // Normal color
            }

            yield return new WaitForSeconds(0.1f);
            remainingTime -= 0.1f;
        }

        if (shadow != null)
        {
            SpriteRenderer shadowSpriteRenderer = shadow.GetComponent<SpriteRenderer>();
            if (shadowSpriteRenderer != null)
            {
                Color shadowColor = shadowSpriteRenderer.color;
                shadowColor.a = 1f;
                shadowSpriteRenderer.color = shadowColor;
            }

            if (shadowCollider != null)
            {
                shadowCollider.enabled = true;
            }
        }

        isShadowImmune = false;
        shadowImmunityActive = false; // Reset immunity state
        shadowImmunityTimerText.text = ""; // Clear the timer text
        shadowImmunityTimerText.color = Color.white;
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
        
        if (lives > 0) {
            lives--;
            UpdateLivesText();
        }

        if (lives > 0 && deathReason != "fall")
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

            ResetPlayerPosition(respawnPosition);
            return;
        }


        float sessionTime = Time.time - sessionStartTime;
        string sessionTimeString = sessionTime.ToString("F2");

        shadowImmunityTimerText.gameObject.SetActive(false);
        powerUpTimerText.gameObject.SetActive(false);

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

        if (shadow != null)
        {
            shadow.SetActive(false); // Deactivate shadow GameObject
            if (shadowCollider != null)
            {
                shadowCollider.enabled = false; // Ensure shadow collider is disabled
            }
        }


        if (scoreManager != null)
        {
            finalScoreToSend = scoreManager.score; // Store the current score to send
            scoreManager.SaveScore();
        }

        coins = 0;
        PlayerPrefs.SetInt("coins", coins);
        UpdateCoinText();

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

    // void ResetPlayerPosition(Vector3 resetPosition)
    // {
    //     transform.position = resetPosition;
    //     playerRigidbody.velocity = Vector2.zero;
    //     currentPlatform = null;
    //     isOnRotatingPlatform = false;
    //     transform.SetParent(null);

    //     if (shadow != null)
    //     {
    //         shadow.transform.position = resetPosition;
    //         recordedPositions.Clear();
    //         shadowStarted = false;
    //         shadowStartTimer = 0f;
    //         shadow.SetActive(false);
    //     }

    //     StopAllCoroutines();
    //     canJumpOnAnyPlatform = false;
    //     isShadowImmune = false;
    //     powerUpActive = false;
    //     shadowImmunityActive = false;
    //     SetPlayerOpacity(1f);
    //     powerUpTimerText.text = "";
    //     shadowImmunityTimerText.text = "";
    // }

    // private void ShadowControl()
    // {
    //     if (shadowStarted && recordedPositions.Count > delayFrames)
    //     {
    //         shadow.transform.position = recordedPositions[0];
    //         recordedPositions.RemoveAt(0);
    //     }
    // }


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

        StopAllCoroutines();

        canJumpOnAnyPlatform = false;
        isShadowImmune = false;
        powerUpActive = false;
        shadowImmunityActive = false;

        SetPlayerOpacity(1f);

        if (powerUpTimerText != null)
            powerUpTimerText.text = "";
        if (shadowImmunityTimerText != null)
            shadowImmunityTimerText.text = "";

        spriteRenderer.enabled = true;
        playerRigidbody.simulated = true;
        playerCollider.enabled = true;

        if (shadow != null)
        {
            shadow.SetActive(true);
            if (shadowCollider != null)
                shadowCollider.enabled = true;
        }
        if (crosshair == null)
        {
            crosshair = Instantiate(crosshairPrefab, transform.position, Quaternion.identity);
        }

        recordedPositions.Clear();

        // Reset any other game-specific variables as needed
        // For example:
        // fallCheckTimer = 0f;
        // shadowStartTimer = 0f;
        // shadowStarted = false;
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
        if (collision.gameObject.CompareTag("Platform"))
        {
            transform.localScale = initialPlayerScale;
            currentPlatform = collision.gameObject;
            platformLastPosition = currentPlatform.transform.position;
            Color platformColor = collision.gameObject.GetComponent<Renderer>().material.color;
            
            PlatformMover platformMover = currentPlatform.GetComponent<PlatformMover>();

            if (platformMover != null) {
                secondLastPlatformType = lastPlatformType;
                if (platformMover.GetBehavior() == PlatformMover.PlatformBehavior.SeeSaw) {
                    lastPlatformType = "See Saw";
                } else if (platformMover.GetBehavior() == PlatformMover.PlatformBehavior.Static) {
                    lastPlatformType = "Static";
                } else if (platformMover.GetBehavior() == PlatformMover.PlatformBehavior.MoveVertically) {
                    lastPlatformType = "Vertically Moving";
                } else if (platformMover.GetBehavior() == PlatformMover.PlatformBehavior.ShrinkAndGrowHorizontally) {
                    lastPlatformType = "Shrinking";
                } else {
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

        // Check for shadow collision
        if (collision.gameObject.CompareTag("shadow"))
        {
            if (!isShadowImmune)
            {
                EndGame("shadow");
                // EndGame();
                Debug.Log("Game Over! Shadow collided with the player.");
            }
            else
            {
                Debug.Log("Shadow collision avoided due to immunity.");
                AbsorbShadow();
            }
        }

        // if (collision.gameObject.CompareTag("Enemy"))
        // {
        //     Debug.Log("Game Over! Enemy collided with the player.");
        //     EndGame(); 
        // }
    }

    public void AbsorbShadow()
    {
        if (shadow != null)
        {
            if (isShadowImmune)
            {
                Debug.Log("Cannot absorb shadow while black power-up is active.");
                return;
            }

            Vector3 mergePosition = shadow.transform.position;
            Instantiate(mergeEffectPrefab, mergePosition, Quaternion.identity);

            shadow.transform.position = transform.position;
            shadow.SetActive(false);

            Debug.Log("Shadow absorbed by player!");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
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

        // if (lives <= 0)
        // {
        //     // Reset everything for a new game
        //     lives = 3;
        //     coins = 0;
        //     PlayerPrefs.SetInt("coins", coins);
        //     UpdateCoinText();
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

    public void CollectCoin()
    {
        coins++;
        UpdateCoinText();
        Debug.Log("Coins collected: " + coins);
        PlayerPrefs.SetInt("coins", coins);
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: $" + coins;
        }
    }
}