using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{

    public GameObject InventoryMenu;
    private bool menuActivated;
    public List<ItemSlot> itemSlots = new List<ItemSlot>();
    public ItemSO[] itemSOs;
    public GameObject itemSlotPrefab; // Assign in Inspector
    public Transform itemSlotParent;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;
    public Image itemDescriptionImage;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize with 10 slots
        for (int i = 0; i < 10; i++)
        {
            CreateNewItemSlot();
        }
    }
    private ItemSlot CreateNewItemSlot()
    {
        GameObject slotObj = Instantiate(itemSlotPrefab, itemSlotParent);
        ItemSlot slot = slotObj.GetComponent<ItemSlot>();
        slot.ItemDescriptionNameText = itemDescriptionNameText;
        slot.ItemDescriptionText = itemDescriptionText;
        slot.itemDescriptionImage = itemDescriptionImage;
        itemSlots.Add(slot);
        return slot;
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
            DeselectAllSlots();

        }
        else if (InputManager.Instance.OpenInventory() && !menuActivated)
        {
            DeselectAllSlots();
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
    public int AddItem(string itemName, int quantity, Sprite itemSprite, Sprite sketchSprite, string itemDescription)
    {
        // Try to add to existing slots
        foreach (var slot in itemSlots)
        {
            if (!slot.isFull && (slot.itemName == itemName || string.IsNullOrEmpty(slot.itemName)))
            {
                int leftover = slot.AddItem(itemName, quantity, itemSprite, sketchSprite, itemDescription);
                if (leftover == 0)
                    return 0;
                quantity = leftover;
            }
        }

        // If still have items, create new slot(s)
        while (quantity > 0)
        {
            ItemSlot newSlot = CreateNewItemSlot();
            int leftover = newSlot.AddItem(itemName, quantity, itemSprite, sketchSprite, itemDescription);
            if (leftover == 0)
                return 0;
            quantity = leftover;
        }

        return 0;
    }

    public void DeselectAllSlots()
    {
        foreach (var slot in itemSlots)
        {
            if (slot != null)
            {
                if (slot.selectedShader != null)
                    slot.selectedShader.SetActive(false);
                slot.thisItemSelected = false;
            }
        }
        ClearDescriptionDisplay();
    }
    public void ClearDescriptionDisplay()
    {
        // If you have a shared description UI, clear it using the first slot's references
        if (itemSlots.Count > 0 && itemSlots[0] != null)
        {
            if (itemSlots.Count > 0 && itemSlots[0] != null)
            {
                var slot = itemSlots[0];
                if (slot.ItemDescriptionNameText != null)
                    slot.ItemDescriptionNameText.text = "";
                if (slot.ItemDescriptionText != null)
                    slot.ItemDescriptionText.text = "";
                if (slot.itemDescriptionImage != null && slot.emptySprite != null)
                    slot.itemDescriptionImage.sprite = slot.emptySprite;
            }
        }
    }
}
