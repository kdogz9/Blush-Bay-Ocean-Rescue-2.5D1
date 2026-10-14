using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FishVisualEntry
{
    public string fishName;
    public GameObject fishPrefab;
}

public class SimpleTankDisplay : MonoBehaviour
{
    [Header("Tank")]
    [SerializeField] private string tankId = "Tank_1";

    [Header("Fish Visuals")]
    [SerializeField] private List<FishVisualEntry> fishVisuals = new List<FishVisualEntry>();
    [SerializeField] private GameObject fallbackFishPrefab;
    [SerializeField] private Transform fishSpawnParent;

    [Header("Spacing")]
    [SerializeField] private float spacing = 0.35f;

    private readonly List<GameObject> spawnedFish = new List<GameObject>();

    private void Start()
    {
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        ClearFish();

        if (RescueManager.Instance == null)
        {
            Debug.LogWarning("SimpleTankDisplay on " + gameObject.name + ": RescueManager missing.");
            return;
        }

        if (fishSpawnParent == null)
        {
            Debug.LogWarning("SimpleTankDisplay on " + gameObject.name + ": Fish Spawn Parent is missing.");
            return;
        }

        List<RescuedFish> fishInTank = RescueManager.Instance.GetFishInTank(tankId);

        Debug.Log("Refreshing " + tankId + ". Fish count: " + fishInTank.Count);

        for (int i = 0; i < fishInTank.Count; i++)
        {
            RescuedFish rescuedFish = fishInTank[i];

            GameObject prefabToSpawn = GetFishPrefab(rescuedFish.fishName);

            if (prefabToSpawn == null)
            {
                Debug.LogWarning("No fish prefab found for: " + rescuedFish.fishName);
                continue;
            }

            Vector3 spawnOffset = new Vector3(i * spacing, 0f, 0f);

            GameObject newFish = Instantiate(
                prefabToSpawn,
                fishSpawnParent.position + spawnOffset,
                fishSpawnParent.rotation,
                fishSpawnParent
            );

            newFish.SetActive(true);

            SpriteRenderer spriteRenderer = newFish.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 0;
            }

            spawnedFish.Add(newFish);

            Debug.Log("Spawned " + rescuedFish.fishName + " in " + tankId);
        }
    }

    private GameObject GetFishPrefab(string fishName)
    {
        foreach (FishVisualEntry entry in fishVisuals)
        {
            if (entry.fishName == fishName)
            {
                return entry.fishPrefab;
            }
        }

        return fallbackFishPrefab;
    }

    private void ClearFish()
    {
        foreach (GameObject fish in spawnedFish)
        {
            if (fish != null)
            {
                Destroy(fish);
            }
        }

        spawnedFish.Clear();
    }
}