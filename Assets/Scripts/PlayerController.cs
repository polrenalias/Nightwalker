// Import necessary libraries
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

// PlayerController class responsible for controlling player behavior
public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float knockbackForce;
    public Animator animator;
    public LayerMask terrainLayer;
    public Rigidbody body;
    public SpriteRenderer sprite;
    public Canvas hud;
    public GameObject healthContainer;
    public GameObject colMessage;
    public AudioClip swordSwing;
    public AudioClip barrierWave;
    public LevelManager manager;
    public Slider barrierBar;

    public int maxLives = 6;
    public float invincibilityDuration = 2f;

    private Color spriteColor;
    private Collider coll;
    private Image[] hearts;
    private AudioSource audioSource;
    private int deathCount;
    private int killCount;
    private int currentLives;

    private bool isInvincible = false;
    private bool isMovingLeft = false;
    private bool isMovingRight = false;
    private bool isJumping = false;
    private bool isDefending = false;
    private bool isAttacking = false;
    private bool isBarrierCasted = false;
    private bool isCollidingWithWall = false;
    private bool alreadyJumping = false;
    private bool alreadyAttacking = false;
    private bool firstTimeDefense = true;
    private float barrierCooldown = 0f;
    private float lastBarrierTime = 0f;
    private float attackCooldown = 1f;
    private float lastAttackTime = 0f;
    
    // Initialize necessary components
    void Start()
    {
        currentLives = maxLives;
        audioSource = GetComponent<AudioSource>();
        body = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponentInChildren<Animator>();
        coll = GetComponent<Collider>();
        FindHearts();
        InitializeHUD();
    }

    // Set the statistics-related values from the LevelManager on start
    void Awake()
    {
        manager.SetLastLevel(1);
        killCount = manager.GetKillCount();
        deathCount = manager.GetDeathCount();
    }

    // Set the statistics-related values from the LevelManager on pause/exit
    void OnApplicationPause()
    {
        manager.SetLastLevel(1);
    }

    // Handle player actions
    void Update()
    {
        WalkHandler();
        JumpHandler();
        AttackHandler();
        DefenseHandler();       
    }

    // Handle player movement
    void WalkHandler()
    {
        body.velocity = new Vector3(body.velocity.x, body.velocity.y, 0f);

        float hAxis = 0f;

        if (isMovingLeft)
        {
            hAxis = -1f;
        }
        else if (isMovingRight)
        {
            hAxis = 1f;
        }

        Vector3 movement = new Vector3(hAxis * speed, 0f, 0f);
        body.MovePosition(transform.position + movement * Time.deltaTime);
        
        if (hAxis < 0)
        {
            sprite.flipX = true;
        }
        else if (hAxis > 0)
        {
            sprite.flipX = false;
        }

        animator.SetFloat("speed", Mathf.Abs(hAxis) * speed);
    }

    // Check if the player is grounded and handle jumping
    void JumpHandler()
    {
        bool isGrounded = CheckGrounded();
        
        if (isJumping && isGrounded && !alreadyJumping)
        {
            alreadyJumping = true;
            body.velocity = new Vector3(body.velocity.x, jumpForce, body.velocity.z);
            animator.SetTrigger("jump");
        }

        if (isGrounded)
        {
            alreadyJumping = false;
        }
    }

    // Handle player's attack
    void AttackHandler()
    {
        bool onGround = CheckGrounded();

        if (isAttacking && !alreadyAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            if (onGround)
            {
                alreadyAttacking = true;
                animator.SetTrigger("attack");
                PlaySwordSound();
                lastAttackTime = Time.time;
                alreadyAttacking = false;
            }
        }
    }

    // Handle player's defense (casting barrier)
    void DefenseHandler()
    {
        if (isDefending && !isBarrierCasted && Time.time - lastBarrierTime >= barrierCooldown && barrierBar.value == 1)
        {
            StartCoroutine(Barrier());
        }
    }

    IEnumerator Barrier()
    {   
        // Activate the barrier and play the barrier sound
        animator.SetTrigger("start_defend");
        isBarrierCasted = true;
        PlayBarrierSound();
        barrierBar.value = 0;
        lastBarrierTime = Time.time; 

        yield return new WaitForSeconds(4f);
        
        // Deactivate the barrier and start the barrier cooldown
        isBarrierCasted = false;
        animator.SetTrigger("stop_defend");
        StartCoroutine(BarrierCooldown());
    }

    // Set the cooldown time for the defense action
    IEnumerator BarrierCooldown()
    {
        if (firstTimeDefense == true)
        {
            barrierCooldown = 12f;
            firstTimeDefense = false;
        }

        float elapsedTime = 0f;
        while (elapsedTime < barrierCooldown)
        {
            elapsedTime += Time.deltaTime;
            float fillAmount = Mathf.Clamp01(elapsedTime / barrierCooldown);
            barrierBar.value = fillAmount;
            yield return null;
        }
        barrierBar.value = 1;
    }

    // Check if the player is grounded by raycasting from the corners of the collider
    bool CheckGrounded()
    {
        float sizeX = coll.bounds.size.x;
        float sizeZ = coll.bounds.size.z;
        float sizeY = coll.bounds.size.y;

        Vector3 corner1 = transform.position + new Vector3(sizeX / 2, -sizeY / 2 + 0.01f, sizeZ / 2);
        Vector3 corner2 = transform.position + new Vector3(-sizeX / 2, -sizeY / 2 + 0.01f, sizeZ / 2);
        Vector3 corner3 = transform.position + new Vector3(sizeX / 2, -sizeY / 2 + 0.01f, -sizeZ / 2);
        Vector3 corner4 = transform.position + new Vector3(-sizeX / 2, -sizeY / 2 + 0.01f, -sizeZ / 2);

        bool grounded1 = Physics.Raycast(corner1, new Vector3(0, -1, 0), 0.01f, terrainLayer);
        bool grounded2 = Physics.Raycast(corner2, new Vector3(0, -1, 0), 0.01f, terrainLayer);
        bool grounded3 = Physics.Raycast(corner3, new Vector3(0, -1, 0), 0.01f, terrainLayer);
        bool grounded4 = Physics.Raycast(corner4, new Vector3(0, -1, 0), 0.01f, terrainLayer);

        return grounded1 || grounded2 || grounded3 || grounded4;
    }

    // Find all the heart images in the health container
    void FindHearts()
    {
        hearts = healthContainer.GetComponentsInChildren<Image>();
    }

    // Initialize the heads-up display (HUD) by setting the barrier bar value to 1 and activating the hearts based on the current number of lives
    void InitializeHUD()
    {
        barrierBar.value = 1;
        foreach (Image heart in hearts)
        {
            heart.gameObject.SetActive(false);
        }
        
        int heartCount = Mathf.Min(currentLives, hearts.Length);
        for (int i = 0; i < heartCount; i++)
        {
            hearts[i].gameObject.SetActive(true);
        }
    }

    // Update the HUD by activating or deactivating hearts based on the current number of lives
    void UpdateHUD()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].gameObject.SetActive(i < currentLives);
        }
    }

    // Play the sound effect for the sword swing
    private void PlaySwordSound()
    {
        audioSource.clip = swordSwing;
        audioSource.Play();
    }

    // Play the sound effect for the energy barrier
    private void PlayBarrierSound()
    {
        audioSource.clip = barrierWave;
        audioSource.Play();
    }

    // Handle the player colliding with an invisible wall by preventing movement in the colliding direction
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("InvisibleWall"))
        {
            Vector3 collisionNormal = collision.contacts[0].normal;

            if (collisionNormal.x > 0)
            {
                SetMovingLeft(false);
            }
            else if (collisionNormal.x < 0)
            {
                SetMovingRight(false);
            }

            body.MovePosition(transform.position + Vector3.zero * Time.deltaTime);
            animator.SetFloat("speed", 0);
        }
    }

    // Display a message when the player is colliding with an invisible wall
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("InvisibleWall"))
        {   
            colMessage.SetActive(true);
        }
    }

    // Stop displaying a message when the player is no longer colliding with an invisible wall
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("InvisibleWall"))
        {
            colMessage.SetActive(false);
        }
    }

    // Handle the player colliding with an enemy by reducing lives, updating the HUD, applying knockback, and starting invincibility frames
    public void HandleEnemyCollision(GameObject enemy)
    {
        if (isBarrierCasted)
        {
            return;
        }
        
        currentLives--;
        UpdateHUD();

        if (currentLives <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Vector3 knockbackDirection = transform.position - enemy.transform.position;
            knockbackDirection.y = 0f;
            knockbackDirection.Normalize();
            body.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            StartCoroutine(InvincibilityFrames());
        }
    }

    // Apply invincibility frames for a duration and flash the player sprite during that time
    IEnumerator InvincibilityFrames()
    {
        Color originalColor = sprite.color;
        isInvincible = true;
        StartCoroutine(Flash());

        yield return new WaitForSeconds(invincibilityDuration);
        sprite.color = originalColor;

        isInvincible = false;
    }

    // Flash the player sprite between transparent and original color to create a visual effect during invincibility frames
    IEnumerator Flash()
    {
        Color originalColor = sprite.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        for (int n = 0; n < 2; n++)
        {
            sprite.color = transparentColor;
            yield return new WaitForSeconds(0.1f);
            sprite.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
        sprite.color = originalColor;
    }

    // Set the flag for moving left
    public void SetMovingLeft(bool value)
    {
        isMovingLeft = value;
    }

    // Set the flag for moving right
    public void SetMovingRight(bool value)
    {
        isMovingRight = value;
    }

    // Set the flag for jumping
    public void SetJumping(bool value)
    {
        isJumping = value;
    }

    // Set the flag for attacking
    public void SetAttacking(bool value)
    {
        isAttacking = value;
    }

    // Set the flag for defending
    public void SetDefending(bool value)
    {
        isDefending = value;
    }
}
