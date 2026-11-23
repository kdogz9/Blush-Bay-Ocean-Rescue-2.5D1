using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindOccupiedTank", story: "Find an occupied rehabilitation tank and assign it to [TargetTank]", category: "Action", id: "d3110e24dcbb5a5d97da68f8f5b6de9f")]
public partial class FindOccupiedTankAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> TargetTank;

    
    
    private float nextCheckTime;

    protected override Status OnStart()
    {
        TargetTank.Value = null;
        nextCheckTime = 0f;

        Debug.Log("Visitor AI: Waiting for a fish to arrive.");

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // Check twice per second rather than every frame.
        if (Time.time < nextCheckTime)
            return Status.Running;

        nextCheckTime = Time.time + 0.5f;

        if (RescueManager.Instance == null)
            return Status.Running;

        string[] tankIds = { "Tank_1", "Tank_2", "Tank_3" };

        foreach (string tankId in tankIds)
        {
            var fishInTank = RescueManager.Instance.GetFishInTank(tankId);

            if (fishInTank != null && fishInTank.Count > 0)
            {
                GameObject tankObject = GameObject.Find(tankId);
                // finds that tank in the scene 

                if (tankObject != null)
                {
                    TargetTank.Value = tankObject;
                    // stores the chosen tank for the visitor 

                    Debug.Log("Visitor AI: Fish found in " + tankId);

                    return Status.Success;
                }
            }
        }

        // No fish yet, so keep waiting.
        return Status.Running;
    }

    
    protected override void OnEnd()
    {
    }
}

