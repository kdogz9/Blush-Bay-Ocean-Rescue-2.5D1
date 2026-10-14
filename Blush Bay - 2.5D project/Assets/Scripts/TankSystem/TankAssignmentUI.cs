using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class FishPreviewEntry
{
    public string fishName;
    public Sprite fishSprite;
}

public class TankAssignmentUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject assignmentPanel;

    [Header("Text")]
    [SerializeField] private TMP_Text fishNameText;
    [SerializeField] private TMP_Text conditionText;
    [SerializeField] private TMP_Text chooseTankText;

    [Header("Fish Preview")]
    [SerializeField] private Image fishPreviewImage;
    [SerializeField] private List<FishPreviewEntry> fishPreviews = new List<FishPreviewEntry>();

    [Header("Tank Buttons")]
    [SerializeField] private Button tankOneButton;
    [SerializeField] private Button tankTwoButton;
    [SerializeField] private Button tankThreeButton;

    [Header("Tank IDs")]
    [SerializeField] private string tankOneId = "Tank_1";
    [SerializeField] private string tankTwoId = "Tank_2";
    [SerializeField] private string tankThreeId = "Tank_3";

    private void Start()
    {
        if (tankOneButton != null)
        {
            tankOneButton.onClick.RemoveAllListeners();
            tankOneButton.onClick.AddListener(() => AssignFishToTank(tankOneId));
        }

        if (tankTwoButton != null)
        {
            tankTwoButton.onClick.RemoveAllListeners();
            tankTwoButton.onClick.AddListener(() => AssignFishToTank(tankTwoId));
        }

        if (tankThreeButton != null)
        {
            tankThreeButton.onClick.RemoveAllListeners();
            tankThreeButton.onClick.AddListener(() => AssignFishToTank(tankThreeId));
        }

        RefreshPanel();
    }

    private void AssignFishToTank(string tankId)
    {
        if (RescueManager.Instance == null) return;

        RescuedFish assignedFish = RescueManager.Instance.AssignNextFishToTank(tankId);

        if (assignedFish != null)
        {
            Debug.Log("Assigned rescued fish to tank: " + tankId);
        }

        SimpleTankDisplay[] tankDisplays = FindObjectsByType<SimpleTankDisplay>(FindObjectsInactive.Exclude);

        foreach (SimpleTankDisplay display in tankDisplays)
        {
            display.RefreshDisplay();
        }

        RefreshPanel();
    }

    private void RefreshPanel()
    {
        if (RescueManager.Instance == null)
        {
            if (assignmentPanel != null)
            {
                assignmentPanel.SetActive(false);
            }

            return;
        }

        RescuedFish fish = RescueManager.Instance.GetNextFishWaitingForTank();

        if (fish == null)
        {
            if (assignmentPanel != null)
            {
                assignmentPanel.SetActive(false);
            }

            return;
        }

        if (assignmentPanel != null)
        {
            assignmentPanel.SetActive(true);
        }

        if (fishNameText != null)
        {
            fishNameText.text = fish.fishName.ToUpper();
        }

        if (conditionText != null)
        {
            conditionText.text = "CONDITION:\n" + fish.injuryType.ToUpper();
        }

        if (chooseTankText != null)
        {
            chooseTankText.text = "CHOOSE A REHABILITATION TANK";
        }

        UpdateFishPreview(fish.fishName);
    }

    private void UpdateFishPreview(string fishName)
    {
        if (fishPreviewImage == null) return;

        Sprite spriteToUse = GetFishPreviewSprite(fishName);

        if (spriteToUse == null)
        {
            fishPreviewImage.enabled = false;
            return;
        }

        fishPreviewImage.enabled = true;
        fishPreviewImage.sprite = spriteToUse;
        fishPreviewImage.preserveAspect = true;
    }

    private Sprite GetFishPreviewSprite(string fishName)
    {
        foreach (FishPreviewEntry entry in fishPreviews)
        {
            if (entry.fishName == fishName)
            {
                return entry.fishSprite;
            }
        }

        return null;
    }
}