using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{

    public GameObject InventoryMenu;
    private bool menuActivated;
    public List<ItemSlot> itemSlots = new List<ItemSlot>();
    public ItemData[] itemsData;
    public GameObject itemSlotPrefab; // Assign in Inspector
    public Transform itemSlotParent;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;
    public Image itemDescriptionImage;
    public GameObject buttonDisplay;
    public Button eatButton;
    public Button dropButton;
    public Button holdButton;
    public ItemSlot currentSelectedSlot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize with 10 slots
        for (int i = 0; i < 10; i++)
        {
            ItemSlot slot = CreateNewItemSlot(i == 0);
        }

        InventoryMenu.SetActive(false);
        menuActivated = false;
    }
    private ItemSlot CreateNewItemSlot(bool selectFirstSlot = false)
    {
        GameObject slotObj = Instantiate(itemSlotPrefab, itemSlotParent);
        ItemSlot slot = slotObj.GetComponent<ItemSlot>();
        slot.itemDescriptionNameText = itemDescriptionNameText;
        slot.itemDescriptionText = itemDescriptionText;
        slot.itemDescriptionImage = itemDescriptionImage;
        slot.buttonDisplay = buttonDisplay;
        slot.eatButton = eatButton;
        slot.dropButton = dropButton;
        slot.holdButton = holdButton;
        itemSlots.Add(slot);

        if (selectFirstSlot)
        {
            EventSystem.current.SetSelectedGameObject(slot.gameObject); // Select the first slot
        }

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
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Select the first slot after menu is active
            if (itemSlots.Count > 0)
                EventSystem.current.SetSelectedGameObject(itemSlots[0].gameObject);
        }
    }
    

    public bool UseItem(string itemName)
    {
        for (int i = 0; i < itemsData.Length; i++)
        {
            if(itemsData[i].itemName == itemName)
            {
                bool usable = itemsData[i].UseItem();
                return usable; // Exit the loop once the item is found and used
            }
        }
        return false; // Item not found
    }


    // changed void to int to return leftover items
    public int AddItem(ItemData itemData, int quantity)
    {
        // Try to add to existing slots
        foreach (var slot in itemSlots)
        {
            // Add to slot if not full and either same item or empty
            if (!slot.isFull && (slot.itemName == itemData.itemName || string.IsNullOrEmpty(slot.itemName)))
            {
                int leftover = slot.AddItem(itemData, quantity);
                if (leftover == 0)
                    return 0;
                quantity = leftover;
            }
        }

        // If still have items, create new slot(s)
        while (quantity > 0)
        {
            ItemSlot newSlot = CreateNewItemSlot();
            int leftover = newSlot.AddItem(itemData, quantity);
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
                if (slot.itemDescriptionNameText != null)
                    slot.itemDescriptionNameText.text = "";
                if (slot.itemDescriptionText != null)
                    slot.itemDescriptionText.text = "";
                if (slot.itemDescriptionImage != null && slot.emptySprite != null)
                    slot.itemDescriptionImage.sprite = slot.emptySprite;
                if (slot.buttonDisplay != null)
                    slot.buttonDisplay.SetActive(false);
            }
        }
    }
    public void OnHoldButtonPressed()
    {
        if (currentSelectedSlot != null)
            currentSelectedSlot.HoldButton();
    }
    public void OnDropButtonPressed()
    {
        if (currentSelectedSlot != null)
            currentSelectedSlot.DropButton();
    }
    public void OnEatButtonPressed()
    {
        if (currentSelectedSlot != null)
            currentSelectedSlot.EatButton();
    }

}
