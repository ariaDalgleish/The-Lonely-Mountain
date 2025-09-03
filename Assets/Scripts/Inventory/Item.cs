using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private string itemName;

    [SerializeField]
    private int quantity;

    [SerializeField]
    private Sprite itemSprite;

    private InventoryManager inventoryManager;
    public Interact openFromInteraction;
    private void OnEnable()
    {
        //SoundManager.PlaySound(SoundType.StartFire);
        // Subscribe to the interaction event if the Interact component is assigned
        if (openFromInteraction != null)
        {
            openFromInteraction.GetInteractEvent.HasInteracted += HandleInteraction;

        }
    }
    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }
    private void HandleInteraction()
    {
        
        inventoryManager.AddItem(itemName, quantity, itemSprite);
        Destroy(gameObject);

    }

    // Replace the OnCollisionEnter3D method with OnCollisionEnter, which uses the standard UnityEngine.Collision type.
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
           
    //    }
    //}
}
