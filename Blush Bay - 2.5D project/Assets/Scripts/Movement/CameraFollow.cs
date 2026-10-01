using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 7f, -7f);
    [SerializeField] private float followSpeed = 5f;

    [Header("Camera Bounds")]
    [SerializeField] private BoxCollider cameraBounds;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 focusPosition = target.position;

        if (cameraBounds != null)
        {
            Bounds bounds = cameraBounds.bounds;

            focusPosition.x = Mathf.Clamp(
                focusPosition.x,
                bounds.min.x,
                bounds.max.x
            );

            focusPosition.z = Mathf.Clamp(
                focusPosition.z,
                bounds.min.z,
                bounds.max.z
            );
        }

        Vector3 targetCameraPosition = focusPosition + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetCameraPosition,
            followSpeed * Time.deltaTime
        );
    }
}