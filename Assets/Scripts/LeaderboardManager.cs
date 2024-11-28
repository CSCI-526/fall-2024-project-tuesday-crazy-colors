
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Dan.Main;
using System.Collections; 

public class LeaderboardManager : MonoBehaviour
{
    public GameObject leaderboardCanvas;
    public TMP_InputField playerNameInput;
    public TextMeshProUGUI[] leaderboardEntries;
    public Button submitButton;
    public Button closeButton;
    private ScoreManager scoreManager;
    private bool isSubmitting = false;  // Add this flag

    public GameObject endGameUI;
    public GameObject startGameUI;
    private bool isGameOver = false;  // Reference to the End Game UI
    public Text livesText;

    void Start()
    {
        playerNameInput.onValueChanged.AddListener(OnInputValueChanged);
        scoreManager = FindObjectOfType<ScoreManager>();
        submitButton.onClick.AddListener(SubmitPlayerName);
        closeButton.onClick.AddListener(HideLeaderboard);
        leaderboardCanvas.SetActive(false);

        playerNameInput.onEndEdit.AddListener((value) => {
        playerNameInput.text = value;  // Preserve the text
        playerNameInput.ActivateInputField();  // Keep the input field active
        });
    }

    
    public void ShowLeaderboard()
    {
        leaderboardCanvas.SetActive(true);
        if (endGameUI != null)
        {
            endGameUI.SetActive(false);
        }

        if (livesText != null)
        {
        livesText.gameObject.SetActive(false);  // Hide lives text
        }

        EnableInputField();
        UpdateLeaderboardUI();
        
        // Ensure the input field is focused and not cleared
        StartCoroutine(FocusInputField());
    }

    private IEnumerator FocusInputField()
    {
        // Wait for the end of the frame to ensure the UI has updated
        yield return new WaitForEndOfFrame();
        playerNameInput.text = playerNameInput.text; // Preserve existing text
        playerNameInput.ActivateInputField();
        playerNameInput.Select();
    }
    private void EnableInputField()
    {
        playerNameInput.gameObject.SetActive(true);
        submitButton.gameObject.SetActive(true);
        // Only clear the input if it's empty
        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            playerNameInput.text = "";
        }
        isSubmitting = false;
    }

    private void OnInputValueChanged(string value)
    {
        // This will be called every time the input field value changes
        playerNameInput.text = value;
    }

    public void HideLeaderboard()
    {
        leaderboardCanvas.SetActive(false);
        if (livesText != null)
        {
        livesText.gameObject.SetActive(true);  // Show lives text again
        }
        if (startGameUI != null)
        {
            startGameUI.SetActive(true);
            // Reset the game state
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.RetryGame();  // This method already exists and handles game reset
            }
        }
    }
    public void SubmitPlayerName()
    {
        if (isSubmitting) return;  // Prevent multiple submissions
        //playerNameInput.interactable = true;
        string playerName = playerNameInput.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            isSubmitting = true;
            playerNameInput.interactable = false;  // Disable input during submission
            submitButton.interactable = false;     // Disable submit button during submission
            
            LeaderboardCreator.UploadNewEntry(Leaderboards.PublicKey, 
                playerName, scoreManager.maxScore, 
                (success) => {
                    if (success)
                    {
                        UpdateLeaderboardUI();
                        // Only hide after successful upload
                        playerNameInput.gameObject.SetActive(false);
                        submitButton.gameObject.SetActive(false);
                    }
                    else
                    {
                        // Re-enable input if submission fails
                        playerNameInput.interactable = true;
                        submitButton.interactable = true;
                    }
                    isSubmitting = false;
                });
        }
    }

    private void UpdateLeaderboardUI()
    {
        LeaderboardCreator.GetLeaderboard(Leaderboards.PublicKey, 
            (entries) => {
                for (int i = 0; i < leaderboardEntries.Length; i++)
                {
                    if (i < entries.Length)
                    {
                        leaderboardEntries[i].text = $"{i + 1}. {entries[i].Username}: {entries[i].Score}";
                    }
                    else
                    {
                        leaderboardEntries[i].text = $"{i + 1}. ---";
                    }
                }
            });
    }
}



