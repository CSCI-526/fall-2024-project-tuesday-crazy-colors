using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class RunMessage : MonoBehaviour
{
    public Button StartGame;  // Reference to the Start Game button
    public GameObject runMessage;


    void Start()
    {
        runMessage.SetActive(false);

        StartGame.onClick.AddListener(ShowRunMessage);
        
    }

    void ShowRunMessage()
    {
        runMessage.SetActive(true);
        StartCoroutine(HideRunMessageAfterTime(2f));
    }

    IEnumerator HideRunMessageAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        runMessage.SetActive(false);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
