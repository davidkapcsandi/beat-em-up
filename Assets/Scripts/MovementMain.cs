using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool isGrounded;
    float horizontalMove = 0;
    float verticalMove = 0;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            horizontalMove += moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            horizontalMove -= moveSpeed * Time.deltaTime;
        }

        // Vertical movement (left/right with 'A'/'D')
        if (Input.GetKey(KeyCode.W))
        {
            verticalMove += moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            verticalMove -= moveSpeed * Time.deltaTime;
        }

        // Move the object by changing the position directly
        transform.position = new Vector3(horizontalMove, transform.position.y, verticalMove);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isGrounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
