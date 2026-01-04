using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

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

        // Make the EventSystem's InputSystemUIInputModule use the same runtime asset instance.
        if (EventSystem.current != null)
        {
            var uiModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
            if (uiModule != null)
            {
                uiModule.actionsAsset = playerControls.asset;
            }
        }
    }

    private void Update()
    {
        // keep for polled access if needed
    }

    private void OnEnable()
    {
        // Enable only the Player action map by default.
        playerControls.Player.Enable();
        playerControls.UI.Disable();
    }

    private void OnDisable()
    {
        // Disable both maps to be safe.
        playerControls.Player.Disable();
        playerControls.UI.Disable();
    }

    // Call this when opening the UI menu
    public void SwitchToUI()
    {
        playerControls.Player.Disable();
        playerControls.UI.Enable();

        // Make cursor visible and unlocked for menus
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Call this when closing the UI menu
    public void SwitchToPlayer()
    {
        playerControls.UI.Disable();
        playerControls.Player.Enable();

        // Restore cursor state for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Polled helpers (unchanged logic)
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

    public bool GetSprintInput()
    {
        return playerControls.Player.Sprint.ReadValue<float>() > 0.1f;
    }

    public bool sprint => GetSprintInput();

    public bool IsInteractKeyPressed()
    {
        return playerControls.Player.Interact.ReadValue<float>() > 0f;
    }

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