using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject goalObject;
    public PlayerController PlayerController;

    private void Start()
    {
        if (goalObject != null)
        {
            goalObject.GetComponent<Collider>().isTrigger = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void CheckGoalReached()
    {
        Debug.Log("In tutorial");
        SceneManager.LoadScene("Endless");
        // if (SceneManager.GetActiveScene().name == "Endless")
        // {
        //     PlayerController.EndGame();
        // }
        // else
        // {
        //     SceneManager.LoadScene("end");
        // }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}