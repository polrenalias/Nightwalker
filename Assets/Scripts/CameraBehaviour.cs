using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    public float distance = 5f;

    private Vector3 desiredPosition;
    private bool moveCamera = true;

    private void FixedUpdate()
    {
        if (moveCamera)
        {
            desiredPosition = new Vector3(player.position.x, transform.position.y, player.position.z - distance);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
