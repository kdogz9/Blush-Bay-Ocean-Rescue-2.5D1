using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TankAssignmentUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject assignmentPanel;
    [SerializeField] private TMP_Text fishInfoText;

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

        if (fishInfoText != null)
        {
            fishInfoText.text =
                "New rescued fish\n\n" +
                fish.fishName +
                "\n\nCondition:\n" +
                fish.injuryType +
                "\n\nChoose a rehabilitation tank.";
        }
    }
}