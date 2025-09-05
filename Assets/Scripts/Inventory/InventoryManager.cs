using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public GameObject InventoryMenu;
    private bool menuActivated;
    public ItemSlot[] itemSlot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //  Toggle Inventory
        if (InputManager.Instance.CloseInventory() && menuActivated)
        {
            SoundManager.PlaySound(SoundType.CLOSEINVENTORY);
            Time.timeScale = 1f;
            InventoryMenu.SetActive(false);
            menuActivated = false;

        }
        else if (InputManager.Instance.OpenInventory() && !menuActivated)
        {
            SoundManager.PlaySound(SoundType.OPENINVENTORY);
            Time.timeScale = 0f;
            InventoryMenu.SetActive(true);
            menuActivated = true;
        }
    }


    public void AddItem(string itemName, int quantity, Sprite itemSprite)
    {
       
        Debug.Log($"Added {quantity} of {itemName} to inventory.");
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if(itemSlot[i].isFull == false)
            {
                itemSlot[i].AddItem(itemName, quantity, itemSprite);
                return;
            }
        }

    }
}
