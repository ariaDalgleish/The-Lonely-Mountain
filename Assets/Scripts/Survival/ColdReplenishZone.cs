using UnityEngine;

public class ColdReplenishZone : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<SphereCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        var survivalManager = other.GetComponent<SurvivalManager>();
        if (survivalManager is null) return;

        survivalManager.SetColdReplenishZone(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        var survivalManager = other.GetComponent<SurvivalManager>();
        if (survivalManager is null) return;

        survivalManager.SetColdReplenishZone(false);
    }
}