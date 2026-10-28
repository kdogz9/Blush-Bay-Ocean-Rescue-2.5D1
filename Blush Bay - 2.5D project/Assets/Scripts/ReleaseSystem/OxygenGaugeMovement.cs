
using UnityEngine;

public class OxygenGaugeMovement : MonoBehaviour
{
    [Header("Oxygen Marker")]
    [SerializeField] private RectTransform marker;

    [Header("Red Zones")]
    [SerializeField] private RectTransform leftRedZone;
    [SerializeField] private RectTransform rightRedZone;

    [Header("Yellow Zones")]
    [SerializeField] private RectTransform leftYellowZone;
    [SerializeField] private RectTransform rightYellowZone;

    [Header("Bubble System")]
    [SerializeField] private BubbleSpawner bubbleSpawner;

    [Header("Movement Settings")]
    [SerializeField] private float crossingTime = 3.5f;

    private RectTransform gauge;
    private float timer;

    // Used to check the positions of the coloured zones.
    private readonly Vector3[] zoneCorners = new Vector3[4];

    private void Awake()
    {
        gauge = GetComponent<RectTransform>();
    }

    private void Start()
    {
        // Start at a different point from the current gauge.
        timer = crossingTime * 0.35f;
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

        // Find the outer edges of the coloured bar.
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

        // Keep the marker within the gauge.
        float halfMarkerWidth = marker.rect.width / 2f;

        float minX = leftEdge + halfMarkerWidth;
        float maxX = rightEdge - halfMarkerWidth;

        // Move the marker left and right.
        timer += Time.deltaTime;

        float progress = Mathf.PingPong(
            timer / Mathf.Max(0.1f, crossingTime),
            1f
        );

        float newX = Mathf.Lerp(
            minX,
            maxX,
            progress
        );

        marker.anchoredPosition = new Vector2(
            newX,
            marker.anchoredPosition.y
        );

        // NEW: Update bubble density based on oxygen level.
        UpdateBubbleDensity();
    }

    private void UpdateBubbleDensity()
    {
        if (bubbleSpawner == null || marker == null)
        {
            return;
        }

        // RED ZONE: Low oxygen, fewer visible bubbles.
        if (IsMarkerInside(leftRedZone) ||
            IsMarkerInside(rightRedZone))
        {
            bubbleSpawner.SetVisibleBubbleLimit(3);
        }

        // YELLOW ZONE: Moderate oxygen.
        else if (IsMarkerInside(leftYellowZone) ||
                 IsMarkerInside(rightYellowZone))
        {
            bubbleSpawner.SetVisibleBubbleLimit(6);
        }

        // GREEN ZONE: Healthy oxygen, more visible bubbles.
        else
        {
            bubbleSpawner.SetVisibleBubbleLimit(10);
        }
    }

    private bool IsMarkerInside(RectTransform zone)
    {
        if (zone == null)
        {
            return false;
        }

        // Get the screen-space boundaries of this zone.
        zone.GetWorldCorners(zoneCorners);

        float markerX = marker.position.x;

        return markerX >= zoneCorners[0].x &&
               markerX <= zoneCorners[2].x;
    }
}
