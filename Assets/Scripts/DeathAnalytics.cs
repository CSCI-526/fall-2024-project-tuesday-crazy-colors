using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class DeathAnalytics : Monobehavior
{
    private static DeathAnalytics instance;
    private long sessionID;
    private bool deathEnemy;
    private bool deathColor;
    private bool deathPlatfrom;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LogDeath(bool diedFromEnemy, bool diedFromColor, bool diedFromPlatform)
    {
        deathEnemy = diedFromEnemy;
        deathColor = diedFromColor;
        deathPlatform = diedFromPlatform;

        // Here you would send the data to your analytics system
        Debug.Log($"Death Analytics - Enemy: {deathEnemy}, Color: {deathColor}, Platform: {deathPlatform}");
    }

    private async Task Post(string sessionID, string deathColor, string deathEnemy, string deathPlatform)
    {

        string url = "https://docs.google.com/forms/u/1/d/e/1FAIpQLSfJZj4Q5X-65JhUFUVRbG9Ns0kfnFqNnTFiF_iUoSGThT2T5A/formResponse";

        WWForm form = new WWWForm();
        form.AddField("entry.1692969652", sessionID);
        form.AddField("entry.1603297951", deathColor);
        form.AddField("entry.1751409824", deathEnemy);
        form.AddField("entry.1151262886", deathPlatform);

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
}