
using UnityEngine;

public class CurrentGaugeMovement : MonoBehaviour
{
    [Header("Marker")]
    [SerializeField] private RectTransform marker;

    [Header("Red Zones")]
    [SerializeField] private RectTransform leftRedZone;
    [SerializeField] private RectTransform rightRedZone;

    [Header("Yellow Zones")]
    [SerializeField] private RectTransform leftYellowZone;
    [SerializeField] private RectTransform rightYellowZone;

    [Header("Wave Reference")]
    [SerializeField] private WaveMovement oceanWaves;

    [Header("Movement")]
    [SerializeField] private float crossingTime = 2.5f;

    private RectTransform gauge;
    private float timer;

    private readonly Vector3[] zoneCorners = new Vector3[4];

    private void Awake()
    {
        gauge = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (marker == null ||
            leftRedZone == null ||
            rightRedZone == null ||
            gauge == null)
        {
            return;
        }

        // Find the full width of the coloured bar.
        float leftEdge = gauge.InverseTransformPoint(
            leftRedZone.TransformPoint(
                new Vector3(leftRedZone.rect.xMin, 0f, 0f)
            )
        ).x;

        float rightEdge = gauge.InverseTransformPoint(
            rightRedZone.TransformPoint(
                new Vector3(rightRedZone.rect.xMax, 0f, 0f)
            )
        ).x;

        float markerHalfWidth = marker.rect.width / 2f;

        float minX = leftEdge + markerHalfWidth;
        float maxX = rightEdge - markerHalfWidth;

        // Move the marker from left to right and back.
        timer += Time.deltaTime;

        float progress = Mathf.PingPong(
            timer / Mathf.Max(0.1f, crossingTime),
            1f
        );

        float xPosition = Mathf.Lerp(
            minX,
            maxX,
            progress
        );

        marker.anchoredPosition = new Vector2(
            xPosition,
            marker.anchoredPosition.y
        );

        // Change wave intensity based on marker position.
        UpdateWaveIntensity();
    }

    private void UpdateWaveIntensity()
    {
        if (oceanWaves == null) return;

        float markerX = marker.position.x;

        // Red means rough water.
        if (IsInsideZone(leftRedZone, markerX) ||
            IsInsideZone(rightRedZone, markerX))
        {
            oceanWaves.SetCurrentIntensity(1f);
        }
        // Yellow means moderate water.
        else if (IsInsideZone(leftYellowZone, markerX) ||
                 IsInsideZone(rightYellowZone, markerX))
        {
            oceanWaves.SetCurrentIntensity(0.5f);
        }
        // Otherwise, the marker is in the green area.
        else
        {
            oceanWaves.SetCurrentIntensity(0f);
        }
    }

    private bool IsInsideZone(
        RectTransform zone,
        float markerX)
    {
        if (zone == null) return false;

        // Find the zone's position on screen.
        zone.GetWorldCorners(zoneCorners);

        float minX = zoneCorners[0].x;
        float maxX = zoneCorners[2].x;

        return markerX >= minX && markerX <= maxX;
    }
}
