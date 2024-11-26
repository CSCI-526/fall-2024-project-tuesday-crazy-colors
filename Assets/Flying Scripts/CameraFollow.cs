using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform
    public float smoothSpeed = 0.125f;  // Smoothing speed for the camera's movement
    public Vector3 offset;  // The offset between the camera and the player

    void Start()
    {
        // Set the initial offset if it's not manually set in the Inspector
        if (offset == Vector3.zero)
        {
            offset = transform.position - player.position;
        }
    }

    void FixedUpdate()
    {
        // Calculate the desired position with the offset
        Vector3 desiredPosition = player.position + offset;

        // Smoothly interpolate between the current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;
    }
}
