using UnityEngine;

public class WarmZone : MonoBehaviour
{
    public enum ZoneType { Fire, Shelter }

    [SerializeField] private ZoneType zoneType;
    //[SerializeField] private bool isFireZone = false; // Indicates if this zone is a fire-based warm zone.
    [SerializeField] private float checkRadius = 3f; // Radius to check for the player
    [SerializeField] private LayerMask playerLayer; // Layer mask to identify the player
    //[SerializeField] private float coldReplenishRate = 1f; // Rate at which cold is replenished

    private bool isFireLit = false; // Tracks if the fire is lit
    private SurvivalManager playerSurvivalManager;

    private void Update()
    {
        if (!isFireLit) return; // Only process if the fire is lit

        // Check if the player is within range
        Collider[] hits = Physics.OverlapSphere(transform.position, checkRadius, playerLayer);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                if (playerSurvivalManager == null)
                {
                    playerSurvivalManager = hit.GetComponent<SurvivalManager>();
                }

                if (playerSurvivalManager != null)
                {
                    // Replenish the player's cold
                    playerSurvivalManager.UpdateColdReplenishment(true, false);
                }

                return; // Exit after processing the first player
            }
        }

        // If no player is in range, stop replenishing cold
        if (playerSurvivalManager != null)
        {
            playerSurvivalManager.UpdateColdReplenishment(false, false);
        }
    }

    public void SetFireState(bool isLit)
    {
        isFireLit = isLit;

        if (!isLit && playerSurvivalManager != null)
        {
            // Reset cold replenishment when the fire is turned off
            playerSurvivalManager.UpdateColdReplenishment(false, false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the check radius in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
