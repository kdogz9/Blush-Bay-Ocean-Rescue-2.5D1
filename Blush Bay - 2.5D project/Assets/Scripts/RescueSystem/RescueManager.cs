using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class RescuedFish
{
    public string fishName;
    public string injuryType;

    public float health = 25f;
    public float maxHealth = 100f;
    public bool hasBeenTreated = false;
}

[System.Serializable]
public class RescueNet
{
    public int netId;
    public bool isDropped;
    public bool hasFish;
    public RescuedFish fish;
}

[System.Serializable]
public class RehabTankData
{
    public string tankId;
    public List<RescuedFish> fishInTank = new List<RescuedFish>();
}

public class RescueManager : MonoBehaviour
{
    public static RescueManager Instance;

    [Header("Rescue Nets")]
    [SerializeField] private int numberOfNets = 3;
    [SerializeField] private List<RescueNet> rescueNets = new List<RescueNet>();

    [Header("Fish Waiting For Tank")]
    [SerializeField] private List<RescuedFish> fishWaitingForTank = new List<RescuedFish>();

    [Header("Rehabilitation Tanks")]
    [SerializeField] private List<RehabTankData> rehabTanks = new List<RehabTankData>();

    private bool netsHaveBeenDropped = false;
    private bool fishGeneratedForThisDrop = false;
    private int timesReturnedToBoatAfterDrop = 0;

    public bool NetsHaveBeenDropped => netsHaveBeenDropped;
    public bool FishAreWaitingForTank => fishWaitingForTank.Count > 0;

    private string[] possibleFish =
    {
        "Clownfish",
        "Butterfly fish",
        "Jelly fish",
        "Squid",
        "Seahorse"
    };

    private string[] possibleInjuries =
    {
        "Small cut",
        "Damaged fin",
        "Weak swimming",
        "Stress marks",
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
        SetUpDefaultTanks();
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

    private void SetUpDefaultTanks()
    {
        EnsureTankExists("Tank_1");
        EnsureTankExists("Tank_2");
        EnsureTankExists("Tank_3");
    }

    private void EnsureTankExists(string tankId)
    {
        foreach (RehabTankData tank in rehabTanks)
        {
            if (tank.tankId == tankId)
            {
                return;
            }
        }

        RehabTankData newTank = new RehabTankData();
        newTank.tankId = tankId;
        rehabTanks.Add(newTank);
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

        if (timesReturnedToBoatAfterDrop >= 1)
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

        fish.maxHealth = 100f;
        fish.hasBeenTreated = false;

        if (fish.injuryType == "Small cut")
        {
            fish.health = 45f;
        }
        else if (fish.injuryType == "Damaged fin")
        {
            fish.health = 30f;
        }
        else if (fish.injuryType == "Weak swimming")
        {
            fish.health = 35f;
        }
        else if (fish.injuryType == "Stress marks")
        {
            fish.health = 50f;
        }
        else
        {
            fish.health = 40f;
        }

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

    public RescuedFish AssignNextFishToTank(string tankId)
    {
        if (fishWaitingForTank.Count == 0)
        {
            Debug.Log("No fish waiting for tank.");
            return null;
        }

        EnsureTankExists(tankId);

        RescuedFish fish = fishWaitingForTank[0];
        fishWaitingForTank.RemoveAt(0);

        foreach (RehabTankData tank in rehabTanks)
        {
            if (tank.tankId == tankId)
            {
                tank.fishInTank.Add(fish);
                Debug.Log(fish.fishName + " assigned to " + tankId);
                return fish;
            }
        }

        return null;
    }

    public List<RescuedFish> GetFishInTank(string tankId)
    {
        EnsureTankExists(tankId);

        foreach (RehabTankData tank in rehabTanks)
        {
            if (tank.tankId == tankId)
            {
                return tank.fishInTank;
            }
        }

        return new List<RescuedFish>();
    }
    
    public void TreatFishInTank(string tankId, int fishIndex, float healthIncrease)
    {
        List<RescuedFish> fishInTank = GetFishInTank(tankId);

        if (fishInTank == null || fishInTank.Count == 0)
        {
            Debug.Log("No fish in tank to treat.");
            return;
        }

        if (fishIndex < 0 || fishIndex >= fishInTank.Count)
        {
            Debug.LogWarning("Fish index out of range.");
            return;
        }

        RescuedFish fish = fishInTank[fishIndex];

        fish.health += healthIncrease;

        if (fish.health > fish.maxHealth)
        {
            fish.health = fish.maxHealth;
        }

        fish.hasBeenTreated = true;

        Debug.Log("Treated " + fish.fishName + ". Health is now " + fish.health);
    }
    
    [Header("Fish Selected for Release")]
    public string SelectedReleaseTankId { get; private set; }
    public int SelectedReleaseFishIndex { get; private set; } = -1;
    public RescuedFish SelectedReleaseFish { get; private set; }
    public Sprite SelectedReleaseFishSprite { get; private set; }

    public bool SelectFishForRelease(
        string tankId,
        int fishIndex,
        Sprite fishSprite)
    {
        List<RescuedFish> fishInTank = GetFishInTank(tankId);

        if (fishInTank == null ||
            fishIndex < 0 ||
            fishIndex >= fishInTank.Count)
        {
            Debug.LogWarning("No valid fish found for release.");
            return false;
        }

        RescuedFish fish = fishInTank[fishIndex];

        // Fish must be fully healed before release.
        if (fish == null || fish.health < fish.maxHealth)
        {
            Debug.LogWarning("Fish is not fully healed.");
            return false;
        }

        // Remember this specific fish and its tank.
        SelectedReleaseTankId = tankId;
        SelectedReleaseFishIndex = fishIndex;
        SelectedReleaseFish = fish;
        SelectedReleaseFishSprite = fishSprite;

        Debug.Log("Selected " + fish.fishName + " for release.");

        return true;
    }

}