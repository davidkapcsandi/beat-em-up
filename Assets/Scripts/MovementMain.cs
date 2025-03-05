using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool isGrounded;
    public Transform camtransform;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
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

        // Jumping logic
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);  // Apply jump force only on Y-axis
        isGrounded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}