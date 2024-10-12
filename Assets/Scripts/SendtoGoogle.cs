using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class SendtoGoogle : MonoBehaviour
{
    // Start is called before the first frame update

    private long sessionID;
    private int maxScore;
    private int gamePlayTime;
    private DateTime sessionStartTime;

    void Awake()
    {
        sessionID = DateTime.Now.Ticks;
        sessionStartTime = DateTime.Now;
    }

    public void Send()
    {
        maxScore = FindObjectOfType<ScoreManager>().maxScore;
        gamePlayTime = (int)(DateTime.Now - sessionStartTime).TotalSeconds;
        StartCoroutine(Post(sessionID.ToString(), maxScore.ToString(), gamePlayTime.ToString()));
        Debug.Log("Sending Data: SessionID: " + sessionID + ", MaxScore: " + maxScore + ", GamePlayTime: " + gamePlayTime);

    }

    private IEnumerator Post(string sessionID, string maxScore, string gamePlayTime)
    {

        string url = "https://docs.google.com/forms/u/3/d/e/1FAIpQLSfwGMSaI8oODyErRUQ_W3BnZICfg0ZizdI4qMs7vOYBB9YQtg/formResponse";

        WWWForm form = new WWWForm();
        form.AddField("entry.1511569988", sessionID);
        form.AddField("entry.963457481", maxScore);
        form.AddField("entry.37702387", gamePlayTime);


        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error sending data: " + www.error);
            }
            else
            {
                Debug.Log("Data sent successfully");
            }
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.S))
        // {
        //     Send();
        // }
    }
}
