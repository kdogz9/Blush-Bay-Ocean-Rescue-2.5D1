using UnityEngine;
using UnityEngine.InputSystem;

public class DivePlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float swimSpeed = 4f;
    [SerializeField] private float fastSwimMultiplier = 1.5f;

    [Header("Swim Area Bounds")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = 0.2f;
    [SerializeField] private float maxY = 1f;

    [Header("Depth Lock")]
    [SerializeField] private float lockedZ = 0f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        lockedZ = transform.position.z;
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        Vector3 move = Vector3.zero;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            move.x -= 1f;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            move.x += 1f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            move.y += 1f;

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            move.y -= 1f;

        move = move.normalized;

        float currentSpeed = swimSpeed;

        if (Keyboard.current.leftShiftKey.isPressed)
        {
            currentSpeed *= fastSwimMultiplier;
        }

        transform.position += move * currentSpeed * Time.deltaTime;

        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, lockedZ);

        if (spriteRenderer != null)
        {
            if (move.x > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else if (move.x < -0.01f)
            {
                spriteRenderer.flipX = true;
            }
        }
    }
}