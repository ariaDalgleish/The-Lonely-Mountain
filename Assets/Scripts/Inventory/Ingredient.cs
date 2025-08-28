using UnityEngine;

namespace TJ
{
    [CreateAssetMenu(fileName = "NewIngredient", menuName = "Inventory/Ingredient")]
    public class Ingredient : ScriptableObject
    {
        public Sprite ingredientIcon;
        // You can add more properties as needed, e.g.:
        // public string description;
        // public int id;
    }
}
