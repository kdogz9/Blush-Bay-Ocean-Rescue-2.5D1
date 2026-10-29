using UnityEngine;
using UnityEngine.InputSystem;

public class ReleaseSpaceController : MonoBehaviour
{
    [Header("Release Gauges")]
    [SerializeField] private CurrentGaugeMovement currentGauge;
    [SerializeField] private OxygenGaugeMovement oxygenGauge;

    private bool releaseAttempted = false;

    private void Update()
    {
        // Prevent the player from releasing more than once.
        if (releaseAttempted) return;

        // Check whether the player presses Space.
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AttemptRelease();
        }
    }

    private void AttemptRelease()
    {
        // Make sure both gauges exist.
        if (currentGauge == null || oxygenGauge == null)
        {
            Debug.LogWarning("Release gauges are not assigned!");
            return;
        }

        releaseAttempted = true;

        // Stop both moving markers.
        currentGauge.enabled = false;
        oxygenGauge.enabled = false;

        Debug.Log("Release attempted! Both gauges stopped.");

        // Next: check the green, yellow and red zones.
    }
}