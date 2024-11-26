using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    public int maxScore = 0;
    public Text scoreText;
    public Text scoreText1;
    public int lastScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        maxScore = PlayerPrefs.GetInt("MaxScore", 0);
        UpdateScoreText();
    }

    public void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString() + "\n" + "Max Score: " + maxScore.ToString();
        scoreText1.text = "Score: " + score.ToString() + "\n" + "Max Score: " + maxScore.ToString();
        // Debug.Log("Current Score: " + score);
        // Debug.Log("Max Score: " + maxScore);

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
        lastScore = score; // Store the current score
        UpdateScoreText();
    }

    // public void ResetScore()
    // {
    //     score = 0;
    //     UpdateScoreText();
    // }
}
