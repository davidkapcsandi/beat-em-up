using System;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float jumpForce = 5f;
    public bool debugCombat;
    private Rigidbody rb;
    private bool isGrounded;
    public Transform camtransform;

    public Animator playerAnimator;
    private SpriteRenderer playerSprite;


    void Start()
    {
        SetupPlayer();
    }

    void SetupPlayer()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        playerAnimator = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();

    }

    void Update()
    {

        HandleMovement();
        HandleAnimations();
        HandleFlip();
        HandleCombat();
        
     
        // Jumping logic
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }
    void HandleFlip()
    {
        if (rb.velocity.x < 0)
        {
            playerSprite.flipX = true;
        }
        else
        {
            playerSprite.flipX = false;

        }
        

    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);  // Apply jump force only on Y-axis
        isGrounded = false;
    }

    void HandleCombat()
    {

        if(Input.GetKeyDown(KeyCode.R))
        {
            if(debugCombat)
            {
                Debug.Log("Punch");

            }
            playerAnimator.SetTrigger("LightPunch");
            
        }
        else
        {
            playerAnimator.ResetTrigger("LightPunch");

        }


    }
    void KickCombat()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (debugCombat)
            {
                Debug.Log("Kick");

            }
            playerAnimator.SetTrigger("LightKick");

        }
        else
        {
            playerAnimator.ResetTrigger("LightKick");

        }
    }

    void HandleAnimations()
    {
        if(rb.velocity.x > 0 | rb.velocity.x <0)
        {

        playerAnimator.SetBool("Walk",true);
         playerAnimator.SetBool("Idle",true);

        }
        else
        {
         playerAnimator.SetBool("Walk", false);
         playerAnimator.SetBool("Idle",true);

        }
       


    }

    void HandleMovement()
    {
           // Horizontal movement (left/right with 'A'/'D')
        float horizontalMove = 0f;
        if (Input.GetKey(KeyCode.D))
        {
            horizontalMove = moveSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            horizontalMove = -moveSpeed;
        }

        // Vertical movement (forward/backward with 'W'/'S')
        float verticalMove = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            verticalMove = moveSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            verticalMove = -moveSpeed;
        }

        // Apply movement to Rigidbody (we only care about X and Z for movement in 2.5D)
        Vector3 moveDirection = new Vector3(horizontalMove, 0, verticalMove);
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);  // Keep the y-velocity for gravity/jumping


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}