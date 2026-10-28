using UnityEngine;

public class WaveMovement : MonoBehaviour
{
    [Header("Calm Waves")]
    [SerializeField] private float waveHeight = 0.05f;
    [SerializeField] private float waveSpeed = 1.5f;

    [Header("Rough Waves")]
    [SerializeField] private float roughWaveHeight = 0.18f;
    [SerializeField] private float roughWaveSpeed = 3.5f;

    [Header("Smooth Transition")]
    [SerializeField] private float transitionTime = 0.5f;

    [Header("Current Intensity")]
    [Range(0f, 1f)]
    [SerializeField] private float currentIntensity = 0f;

    private float targetIntensity = 0f;
    private float intensityVelocity = 0f;

    private Vector3 startingPosition;
    private float waveTimer;

    private void Start()
    {
        startingPosition = transform.position;
    }

    private void Update()
    {
        // Smoothly change to the target intensity.
        currentIntensity = Mathf.SmoothDamp(
            currentIntensity,
            targetIntensity,
            ref intensityVelocity,
            transitionTime
        );

        // Calculate the wave height.
        float height = Mathf.Lerp(
            waveHeight,
            roughWaveHeight,
            currentIntensity
        );

        // Calculate the wave speed.
        float speed = Mathf.Lerp(
            waveSpeed,
            roughWaveSpeed,
            currentIntensity
        );

        // Keep the wave animation continuous.
        waveTimer += Time.deltaTime * speed;

        float movement = Mathf.Sin(waveTimer) * height;

        transform.position = startingPosition +
                             new Vector3(0f, movement, 0f);
    }

    // Called by the Ocean Current gauge.
    public void SetCurrentIntensity(float intensity)
    {
        targetIntensity = Mathf.Clamp01(intensity);
    }
}