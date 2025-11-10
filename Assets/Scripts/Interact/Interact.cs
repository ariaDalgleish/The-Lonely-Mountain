using UnityEngine;

public class Interact : MonoBehaviour
{
    InteractEvent interact = new InteractEvent();
    PlayerInteract playerInteract;

    public ItemData itemData; // For items
    [SerializeField] private string customPrompt; // For static objects

    // Example: Campfire state
    public bool hasPotOnCampfire;

    public InteractEvent GetInteractEvent
    {
        get
        {
            if (interact == null) interact = new InteractEvent();
            return interact;
        }
    }

    public PlayerInteract GetPlayerInteract
    {
        get
        {
            return playerInteract;
        }
    }

    public void CallInteract(PlayerInteract interactedPlayer)
    {
        playerInteract = interactedPlayer;
        interact.CallInteractEvent();
    }

    public string GetPromptText(PlayerInteract player)
    {
        
        // Fallback
        if (!string.IsNullOrEmpty(customPrompt))
            return customPrompt;

        return gameObject.name;
    }
}

public class InteractEvent
{
    public delegate void InteractHandler();

    public event InteractHandler HasInteracted;

    // Method to invoke the event
    public void CallInteractEvent() => HasInteracted?.Invoke();
}
