using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float despawnX = -10f;
    public float changeDirectionDistance = 5f;
    public float idleDuration = 2f;

    private bool isMovingRight = false;
    private Vector3 originalSpawnPosition;
    private float idleTimer;
    private bool isIdle;
    private Animator animator;

    private void Start()
    {
        originalSpawnPosition = transform.position;
        ResetIdleTimer();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (isIdle)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                isIdle = false;
                ResetIdleTimer();
                animator.SetBool("isMoving", true);
            }
        }
        else
        {
            // Move the enemy in the desired direction
            Vector3 movement = (isMovingRight ? Vector3.right : Vector3.left) * movementSpeed * Time.deltaTime;
            transform.Translate(movement);

            // Check if the enemy should change direction
            float distanceFromSpawn = Mathf.Abs(transform.position.x - originalSpawnPosition.x);
            if (distanceFromSpawn >= changeDirectionDistance)
            {
                isMovingRight = !isMovingRight;
                isIdle = true;
                animator.SetBool("isMoving", false);
            }
        }

        // Check if the enemy should be despawned
        if (transform.position.x < despawnX)
        {
            Destroy(gameObject);
        }
    }

    private void ResetIdleTimer()
    {
        idleTimer = idleDuration;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Handle the collision with the player
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.HandleEnemyCollision();
            }
        }
    }
}
