// Import necessary libraries
using UnityEngine;

/// <summary>
/// CameraBehaviour class responsible for controlling the camera's position
/// </summary>
public class CameraBehaviour : MonoBehaviour
{
    // Reference to the player's transform
    public Transform player;

    // Smoothing speed for camera movement
    public float smoothSpeed = 0.125f;

    // Distance between the camera and the player
    public float distance = 5f;

    // Desired position for the camera
    private Vector3 desiredPosition;

    // Flag indicating if the camera should move
    private bool moveCamera = true;

    /// <summary>
    /// Handles camera movement, is called at a fixed interval
    /// </summary>
    private void FixedUpdate()
    {
        // Check if the camera should move
        if (moveCamera)
        {
            // Calculate the desired position for the camera
            desiredPosition = new Vector3(player.position.x, transform.position.y, player.position.z - distance);

            // Smoothly move the camera towards the desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}