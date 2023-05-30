// Import necessary libraries
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// PlayerController class responsible for controlling player behavior
/// </summary>
public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float knockbackForce;
    public LayerMask terrainLayer;
    public Rigidbody body;
    public GameObject character;
    public GameObject healthContainer;
    public GameObject colMessage;
    public GameObject hitbox;
    public AudioClip swordSwing;
    public AudioClip barrierWave;
    public LevelManager manager;
    public Slider barrierBar;

    public int maxLives = 6;
    public float invincibilityDuration = 2f;

    private SpriteRenderer sprite;
    private Color spriteColor;
    private Collider coll;
    private Image[] hearts;
    private AudioSource audioSource;
    private Animator animator;
    private int killCount;
    private int currentLives;
    private float playTime;

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
    
    /// <summary>
    /// Initialize necessary components
    /// </summary>
    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        body = gameObject.GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        coll = GetComponent<Collider>();

        spriteColor = sprite.color;

        FindHearts();
        InitializeHUD();
    }

    /// <summary>
    /// Set the statistics-related values from the LevelManager before start
    /// </summary>
    void Awake()
    {
        playTime = manager.GetPlayTime();
        currentLives = manager.GetCurrentHealth();
        
        if (currentLives == 0) {
            currentLives = maxLives;
        }
    }

    /// <summary>
    /// Set the statistics-related values from the LevelManager on pause/exit/death
    /// </summary>
    void SetStats()
    {
        manager.SetLastLevel(1);
        manager.SetPlayTime(playTime);
        manager.SetCurrentHealth(currentLives);
    }

    /// <summary>
    /// Handle player actions
    /// </summary>
    void Update()
    {
        playTime += Time.deltaTime;

        if (isInvincible == false) {
            sprite.color = spriteColor;
        }

        WalkHandler();
        JumpHandler();
        AttackHandler();
        DefenseHandler();       
    }

    /// <summary>
    /// Handle player movement
    /// </summary>
    void WalkHandler()
    {
        body.velocity = new Vector3(body.velocity.x, body.velocity.y, 0f);

        float previousHAxis = 0f;
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

        if (hAxis < 0 && character.transform.rotation.eulerAngles.y != -180f)
        {
            character.transform.rotation = Quaternion.Euler(0f, -180f, 0f);
        }
        else if (hAxis > 0 && character.transform.rotation.eulerAngles.y != 0f)
        {
            character.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        previousHAxis = hAxis;

        animator.SetFloat("speed", Mathf.Abs(hAxis) * speed);
    }

    /// <summary>
    /// Check if the player is grounded and handle jumping
    /// </summary>
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

    /// <summary>
    /// Handle player's attack
    /// </summary>
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

    /// <summary>
    /// Handle player's defense (casting barrier)
    /// </summary>
    void DefenseHandler()
    {
        if (isDefending && !isBarrierCasted && Time.time - lastBarrierTime >= barrierCooldown && barrierBar.value == 1)
        {
            StartCoroutine(Barrier());
        }
    }

    /// <summary>
    /// Barrier action handler
    /// </summary>
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

    /// <summary>
    /// Set the cooldown time for the defense action
    /// </summary>
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

    /// <summary>
    /// Check if the player is grounded by raycasting from the corners of the collider
    /// </summary>
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

    /// <summary>
    /// Find all the heart images in the health container
    /// </summary>
    void FindHearts()
    {
        hearts = healthContainer.GetComponentsInChildren<Image>();
    }

    /// <summary>
    /// Initialize the heads-up display (HUD) by setting the barrier bar value to 1 and activating the hearts based on the current number of lives
    /// </summary>
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

    /// <summary>
    /// Update the HUD by activating or deactivating hearts based on the current number of lives
    /// </summary>
    void UpdateHUD()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].gameObject.SetActive(i < currentLives);
        }
    }

    /// <summary>
    /// Play the sound effect for the sword swing
    /// </summary>
    private void PlaySwordSound()
    {
        audioSource.clip = swordSwing;
        audioSource.Play();
    }

    /// <summary>
    /// Play the sound effect for the energy barrier
    /// </summary>
    private void PlayBarrierSound()
    {
        audioSource.clip = barrierWave;
        audioSource.Play();
    }

    /// <summary>
    /// Handle the player colliding with an invisible wall by preventing movement in the colliding direction
    /// </summary>
    /// <param name="collision"></param>
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

    /// <summary>
    /// Display a message when the player is colliding with an invisible wall
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("InvisibleWall"))
        {   
            colMessage.SetActive(true);
        }
    }

    /// <summary>
    /// Stop displaying a message when the player is no longer colliding with an invisible wall
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("InvisibleWall"))
        {
            colMessage.SetActive(false);
        }
    }

    /// <summary>
    /// Handle the player colliding with an enemy by reducing lives, updating the HUD, applying knockback, and starting invincibility frames
    /// </summary>
    /// <param name="enemy"></param>
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
            manager.IncreaseDeaths();
            gameObject.SetActive(false);
            SetStats();
            SceneManager.LoadScene("GameOver");
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

    /// <summary>
    /// Apply invincibility frames for a duration and flash the player sprite during that time
    /// </summary>
    /// <returns></returns>
    IEnumerator InvincibilityFrames()
    {
        Color originalColor = sprite.color;
        isInvincible = true;
        yield return StartCoroutine(Flash());

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
    }

    /// <summary>
    /// Flash the player sprite between transparent and original color to create a visual effect during invincibility frames
    /// </summary>
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

    /// <summary>
    /// Set the flag for moving left
    /// </summary>
    /// <param name="value"></param>
    public void SetMovingLeft(bool value)
    {
        isMovingLeft = value;
    }

    /// <summary>
    /// Set the flag for moving right
    /// </summary>
    /// <param name="value"></param>
    public void SetMovingRight(bool value)
    {
        isMovingRight = value;
    }

    /// <summary>
    /// Set the flag for jumping
    /// </summary>
    /// <param name="value"></param>
    public void SetJumping(bool value)
    {
        isJumping = value;
    }

    /// <summary>
    /// Set the flag for attacking
    /// </summary>
    /// <param name="value"></param>
    public void SetAttacking(bool value)
    {
        isAttacking = value;
    }

    /// <summary>
    /// Set the flag for defending
    /// </summary>
    /// <param name="value"></param>
    public void SetDefending(bool value)
    {
        isDefending = value;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemySword"))
        {
            if (isBarrierCasted)
            {
                return;
            }
            
            currentLives--;
            UpdateHUD();

            if (currentLives <= 0)
            {
                manager.IncreaseDeaths();
                gameObject.SetActive(false);
                SetStats();
                SceneManager.LoadScene("GameOver");
            }
            else
            {
                Vector3 knockbackDirection = transform.position - other.transform.position;
                knockbackDirection.y = 0f;
                knockbackDirection.Normalize();
                body.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
                StartCoroutine(InvincibilityFrames());
            }
        }
    }*/

    /// <summary>
    /// Sets stats on pause
    /// </summary>
    void OnApplicationPause(bool pauseStatus) {
        if (pauseStatus == true) {
            SetStats();
        }
    }

    /// <summary>
    /// Sets stats on exit
    /// </summary>
    void OnApplicationQuit() {
        SetStats();
    }

    /// <summary>
    /// Sets stats on death
    /// </summary>
    void OnDestroy() {
        SetStats();
    }
}
