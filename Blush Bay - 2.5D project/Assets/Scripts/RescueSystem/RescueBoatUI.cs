using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RescueBoatUI : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string outsideSceneName = "OutsideScene";
    [SerializeField] private string rescueDiveSceneName = "RescueDiveScene";

    [Header("UI")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button dropNetsButton;
    [SerializeField] private Button goDivingButton;
    [SerializeField] private Button returnOutsideButton;

    private void Awake()
    {
        EnsureRescueManagerExists();
    }

    private void Start()
    {
        EnsureRescueManagerExists();

        if (RescueManager.Instance != null)
        {
            RescueManager.Instance.RegisterBoatSceneOpened();
        }

        if (dropNetsButton != null)
        {
            dropNetsButton.onClick.RemoveAllListeners();
            dropNetsButton.onClick.AddListener(DropNets);
        }

        if (goDivingButton != null)
        {
            goDivingButton.onClick.RemoveAllListeners();
            goDivingButton.onClick.AddListener(GoDiving);
        }

        if (returnOutsideButton != null)
        {
            returnOutsideButton.onClick.RemoveAllListeners();
            returnOutsideButton.onClick.AddListener(ReturnOutside);
        }

        UpdateUI();
    }

    private void EnsureRescueManagerExists()
    {
        if (RescueManager.Instance != null) return;

        RescueManager existingManager = FindAnyObjectByType<RescueManager>();

        if (existingManager != null) return;

        GameObject managerObject = new GameObject("RescueManager");
        managerObject.AddComponent<RescueManager>();

        Debug.Log("RescueManager was missing, so one was created automatically.");
    }

    private void DropNets()
    {
        EnsureRescueManagerExists();

        if (RescueManager.Instance == null)
        {
            if (statusText != null)
            {
                statusText.text = "Rescue system still missing.";
            }

            return;
        }

        RescueManager.Instance.DropNets();
        UpdateUI();
    }

    private void GoDiving()
    {
        EnsureRescueManagerExists();

        if (RescueManager.Instance == null) return;

        if (!RescueManager.Instance.NetsHaveBeenDropped)
        {
            if (statusText != null)
            {
                statusText.text = "You need to drop the rescue nets first.";
            }

            return;
        }

        SceneManager.LoadScene(rescueDiveSceneName);
    }

    private void ReturnOutside()
    {
        SceneManager.LoadScene(outsideSceneName);
    }

    private void UpdateUI()
    {
        EnsureRescueManagerExists();

        if (RescueManager.Instance == null)
        {
            if (statusText != null)
            {
                statusText.text = "Rescue system missing.";
            }

            return;
        }

        bool netsDropped = RescueManager.Instance.NetsHaveBeenDropped;
        bool fishReady = RescueManager.Instance.AnyNetHasFish();

        if (!netsDropped)
        {
            if (statusText != null)
            {
                statusText.text = "The boat is ready. Drop your fish-safe bait nets.";
            }

            if (dropNetsButton != null) dropNetsButton.interactable = true;
            if (goDivingButton != null) goDivingButton.interactable = false;
        }
        else if (fishReady)
        {
            if (statusText != null)
            {
                statusText.text = "The nets are moving. There may be injured fish to rescue.";
            }

            if (dropNetsButton != null) dropNetsButton.interactable = false;
            if (goDivingButton != null) goDivingButton.interactable = true;
        }
        else
        {
            if (statusText != null)
            {
                statusText.text = "The rescue nets have been dropped. Come back later to check them.";
            }

            if (dropNetsButton != null) dropNetsButton.interactable = false;
            if (goDivingButton != null) goDivingButton.interactable = false;
        }
    }
}