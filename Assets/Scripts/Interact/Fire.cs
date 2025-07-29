using UnityEngine;

public class Fire : MonoBehaviour
{
    // Interactable object
    public Interact openFromInteraction;

    [SerializeField] private GameObject baseFire;
    [SerializeField] private AudioSource fireLoopSource; // assign in inspector, for ambient fire


    // Fire state
    private bool isFireOn = false;

    

    private void OnEnable()
    {
        //SoundManager.PlaySound(SoundType.StartFire);
        // Subscribe to the interaction event if the Interact component is assigned
        if (openFromInteraction != null)
        {
            openFromInteraction.GetInteractEvent.HasInteracted += HandleInteraction;
            
        }
    }

    private void OnDisable()
    {
        //SoundManager.PlaySound(SoundType.Menu);

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

    }

    private void ToggleFire()
    {
        // Toggle the fire state
        isFireOn = !isFireOn;

        // Play the fire sound effect and disable the fire sound if the fire is off
        if (isFireOn)
        {
            // Fire enabled sound
            SoundManager.PlaySound(SoundType.FIREMATCH);

            // Start ambient fire sound
            if (fireLoopSource != null && fireLoopSource.clip != null)
            {
                fireLoopSource.loop = true;
                fireLoopSource.Play();
            }
        }
        else
        {
            // Fire disabled sound
            SoundManager.PlaySound(SoundType.FIRESTOP);

            // Stop ambient fire sound
            if (fireLoopSource != null)
            {
                fireLoopSource.Stop();
            }
        }


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
