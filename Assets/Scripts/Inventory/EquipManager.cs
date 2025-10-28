using System.Collections.Generic;
using UnityEngine;

public class EquipManager : MonoBehaviour
{
    public Transform toolParent;
    public Transform craftableParent;
    public Transform ingredientParent1;
    public Transform ingredientParent2;
    public Transform ingredientParent3;

    private GameObject equippedTool;
    private ItemData equippedToolData;

    private GameObject equippedCraftable;
    private ItemData equippedCraftableData;

    private GameObject[] equippedIngredients = new GameObject[3];
    private ItemData[] equippedIngredientsData = new ItemData[3];

    // Stack tracks (itemType, ingredientIndex)
    private Stack<(ItemType, int)> equippedOrder = new Stack<(ItemType, int)>();

    public InventoryManager inventoryManager;
    public bool itemEquipped = false;

    [SerializeField]
    private RectTransform putAwayText;

    public GameObject GetEquippedTool() => equippedTool;
    public GameObject GetEquippedCraftable() => equippedCraftable;
    public GameObject[] GetEquippedIngredients() => equippedIngredients;

    private void Update()
    {
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
                if (equippedTool != null || equippedCraftable != null || HasAnyIngredientsEquipped())
                {
                    Debug.Log("Cannot equip tool: another item is already equipped.");
                    return false;
                }
                EquipTool(itemData);
                return true;
            case ItemType.craftable:
                if (equippedCraftable != null || equippedTool != null || HasAnyIngredientsEquipped())
                {
                    Debug.Log("Cannot equip craftable: another item is already equipped.");
                    return false;
                }
                EquipCraftable(itemData);
                return true;
            case ItemType.ingredient:
                if (equippedTool != null || equippedCraftable != null)
                {
                    Debug.Log("Cannot equip ingredient: tool or craftable is already equipped.");
                    return false;
                }
                if (GetIngredientCount() >= 3)
                {
                    Debug.Log("Cannot equip ingredient: already equipped maximum number of ingredients.");
                    return false;
                }
                EquipIngredient(itemData);
                return true;
        }
        Debug.Log("Cannot equip: unsupported item type or other error.");
        return false;
    }

    private void EquipTool(ItemData itemData)
    {
        equippedTool = Instantiate(itemData.equipPrefab, toolParent);
        equippedToolData = itemData;
        equippedTool.transform.localPosition = Vector3.zero;
        itemEquipped = true;
        equippedOrder.Push((ItemType.tool, 0));
        putAwayText.gameObject.SetActive(true);
    }

    private void EquipCraftable(ItemData itemData)
    {
        equippedCraftable = Instantiate(itemData.equipPrefab, craftableParent);
        equippedCraftableData = itemData;
        equippedCraftable.transform.localPosition = Vector3.zero;
        itemEquipped = true;
        equippedOrder.Push((ItemType.craftable, 0));
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
            case ItemType.craftable:
                UnequipCraftable();
                break;
            case ItemType.ingredient:
                UnequipIngredient(index);
                break;
        }
        itemEquipped = HasEquippedItem();

        if (putAwayText != null && !HasEquippedItem())
            putAwayText.gameObject.SetActive(false);
    }

    public bool HasEquippedItem()
    {
        return equippedTool != null || equippedCraftable != null || HasAnyIngredientsEquipped();
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

    public void UnequipCraftable()
    {
        if (equippedCraftable != null)
        {
            Destroy(equippedCraftable);
            if (equippedCraftableData != null)
            {
                inventoryManager.AddItem(equippedCraftableData, 1);
                equippedCraftableData = null;
            }
            equippedCraftable = null;
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