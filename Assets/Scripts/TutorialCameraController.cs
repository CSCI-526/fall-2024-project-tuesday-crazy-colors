using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCameraController : MonoBehaviour
{
    public TutorialPlayerController player;
    public Vector3 offset; 

    void Start()
    {
        player = FindObjectOfType<TutorialPlayerController>(); 
        if (player != null)
        {
            offset = transform.position - player.transform.position; 
        }
    }

    void LateUpdate()
    {
        if (player != null) 
        {
            Vector3 desiredPosition = player.transform.position + offset; 
            transform.position = desiredPosition;
        }
    }
}
