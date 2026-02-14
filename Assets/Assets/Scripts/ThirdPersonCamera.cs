using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;

    [Header("Rotation")]
    public float mouseSensitivity = 3f;
    public float minYAngle = -60f;
    public float maxYAngle = 80f;

    [Header("Zoom")]
    public float distance = 5f;
    public float minDistance = 2f;
    public float maxDistance = 8f;
    public float zoomSpeed = 2f;

    [Header("Collision")]
    public float cameraRadius = 0.3f;
    public LayerMask collisionMask;
    public float collisionOffset = 0.1f;

    private float yaw;
    private float pitch;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    void LateUpdate()
    {
        if (!target)
        {
            TryFindPlayer();
            return;
        }

        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle);

        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 desiredCameraPos =
            target.position + rotation * new Vector3(0f, 0f, -distance);

        Vector3 direction = desiredCameraPos - target.position;
        float targetDistance = direction.magnitude;
        direction.Normalize();

        float finalDistance = targetDistance;

        if (Physics.SphereCast(
            target.position,
            cameraRadius,
            direction,
            out RaycastHit hit,
            targetDistance,
            collisionMask
        ))
        {
            finalDistance = hit.distance - collisionOffset;
        }

        finalDistance = Mathf.Clamp(finalDistance, minDistance, distance);

        transform.position =
            target.position + direction * finalDistance;

        transform.rotation = rotation;
    }

    void TryFindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            target = player.transform;

            yaw = transform.eulerAngles.y;
            pitch = transform.eulerAngles.x;
        }
    }
}
