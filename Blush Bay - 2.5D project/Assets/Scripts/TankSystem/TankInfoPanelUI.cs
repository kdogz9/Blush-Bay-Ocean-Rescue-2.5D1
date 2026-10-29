
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class TankFishPreviewEntry
{
    public string fishName;
    public Sprite fishSprite;
}

public class TankInfoPanelUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Text")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text fishInfoText;

    [Header("Fish Preview")]
    [SerializeField] private Image fishPreviewImage;
    [SerializeField] private List<TankFishPreviewEntry> fishPreviews =
        new List<TankFishPreviewEntry>();

    [Header("Health UI")]
    [SerializeField] private Slider healthSlider;

    [Header("Buttons")]
    [SerializeField] private Button treatButton;
    [SerializeField] private Button releaseButton;
    [SerializeField] private Button closeButton;

    private string currentTankId;
    private RescuedFish currentFish;

    private void Start()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(ClosePanel);
        }

        if (treatButton != null)
        {
            treatButton.onClick.RemoveAllListeners();
            treatButton.onClick.AddListener(StartTreatment);
        }

        if (releaseButton != null)
        {
            releaseButton.onClick.RemoveAllListeners();
            releaseButton.onClick.AddListener(StartRelease);
        }

        if (fishPreviewImage != null)
        {
            fishPreviewImage.enabled = false;
        }

        UpdateHealthUI();
        UpdateButtons();
    }

    public void OpenPanel(string tankId)
    {
        currentTankId = tankId;
        currentFish = null;

        if (panel != null)
        {
            panel.SetActive(true);
        }

        RefreshPanel();
    }

    public void ClosePanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        currentTankId = "";
        currentFish = null;

        if (fishPreviewImage != null)
        {
            fishPreviewImage.enabled = false;
        }

        UpdateHealthUI();
        UpdateButtons();
    }

    private void RefreshPanel()
    {
        if (titleText != null)
        {
            titleText.text =
                currentTankId.Replace("_", " ").ToUpper();
        }

        if (RescueManager.Instance == null)
        {
            currentFish = null;

            if (fishInfoText != null)
            {
                fishInfoText.text = "RESCUE SYSTEM MISSING.";
            }

            if (fishPreviewImage != null)
            {
                fishPreviewImage.enabled = false;
            }

            UpdateHealthUI();
            UpdateButtons();
            return;
        }

        List<RescuedFish> fishInTank =
            RescueManager.Instance.GetFishInTank(currentTankId);

        if (fishInTank == null || fishInTank.Count == 0)
        {
            currentFish = null;

            if (fishInfoText != null)
            {
                fishInfoText.text =
                    "NO RESCUED FISH IN THIS TANK YET.";
            }

            if (fishPreviewImage != null)
            {
                fishPreviewImage.enabled = false;
            }

            UpdateHealthUI();
            UpdateButtons();
            return;
        }

        // Currently displays the first fish in the tank.
        currentFish = fishInTank[0];

        if (fishInfoText != null)
        {
            fishInfoText.text =
                currentFish.fishName.ToUpper() +
                "\n\nCONDITION:\n" +
                currentFish.injuryType.ToUpper();
        }

        UpdateFishPreview(currentFish.fishName);
        UpdateHealthUI();
        UpdateButtons();
    }

    private void UpdateHealthUI()
    {
        if (healthSlider == null) return;

        if (currentFish == null)
        {
            healthSlider.gameObject.SetActive(false);
            return;
        }

        healthSlider.gameObject.SetActive(true);
        healthSlider.minValue = 0f;
        healthSlider.maxValue = currentFish.maxHealth;
        healthSlider.value = currentFish.health;
    }

    private void UpdateButtons()
    {
        // Disable both buttons when there is no fish.
        if (currentFish == null)
        {
            if (treatButton != null)
                treatButton.interactable = false;

            if (releaseButton != null)
                releaseButton.interactable = false;

            return;
        }

        // Check whether this particular fish is fully healed.
        bool fullyHealed =
            currentFish.health >= currentFish.maxHealth;

        // Fish still needs treatment.
        if (treatButton != null)
        {
            treatButton.interactable = !fullyHealed;
        }

        // Fish can only be released at full health.
        if (releaseButton != null)
        {
            releaseButton.interactable = fullyHealed;
        }
    }

    private void UpdateFishPreview(string fishName)
    {
        if (fishPreviewImage == null) return;

        Sprite spriteToUse = GetFishSprite(fishName);

        if (spriteToUse == null)
        {
            fishPreviewImage.enabled = false;
            return;
        }

        fishPreviewImage.enabled = true;
        fishPreviewImage.sprite = spriteToUse;
        fishPreviewImage.preserveAspect = true;
    }

    private Sprite GetFishSprite(string fishName)
    {
        foreach (TankFishPreviewEntry entry in fishPreviews)
        {
            if (entry.fishName == fishName)
            {
                return entry.fishSprite;
            }
        }

        return null;
    }

    private void StartTreatment()
    {
        if (currentFish == null) return;

        // Don't treat fish that are already fully healed.
        if (currentFish.health >= currentFish.maxHealth)
            return;

        if (OintmentMiniGame.Instance == null)
        {
            Debug.LogWarning(
                "OintmentMiniGame is missing from the scene."
            );
            return;
        }

        Sprite selectedFishSprite =
            GetFishSprite(currentFish.fishName);

        bool started = OintmentMiniGame.Instance.StartMiniGame(
            currentTankId,
            0,
            selectedFishSprite
        );

        if (started)
        {
            ClosePanel();
        }
    }

    
    private void StartRelease()
    {
        if (currentFish == null) return;

        if (RescueManager.Instance == null)
        {
            Debug.LogWarning("RescueManager is missing.");
            return;
        }

        // Find the correct sprite for this fish.
        Sprite selectedFishSprite =
            GetFishSprite(currentFish.fishName);

        // Store the selected fish before switching scenes.
        bool readyToRelease =
            RescueManager.Instance.SelectFishForRelease(
                currentTankId,
                0,
                selectedFishSprite
            );

        if (!readyToRelease)
        {
            return;
        }

        // Open the release mini-game scene.
        SceneManager.LoadScene("ReleaseScene");
    }

}
