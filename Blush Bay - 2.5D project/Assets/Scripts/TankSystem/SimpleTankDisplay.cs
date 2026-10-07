using System.Collections.Generic;
using UnityEngine;

public class SimpleTankDisplay : MonoBehaviour
{
    [Header("Tank")]
    [SerializeField] private string tankId = "Tank_1";

    [Header("Fish Visual")]
    [SerializeField] private GameObject fishVisualPrefab;
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

        if (fishVisualPrefab == null)
        {
            Debug.LogWarning("SimpleTankDisplay on " + gameObject.name + ": Fish Visual Prefab is missing.");
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
            Vector3 spawnOffset = new Vector3(i * spacing, 0f, 0f);

            GameObject newFish = Instantiate(
                fishVisualPrefab,
                fishSpawnParent.position + spawnOffset,
                fishSpawnParent.rotation,
                fishSpawnParent
            );

            newFish.SetActive(true);

            SpriteRenderer spriteRenderer = newFish.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 20;
            }

            spawnedFish.Add(newFish);

            Debug.Log("Spawned fish visual in " + tankId + " at " + newFish.transform.position);
        }
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