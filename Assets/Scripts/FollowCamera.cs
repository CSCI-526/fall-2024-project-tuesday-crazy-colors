using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform cameraTransform; // Assign the camera in the Inspector
    public Vector3 offset; // Set the initial offset distance in the Inspector
    public float followSpeed = 1.0f; // Speed at which the clouds follow the camera
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.8f;
    void LateUpdate()
    {
        if (cameraTransform != null)
        {
            // Calculate the target position for the clouds
            Vector3 targetPosition = cameraTransform.position + offset;

            // Move the cloud smoothly to follow the camera
            // transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}
