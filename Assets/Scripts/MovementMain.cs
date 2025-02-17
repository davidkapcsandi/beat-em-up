using UnityEngine;

public class MovementMain : MonoBehaviour
{
    public float speed = 2.0f;

    private Vector3 movementInput;
    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }


    private void Update()
    {
        movementInput.x = Input.GetAxis("Horizontal");
        movementInput.z = Input.GetAxis("Vertical");

    }

}
