using UnityEngine;
using UnityEngine.InputSystem;

public class DivePlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float swimSpeed = 4f;

    [Header("Swim Area")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    [SerializeField] private float minY = -3.5f;
    [SerializeField] private float maxY = 3.5f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
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

        transform.position += move * swimSpeed * Time.deltaTime;

        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);

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