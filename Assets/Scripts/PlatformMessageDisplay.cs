using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlatformMessageDisplay : MonoBehaviour
{
    public TMP_Text messageLabel; // Assign this in the Inspector
    private int platformCount = 0;

    // Method to call each time a platform is generated
    public void PlatformGenerated()
    {
        platformCount++;
        
        // Check if platform count is a multiple of 5
        if (platformCount % 5 == 0)
        {
            ShowMessage("Good Work!");
        }
    }

    // Show the message and reset it after a short delay
    private void ShowMessage(string message)
    {
        messageLabel.text = message;
        StartCoroutine(ResetMessage());
    }

    private System.Collections.IEnumerator ResetMessage()
    {
        yield return new WaitForSeconds(2f); // Display for 2 seconds
        messageLabel.text = ""; // Clear the message
    }
}
