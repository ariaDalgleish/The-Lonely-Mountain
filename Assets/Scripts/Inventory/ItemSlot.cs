using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    #region Item Data
    [Header("Item Data")]
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;
    #endregion

    #region item Slot
    [Header("Item Slot")]
    [SerializeField]
    public int maxNumberOfItems;
    [SerializeField] private TMP_Text quantityText;

    [SerializeField]
    private Image itemImage;
    #endregion

    #region Item Description Slot"
    [Header("Item Description Slot")]
    public Image itemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;
    #endregion

    public GameObject selectedShader;
    public bool thisItemSelected;
    

    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }
    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        // Check to see if the slot is alreadty full
        if (isFull)
            return quantity;

        // Update NAME
        this.itemName = itemName;

        // Update IMAGE
        this.itemSprite = itemSprite;
        itemImage.sprite = itemSprite;

        // Update DESCRIPTION
        this.itemDescription = itemDescription;

        // Update QUANTITY
        this.quantity += quantity;
        if (this.quantity >= maxNumberOfItems)
        {
            quantityText.text = maxNumberOfItems.ToString();
            quantityText.enabled = true;
            isFull = true;

            /// return the LEFTOVERS
            int extraItems = this.quantity - maxNumberOfItems;
            this.quantity = maxNumberOfItems;
            return extraItems;
        }

        //Update QUANTITY TEXT
        quantityText.text = this.quantity.ToString();
        quantityText.enabled = true;

        // Slot is now full
        return 0;

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button== PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
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

        else
        {
            inventoryManager.DeselectAllSlots();
            selectedShader.SetActive(true);
            thisItemSelected = true;

            ItemDescriptionNameText.text = itemName;
            ItemDescriptionText.text = itemDescription;
            itemDescriptionImage.sprite = itemSprite;

            if (itemDescriptionImage.sprite == null)
                itemDescriptionImage.sprite = emptySprite;
        }
    }

    private void EmptySlot()
    {
        quantityText.enabled = false;
        itemImage.sprite = emptySprite;
        ItemDescriptionNameText.text = "";
        ItemDescriptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;
    }

    public void OnRightClick()
    {
        
    }
}
