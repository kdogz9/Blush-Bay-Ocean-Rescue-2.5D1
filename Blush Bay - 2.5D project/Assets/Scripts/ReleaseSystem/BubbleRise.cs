using UnityEngine;

public class BubbleRise : MonoBehaviour
{
    [Header("Bubble Movement")]
    [SerializeField] private float riseSpeed = 0.5f;

    [Header("Ocean Reference")]
    [SerializeField] private SpriteRenderer oceanBackground;

    private Vector3 startPosition;

    private void Start()
    {
        // Remember where the bubble started.
        startPosition = transform.position;
    }

    private void Update()
    {
        // Move the bubble upwards every frame.
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        // Check whether the bubble has reached the surface.
        if (oceanBackground != null &&
            transform.position.y >= oceanBackground.bounds.max.y)
        {
            // Reset it to the starting position for now.
            transform.position = startPosition;
        }
    }
}