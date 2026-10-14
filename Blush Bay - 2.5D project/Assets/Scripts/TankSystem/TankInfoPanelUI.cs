using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private List<TankFishPreviewEntry> fishPreviews = new List<TankFishPreviewEntry>();

    [Header("Health UI")]
    [SerializeField] private Slider healthSlider;

    [Header("Buttons")]
    [SerializeField] private Button treatButton;
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

        if (fishPreviewImage != null)
        {
            fishPreviewImage.enabled = false;
        }

        UpdateHealthUI();
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
    }

    private void RefreshPanel()
    {
        if (titleText != null)
        {
            titleText.text = currentTankId.Replace("_", " ").ToUpper();
        }

        if (RescueManager.Instance == null)
        {
            currentFish = null;

            if (fishInfoText != null)
            {
                fishInfoText.text = "RESCUE SYSTEM MISSING.";
            }

            if (treatButton != null)
            {
                treatButton.interactable = false;
            }

            if (fishPreviewImage != null)
            {
                fishPreviewImage.enabled = false;
            }

            UpdateHealthUI();
            return;
        }

        List<RescuedFish> fishInTank = RescueManager.Instance.GetFishInTank(currentTankId);

        if (fishInTank == null || fishInTank.Count == 0)
        {
            currentFish = null;

            if (fishInfoText != null)
            {
                fishInfoText.text = "NO RESCUED FISH IN THIS TANK YET.";
            }

            if (treatButton != null)
            {
                treatButton.interactable = false;
            }

            if (fishPreviewImage != null)
            {
                fishPreviewImage.enabled = false;
            }

            UpdateHealthUI();
            return;
        }

        // For now we show the first fish in the tank.
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

        if (treatButton != null)
        {
            treatButton.interactable = true;
        }
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
        if (currentFish == null)
        {
            Debug.Log("No fish selected for treatment.");
            return;
        }

        Debug.Log("Start treatment for " + currentFish.fishName + " in " + currentTankId);

        // Treatment mini-game will be connected here next.
    }
}