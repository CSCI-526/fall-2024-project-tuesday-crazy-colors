using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public enum PlatformBehavior { Static, MoveVertically, ShrinkAndGrowHorizontally, SeeSaw, TutorialSeeSaw }
    public PlatformBehavior platformBehavior;

    public float rotationSpeed = 1.0f; // Rotation speed for see-saw
    public float targetRotation; // Target angle for see-saw rotation

    private float moveSpeed;
    private float initialYPosition;
    private Vector3 initialScale;

    public float movementRange = 2.0f;
    public float shrinkGrowSpeed = 2.0f;

    void Start()
    {
        initialYPosition = transform.position.y;
        initialScale = transform.localScale;
        moveSpeed = Random.Range(1f, 5f);
        targetRotation = 0f; 
    }

    void Update()
    {
        switch (platformBehavior)
        {
            case PlatformBehavior.MoveVertically:
                MoveVertically();
                break;
            case PlatformBehavior.ShrinkAndGrowHorizontally:
                ShrinkAndGrowHorizontally();
                break;
            case PlatformBehavior.SeeSaw:
                ApplySeeSawRotation();
                break;
            case PlatformBehavior.Static:
            default:
                // No movement for static behavior
                break;
        }
    }

    public void SetBehavior(PlatformBehavior behavior)
    {
        platformBehavior = behavior;
    }

    public PlatformBehavior GetBehavior()
    {
        return platformBehavior;
    }

    private void MoveVertically()
    {
        float newY = initialYPosition + Mathf.Sin(Time.time * moveSpeed) * movementRange;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void ShrinkAndGrowHorizontally()
    {
        float scaleX = initialScale.x + Mathf.Sin(Time.time * shrinkGrowSpeed) * (initialScale.x - 2.0f) / 2.0f;
        transform.localScale = new Vector3(Mathf.Clamp(scaleX, 2.0f, initialScale.x), initialScale.y, initialScale.z);
    }
    public float currentRotation;
    private void ApplySeeSawRotation()
    {
        currentRotation = Mathf.LerpAngle(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);
    }

    public void AdjustSeeSawRotation(bool isLandingLeft)
    {
        targetRotation = isLandingLeft ? 75f : -75f; // Rotate 15 degrees in respective direction
    }
}
