using UnityEngine;

public class CameraFollowVolume : MonoBehaviour
{
    [Header("What the camera follows")]
    [SerializeField] private Transform target;

    [Header("Camera movement")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -8f);
    [SerializeField] private float followSpeed = 10f;

    [Header("Camera bounds volume")]
    [SerializeField] private BoxCollider cameraBounds;

    [Header("Camera rotation")]
    [SerializeField] private Vector3 cameraRotation = new Vector3(45f, 0f, 0f);

    private void LateUpdate()
    {
        if (target == null || cameraBounds == null)
            return;

        // Where the camera wants to go
        Vector3 desiredPosition = target.position + offset;

        // Get the allowed 3D area from the BoxCollider
        Bounds bounds = cameraBounds.bounds;

        // Clamp the camera position so it stays inside the box
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, bounds.min.x, bounds.max.x);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, bounds.min.y, bounds.max.y);
        desiredPosition.z = Mathf.Clamp(desiredPosition.z, bounds.min.z, bounds.max.z);

        // Smooth move
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        // Keep the camera angle fixed
        transform.rotation = Quaternion.Euler(cameraRotation);
    }

    // Optional helper if you forget to assign the player
    private void Start()
    {
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                target = playerObject.transform;
            }
        }
    }
}