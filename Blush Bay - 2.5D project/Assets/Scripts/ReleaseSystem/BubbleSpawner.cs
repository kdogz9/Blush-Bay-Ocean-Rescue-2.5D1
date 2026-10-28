
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer bubblePrefab;
    [SerializeField] private SpriteRenderer oceanBackground;
    [SerializeField] private Camera sceneCamera;

    [Header("Spawning")]
    [SerializeField] private float spawnInterval = 0.8f;
    [SerializeField] private int startingBubbles = 8;
    [SerializeField] private int maxBubbles = 10;

    [Header("Bubble Movement")]
    [SerializeField] private float minRiseSpeed = 0.25f;
    [SerializeField] private float maxRiseSpeed = 0.6f;

    [Header("Bubble Sizes")]
    [SerializeField] private float minSize = 0.45f;
    [SerializeField] private float maxSize = 0.95f;

    [Header("Side-to-Side Movement")]
    [SerializeField] private float minWobbleAmount = 0.05f;
    [SerializeField] private float maxWobbleAmount = 0.15f;
    [SerializeField] private float minWobbleSpeed = 1.5f;
    [SerializeField] private float maxWobbleSpeed = 2.5f;

    private float spawnTimer;
    
    private int visibleBubbleLimit = 10;

    public void SetVisibleBubbleLimit(int limit)
    {
        visibleBubbleLimit = Mathf.Clamp(limit, 0, maxBubbles);
    }

    private class BubbleData
    {
        public SpriteRenderer bubble;
        public float riseSpeed;

        public float wobbleCentreX;
        public float wobbleAmount;
        public float wobbleSpeed;
        public float wobblePhase;
        public float lifetime;
    }

    private List<BubbleData> activeBubbles =
        new List<BubbleData>();

    private void Start()
    {
        if (sceneCamera == null)
        {
            sceneCamera = Camera.main;
        }

        if (bubblePrefab == null ||
            oceanBackground == null ||
            sceneCamera == null)
        {
            Debug.LogWarning(
                "BubbleSpawner references are missing!"
            );

            enabled = false;
            return;
        }

        // Create some bubbles at random heights.
        for (int i = 0;
             i < Mathf.Min(startingBubbles, maxBubbles);
             i++)
        {
            SpawnBubble(true);
        }
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            if (activeBubbles.Count < maxBubbles)
            {
                SpawnBubble(false);
            }

            spawnTimer = 0f;
        }

        MoveBubbles();
    }

    private void SpawnBubble(bool randomHeight)
    {
        float cameraHeight = sceneCamera.orthographicSize;
        float cameraWidth = cameraHeight * sceneCamera.aspect;

        float leftEdge = Mathf.Max(
            oceanBackground.bounds.min.x,
            sceneCamera.transform.position.x - cameraWidth
        );

        float rightEdge = Mathf.Min(
            oceanBackground.bounds.max.x,
            sceneCamera.transform.position.x + cameraWidth
        );

        float bottomEdge = Mathf.Max(
            oceanBackground.bounds.min.y,
            sceneCamera.transform.position.y - cameraHeight
        );

        float surfaceY = oceanBackground.bounds.max.y;

        float randomX = Random.Range(
            leftEdge + 0.4f,
            rightEdge - 0.4f
        );

        float randomY = bottomEdge + 0.3f;

        if (randomHeight)
        {
            randomY = Random.Range(
                bottomEdge + 0.3f,
                surfaceY - 0.3f
            );
        }

        Vector3 spawnPosition = new Vector3(
            randomX,
            randomY,
            bubblePrefab.transform.position.z
        );

        SpriteRenderer newBubble = Instantiate(
            bubblePrefab,
            spawnPosition,
            Quaternion.identity,
            transform
        );

        float randomSize = Random.Range(
            minSize,
            maxSize
        );

        newBubble.transform.localScale =
            bubblePrefab.transform.localScale * randomSize;

        newBubble.sortingOrder = 2;

        // Each bubble has its own movement values.
        float randomWobbleAmount = Random.Range(
            minWobbleAmount,
            maxWobbleAmount
        );

        float randomWobbleSpeed = Random.Range(
            minWobbleSpeed,
            maxWobbleSpeed
        );

        float randomPhase = Random.Range(
            0f,
            Mathf.PI * 2f
        );

        activeBubbles.Add(new BubbleData
        {
            bubble = newBubble,

            riseSpeed = Random.Range(
                minRiseSpeed,
                maxRiseSpeed
            ),

            wobbleAmount = randomWobbleAmount,
            wobbleSpeed = randomWobbleSpeed,
            wobblePhase = randomPhase,

            // Make the wobble start from the spawn position.
            wobbleCentreX = randomX -
                Mathf.Sin(randomPhase) * randomWobbleAmount,

            lifetime = 0f
        });
    }

    private void MoveBubbles()
    {
        float surfaceY = oceanBackground.bounds.max.y;

        for (int i = activeBubbles.Count - 1; i >= 0; i--)
        {
            BubbleData data = activeBubbles[i];
            
            // Gradually fade bubbles depending on oxygen level.
            float targetAlpha = i < visibleBubbleLimit ? 1f : 0f;

            Color bubbleColor = data.bubble.color;

            bubbleColor.a = Mathf.MoveTowards(
                bubbleColor.a,
                targetAlpha,
                Time.deltaTime * 2f
            );

            data.bubble.color = bubbleColor;

            // Track how long this bubble has existed.
            data.lifetime += Time.deltaTime;

            Vector3 position = data.bubble.transform.position;

            // Move upwards.
            position.y += data.riseSpeed * Time.deltaTime;

            // Gently wobble from side to side.
            position.x =
                data.wobbleCentreX +
                Mathf.Sin(
                    data.lifetime * data.wobbleSpeed +
                    data.wobblePhase
                ) * data.wobbleAmount;

            data.bubble.transform.position = position;

            // Remove bubble once its top reaches the surface.
            if (data.bubble.bounds.max.y >= surfaceY)
            {
                Destroy(data.bubble.gameObject);
                activeBubbles.RemoveAt(i);
            }
        }
    }
}
