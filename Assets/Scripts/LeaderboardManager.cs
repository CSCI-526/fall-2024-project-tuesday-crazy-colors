
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


    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        submitButton.onClick.AddListener(SubmitPlayerName);
        closeButton.onClick.AddListener(HideLeaderboard);
        leaderboardCanvas.SetActive(false);

        playerNameInput.onEndEdit.AddListener((value) => {
        playerNameInput.text = value;  // Preserve the text
        playerNameInput.ActivateInputField();  // Keep the input field active
        });
    }


    // public void ShowLeaderboard()
    // {
    //     leaderboardCanvas.SetActive(true);
    //     EnableInputField();
    //     UpdateLeaderboardUI();
    // }

    public void ShowLeaderboard()
{
    leaderboardCanvas.SetActive(true);
    if (endGameUI != null)
    {
        endGameUI.SetActive(false); // Hide end-game UI during leaderboard display
    }
    EnableInputField();
    UpdateLeaderboardUI();
    
}


private void EnableInputField()
{
    playerNameInput.gameObject.SetActive(true);
    submitButton.gameObject.SetActive(true);
    playerNameInput.text = ""; // Clear input field text
    if (!playerNameInput.isFocused)
    {
        playerNameInput.ActivateInputField(); // Only activate if not already focused
    }
    isSubmitting = false;
}

 

public void HideLeaderboard()
{
    leaderboardCanvas.SetActive(false);
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

    // public void SubmitPlayerName()
    // {
    //     if (isSubmitting) return;  // Prevent multiple submissions
        
    //     string playerName = playerNameInput.text;
    //     if (!string.IsNullOrEmpty(playerName))
    //     {
    //         isSubmitting = true;
    //         LeaderboardCreator.UploadNewEntry(Leaderboards.PublicKey, 
    //             playerName, scoreManager.maxScore, 
    //             (success) => {
    //                 if (success)
    //                 {
    //                     UpdateLeaderboardUI();
    //                     playerNameInput.gameObject.SetActive(false);
    //                     submitButton.gameObject.SetActive(false);
    //                 }
    //                 isSubmitting = false;
    //             });
    //     }
    // }
    public void SubmitPlayerName()
{
    if (isSubmitting) return;  // Prevent multiple submissions
    
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


// using UnityEngine;
// using TMPro;
// using UnityEngine.UI;
// using Dan.Main;
// using System.Collections; 

// public class LeaderboardManager : MonoBehaviour
// {
//     public GameObject leaderboardCanvas;
//     public TMP_InputField playerNameInput;
//     public TextMeshProUGUI[] leaderboardEntries;
//     public Button submitButton;
//     public Button closeButton;
//     private ScoreManager scoreManager;

//     void Start()
//     {
//         scoreManager = FindObjectOfType<ScoreManager>();
//         submitButton.onClick.AddListener(SubmitPlayerName);
//         closeButton.onClick.AddListener(HideLeaderboard);
//         leaderboardCanvas.SetActive(false);

//         // Set up the input field
//         playerNameInput.onDeselect.AddListener(delegate{});
//         playerNameInput.onEndEdit.AddListener(delegate{});
//     }

//     public void ShowLeaderboard()
//     {
//         leaderboardCanvas.SetActive(true);
//         playerNameInput.gameObject.SetActive(true);
//         submitButton.gameObject.SetActive(true);
//         playerNameInput.text = "";
        
//         // Focus the input field
//         playerNameInput.Select();
//         playerNameInput.ActivateInputField();
//     }
//     public void HideLeaderboard()
//     {
//         leaderboardCanvas.SetActive(false);
//     }

//     public void SubmitPlayerName()
//     {
//         string playerName = playerNameInput.text;
//         if (!string.IsNullOrEmpty(playerName))
//         {
//             LeaderboardCreator.UploadNewEntry(Leaderboards.PublicKey, 
//                 playerName, scoreManager.maxScore, 
//                 (success) => {
//                     if (success)
//                     {
//                         UpdateLeaderboardUI();
//                         // Don't hide input field immediately
//                         StartCoroutine(HideInputAfterDelay());
//                     }
//                 });
//         }
//     }

//     private IEnumerator HideInputAfterDelay()
//     {
//         yield return new WaitForSeconds(0.5f);
//         playerNameInput.gameObject.SetActive(false);
//         submitButton.gameObject.SetActive(false);
//     }
//     private void UpdateLeaderboardUI()
//     {
//         LeaderboardCreator.GetLeaderboard(Leaderboards.PublicKey, 
//             (entries) => {
//                 for (int i = 0; i < leaderboardEntries.Length; i++)
//                 {
//                     if (i < entries.Length)
//                     {
//                         leaderboardEntries[i].text = $"{i + 1}. {entries[i].Username}: {entries[i].Score}";
//                     }
//                     else
//                     {
//                         leaderboardEntries[i].text = $"{i + 1}. ---";
//                     }
//                 }
//             });
//     }
// }