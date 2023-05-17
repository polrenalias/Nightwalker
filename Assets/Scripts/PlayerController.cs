using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public Animator animator;
    public LayerMask terrainLayer;
    public Rigidbody body;
    public SpriteRenderer sprite;

    private bool isMovingLeft = false;
    private bool isMovingRight = false;
    private bool isJumping = false;
    private bool isAttacking = false;
    private bool alreadyJumping = false;
    private bool isGrounded;
    private Collider coll;

    void Start()
    {
        body = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponentInChildren<Animator>();
        coll = GetComponent<Collider>();
    }

    void Update()
    {
        WalkHandler();
        JumpHandler();
        AttackHandler();
    }

    void WalkHandler()
    {
        body.velocity = new Vector3(body.velocity.x, 0f, 0f);

        float hAxis = 0f;

        if (isMovingLeft)
            hAxis = -1f;
        else if (isMovingRight)
            hAxis = 1f;

        Vector3 movement = new Vector3(hAxis * speed, 0f, 0f);
        body.MovePosition(transform.position + movement * Time.deltaTime);
        animator.SetFloat("speed", Mathf.Abs(hAxis) * speed);

        if (hAxis < 0)
        {
            sprite.flipX = true;
        }
        else if (hAxis > 0)
        {
            sprite.flipX = false;
        }
    }

    void JumpHandler()
    {
        isGrounded = CheckGrounded();

        if (isJumping)
        {
            if (!alreadyJumping && isGrounded)
            {
                alreadyJumping = true;
                body.velocity = body.velocity + new Vector3(0f, jumpForce, 0f);
                animator.SetTrigger("jump");
            }
        }
        else
        {
            alreadyJumping = false;
        }
    }

    void AttackHandler()
    {
        if (isAttacking)
        {
            animator.SetTrigger("attack");
        }
    }

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

    public void HandleEnemyCollision()
    {
        gameObject.SetActive(false);
    }

    public void SetMovingLeft(bool value)
    {
        isMovingLeft = value;
    }

    public void SetMovingRight(bool value)
    {
        isMovingRight = value;
    }

    public void SetJumping(bool value)
    {
        isJumping = value;
    }

    public void SetAttacking(bool value)
    {
        isAttacking = value;
    }
}
