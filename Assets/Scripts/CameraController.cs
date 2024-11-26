using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerController player;
    public Vector3 offset; 

    void Start()
    {
        player = FindObjectOfType<PlayerController>(); 
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
