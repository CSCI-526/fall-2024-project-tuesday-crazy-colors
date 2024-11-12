using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class DeathAnalytics : MonoBehaviour
{
    public static DeathAnalytics instance;
    private long sessionID;
    private bool deathEnemy;
    private bool deathColor;
    private bool deathPlatform;
    private float cooldownTime = 10f; // Adjust the cooldown duration as needed

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            sessionID = DateTime.Now.Ticks;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public async void DeathLog(bool diedFromEnemy, bool diedFromColor, bool diedFromPlatform, int enemyKillCount, string sessionTime)
    {
        deathEnemy = diedFromEnemy;
        deathColor = diedFromColor;
        deathPlatform = diedFromPlatform;

        Debug.Log($"Death Analytics - Enemy: {deathEnemy}, Color: {deathColor}, Platform: {deathPlatform}");

        await Post(sessionID.ToString(), deathColor.ToString(), deathEnemy.ToString(), deathPlatform.ToString(), enemyKillCount.ToString(), sessionTime);

        // Start cooldown
        await Task.Delay(TimeSpan.FromSeconds(cooldownTime));
        Debug.Log("Cooldown completed, ready to log death again.");
    }

    // public async void DeathLog(bool diedFromEnemy, bool diedFromColor, bool diedFromPlatform) //call this function somewhere
    // {
    //     if (hasLoggedDeath) return; // Prevent logging more than once

    //     hasLoggedDeath = true; // Mark as logged
    //     deathEnemy = diedFromEnemy;
    //     deathColor = diedFromColor;
    //     deathPlatform = diedFromPlatform;

    //     Debug.Log($"Death Analytics - Enemy: {deathEnemy}, Color: {deathColor}, Platform: {deathPlatform}");

    //     Post(sessionID.ToString(), deathColor.ToString(), deathEnemy.ToString(), deathPlatform.ToString(), enemyKillCount.ToString(), sessionTime);
    // }

    private async Task Post(string sessionID, string deathColor, string deathEnemy, string deathPlatform, string enemyKillCount, string sessionTime)
    {
        Debug.Log($"Enemies Killed: {enemyKillCount}");
        Debug.Log($"Session Time: {sessionTime}");

        string url = "https://docs.google.com/forms/u/1/d/e/1FAIpQLSfJZj4Q5X-65JhUFUVRbG9Ns0kfnFqNnTFiF_iUoSGThT2T5A/formResponse";

        WWWForm form = new WWWForm();
        form.AddField("entry.1692969652", sessionID.ToString());
        form.AddField("entry.1603297951", deathColor);
        form.AddField("entry.1751409824", deathEnemy);
        form.AddField("entry.1151262886", deathPlatform);
        form.AddField("entry.1366656562", enemyKillCount.ToString());
        form.AddField("entry.2061567090", sessionTime.ToString());

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
                    await Post(sessionID, deathColor, deathEnemy, deathPlatform, enemyKillCount, sessionTime); // Retry sending
                }
                else
                {
                    Debug.Log("Error sending data: " + www.error);
                }
            }
            else
            {
                Debug.Log("Death Data sent successfully!");
            }
        }
    }
}