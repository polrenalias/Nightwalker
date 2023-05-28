// Import necessary libraries
using UnityEngine;

// EnemyController class responsible for controlling enemy behavior
public class EnemyController : MonoBehaviour
{
    // Reference to the enemy's sprite renderer
    public SpriteRenderer sprite;

    // Movement speed of the enemy
    public float movementSpeed = 5f;

    // X position at which the enemy despawns
    public float despawnX = -10f;

    // Distance at which the enemy changes direction
    public float changeDirectionDistance = 5f;

    // Duration of idle state before changing direction
    public float idleDuration = 2f;

    // Direction check
    private bool isMovingRight = false;

    private Vector3 originalSpawnPosition;
    private float idleTimer;
    private bool isIdle;
    private Animator animator;

    private void Start()
    {
        // Store the original spawn position of the enemy
        originalSpawnPosition = transform.position;

        // Reset the idle timer and get the animator component
        ResetIdleTimer();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (isIdle)
        {
            // Countdown the idle timer
            idleTimer -= Time.deltaTime;

            if (idleTimer <= 0f)
            {
                // Flip the sprite, exit idle state, reset idle timer, and start moving
                FlipSprite();
                isIdle = false;
                ResetIdleTimer();
                animator.SetBool("isMoving", true);
            }
        }
        else
        {
            // Move the enemy in the desired direction based on the current movement state
            Vector3 movement = (isMovingRight ? Vector3.right : Vector3.left) * movementSpeed * Time.deltaTime;
            transform.Translate(movement);

            // Check if the enemy should change direction
            float distanceFromSpawn = Mathf.Abs(transform.position.x - originalSpawnPosition.x);
            if (distanceFromSpawn >= changeDirectionDistance)
            {
                // Change direction, enter idle state, and stop moving
                isMovingRight = !isMovingRight;
                isIdle = true;
                animator.SetBool("isMoving", false);
            }
        }
    }

    public void TakeDamage()
    {
        // Deactivate the enemy game object
        gameObject.SetActive(false);
    }

    private void ResetIdleTimer()
    {
        // Reset the idle timer to the specified duration
        idleTimer = idleDuration;
    }

    private void FlipSprite()
    {
        // Flip the sprite horizontally
        sprite.flipX = !sprite.flipX;
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*if (collision.gameObject.CompareTag("PlayerSword"))
        {
            // Take damage if collided with player's sword
            TakeDamage();
        }*/

        // Check if the collision is with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Handle the collision with the player
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.HandleEnemyCollision(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerSword"))
        {
            // Take damage if triggered by player's sword
            TakeDamage();
        }
    }
}