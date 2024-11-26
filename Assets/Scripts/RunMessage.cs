using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class RunMessage : MonoBehaviour
{
    public Button StartGame;  // Reference to the Start Game button
    public GameObject runMessage;
    public GameObject coinsMessage;


    void Start()
    {
        runMessage.SetActive(false);
        coinsMessage.SetActive(false);

        StartGame.onClick.AddListener(ShowMessages);
        
    }

    void ShowMessages()
    {
        runMessage.SetActive(true);
        // coinsMessage.SetActive(true);
        StartCoroutine(HideRunMessageAfterTime(2f));
        // StartCoroutine(HideCoinsMessageAfterTime(5f));
    }

    IEnumerator HideRunMessageAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        runMessage.SetActive(false);

    }

    IEnumerator HideCoinsMessageAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        coinsMessage.SetActive(false);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
