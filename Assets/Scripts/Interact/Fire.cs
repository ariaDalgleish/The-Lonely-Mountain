using UnityEngine;

public class Fire : MonoBehaviour
{
    // Interactable object
    public Interact openFromInteraction;

    [SerializeField] private GameObject baseFire;

    // Fire state
    private bool isFireOn = false;

    

    private void OnEnable()
    {
        // Subscribe to the interaction event if the Interact component is assigned
        if (openFromInteraction != null)
        {
            openFromInteraction.GetInteractEvent.HasInteracted += HandleInteraction;
            
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the interaction event to avoid memory leaks
        if (openFromInteraction != null)
        {
            openFromInteraction.GetInteractEvent.HasInteracted -= HandleInteraction;
        }
    }

    private void HandleInteraction()
    {
        // Toggle the fire state when the interaction event is triggered
        ToggleFire();
        SoundManager.PlaySound(SoundType.StartFire);
    }

    private void ToggleFire()
    {
        // Toggle the fire state
        isFireOn = !isFireOn;

        // Play the fire sound effect and disable the fire sound if the fire is off
        


        // Enable or disable the fire particle effect
        if (baseFire != null)
        {
            baseFire.SetActive(isFireOn);
        }

        // Update the warm zone state
        WarmZone warmZone = GetComponent<WarmZone>();
        if (warmZone != null)
        {
            warmZone.SetFireState(isFireOn);
        }

        Debug.Log(isFireOn ? "Fire enabled." : "Fire disabled.");
    }

}
