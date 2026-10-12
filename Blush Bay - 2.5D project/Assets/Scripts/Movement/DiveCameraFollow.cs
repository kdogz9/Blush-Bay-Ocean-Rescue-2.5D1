using UnityEngine;

public class DiveCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Follow")]
    [SerializeField] private Vector3 offset = new Vector3(0f, -0.5f, -10f);
    [SerializeField] private float followSpeed = 5f;

    [Header("Follow Axes")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;

    [Header("Camera Side Limits")]
    [SerializeField] private Transform leftCameraLimit;
    [SerializeField] private Transform rightCameraLimit;
    [SerializeField] private bool useSideLimits = true;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = transform.position;

        if (followX)
        {
            desiredPosition.x = target.position.x + offset.x;
        }

        if (followY)
        {
            desiredPosition.y = target.position.y + offset.y;
        }

        desiredPosition.z = startPosition.z;

        if (useSideLimits && leftCameraLimit != null && rightCameraLimit != null)
        {
            float leftX = leftCameraLimit.position.x;
            float rightX = rightCameraLimit.position.x;

            desiredPosition.x = Mathf.Clamp(desiredPosition.x, leftX, rightX);
        }

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );
    }
}