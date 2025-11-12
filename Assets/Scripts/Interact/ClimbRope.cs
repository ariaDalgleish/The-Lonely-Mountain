using UnityEngine;

public class ClimbRope : MonoBehaviour
{
    public Interact openFromInteraction;
    [Tooltip("The destination anchor (the other rope anchor). Assign the other GameObject's transform in the Inspector.")]
    public Transform otherAnchor;

    private void OnEnable()
    {
        if (openFromInteraction != null)
        {
            openFromInteraction.GetInteractEvent.HasInteracted += HandleInteraction;
        }
    }

    private void OnDisable()
    {
        if (openFromInteraction != null)
        {
            openFromInteraction.GetInteractEvent.HasInteracted -= HandleInteraction;
        }
    }

    // Called when the Interact event fires.
    private void HandleInteraction()
    {
        if (otherAnchor == null)
        {
            Debug.LogWarning($"ClimbRope on '{gameObject.name}' has no otherAnchor assigned.");
            return;
        }

        if (openFromInteraction == null)
        {
            Debug.LogWarning($"ClimbRope on '{gameObject.name}' has no openFromInteraction assigned.");
            return;
        }

        var player = openFromInteraction.GetPlayerInteract;
        if (player == null)
        {
            Debug.LogWarning("ClimbRope: interacting player reference is null.");
            return;
        }

        Transform playerT = player.transform;
        if (playerT == null)
        {
            Debug.LogWarning("ClimbRope: interacting player's transform is null.");
            return;
        }

        // If the player uses a CharacterController, disable it before teleporting to avoid physics issues.
        var cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            playerT.position = otherAnchor.position;
            // Optionally match rotation:
            // playerT.rotation = otherAnchor.rotation;
            cc.enabled = true;
        }
        else
        {
            playerT.position = otherAnchor.position;
        }
    }
}
