using UnityEngine;

namespace TJ
{
    // Define the Ingredient ScriptableObject
    [CreateAssetMenu(fileName = "NewIngredient", menuName = "Inventory/Ingredient")]
    public class Ingredient : ScriptableObject
    {
        public Sprite ingredientIcon;
        // You can add more properties as needed, e.g.:
        // public string description;
        // public int id;
    }
}
