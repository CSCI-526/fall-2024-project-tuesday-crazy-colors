using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    private float moveSpeed;
    private float initialYPosition;
    public float movementRange = 2.0f; //Height

    void Start()
    {
        //speed
        moveSpeed = Random.Range(0f, 5f);
        
        //original position
        initialYPosition = transform.position.y;
    }

    void Update()
    {
        //move up-down
        float newY = initialYPosition + Mathf.Sin(Time.time * moveSpeed) * movementRange;
        //New position
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
