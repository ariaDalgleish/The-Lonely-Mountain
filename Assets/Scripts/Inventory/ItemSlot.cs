using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    #region Item Data
    [Header("Item Data")]
    public string itemName;

    public int quantity;
    public Sprite itemSprite;
    //public GameObject prefabObject;
    public Sprite sketchSprite;
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

    #region Item Display
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
    public int AddItem(string itemName, int quantity, Sprite itemSprite, Sprite sketchSprite, string itemDescription /*, GameObject prefabObject*/)
    {
        // Check to see if the slot is alreadty full
        if (isFull)
            return quantity;

        // Update NAME
        this.itemName = itemName;

        // Update IMAGE
        this.itemSprite = itemSprite;
        itemImage.sprite = itemSprite;

        this.sketchSprite = sketchSprite;

        //this.prefabObject = prefabObject;

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
            if (quantity > 0)
            {
                inventoryManager.DeselectAllSlots();
                selectedShader.SetActive(true);
                thisItemSelected = true;

                ItemDescriptionNameText.text = itemName;
                ItemDescriptionText.text = itemDescription;
                itemDescriptionImage.sprite = sketchSprite;

                if (itemDescriptionImage.sprite == null)
                    itemDescriptionImage.sprite = emptySprite;
            }
        }
    }

    public void EmptySlot()
    {
        quantityText.enabled = false;
        itemImage.sprite = emptySprite;
        ItemDescriptionNameText.text = "";
        ItemDescriptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;

        // Reset all item data
        itemName = "";
        itemSprite = null;
        sketchSprite = null;
        itemDescription = "";
        isFull = false;
        quantity = 0;
        thisItemSelected = false;

        thisItemSelected = false;
        if (selectedShader != null)
            selectedShader.SetActive(false);
    }
    public void OnDropButtonClick()
    {
        if (thisItemSelected && quantity > 0)
        {
            ItemSO itemSO = inventoryManager.itemSOs
                .FirstOrDefault(so => so.itemName == itemName);

            if (itemSO != null && itemSO.itemPrefab != null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    Instantiate(itemSO.itemPrefab, player.transform.position, Quaternion.identity);
                    quantity -= 1;
                    quantityText.text = quantity.ToString();
                    if (quantity <= 0)
                        EmptySlot();
                }
            }
        }
    }
    public void OnRightClick()
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
                    quantityText.text = this.quantity.ToString();
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
