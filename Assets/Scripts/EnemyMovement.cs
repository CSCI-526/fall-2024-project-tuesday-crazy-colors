using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 2.0f;
    private float leftBound;
    private float rightBound;
    private bool movingRight = true;

    public void SetMovementBounds(float left, float right)
    {
        leftBound = left;
        rightBound = right;
    }

    void Update()
    {
        if (movingRight)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            if (transform.localPosition.x >= rightBound)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
            if (transform.localPosition.x <= leftBound)
            {
                movingRight = true;
            }
        }
    }
}