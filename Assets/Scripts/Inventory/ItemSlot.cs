using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, ISubmitHandler
{
    #region Item Data
    [Header("Item Data")]
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public Sprite sketchSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;
    #endregion

    #region item Slot
    [Header("Item Slot")]
    [SerializeField] public int maxNumberOfItems;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;
    #endregion

    #region Item Display
    [Header("Item Display")]
    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;
    public GameObject buttonDisplay;
    public Button eatButton;
    public Button holdButton;
    public Button dropButton;
    #endregion

    public bool thisItemSelected;
    public ItemSO itemSO; // Add this field

    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();

        // Button Click Events
        if (eatButton != null) eatButton.onClick.AddListener(OnLeftClick);
        if (holdButton != null) holdButton.onClick.AddListener(HoldButton);
        if (dropButton != null) dropButton.onClick.AddListener(DropButton);

        // Hide buttons initially
        SetDisplayButtonsActive(false);
    }

  
    private void SetDisplayButtonsActive(bool isActive)
    {
        // Only set active if all buttons are not null
        if (eatButton != null && holdButton != null && dropButton != null)
        {
            buttonDisplay.gameObject.SetActive(isActive);
        }
    }

    public int AddItem(ItemSO itemSO, int quantity)
    {
        if (isFull)
            return quantity;

        SetItemData(itemSO); // Set item data from ItemSO
        UpdateItemUI(); // Update UI elements with new item data

        this.quantity += quantity; // Add incoming quantity
        if (this.quantity >= maxNumberOfItems) // Check if exceeds max
        {
            int extraItems = this.quantity - maxNumberOfItems; // Calculate leftover items
            this.quantity = maxNumberOfItems; // Cap to max
            isFull = true;
            UpdateQuantityUI();
            return extraItems;
        }

        isFull = false;
        UpdateQuantityUI();
        return 0;
    }

    // Helper to set item data from ItemSO
    private void SetItemData(ItemSO itemSO)
    {
        this.itemSO = itemSO; // Store reference
        itemName = itemSO.itemName;
        itemSprite = itemSO.itemSprite;
        sketchSprite = itemSO.sketchSprite;
        itemDescription = itemSO.itemDescription;
    }

    // Helper to update all item UI elements
    private void UpdateItemUI()
    {
        if (itemDescriptionNameText != null)
            itemDescriptionNameText.text = itemName;
        if (itemDescriptionText != null)
            itemDescriptionText.text = itemDescription;
        if (itemDescriptionImage != null)
            itemDescriptionImage.sprite = sketchSprite;
        if (itemImage != null)
            itemImage.sprite = itemSprite;
    }

    // Helper to update quantity UI
    private void UpdateQuantityUI()
    {
        if (quantityText != null)
        {
            quantityText.text = quantity.ToString();
            quantityText.enabled = quantity > 0;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
    }
    public void OnSubmit(BaseEventData eventData)
    {
        OnLeftClick();
    }



    public void OnLeftClick()
    {
        if (quantity > 0)
        {
            inventoryManager.DeselectAllSlots();
            thisItemSelected = true;
            itemDescriptionNameText.text = itemName;
            itemDescriptionText.text = itemDescription;
            itemDescriptionImage.sprite = sketchSprite;

            if (itemDescriptionImage.sprite == null)
                itemDescriptionImage.sprite = emptySprite;

            SetDisplayButtonsActive(true);
            }
        
    }

    public void EmptySlot()
    {
        quantityText.enabled = false;
        itemImage.sprite = emptySprite;
        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;

        // Reset all item data
        itemSO = null; // Clear reference
        itemName = "";
        itemSprite = null;
        sketchSprite = null;
        itemDescription = "";
        isFull = false;
        quantity = 0;
        thisItemSelected = false;

        thisItemSelected = false;
       
    }

    


    public void EatButton()
    {
        if (thisItemSelected)
        {
            bool usable = inventoryManager.UseItem(itemName);
            if (usable)
            {
                this.quantity -= 1;
                //update quantity text
                quantityText.text = this.quantity.ToString();
                if (this.quantity <= 0)
                    EmptySlot();
            }
        }
    }
    public void HoldButton()
    {
        // Currently does nothing
        return;
    }

    public void DropButton()
    {
        if (thisItemSelected)
        {
            // Find the ItemSO that matches the itemName
            ItemSO itemSO = inventoryManager.itemSOs
            .FirstOrDefault(so => so.itemName == itemName);

            // If found and has a prefab, instantiate it
            if (itemSO != null && itemSO.itemPrefab != null)
            {
                // Find the player
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    // Instantiate the prefab at the player's position
                    Instantiate(itemSO.itemPrefab, player.transform.position, Quaternion.identity);

                    // Successfully used the item, now decrease quantity
                    this.quantity -= 1;
                    UpdateQuantityUI();
                    if (this.quantity <= 0)
                    {
                        EmptySlot();
                    }
                }
            }
        }
        else
        {
            return;
        }
    }

}

