using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 2.0f;          
    public Transform pointA;            
    public Transform pointB;
    public float size = 0.1f;

    private bool movingRight = true;                 

    void Update()
    {
        
        if (movingRight)
        {
            transform.position = Vector2.MoveTowards(transform.position, pointB.position, speed * Time.deltaTime);
            if (transform.position.x >= pointB.position.x)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, pointA.position, speed * Time.deltaTime);
            if (transform.position.x <= pointA.position.x)
            {
                movingRight = true;
            }
        }
    }
}
