// 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dan.Main;
using Dan.Models;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    public int maxScore = 0;
    public Text scoreText;
    public Text scoreText1;
    public int lastScore = 0;

    void Start()
    {
        maxScore = PlayerPrefs.GetInt("MaxScore", 0);
        UpdateScoreText();
    }

    public void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString() + "\n" + "Max Score: " + maxScore.ToString();
        scoreText1.text = "Score: " + score.ToString() + "\n" + "Max Score: " + maxScore.ToString();

        if (score > maxScore)
        {
            maxScore = score;
            scoreText.text = "Score: " + score.ToString() + "\n" + "Max Score: " + maxScore.ToString();
            scoreText1.text = "Score: " + score.ToString() + "\n" + "Max Score: " + maxScore.ToString();
            PlayerPrefs.SetInt("MaxScore", maxScore);
        }
    }

    public void SaveScore()
    {
        lastScore = score;
        if (score > maxScore)
        {
            maxScore = score;
            PlayerPrefs.SetInt("MaxScore", maxScore);
        }
        UpdateScoreText();
    }

    public void AddScoreToLeaderboard(string playerName)
    {
        LeaderboardCreator.UploadNewEntry(Leaderboards.PublicKey, playerName, maxScore);
    }

    public void GetLeaderboard(System.Action<Entry[]> callback)
    {
        LeaderboardCreator.GetLeaderboard(Leaderboards.PublicKey, callback);
    }
}