using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class SendToGoogle : MonoBehaviour
{
    private static SendToGoogle instance; // Singleton instance
    private long sessionID;
    private int score;
    private int maxScore;
    private int gamePlayTime;
    private DateTime sessionStartTime;
    private bool dataSent = false; // Flag to prevent multiple sends
    private const float CooldownTime = 10f; // Time to wait before allowing another send
    private float lastSendTime = -10f; // Initialize to a value so the first send is allowed

    void Awake()
    {
        // Ensure only one instance exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            sessionID = DateTime.Now.Ticks;
            sessionStartTime = DateTime.Now;
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    public static SendToGoogle Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("SendToGoogle instance not found.");
            }
            return instance;
        }
    }

    public async void Send(int score)
    {
        if (Time.time - lastSendTime < CooldownTime)
        {
            Debug.Log("Sending data is on cooldown. Please wait.");
            return; // Skip sending if on cooldown
        }

        if (dataSent) // Check if data has already been sent
        {
            Debug.Log("Data has already been sent. Skipping.");
            return;
        }
        
        // Set the flag to true, as we're about to send data
        dataSent = true;

        maxScore = FindObjectOfType<ScoreManager>().maxScore;
        Debug.Log("Score to send: " + score);
        Debug.Log("Max Score to send: " + maxScore);

        gamePlayTime = (int)(DateTime.Now - sessionStartTime).TotalSeconds;

        Debug.Log("Sending Data: SessionID: " + sessionID + ", Score: " + score + ", MaxScore: " + maxScore + ", GamePlayTime: " + gamePlayTime);

        await Post(sessionID.ToString(), score.ToString(), maxScore.ToString(), gamePlayTime.ToString()); // Call async method

        lastSendTime = Time.time; // Update last send time
    }

    // Post method using async/await
    private async Task Post(string sessionID, string score, string maxScore, string gamePlayTime)
    {
        string url = "https://docs.google.com/forms/u/3/d/e/1FAIpQLSctvVdg-nCqTiEFvui64a1mdy0h8lQ-oqPx4UF1IMhsS2AdOw/formResponse";

        WWWForm form = new WWWForm();
        form.AddField("entry.1511569988", sessionID);
        form.AddField("entry.501520240", score);
        form.AddField("entry.963457481", maxScore);
        form.AddField("entry.37702387", gamePlayTime);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            var operation = www.SendWebRequest(); // Send the web request

            while (!operation.isDone) // Wait until the request is done
            {
                await Task.Yield();
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (www.responseCode == 429) // Too Many Requests
                {
                    Debug.Log("Received 429 error. Retrying after delay...");
                    await Task.Delay(10000); // Wait for 10 seconds
                    await Post(sessionID, score, maxScore, gamePlayTime); // Retry sending
                }
                else
                {
                    Debug.Log("Error sending data: " + www.error);
                }
            }
            else
            {
                Debug.Log("Data sent successfully!");
            }
        }
    }

    public void ResetDataSent()
    {
        dataSent = false; // Reset the data sent flag
    }

    void Start()
    {
        // Any initialization logic can go here
    }

    void Update()
    {
        // Update logic can go here if needed
    }
}
