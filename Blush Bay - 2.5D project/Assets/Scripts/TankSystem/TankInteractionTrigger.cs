using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankInteractionTrigger : MonoBehaviour
{
    [Header("Tank")]
    [SerializeField] private string tankId = "Tank_1";

    [Header("UI")]
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private string promptMessage = "Press E to view tank";
    [SerializeField] private TankInfoPanelUI tankInfoPanel;

    [Header("Player")]
    [SerializeField] private string playerTag = "Player";

    private bool playerNearby = false;

    private void Start()
    {
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }

        if (tankInfoPanel == null)
        {
            tankInfoPanel = FindAnyObjectByType<TankInfoPanelUI>();
        }
    }

    private void Update()
    {
        if (!playerNearby) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (tankInfoPanel != null)
            {
                tankInfoPanel.OpenPanel(tankId);
            }
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