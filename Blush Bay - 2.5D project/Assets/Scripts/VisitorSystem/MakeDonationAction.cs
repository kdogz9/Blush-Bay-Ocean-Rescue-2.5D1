using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Make donation", story: "Visitor donates coins to the aquarium", category: "Action", id: "345ed449daf3137e252ee91a2863da6c")]
public partial class MakeDonationAction : Action
{
    // Amount donated by each visitor.
    private const int DonationAmount = 10;

    // Prevent this action from donating more than once.
    private bool hasDonated = false;

    protected override Status OnStart()
    {
        // Don't donate again if this visitor already donated.
        if (hasDonated)
        {
            return Status.Success;
        }

        // Check that CurrencyManager exists.
        if (CurrencyManager.Instance == null)
        {
            Debug.LogWarning(
                "Visitor AI: CurrencyManager not found."
            );

            return Status.Failure;
        }

        // Add 10 coins to the aquarium balance.
        CurrencyManager.Instance.ReceiveDonation(DonationAmount);

        // Remember that this visitor has donated.
        hasDonated = true;

        Debug.Log(
            "Visitor AI: Donation completed successfully!"
        );

        // Tell the Behaviour Tree to continue.
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

