using UnityEngine;

public class CampfirePotInteraction : MonoBehaviour
{
    [SerializeField] private Fire fireScript;
    [SerializeField] private GameObject campfirePot;

    public void TryPlacePot()
    {
        // Find EquipManager in the scene instead of using a non-existent Instance property
        var equipManager = EquipManager.Instance;
        if (equipManager == null)
        {
            Debug.LogError("EquipManager not found in the scene.");
            return;
        }
        var equippedCraftable = equipManager.GetEquippedCraftable();
        var itemData = equippedCraftable != null ? equippedCraftable.GetComponent<ItemData>() : null;

        if (fireScript == null || !fireScript.isFireOn) return;
        if (itemData == null || itemData.itemType != ItemType.craftable || itemData.itemName != "Pot") return;

        if (campfirePot != null) campfirePot.SetActive(true);
        equipManager.UnequipCraftable();
        Debug.Log("Pot placed on campfire.");
    }
}