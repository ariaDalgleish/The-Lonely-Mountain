using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UIElements;

public class CH_Inventory : MonoBehaviour
{
    /*
    private PlayerController playerController;
    private SurvivalManager survivalManager;

    private bool inventoryOpended;

    public Texture EmptySlotTexture;
    public Texture ChestIcon;
    public Texture LeftArmIcon;
    public Texture RightWeaponIcon;
    public Texture HeadIcon;
    public Texture ShoulderIcon;
    public Texture BootsIcon;
    public Texture MedKitIcon;
    public Texture AmmoIcon;

    private Rect windowRect = new(20, 20, 200, 200);

    private readonly DraggableObject DraggedObject;
    private Rect draggablesection = new(10, 10, 30, 30);
    private Rect ChestSlotLoc = new(5, 130, 32, 32);
    private readonly InventorySlot ChestSlot;
    private Rect RightWeaponSlotLoc = new(5, 85, 32, 32);
    private readonly InventorySlot RightWeaponSlot;
    private Rect LeftArmSlotLoc = new(160, 85, 32, 32);
    private readonly InventorySlot LeftArmSlot;
    private Rect HeadSlotLoc = new(5, 40, 32, 32);
    private readonly InventorySlot HeadSlot;
    private Rect ShoulderSlotLoc = new(160, 40, 32, 32);
    private readonly InventorySlot ShoulderSlot;
    private Rect BootsSlotLoc = new(160, 130, 32, 32);
    private readonly InventorySlot BootsSlot;
    public InventorySlot[] InventorySet;
    bool dragging = false;
    private Vector2 coord = Vector2.zero;

    private int counter = 0;
    private Vector2 scrollPosition;

    public int ClickCount;
    public InventorySlot LastClick;

    private void Awake()
    {
        InventorySet = new InventorySlot[40];
        ChestSlot.icon = ChestIcon;
        ChestSlot.Type = "Chest";
        ChestSlot.ConstTtype = true;
        RightWeaponSlot.icon = RightWeaponIcon;
        RightWeaponSlot.Type = "RightWeapon";
        RightWeaponSlot.ConstTtype = true;
        LeftArmSlot.icon = LeftArmIcon;
        LeftArmSlot.Type = "LeftArm";
        LeftArmSlot.ConstTtype = true;
        HeadSlot.icon = HeadIcon;
        HeadSlot.Type = "Head";
        HeadSlot.ConstTtype = true;
        ShoulderSlot.icon = ShoulderIcon;
        ShoulderSlot.Type = "Shoulder";
        ShoulderSlot.ConstTtype = true;
        BootsSlot.icon = BootsIcon;
        BootsSlot.Type = "Boots";
        BootsSlot.ConstTtype = true;

    }

    private void Start()
    {
        // Reference scripts
        playerController = GetComponent<PlayerController>();
        survivalManager = GetComponent<SurvivalManager>();
    }

    void OnGUI()
    {
        windowRect = GUI.Window(0, windowRect, DoMyWindow, "Inventory");
        Vector3 coord = Input.mousePosition;
        coord.y = Screen.height - coord.y;

        if (dragging)
        {
            // Update the draggable section's position based on mouse coordinates
            Rect draggablesection = new (coord.x - 15, coord.y - 15, 30, 30);

            // Ensure the draggable object exists before drawing
            if (DraggedObject != null && DraggedObject.icon != null)
            {
                GUI.Box(draggablesection, new GUIContent(DraggedObject.icon));
            }
        }

        // Check if dragging is true and the mouse is released
        // Check if hovering slot type is the same as the dragged object type
        // If so, assign the dragged object to the hovering slot and make it empty.
        // FinishingDrag function
        if (dragging && Event.current.type == EventType.MouseUp)
        {
            if (DraggedObject.HoveringSlot.Focused && DraggedObject.HoveringSlot.isEmpty)
            {
                if (DraggedObject.HoveringSlot.Type == DraggedObject.Type ||
                    !DraggedObject.HoveringSlot.ConstTtype)
                {
                    switch (DraggedObject.HoveringSlot.Type)
                    {
                        case " RightWeapon":
                            //Controller.EquipWeapon();
                            break;
                        case "Chest":
                            break;
                        case "Ammo":
                            break;
                        default:
                            break;
                    }
                    FinishingDrag(true);
                }
                else
                {
                    FinishingDrag(false);
                }
                dragging = false;
            }
        }
    }

    private void FinishingDrag(bool successful)
    {
        switch (successful)
        {
            case true:
                DraggedObject.HoveringSlot.icon = DraggedObject.icon;
                DraggedObject.HoveringSlot.Type = DraggedObject.Type;
                DraggedObject.HoveringSlot.isEmpty = false;
                break;

            // Default case
            default:
                DraggedObject.LastSlot.isEmpty = false;
                DraggedObject.LastSlot.icon = DraggedObject.icon;
                DraggedObject.LastSlot.Type = DraggedObject.Type;
                break;
        }
        DraggedObject.LastSlot = null;
    }



    // GUIUtility.GUIToScreenPoint (),
    // takes the position of the GUI within the group and returns its positin relavtive to the screen.
    // Handle all window manipulations.
    // Check if slots are being hoverred by mouse
    // Check if we are currently dragging an object or ttrying to drag out an item from that slot
    // Assign the current slot to the draggable object - hoveringSlot. we will toggle dragging and make current slot empty.
    // Next to get buttons on the screen.
    void DoMyWindow(int windowID)
    {
        if (GUI.Button(new Rect(windowRect.width - 32, 0, 32, 32), "X"))
        {
            inventoryOpended = false;
        }
        ChestSlot.location = GUIUtility.GUIToScreenPoint(new Vector2(ChestSlotLoc.x, ChestSlotLoc.y));
        RightWeaponSlot.location = GUIUtility.GUIToScreenPoint(new Vector2(RightWeaponSlotLoc.x, RightWeaponSlotLoc.y));
        LeftArmSlot.location = GUIUtility.GUIToScreenPoint(new Vector2(LeftArmSlotLoc.x, LeftArmSlotLoc.y));
        HeadSlot.location = GUIUtility.GUIToScreenPoint(new Vector2(HeadSlotLoc.x, HeadSlotLoc.y));
        ShoulderSlot.location = GUIUtility.GUIToScreenPoint(new Vector2(ShoulderSlotLoc.x, ShoulderSlotLoc.y));
        BootsSlot.location = GUIUtility.GUIToScreenPoint(new Vector2(BootsSlotLoc.x, BootsSlotLoc.y));
        ChestSlot.CheckFocus();
        RightWeaponSlot.CheckFocus();
        LeftArmSlot.CheckFocus();
        HeadSlot.CheckFocus();
        ShoulderSlot.CheckFocus();
        BootsSlot.CheckFocus();
        if (ChestSlot.Focused)
        {
            if (dragging)
            {
                DraggedObject.HoveringSlot = ChestSlot;
            }
            else if (Event.current.type == EventType.MouseDrag && ChestSlot. isEmpty == false)
            {
                if (!dragging)
                {
                    DraggedObject.icon = ChestSlot.icon;
                    DraggedObject.Type = ChestSlot.Type;
                    DraggedObject.LastSlot = ChestSlot;
                    ChestSlot.isEmpty = true;
                    ChestSlot.icon = ChestIcon;
                }
                dragging = true;
            }
        }

        if (RightWeaponSlot.Focused)
        {
            if (dragging)
            {
                DraggedObject.HoveringSlot = RightWeaponSlot;
            }
            else if (Event.current.type == EventType.MouseDrag && RightWeaponSlot.isEmpty == false)
            {
                if (!dragging)
                {
                    DraggedObject.icon = RightWeaponSlot.icon;
                    DraggedObject.Type = RightWeaponSlot.Type;
                    DraggedObject.LastSlot = RightWeaponSlot;
                    RightWeaponSlot.isEmpty = true;
                    RightWeaponSlot.icon = RightWeaponIcon;
                }
                dragging = true;
            }
        }
        
        if (LeftArmSlot.Focused)
        {
            if (dragging)
            {
                DraggedObject.HoveringSlot = LeftArmSlot;
            }
            else if (Event.current.type == EventType.MouseDrag && LeftArmSlot.isEmpty == false)
            {
                if (!dragging)
                {
                    DraggedObject.icon = LeftArmSlot.icon;
                    DraggedObject.Type = LeftArmSlot.Type;
                    DraggedObject.LastSlot = LeftArmSlot;
                    LeftArmSlot.isEmpty = true;
                    LeftArmSlot.icon = LeftArmIcon;
                }
                dragging = true;
            }
        }
        if (HeadSlot.Focused)
        {
            if (dragging)
            {
                DraggedObject.HoveringSlot = HeadSlot;
            }
            else if (Event.current.type == EventType.MouseDrag && HeadSlot.isEmpty == false)
            {
                if (!dragging)
                {
                    DraggedObject.icon = HeadSlot.icon;
                    DraggedObject.Type = HeadSlot.Type;
                    DraggedObject.LastSlot = HeadSlot;
                    HeadSlot.isEmpty = true;
                    HeadSlot.icon = HeadIcon;
                }
                dragging = true;
            }
        }

        if (ShoulderSlot.Focused)
        {
            if (dragging)
            {
                DraggedObject.HoveringSlot = ShoulderSlot;
            }
            else if (Event.current.type == EventType.MouseDrag && ShoulderSlot.isEmpty == false)
            {
                if (!dragging)
                {
                    DraggedObject.icon = ShoulderSlot.icon;
                    DraggedObject.Type = ShoulderSlot.Type;
                    DraggedObject.LastSlot = LeftArmSlot;
                    ShoulderSlot.isEmpty = true;
                    ShoulderSlot.icon = ShoulderIcon;
                }
                dragging = true;
            }
        }

        if (BootsSlot.Focused)
        {
            if (dragging)
            {
                DraggedObject.HoveringSlot = BootsSlot;
            }
            else if (Event.current.type == EventType.MouseDrag && BootsSlot.isEmpty == false)
            {
                if (!dragging)
                {
                    DraggedObject.icon = BootsSlot.icon;
                    DraggedObject.Type = BootsSlot.Type;
                    DraggedObject.LastSlot = BootsSlot;
                    BootsSlot.isEmpty = true;
                    BootsSlot.icon = BootsIcon;
                }
                dragging = true;
            }
        }

        if (GUI.RepeatButton(ChestSlotLoc, ChestSlot.icon) && !dragging) { }
        if (GUI.RepeatButton(RightWeaponSlotLoc, RightWeaponSlot.icon) && !dragging) { }
        if (GUI.RepeatButton(LeftArmSlotLoc, LeftArmSlot.icon) && !dragging) { }
        if (GUI.RepeatButton(HeadSlotLoc, HeadSlot.icon) && !dragging) { }
        if (GUI.RepeatButton(ShoulderSlotLoc, ShoulderSlot.icon) && !dragging) { }
        if (GUI.RepeatButton(BootsSlotLoc, BootsSlot.icon) && !dragging) { }
     

        GUI.Label(new Rect(60, 150, 100, 20), "Money: " + 0);
        GUI.Label(new Rect(60, 150, 100, 20), "Money: " + 0 Stats.GetMoney());

     
        // Create rows and columns for the inventory slots
        // Find the location of each inventory slot and store it inside the previously created InventorySet array.
        // First, check if InventorySlot with a specific index number exists
        // If exists, next step is storying its location in the inside location variable of the specific InventorySlot
        // Check if the mouse is over this slot and the user to trying to drag the object out of the slot.
        
        scrollPosition = GUI.BeginScrollView(new Rect(0, 200, 200, 100), scrollPosition, new Rect(0, 0, 300, 120));
        counter = 0;
        for (int i = 0; i < 10; i ++)
        {
            for ( int j = 0; j < 4; j++)
            {
                if (InventorySet[counter] != null)
                {
                    InventorySet[counter].location = GUIUtility.GUIToScreenPoint(new Vector2(30 * i, 30 * j));
                    if (InventorySet[counter].isEmpty)
                    {
                        InventorySet[counter].icon = EmptySlotTexture;
                    }
                    InventorySet[counter].CheckFocus();
                    if (InventorySet[counter].Focused == true && Event.current.type == 
                        EventType.MouseDrag && InventorySet[counter].isEmpty == false)
                    {
                        if (!dragging)
                        {
                            DraggedObject.icon = InventorySet[counter].icon;
                            DraggedObject.Type = InventorySet[counter].Type;
                            DraggedObject.LastSlot = InventorySet[counter];
                            InventorySet[counter].isEmpty = true;
                            InventorySet[counter].icon = EmptySlotTexture;
                        }

                        dragging = true;
                        DraggedObject.HoveringSlot = InventorySet[counter];                      
                        
                    }
                    else if (InventorySet[counter].Focused && Event.current.type == EventType.MouseDown)
                    {
                        if (ClickCount == 0)
                        {
                            LastClick = InventorySet[counter];
                            if (!dragging) ClickCount++;
                        }
                        else
                        {
                            if (LastClick.location == InventorySet[counter].location && ClickCount == 1)
                            {
                                switch (InventorySet[counter].Type)
                                {
                                    case "RightWeapon":
                                        if (RightWeaponSlot.isEmpty)
                                        {
                                            RightWeaponSlot.isEmpty = false;
                                            RightWeaponSlot.icon = InventorySet[counter].icon;
                                            InventorySet[counter].isEmpty = true;
                                            InventorySet[counter].icon = EmptySlotTexture;
                                        }
                                    break;

                                    case "LeftArm":
                                        if (LeftArmSlot.isEmpty)
                                        {
                                            LeftArmSlot.isEmpty = false;
                                            LeftArmSlot.icon = InventorySet[counter].icon;
                                            InventorySet[counter].isEmpty = true;
                                            InventorySet[counter].icon = EmptySlotTexture;
                                        }
                                    break;

                                    case "Head":
                                        if (HeadSlot.isEmpty)
                                        {
                                            HeadSlot.isEmpty = false;
                                            HeadSlot.icon = InventorySet[counter].icon;
                                            InventorySet[counter].isEmpty = true;
                                            InventorySet[counter].icon = EmptySlotTexture;
                                        }
                                    break;

                                    case "Shoulder":
                                        if (RightWeaponSlot.isEmpty)
                                        {
                                            ShoulderSlot.isEmpty = false;
                                            ShoulderSlot.icon = InventorySet[counter].icon;
                                            InventorySet[counter].isEmpty = true;
                                            InventorySet[counter].icon = EmptySlotTexture;
                                        }
                                    break;

                                    case "Boots":
                                        if (BootsSlot.isEmpty)
                                        {
                                            BootsSlot.isEmpty = false;
                                            BootsSlot.icon = InventorySet[counter].icon;
                                            InventorySet[counter].isEmpty = true;
                                            InventorySet[counter].icon = EmptySlotTexture;
                                        }
                                    break;

                                    case "Chest":
                                        if (RightWeaponSlot.isEmpty)
                                        {
                                            ChestSlot.isEmpty = false;
                                            ChestSlot.icon = InventorySet[counter].icon;
                                            InventorySet[counter].isEmpty = true;
                                            InventorySet[counter].icon = EmptySlotTexture;
                                        }
                                    break;

                                    case "MedKit":

                                        if (survivalManager != null && survivalManager.HealthPercent < 1.0f)
                                        {
                                            
                                            survivalManager.RegenerateHealth();
                                            InventorySet[counter].isEmpty = true;
                                            InventorySet[counter].icon = EmptySlotTexture;
                                            Debug.Log("Used MedKit, current health percent: " + survivalManager.HealthPercent);
                                        }
                                    break;

                                }
                                
                            }

                            ClickCount = 0;


                        }

                    }

                    else if (InventorySet[counter].Focused && dragging)
                    {
                        DraggedObject.HoveringSlot = InventorySet[counter];
                    }
              
                }

                if (InventorySet[0] == null)
                {
                    GUI.Box(new Rect(30 * i, 30 * j, 30, 30), new GUIContent(EmptySlotTexture));
                }
                else
                {
                    GUI.Box(new Rect(30 * i, 30 * j, 30, 30), new GUIContent(InventorySet[counter].icon));
                }
                counter++;
            }

        }

        GUI.EndScrollView();
        GUI.DragWindow(); 
 
    }
            
}


[System.Serializable] // class serializable in Unity's inspector
public class InventorySlot
{
    public Texture icon;
        public string Type;
        public Vector2 location;
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

        // Double click empty inventory slot to automatically assign a weaoin; or double click on med kit increases the character health.
}
  
    

[System.Serializable]
class DraggableObject
{
     public InventorySlot LastSlot; // changed from private to public
     public InventorySlot HoveringSlot;
     public Texture icon;
     public string Type;
}
*/
}


