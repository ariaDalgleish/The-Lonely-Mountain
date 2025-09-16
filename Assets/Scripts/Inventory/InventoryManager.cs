using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public GameObject InventoryMenu;
    private bool menuActivated;
    public ItemSlot[] itemSlot;

    public ItemSO[] itemSOs;

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
            // mouse cursor invisible and locked to center of screen
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;


        }
        else if (InputManager.Instance.OpenInventory() && !menuActivated)
        {
            SoundManager.PlaySound(SoundType.OPENINVENTORY);
            Time.timeScale = 0f;
            InventoryMenu.SetActive(true);
            menuActivated = true;
            // mouse cursor visible and unlocked to move freely
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    

    public bool UseItem(string itemName)
    {
        for (int i = 0; i < itemSOs.Length; i++)
        {
            if(itemSOs[i].itemName == itemName)
            {
                bool usable = itemSOs[i].UseItem();
                return usable; // Exit the loop once the item is found and used
            }
        }
        return false; // Item not found
    }


    // changed void to int to return leftover items
    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
       
        //Debug.Log($"Added {quantity} of {itemName} to inventory.");
        for (int i = 0; i < itemSlot.Length; i++)
        {
            // Check if item matches the item we're adding or if slot is completely empty
            if (itemSlot[i].isFull == false && itemSlot[i].itemName == itemName || itemSlot[i].quantity == 0)
            {
                int leftOverItems = itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                if (leftOverItems > 0)
                  {
                    // recursively call AddItem to add the left over items to the next available slot
                    leftOverItems = AddItem(itemName, leftOverItems, itemSprite, itemDescription);
                  }   
                return leftOverItems;
            }
        }
        // if inventory is full and there are still items left return the quantity. 
        // Outside of for loop because our slots are full
        // Item is not added to inventory and keeps the same quantity
        return quantity; 

    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectedShader.SetActive(false);
            itemSlot[i].thisItemSelected = false;
        }   
    }
}
