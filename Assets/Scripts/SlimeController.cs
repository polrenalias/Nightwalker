// Import necessary libraries
using UnityEngine;

/// <summary>
/// SlimeController class responsible for controlling slime enemy behavior
/// </summary>
public class SlimeController : MonoBehaviour
{
    public LevelManager manager;

    // Reference to the enemy's sprite renderer
    public SpriteRenderer sprite;
    // Movement speed of the enemy
    public float movementSpeed = 5f;
    // Distance at which the enemy changes direction
    public float changeDirectionDistance = 5f;
    // Duration of idle state before changing direction
    public float idleDuration = 2f;
    
    private bool isMovingRight = false;
    private Vector3 originalSpawnPosition;
    private float idleTimer;
    private bool isIdle;
    private Animator animator;

    /// <summary>
    /// Initialize necessary components
    /// </summary>
    private void Start()
    {
        // Store the original spawn position of the enemy
        originalSpawnPosition = transform.position;

        // Reset the idle timer and get the animator component
        ResetIdleTimer();
        animator = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// Handle enemy states
    /// </summary>
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

    /// <summary>
    /// Handle receiving damage
    /// </summary>
    public void TakeDamage()
    {
        // Deactivate the enemy game object
        manager.IncreaseKills();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Resets the timer that determines the duration of the enemy
    /// being still on the movement routine
    /// </summary>
    private void ResetIdleTimer()
    {
        idleTimer = idleDuration;
    }

    /// <summary>
    /// Flips the sprite horizontally when changing directions
    /// </summary>
    private void FlipSprite()
    {
        sprite.flipX = !sprite.flipX;
    }

    /// <summary>
    /// Collision handling for attacking the player
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
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

    /// <summary>
    /// Trigger-type collision handling for receiving attacks from the player
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerSword"))
        {
            // Take damage if triggered by player's sword
            TakeDamage();
        }
    }
}