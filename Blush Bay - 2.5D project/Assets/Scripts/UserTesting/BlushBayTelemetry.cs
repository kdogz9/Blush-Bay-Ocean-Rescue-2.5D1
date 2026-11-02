
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class BlushBaySession
{
    public string gameVersion;
    public string platform;
    public string date;

    public int animalsRescued;
    public int animalsTreated;

    public float playtimeSeconds;
}

public class BlushBayTelemetry : MonoBehaviour
{
    public static BlushBayTelemetry Instance { get; private set; }

    [Header("Playtest Settings")]
    [SerializeField] private bool consentGranted = false;
    [SerializeField] private string gameVersion = "0.1";

    private BlushBaySession session;
    private float startTime;
    private bool sessionSaved;

    // Prevent the same fish being counted more than once.
    private readonly HashSet<RescuedFish> rescuedFish =
        new HashSet<RescuedFish>();

    private readonly HashSet<RescuedFish> treatedFish =
        new HashSet<RescuedFish>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        // Keep telemetry alive when changing scenes.
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!consentGranted)
        {
            Debug.Log("Telemetry disabled: no consent.");
            return;
        }

        session = new BlushBaySession
        {
            gameVersion = gameVersion,
            platform = Application.platform.ToString(),
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            animalsRescued = 0,
            animalsTreated = 0,
            playtimeSeconds = 0f
        };

        startTime = Time.realtimeSinceStartup;

        Debug.Log("Blush Bay telemetry started!");
    }

    public void RecordRescue(RescuedFish fish)
    {
        if (session == null || sessionSaved || fish == null)
            return;

        // Only count each rescued fish once.
        if (!rescuedFish.Add(fish))
            return;

        session.animalsRescued++;

        Debug.Log(
            "Animals rescued: " + session.animalsRescued
        );
    }

    public void RecordTreatment(RescuedFish fish)
    {
        if (session == null || sessionSaved || fish == null)
            return;

        // Only count the first treatment of each fish.
        if (!treatedFish.Add(fish))
            return;

        session.animalsTreated++;

        Debug.Log(
            "Animals treated: " + session.animalsTreated
        );
    }

    public void SaveSession()
    {
        if (session == null || sessionSaved)
            return;

        session.playtimeSeconds =
            Time.realtimeSinceStartup - startTime;

        string json = JsonUtility.ToJson(session, true);

        string filename =
            "BlushBay_Playtest_" +
            DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff") +
            ".json";

        string path = Path.Combine(
            Application.persistentDataPath,
            filename
        );

        try
        {
            File.WriteAllText(path, json);

            sessionSaved = true;

            Debug.Log("Playtest saved to: " + path);
        }
        catch (Exception error)
        {
            Debug.LogError(
                "Could not save telemetry: " + error.Message
            );
        }
    }

    private void OnApplicationQuit()
    {
        SaveSession();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        // F8 manually saves the playtest session.
        if (Keyboard.current != null &&
            Keyboard.current.f8Key.wasPressedThisFrame)
        {
            SaveSession();
        }
    }
#endif
}
