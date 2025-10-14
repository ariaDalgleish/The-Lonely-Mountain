using UnityEngine;
public enum ItemType { ingredient, tool, craftable, other }

[CreateAssetMenu(menuName = "Inventory/ItemData")] // This attribute allows you to create instances of this ScriptableObject from the Unity Editor
public class ItemData : ScriptableObject
{

    #region Item Data"
    [Header("Iem Data")]
    public string itemName;
    public string itemDescription; 
    public Sprite itemSprite;      
    public Sprite sketchSprite;    
    public GameObject itemPrefab;
    public GameObject equipPrefab;
    #endregion

    public ItemType itemType;
    public StatToChange statToChange = new StatToChange();
    public AttributesToChange attributesToChange = new AttributesToChange();
    public int amountToChangeStat;
    public int amountToChangeAttribute;


    private SurvivalManager survivalManager;


    public bool UseItem()
    {
        if (survivalManager == null)
        {
            var obj = GameObject.Find("Player");
            if (obj != null)
                survivalManager = obj.GetComponent<SurvivalManager>();
        }

        if (survivalManager == null)
        {
            Debug.LogError("SurvivalManager not found in scene!");
        }

        if (statToChange == StatToChange.health)
        {
            if (survivalManager._currentHealth >= survivalManager._maxHealth - amountToChangeStat)
            {
                // UIMessageManager.Instance?.ShowMessage("Cannot use item: Health is already full!");
                Debug.Log("Cannot use item, health is full.");
                // Do nothing if using the item would overfill health
                return false;
            }
            else 
            { 
                survivalManager.AddHealth(amountToChangeStat);
                return true;
            }
        }

        else if (statToChange == StatToChange.cold)
        {
            if (survivalManager._currentCold >= survivalManager._maxCold - amountToChangeStat)
            {
                // UIMessageManager.Instance?.ShowMessage("Cannot use item: Cold is already full!");
                Debug.Log("Cannot use item, cold is full.");
                // Do nothing if using the item would overfill cold
                return false;
            }
            else 
            { 
                survivalManager.AddCold(amountToChangeStat);
                return true;
            }
        }

        else if(statToChange == StatToChange.hunger)
        {
            if (survivalManager._currentHunger >= survivalManager._maxHunger - amountToChangeStat)
            {
                // UIMessageManager.Instance?.ShowMessage("Cannot use item: Hunger is already full!");
                Debug.Log("Cannot use item, hunger is full.");
                // Do nothing if using the item would overfill hunger
                return false;
            }
            else
            {
                survivalManager.AddHunger(amountToChangeStat);
                return true;
            }
        }

        else if(statToChange == StatToChange.thirst)
        {
            if (survivalManager._currentThirst >= survivalManager._maxThirst - amountToChangeStat)
            {
                // UIMessageManager.Instance?.ShowMessage("Cannot use item: Thirst is already full!");
                Debug.Log("Cannot use item, thirst is full.");
                // Do nothing if using the item would overfill thirst
                return false;
            }
            else
            {
                survivalManager.AddThirst(amountToChangeStat);
                return true;
            }
        }

        else if(statToChange == StatToChange.fatigue)
        {
            if (survivalManager._currentFatigue >= survivalManager._maxFatigue - amountToChangeStat)
            {
                // UIMessageManager.Instance?.ShowMessage("Cannot use item: Fatigue is already full!");
                Debug.Log("Cannot use item, fatigue is full.");
                // Do nothing if using the item would overfill fatigue
                return false;
            }
            else
            {
                survivalManager.AddFatigue(amountToChangeStat);
                return true;
            }
        }
        return false;
    }

    public enum StatToChange
    {
        none,
        health,
        stamina,
        cold,
        hunger,
        thirst,
        fatigue
    };
    public enum AttributesToChange
    {
        none,
        health,
        stamina,
        cold,
        hunger,
        thirst,
        fatigue
    };
}
