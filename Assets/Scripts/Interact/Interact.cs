using UnityEngine;

public class Interact : MonoBehaviour
{
    InteractEvent interact = new InteractEvent();
    PlayerController playerController;

    public InteractEvent GetInteractEvent
    {
        get
        {
            if (interact == null) interact = new InteractEvent();
            return interact;
        }
    }

    public PlayerController GetPlayerController
    {
        get
        {
            return playerController;
        }
    }

    public void CallInteract(PlayerController interactedPlayer)
    {
        playerController = interactedPlayer;
        interact.CallInteractEvent();
    }
}

public class InteractEvent
{
    public delegate void InteractHandler();

    public event InteractHandler HasInteracted;

    public void CallInteractEvent() => HasInteracted?.Invoke();
}
