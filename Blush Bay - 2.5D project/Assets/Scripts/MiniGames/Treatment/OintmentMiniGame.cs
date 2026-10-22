
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class OintmentMiniGame : MonoBehaviour
{
    public static OintmentMiniGame Instance;

    [Header("UI Objects")]
    [SerializeField] private GameObject panel;
    [SerializeField] private RectTransform timingBar;
    [SerializeField] private RectTransform calmZone;
    [SerializeField] private RectTransform marker;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text resultText;

    [Header("Fish Visual")]
    [SerializeField] private Image miniGameFishImage;
    [SerializeField] private GameObject ointmentEffect;

    [Header("Mini Game Settings")]
    [SerializeField] private float markerSpeed = 220f;

    [SerializeField] private int perfectHealAmount = 35;
    [SerializeField] private int goodHealAmount = 20;
    [SerializeField] private int missHealAmount = 5;

    [Header("Result")]
    [SerializeField] private float resultDisplayTime = 1.6f;

    // Information about the fish being treated
    private string currentTankId;
    private int currentFishIndex;
    private string currentFishName;

    private bool gameRunning = false;
    private bool waitingToClose = false;
    private int moveDirection = 1;

    private void Awake()
    {
        Instance = this;

        if (panel != null)
        {
            panel.SetActive(false);
        }

        if (ointmentEffect != null)
        {
            ointmentEffect.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (!gameRunning) return;
        if (Keyboard.current == null) return;

        MoveMarker();

        // Press Space to apply ointment
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ApplyOintment();
        }
    }

    // Called by the Treat button in TankInfoPanelUI
    public bool StartMiniGame(
        string tankId,
        int fishIndex,
        Sprite fishSprite)
    {
        if (gameRunning || waitingToClose)
        {
            return false;
        }

        if (RescueManager.Instance == null)
        {
            Debug.LogWarning("RescueManager is missing.");
            return false;
        }

        if (panel == null ||
            timingBar == null ||
            calmZone == null ||
            marker == null)
        {
            Debug.LogWarning(
                "Ointment mini-game UI is not fully assigned."
            );
            return false;
        }

        List<RescuedFish> fishInTank =
            RescueManager.Instance.GetFishInTank(tankId);

        if (fishInTank == null ||
            fishIndex < 0 ||
            fishIndex >= fishInTank.Count)
        {
            Debug.LogWarning("No valid fish to treat.");
            return false;
        }

        RescuedFish fish = fishInTank[fishIndex];

        if (fish.health >= fish.maxHealth)
        {
            Debug.Log("This fish already has full health.");
            return false;
        }

        // Remember which fish is receiving treatment
        currentTankId = tankId;
        currentFishIndex = fishIndex;
        currentFishName = fish.fishName;

        panel.SetActive(true);

        if (titleText != null)
        {
            titleText.text = "APPLY OINTMENT";
        }

        if (resultText != null)
        {
            resultText.text = "";
        }

        // Show the sprite matching the rescued fish
        if (miniGameFishImage != null)
        {
            miniGameFishImage.sprite = fishSprite;
            miniGameFishImage.preserveAspect = true;

            miniGameFishImage.gameObject.SetActive(
                fishSprite != null
            );
        }

        if (ointmentEffect != null)
        {
            ointmentEffect.SetActive(false);
        }

        Canvas.ForceUpdateCanvases();

        // Reset the marker to the left side
        float maxX = Mathf.Max(
            0f,
            (timingBar.rect.width - marker.rect.width) / 2f
        );

        marker.anchoredPosition = new Vector2(
            -maxX,
            marker.anchoredPosition.y
        );

        moveDirection = 1;
        gameRunning = true;

        Debug.Log(
            "Ointment treatment started for " + currentFishName
        );

        return true;
    }

    private void MoveMarker()
    {
        float halfBarWidth = timingBar.rect.width / 2f;
        float halfMarkerWidth = marker.rect.width / 2f;

        float maxX = Mathf.Max(
            0f,
            halfBarWidth - halfMarkerWidth
        );

        float newX =
            marker.anchoredPosition.x +
            moveDirection * markerSpeed * Time.deltaTime;

        if (newX > maxX)
        {
            newX = maxX;
            moveDirection = -1;
        }

        if (newX < -maxX)
        {
            newX = -maxX;
            moveDirection = 1;
        }

        marker.anchoredPosition = new Vector2(
            newX,
            marker.anchoredPosition.y
        );
    }

    private void ApplyOintment()
    {
        gameRunning = false;
        waitingToClose = true;

        float markerX = marker.anchoredPosition.x;
        float calmZoneX = calmZone.anchoredPosition.x;

        float distanceFromCentre =
            Mathf.Abs(markerX - calmZoneX);

        float halfCalmZoneWidth =
            calmZone.rect.width / 2f;

        float perfectZone =
            halfCalmZoneWidth * 0.35f;

        int healAmount;
        string result;

        if (distanceFromCentre <= perfectZone)
        {
            healAmount = perfectHealAmount;
            result = "PERFECT!";
        }
        else if (distanceFromCentre <= halfCalmZoneWidth)
        {
            healAmount = goodHealAmount;
            result = "GOOD!";
        }
        else
        {
            healAmount = missHealAmount;
            result = "WRIGGLED!";
        }

        // Update the correct rescued fish's health
        if (RescueManager.Instance != null)
        {
            RescueManager.Instance.TreatFishInTank(
                currentTankId,
                currentFishIndex,
                healAmount
            );
        }

        if (resultText != null)
        {
            resultText.text = result;
        }

        ShowOintmentEffect();

        Debug.Log(
            currentFishName +
            " received " +
            healAmount +
            " health from ointment."
        );

        StartCoroutine(CloseAfterDelay());
    }

    private void ShowOintmentEffect()
    {
        if (ointmentEffect == null) return;

        ointmentEffect.SetActive(true);
        ointmentEffect.transform.localScale = Vector3.one;

        StartCoroutine(OintmentAbsorbEffect());
    }

    private IEnumerator OintmentAbsorbEffect()
    {
        if (ointmentEffect == null) yield break;

        Image ointmentImage =
            ointmentEffect.GetComponent<Image>();

        if (ointmentImage == null) yield break;

        Color startColor = ointmentImage.color;
        Vector3 startScale =
            ointmentEffect.transform.localScale;

        float absorbTime = 1.5f;
        float timer = 0f;

        while (timer < absorbTime)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(
                timer / absorbTime
            );

            float scale = Mathf.Lerp(
                1f,
                0.2f,
                progress
            );

            ointmentEffect.transform.localScale =
                startScale * scale;

            float alpha = Mathf.Lerp(
                startColor.a,
                0f,
                progress
            );

            ointmentImage.color = new Color(
                startColor.r,
                startColor.g,
                startColor.b,
                alpha
            );

            yield return null;
        }

        ointmentEffect.SetActive(false);
        ointmentEffect.transform.localScale = startScale;
        ointmentImage.color = startColor;
    }

    private IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(resultDisplayTime);

        string completedTankId = currentTankId;

        gameRunning = false;
        waitingToClose = false;

        if (panel != null)
        {
            panel.SetActive(false);
        }

        if (ointmentEffect != null)
        {
            ointmentEffect.SetActive(false);
        }

        currentTankId = "";
        currentFishIndex = 0;
        currentFishName = "";

        // Reopen the tank information with updated health
        TankInfoPanelUI tankInfoPanel =
            FindAnyObjectByType<TankInfoPanelUI>();

        if (tankInfoPanel != null)
        {
            tankInfoPanel.OpenPanel(completedTankId);
        }
    }
}
