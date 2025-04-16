using UnityEngine;

public class ColdReplenishZone : MonoBehaviour
{
    public enum ZoneType { Fire, Shelter }

    [SerializeField] private ZoneType zoneType;
    [SerializeField] private bool isFireZone = false; // Indicates if this zone is a fire-based warm zone.

    private void Awake()
    {
        GetComponent<SphereCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        var survivalManager = other.GetComponent<SurvivalManager>();
        if (survivalManager is null) return;

        survivalManager.UpdateColdReplenishment(zoneType == ZoneType.Fire, false);

        Debug.Log($"Player entered {zoneType} zone.");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        var survivalManager = other.GetComponent<SurvivalManager>();
        if (survivalManager is null) return;

        // Reset cold replenishment state when leaving the zone.
        survivalManager.UpdateColdReplenishment(false, false);

        Debug.Log("Player exited cold replenish zone. Fire zone: " + isFireZone);
    }
}