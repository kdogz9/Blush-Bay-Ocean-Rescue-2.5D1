using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ReleaseFishDisplay : MonoBehaviour
{
    private SpriteRenderer fishRenderer;

    private void Awake()
    {
        // Find the SpriteRenderer on this object.
        fishRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Check that the rescue system exists.
        if (RescueManager.Instance == null)
        {
            Debug.LogWarning(
                "RescueManager is missing. No fish to display."
            );

            fishRenderer.enabled = false;
            return;
        }

        // Get the fish selected in AquariumScene.
        RescuedFish selectedFish =
            RescueManager.Instance.SelectedReleaseFish;

        Sprite selectedSprite =
            RescueManager.Instance.SelectedReleaseFishSprite;

        // Make sure a fish has been selected.
        if (selectedFish == null || selectedSprite == null)
        {
            Debug.LogWarning(
                "No fish or sprite selected for release."
            );

            fishRenderer.enabled = false;
            return;
        }

        // Display the correct fish sprite.
        fishRenderer.sprite = selectedSprite;
        fishRenderer.enabled = true;

        Debug.Log(
            "Displaying " + selectedFish.fishName +
            " in ReleaseScene."
        );
    }
}