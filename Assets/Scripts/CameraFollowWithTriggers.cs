using UnityEngine;

public class CameraFollowWithTriggers : MonoBehaviour
{
    public Transform player;            // Reference to the player
    public Vector3 offset;              // Offset from the player
    public float smoothSpeed = 0.125f;  // Camera follow smoothness

    private Vector3 desiredPosition;    // The desired camera position
    private Vector3 smoothedPosition;   // The smoothed camera position

    private float minX, maxX, minY, maxY; // The minimum and maximum values for the camera's X and Y boundaries
    private bool isInTriggerZone = false;  // Flag to check if the camera is in a trigger zone

    // Optional: Specify default boundaries (if no triggers are used).
    public float defaultMinX = -10f;
    public float defaultMaxX = 10f;
    public float defaultMinY = -5f;
    public float defaultMaxY = 5f;

    void Start()
    {
        // Set the initial camera boundaries to default
        minX = defaultMinX;
        maxX = defaultMaxX;
        minY = defaultMinY;
        maxY = defaultMaxY;
    }

    void LateUpdate()
    {
        // Follow the player if not in a trigger zone
        if (!isInTriggerZone)
        {
            desiredPosition = player.position + offset;

            // Clamp the camera position within the boundaries
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);

            // Smoothly move the camera to the desired position
            smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }

    // Called when the camera enters a trigger zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CameraTrigger"))  // Check if the trigger is tagged as "CameraTrigger"
        {
            // Set the flag to stop the camera and adjust the camera boundaries
            isInTriggerZone = true;

            // Update camera boundaries based on the trigger's bounds (or set custom values)
            minX = other.bounds.min.x;
            maxX = other.bounds.max.x;
            minY = other.bounds.min.y;
            maxY = other.bounds.max.y;

            // Optionally, clamp the camera's position inside the trigger bounds immediately
            desiredPosition = transform.position;
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
            transform.position = desiredPosition;
        }
    }

    // Called when the camera exits the trigger zone
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CameraTrigger"))
        {
            // Reset to default boundary values when the camera exits the trigger zone
            isInTriggerZone = false;
            minX = defaultMinX;
            maxX = defaultMaxX;
            minY = defaultMinY;
            maxY = defaultMaxY;
        }
    }
}