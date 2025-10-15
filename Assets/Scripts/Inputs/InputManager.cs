using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;



    public static InputManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public PlayerControls playerControls;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        playerControls = new PlayerControls();
    }
    private void Update()
    {
        //MenuOpenInput = menuOpenAction.WasPressedThisFrame();
           // != null && menuOpenAction.triggered;
        //UIMenuClose = _UIMenuCloseAction.WasPressedThisFrame();
        //InventoryOpen = _inventoryOpenAction != null && _inventoryOpenAction.triggered;
        //InventoryClose = _inventoryCloseAction != null && _inventoryCloseAction.triggered;
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public Vector2 GetMovementInput()
    {
        return playerControls.Player.Movement.ReadValue<Vector2>();

    }

    public Vector2 GetMouseDelta()
    {
        return playerControls.Player.Look.ReadValue<Vector2>();
    }

    public bool IsJumpKeyPressed()
    {
        return playerControls.Player.Jump.triggered;
    }

    // Example method to check if the sprint key is pressed
    public bool GetSprintInput()
    {
        return playerControls.Player.Sprint.ReadValue<float>() > 0.1f;
    }
    public bool sprint => GetSprintInput();

    public bool IsInteractKeyPressed()
    {
        // Returns true as long as the interact key is held down
        return playerControls.Player.Interact.ReadValue<float>() > 0f;
    }

    //public bool IsCrouchKeyPressed()
    //{
    //    return playerControls.Player.Crouch.triggered;
    //}

    // Example method to check if the inventory key is pressed

    public bool InventoryOpen()
    {
        return playerControls.Player.InventoryOPEN.triggered;
    }
    public bool InventoryClose()
    {
        return playerControls.UI.InventoryCLOSE.triggered;
    }
    public bool MenuOpen()
    {
        return playerControls.Player.MenuOPEN.triggered;
    }
    public bool MenuClose()
    {
        return playerControls.UI.MenuCLOSE.triggered;
    }
    public bool Y()
    {
        return playerControls.Player.Y.triggered;
    }

    public bool PutAway()
    {
        return playerControls.Player.PutAway.triggered;
    }

    
}