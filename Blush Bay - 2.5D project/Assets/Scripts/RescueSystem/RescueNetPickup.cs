using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RescueNetPickup : MonoBehaviour
{
    [Header("Net")]
    [SerializeField] private int netId = 0;

    [Header("Visuals")]
    [SerializeField] private GameObject emptyVisual;
    [SerializeField] private GameObject fullVisual;

    [Header("Prompt")]
    [SerializeField] private TMP_Text promptText;

    private bool playerNearby = false;

    private void Start()
    {
        UpdateVisual();

        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!playerNearby) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            CheckNet();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = true;

        if (promptText != null)
        {
            promptText.gameObject.SetActive(true);
            promptText.text = "Press E to check net";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = false;

        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }

    private void CheckNet()
    {
        if (RescueManager.Instance == null)
        {
            if (promptText != null)
            {
                promptText.text = "Rescue system missing.";
            }

            return;
        }

        bool collected = RescueManager.Instance.TryCollectFishFromNet(
            netId,
            out RescuedFish collectedFish
        );

        if (collected)
        {
            if (promptText != null)
            {
                promptText.text = "Rescued: " + collectedFish.fishName;
            }
        }
        else
        {
            if (promptText != null)
            {
                promptText.text = "This net is empty.";
            }
        }

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (RescueManager.Instance == null)
        {
            if (emptyVisual != null) emptyVisual.SetActive(true);
            if (fullVisual != null) fullVisual.SetActive(false);
            return;
        }

        RescueNet net = RescueManager.Instance.GetNet(netId);

        bool hasFish = net != null && net.hasFish;

        if (emptyVisual != null)
        {
            emptyVisual.SetActive(!hasFish);
        }

        if (fullVisual != null)
        {
            fullVisual.SetActive(hasFish);
        }
    }
}