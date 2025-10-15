using UnityEngine;
using UnityEngine;
using System.Collections.Generic;

public class EquipManager : MonoBehaviour
{
    public Transform toolParent;
    public Transform ingredientParent1;
    public Transform ingredientParent2;
    public Transform ingredientParent3;

    private GameObject equippedTool;
    private ItemData equippedToolData;

    private GameObject[] equippedIngredients = new GameObject[3];
    private ItemData[] equippedIngredientsData = new ItemData[3];

    // Stack tracks (itemType, ingredientIndex)
    private Stack<(ItemType, int)> equippedOrder = new Stack<(ItemType, int)>();

    public InventoryManager inventoryManager;
    public bool itemEquipped = false;

    [SerializeField]
    private RectTransform putAwayText;

    public GameObject GetEquippedTool() => equippedTool;
    public GameObject[] GetEquippedIngredients() => equippedIngredients;

    private void Update()
    {
        // "PutAway" input unequips the most recently equipped item, if any.
        if (InputManager.Instance.PutAway() && HasEquippedItem())
        {
            UnequipLast();
        }
    }

    public bool Equip(ItemData itemData)
    {
        if (itemData == null)
        {
            Debug.LogError("EquipManager.Equip called with null itemData!");
            return false;
        }

        switch (itemData.itemType)
        {
            case ItemType.tool:
                if (equippedTool != null)
                {
                    Debug.Log("Cannot equip tool: a tool is already equipped.");
                    return false; // Block equip
                }
                if (HasAnyIngredientsEquipped())
                {
                    Debug.Log("Cannot equip tool: ingredients are already equipped.");
                    return false; // Block equip
                }
                EquipTool(itemData);      
                return true;
            case ItemType.ingredient:
                if (equippedTool != null)
                {
                    Debug.Log("Cannot equip ingredient: a tool is already equipped.");
                    return false; // Block equip
                }
                if (GetIngredientCount() >= 3)
                {
                    Debug.Log("Cannot equip ingredient: already equipped maximum number of ingredients.");
                    return false; // Block equip
                }
                EquipIngredient(itemData);
                return true;
        }
        Debug.Log("Cannot equip: unsupported item type or other error.");
        return false;
    }

    private void EquipTool(ItemData itemData)
    {
        //if (equippedTool != null) Destroy(equippedTool);
        equippedTool = Instantiate(itemData.equipPrefab, toolParent);
        equippedToolData = itemData;
        equippedTool.transform.localPosition = Vector3.zero;
        itemEquipped = true;
        equippedOrder.Push((ItemType.tool, 0)); // 0 index for tool
        // No need to check for existing tool since Equip blocks if one is already equipped
        putAwayText.gameObject.SetActive(true);
    }

    private void EquipIngredient(ItemData itemData)
    {
        for (int i = 0; i < equippedIngredients.Length; i++)
        {
            if (equippedIngredients[i] == null)
            {
                equippedIngredients[i] = Instantiate(itemData.equipPrefab, GetIngredientParent(i));
                equippedIngredients[i].transform.localPosition = Vector3.zero;
                equippedIngredientsData[i] = itemData;
                itemEquipped = true;
                equippedOrder.Push((ItemType.ingredient, i));
                if (putAwayText != null && !putAwayText.gameObject.activeSelf)
                    putAwayText.gameObject.SetActive(true);
                break;
            }
        }
    }

    public void UnequipLast()
    {
        if (equippedOrder.Count == 0)
            return;

        var (itemType, index) = equippedOrder.Pop();
        switch (itemType)
        {
            case ItemType.tool:
                UnequipTool();
                break;
            case ItemType.ingredient:
                UnequipIngredient(index);
                break;
        }
        // Update itemEquipped status
        itemEquipped = HasEquippedItem();

        // Deactivate putAwayText if nothing is equipped
        if (putAwayText != null && !HasEquippedItem())
            putAwayText.gameObject.SetActive(false);
    }

    public bool HasEquippedItem()
    {
        return equippedTool != null || HasAnyIngredientsEquipped();
    }

    private bool HasAnyIngredientsEquipped()
    {
        foreach (var ing in equippedIngredients)
            if (ing != null) return true;
        return false;
    }

    private int GetIngredientCount()
    {
        int count = 0;
        foreach (var ing in equippedIngredients)
            if (ing != null) count++;
        return count;
    }

    public void UnequipTool()
    {
        if (equippedTool != null)
        {
            Destroy(equippedTool);
            if (equippedToolData != null)
            {
                inventoryManager.AddItem(equippedToolData, 1);
                equippedToolData = null;
            }
            equippedTool = null;
        }
    }

    public void UnequipIngredient(int slot)
    {
        if (equippedIngredients[slot] != null)
        {
            Destroy(equippedIngredients[slot]);
            if (equippedIngredientsData[slot] != null)
            {
                inventoryManager.AddItem(equippedIngredientsData[slot], 1);
                equippedIngredientsData[slot] = null;
            }
            equippedIngredients[slot] = null;
        }
    }

    private Transform GetIngredientParent(int slot)
    {
        switch (slot)
        {
            case 0: return ingredientParent1;
            case 1: return ingredientParent2;
            case 2: return ingredientParent3;
            default: return null;
        }
    }
}