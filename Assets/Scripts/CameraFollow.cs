using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;         // The player object to follow
    public Vector3 offset;           // Offset from the player (usually slightly behind)
    public float smoothSpeed = 0.125f; // Smooth follow speed
    public float zoomSpeed = 0.1f;   // Speed of zooming in and out

    private Vector3 desiredPosition; // The target position the camera will follow
    private Vector3 smoothedPosition; // The actual camera position after smoothing

    private Camera cam;              // Camera component reference
    private float initialOrthographicSize; // For handling zoom (if using orthographic camera)

    void Start()
    {
        cam = Camera.main;
        initialOrthographicSize = cam.orthographicSize;  // Save the initial zoom level
    }

    void LateUpdate()
    {
        // Calculate desired position based on the player's position + offset
        desiredPosition = player.position + offset;

        // Smoothly transition the camera to the desired position
        smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Optional: Zoom in/out (could be based on conditions like nearby enemies or special events)
        AdjustZoom();
    }

    void AdjustZoom()
    {
        // Zoom in if you want to focus on specific moments (e.g., boss fight)
        // You could trigger this with specific events (like proximity to certain triggers)
        if (Input.GetKey(KeyCode.Z))  // Example: zoom in on pressing "Z"
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, initialOrthographicSize - 2f, zoomSpeed);
        }
        else
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, initialOrthographicSize, zoomSpeed);
        }
    }
}