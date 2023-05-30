// Import necessary libraries
using UnityEngine;
using System.Collections;

/// <summary>
/// AlterController class responsible for controlling alter ego enemy behavior
/// </summary>
public class AlterController : MonoBehaviour
{
    public LevelManager manager;

    // Reference to the enemy's sprite renderer
    public GameObject character;
    // Reference to the player's transform
    public Transform player;
    // Distance at which the enemy starts following the player
    public float followDistance = 5f;
    // Distance at which the enemy stops following the player
    public float stopFollowDistance = 4f;
    // Distance threshold from the original spawn point at which the enemy stops following the player
    public float followDistanceThreshold = 4f;
    // Distance threshold from the original spawn point at which the enemy should change direction
    public float changeDirectionThreshold = 3f;
    // Movement speed of the enemy
    public float movementSpeed = 5f;

    private bool isReturning = false;
    private SpriteRenderer sprite;
    private AudioSource audioSource;
    private Vector3 originalSpawnPosition;
    private Animator animator;
    private float distanceToSpawnPoint;
    private float distanceToPlayer;
    
    /*public float hitDistance = 1f;
    private bool alreadyAttacking = false;
    private float attackCooldown = 2f;
    private float lastAttackTime = 0f;*/

    /// <summary>
    /// Initialize necessary components
    /// </summary>
    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();

        // Store the original spawn position of the enemy
        originalSpawnPosition = transform.position;

        // Get the audio source component
        audioSource = GetComponent<AudioSource>();

        // Get the animator component
        animator = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// Handle enemy states
    /// </summary>
    void Update()
    {
        // Calculate the distance between the enemy and the player
        distanceToPlayer = Vector3.Distance(transform.position, player.position);
        // Calculate the distance between the enemy and the original spawn point
        distanceToSpawnPoint = Vector3.Distance(transform.position, originalSpawnPosition);
        // Check if the enemy is at the original spawn point
        bool isAtSpawnPoint = distanceToSpawnPoint == 0f;

        if (isAtSpawnPoint || distanceToPlayer <= followDistance)
        {
            if (distanceToSpawnPoint >= followDistanceThreshold || distanceToPlayer > stopFollowDistance)
            {
                // The enemy is far from the original spawn point or the player is too far away, stop following and return to the original point
                StopFollowing();
                ReturnToOriginalPoint();
            }
            else if (distanceToSpawnPoint >= changeDirectionThreshold && distanceToSpawnPoint > followDistanceThreshold + (stopFollowDistance - followDistanceThreshold) * 0.75f)
            {
                // The enemy is close to the original spawn point, but not far enough to change direction, stop following and return to the original point
                StopFollowing();
                ReturnToOriginalPoint();
            }
            else
            {
                // The player is within the follow and stop follow distances, start following
                FollowPlayer();

                /*// Check if the player is within hit distance
                if (distanceToPlayer <= hitDistance)
                {
                    Attack();
                }*/
            }

            // Set the isMoving parameter of the animator based on the movement state
            animator.SetBool("isMoving", !isAtSpawnPoint);
        }
        else
        {
            // The player is outside the follow distance range, stop following and return to the original point
            StopFollowing();
            ReturnToOriginalPoint();
            animator.SetBool("isMoving", false); // Stop playing the moving animation
        }
    }

    /// <summary>
    /// Handles enabling seek-and-destroy behaviour
    /// </summary>
    void FollowPlayer()
    {
        // Reset the return flag if the enemy was previously returning
        if (isReturning && distanceToSpawnPoint <= followDistanceThreshold * 0.50f)
        {
            isReturning = false;
        }

        // Calculate the direction to the player
        Vector3 direction = (player.position - transform.position).normalized;

        // Move the enemy towards the player
        transform.position += direction * movementSpeed * Time.deltaTime;

        // Rotate the enemy based on the movement direction if necessary
        if (direction.x < 0f && character.transform.rotation.eulerAngles.y != -180f)
        {
            character.transform.rotation = Quaternion.Euler(0f, -180f, 0f);
        }
        else if (direction.x > 0f && character.transform.rotation.eulerAngles.y != 0f)
        {
            character.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        // Set the animator parameter to indicate movement
        animator.SetBool("isMoving", true);
    }

    /// <summary>
    /// Handles disabling seek-and-destroy behaviour
    /// </summary>
    void StopFollowing()
    {
        // Set the return flag to indicate that the enemy should return to the original point
        isReturning = true;
    }

    /// <summary>
    /// Makes the enemy return to the original point
    /// </summary>
    void ReturnToOriginalPoint()
    {
        if (isReturning)
        {
            // Calculate the direction to the original point
            Vector3 direction = (originalSpawnPosition - transform.position).normalized;

            // Move the enemy towards the original point
            transform.position += direction * movementSpeed * Time.deltaTime;

            // Rotate the enemy based on the movement direction if necessary
            if (direction.x < 0f && character.transform.rotation.eulerAngles.y != -180f)
            {
                character.transform.rotation = Quaternion.Euler(0f, -180f, 0f);
            }
            else if (direction.x > 0f && character.transform.rotation.eulerAngles.y != 0f)
            {
                character.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }

            // Set the animator parameter to indicate movement
            animator.SetBool("isMoving", true);
        }
        else
        {
            // Set the animator parameter to indicate no movement
            animator.SetBool("isMoving", false);
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

    /*
    void Attack()
    {
        if (!alreadyAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            alreadyAttacking = true;
            
            // Set the animator trigger to indicate attacking
            animator.SetTrigger("attack", true);
            audioSource.Play();
            lastAttackTime = Time.time;
            alreadyAttacking = false;
        }
    }*/

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
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerSword"))
        {
            // Take damage if triggered by player's sword
            TakeDamage();
        }
    }
}