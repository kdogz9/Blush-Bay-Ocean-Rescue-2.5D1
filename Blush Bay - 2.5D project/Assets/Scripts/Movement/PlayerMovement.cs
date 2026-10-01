using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private CharacterController controller;
    private Vector3 moveDirection;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        moveDirection = Vector3.zero;

        if (Keyboard.current == null) return;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
        {
            moveDirection.z += 1f;
        }

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        {
            moveDirection.z -= 1f;
        }

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            moveDirection.x -= 1f;
        }

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            moveDirection.x += 1f;
        }

        moveDirection = moveDirection.normalized;

        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Flip only the sprite image, not the whole player object.
        if (spriteRenderer != null)
        {
            if (moveDirection.x > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else if (moveDirection.x < -0.01f)
            {
                spriteRenderer.flipX = true;
            }
        }
    }
}