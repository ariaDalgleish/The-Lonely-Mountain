using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CH_Inventory : MonoBehaviour
{
    private bool inventoryOpended;

    public Texture EmprtySlotTexture;
    public Texture ChestIcon;
    public Texture LeftArmIcon;
    public Texture RightArmIcon;
    public Texture HeadIcon;
    public Texture ShoulderIcon;
    public Texture BootsIcon;
    public Texture MedKitIcon;
    public Texture AmmoIcon;

    private Rect windowRect = new(20, 20, 200, 200);

    void OnGUIU()
    {
        windowRect = GUI.Window(0, windowRect, DoMyWindow, "Inventory");
    }

    void DoMyWindow(int windowID)
    {
        if (GUI.Button(new Rect(windowRect.width - 32, 0, 32, 32), "X"))
        {
            inventoryOpended = false;
        }
    }

    [System.Serializable] // class serializable in Unity's inspector
    class InventorySlot
    {
        public Texture icon;
        public string Type;
        private Vector2 location;
        public bool isEmpty = true; // Indicates if the slot is empty
        public bool Focused; // Is mouse over the slot
        public bool ConstTtype = false; // Is generic slot type or specific type

        public void CheckFocus() 
        { 
            if (Input.mousePosition.y > (Screen.height - location.y -32) && 
                Input.mousePosition.y < (Screen.height - location.y) && 
                Input.mousePosition.x > location.x && 
                Input.mousePosition.x < (location.x + 32))
            {
                Focused = true;
            }
            else
            {
                Focused = false;
            }
        }
    }
  
    private DraggableObject DraggedObject;


    [System.Serializable]
    class DraggableObject
    {
        private InventorySlot LastSlot; // readonly
        private InventorySlot HoveringSlot; // readonly
        public Texture icon;
        public string Type;
    }
   
    private InventorySlot[] InventorySet;

    private void Awake()
    {
        InventorySet = new InventorySlot[40];
    }

}
