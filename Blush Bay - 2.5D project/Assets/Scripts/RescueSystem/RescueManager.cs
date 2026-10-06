using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RescuedFish
{
    public string fishName;
    public string injuryType;
}

[System.Serializable]
public class RescueNet
{
    public int netId;
    public bool isDropped;
    public bool hasFish;
    public RescuedFish fish;
}

public class RescueManager : MonoBehaviour
{
    public static RescueManager Instance;

    [Header("Rescue Nets")]
    [SerializeField] private int numberOfNets = 3;
    [SerializeField] private List<RescueNet> rescueNets = new List<RescueNet>();

    [Header("Fish Waiting For Tank")]
    [SerializeField] private List<RescuedFish> fishWaitingForTank = new List<RescuedFish>();

    private bool netsHaveBeenDropped = false;
    private bool fishGeneratedForThisDrop = false;
    private int timesReturnedToBoatAfterDrop = 0;

    public bool NetsHaveBeenDropped => netsHaveBeenDropped;
    public bool FishAreWaitingForTank => fishWaitingForTank.Count > 0;

    private string[] possibleFish =
    {
        "Clownfish",
        "Blue Tang",
        "Royal Gramma",
        "Firefish",
        "Pufferfish"
    };

    private string[] possibleInjuries =
    {
        "Small cut",
        "Damaged fin",
        "Weak swimming",
        "Stress marks",
        "Needs observation"
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetUpNets();
    }

    private void SetUpNets()
    {
        if (rescueNets.Count > 0) return;

        for (int i = 0; i < numberOfNets; i++)
        {
            RescueNet newNet = new RescueNet();
            newNet.netId = i;
            newNet.isDropped = false;
            newNet.hasFish = false;
            newNet.fish = null;

            rescueNets.Add(newNet);
        }
    }

    public void DropNets()
    {
        netsHaveBeenDropped = true;
        fishGeneratedForThisDrop = false;
        timesReturnedToBoatAfterDrop = 0;

        foreach (RescueNet net in rescueNets)
        {
            net.isDropped = true;
            net.hasFish = false;
            net.fish = null;
        }

        Debug.Log("Rescue nets dropped.");
    }

    public void RegisterBoatSceneOpened()
    {
        if (!netsHaveBeenDropped) return;
        if (fishGeneratedForThisDrop) return;

        timesReturnedToBoatAfterDrop++;

        if (timesReturnedToBoatAfterDrop >= 2)
        {
            GenerateFishInNets();
        }
    }

    private void GenerateFishInNets()
    {
        bool atLeastOneFish = false;

        for (int i = 0; i < rescueNets.Count; i++)
        {
            RescueNet net = rescueNets[i];

            if (!net.isDropped) continue;

            bool shouldAddFish = Random.value <= 0.6f;

            if (i == rescueNets.Count - 1 && !atLeastOneFish)
            {
                shouldAddFish = true;
            }

            if (shouldAddFish)
            {
                net.hasFish = true;
                net.fish = CreateRandomFish();
                atLeastOneFish = true;
            }
        }

        fishGeneratedForThisDrop = true;

        Debug.Log("Fish generated in rescue nets.");
    }

    private RescuedFish CreateRandomFish()
    {
        RescuedFish fish = new RescuedFish();

        fish.fishName = possibleFish[Random.Range(0, possibleFish.Length)];
        fish.injuryType = possibleInjuries[Random.Range(0, possibleInjuries.Length)];

        return fish;
    }

    public RescueNet GetNet(int netId)
    {
        foreach (RescueNet net in rescueNets)
        {
            if (net.netId == netId)
            {
                return net;
            }
        }

        return null;
    }

    public bool AnyNetHasFish()
    {
        foreach (RescueNet net in rescueNets)
        {
            if (net.hasFish)
            {
                return true;
            }
        }

        return false;
    }

    public bool TryCollectFishFromNet(int netId, out RescuedFish collectedFish)
    {
        collectedFish = null;

        RescueNet net = GetNet(netId);

        if (net == null)
        {
            Debug.LogWarning("No rescue net found with ID: " + netId);
            return false;
        }

        if (!net.isDropped)
        {
            Debug.Log("This net has not been dropped yet.");
            return false;
        }

        if (!net.hasFish || net.fish == null)
        {
            Debug.Log("This net is empty.");
            return false;
        }

        collectedFish = net.fish;
        fishWaitingForTank.Add(collectedFish);

        net.hasFish = false;
        net.fish = null;

        Debug.Log("Collected fish: " + collectedFish.fishName);

        return true;
    }

    public RescuedFish GetNextFishWaitingForTank()
    {
        if (fishWaitingForTank.Count == 0) return null;

        return fishWaitingForTank[0];
    }

    public RescuedFish RemoveNextFishWaitingForTank()
    {
        if (fishWaitingForTank.Count == 0) return null;

        RescuedFish fish = fishWaitingForTank[0];
        fishWaitingForTank.RemoveAt(0);

        return fish;
    }
}