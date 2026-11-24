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
        // Check every half-second instead of every frame.
        if (Time.time < nextCheckTime)
        {
            return Status.Running;
        }

        nextCheckTime = Time.time + 0.5f;

        // Wait until RescueManager is available.
        if (RescueManager.Instance == null)
        {
            return Status.Running;
        }

        // Check the three rehabilitation tanks.
        string[] tankIds = { "Tank_1", "Tank_2", "Tank_3" };

        foreach (string tankId in tankIds)
        {
            // Check if this tank contains fish.
            var fishInTank =
                RescueManager.Instance.GetFishInTank(tankId);

            if (fishInTank != null && fishInTank.Count > 0)
            {
                // Find the occupied tank in the scene.
                GameObject tankObject = GameObject.Find(tankId);

                if (tankObject != null)
                {
                    // Find the visitor standing point.
                    Transform standPoint =
                        tankObject.transform.Find("VisitorStandPoint");

                    if (standPoint != null)
                    {
                        // Assign the standing point as the target.
                        TargetTank.Value = standPoint.gameObject;

                        Debug.Log(
                            "Visitor AI: Walking to " + tankId
                        );

                        // Continue to the movement action.
                        return Status.Success;
                    }
                    else
                    {
                        Debug.LogWarning(
                            "VisitorStandPoint missing from " + tankId
                        );
                    }
                }
            }
        }

        // No occupied tank found, so keep waiting.
        return Status.Running;
    }



    protected override void OnEnd()
    {
    }
}

