using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float walkSpeed;
    public float jumpSpeed;
    public Rigidbody body;
    public Collider col;
    public Animator animator;
    public SpriteRenderer sprite;
    
    bool pressedJump = false;
    
    void Start () {
        body = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        animator = GetComponentInChildren<Animator>();
    }
  
    void Update ()
    {
        WalkHandler();
        JumpHandler();
        AttackHandler();
    }

    void WalkHandler()
    {
        body.velocity = new Vector3(0, body.velocity.y, 0);
        float distance = walkSpeed * Time.deltaTime;
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(hAxis * distance, 0f, vAxis * distance);
        Vector3 currPosition = transform.position;
        Vector3 newPosition = currPosition + movement;
         animator.SetFloat("speed", Mathf.Abs(hAxis) * walkSpeed);

        if (hAxis < 0)
        {
            sprite.flipX = true;
        }
        else if (hAxis > 0)
        {
            sprite.flipX = false;
        }

        body.MovePosition(newPosition);
    }
    void JumpHandler()
    {
        float jAxis = Input.GetAxis("Jump");
        bool isGrounded = CheckGrounded();
   
        if (jAxis > 0f)
        {
            if(!pressedJump && isGrounded)
            {
                pressedJump = true;
                animator.SetTrigger("jump");
                Vector3 jumpVector = new Vector3(0f, jumpSpeed, 0f);
                body.velocity = body.velocity + jumpVector;
            }            
        }
        else
        {
            pressedJump = false;
        }
    }

    void AttackHandler()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            animator.SetTrigger("attack");
        }
    }

    bool CheckGrounded()
    {
        float sizeX = col.bounds.size.x;
        float sizeZ = col.bounds.size.z;
        float sizeY = col.bounds.size.y;
        
        Vector3 corner1 = transform.position + new Vector3(sizeX/2, -sizeY / 2 + 0.01f, sizeZ / 2);
        Vector3 corner2 = transform.position + new Vector3(-sizeX / 2, -sizeY / 2 + 0.01f, sizeZ / 2);
        Vector3 corner3 = transform.position + new Vector3(sizeX / 2, -sizeY / 2 + 0.01f, -sizeZ / 2);
        Vector3 corner4 = transform.position + new Vector3(-sizeX / 2, -sizeY / 2 + 0.01f, -sizeZ / 2);
        
        bool grounded1 = Physics.Raycast(corner1, new Vector3(0, -1, 0), 0.01f);
        bool grounded2 = Physics.Raycast(corner2, new Vector3(0, -1, 0), 0.01f);
        bool grounded3 = Physics.Raycast(corner3, new Vector3(0, -1, 0), 0.01f);
        bool grounded4 = Physics.Raycast(corner4, new Vector3(0, -1, 0), 0.01f);
        
        return (grounded1 || grounded2 || grounded3 || grounded4);
    }
}