using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneInteractionPortal : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string sceneToLoad = "OutsideScene";

    [Header("Prompt")]
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private string promptMessage = "Press E to go outside";

    [Header("Player")]
    [SerializeField] private string playerTag = "Player";

    private bool playerNearby = false;
    private bool isLoading = false;

    private void Start()
    {
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!playerNearby) return;
        if (isLoading) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            isLoading = true;
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerNearby = true;

        if (promptText != null)
        {
            promptText.gameObject.SetActive(true);
            promptText.text = promptMessage;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerNearby = false;

        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }
}