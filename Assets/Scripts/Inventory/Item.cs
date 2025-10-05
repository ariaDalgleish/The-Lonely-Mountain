using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    public ItemSO itemSO; // <-- Add this

    [SerializeField]
    public int quantity;

    private InventoryManager inventoryManager;
    public Interact openFromInteraction;

    private void OnEnable()
    {
        if (openFromInteraction != null)
        {
            openFromInteraction.GetInteractEvent.HasInteracted += HandleInteraction;
        }
    }

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }

    private void HandleInteraction()
    {
        if (itemSO == null)
        {
            Debug.LogError("ItemSO reference is missing on this Item!");
            return;
        }

        int leftOverItems = inventoryManager.AddItem(itemSO, quantity);
        if (leftOverItems <= 0)
        {
            SoundManager.PlaySound(SoundType.PICKUPITEM);
            Destroy(gameObject);
        }
        else
        {
            quantity = leftOverItems;
        }
    }
}
