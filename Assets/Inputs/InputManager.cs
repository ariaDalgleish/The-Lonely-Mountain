using UnityEngine;

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

    private PlayerControls playerControls;

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

    public bool GetSprintInput()
    {
        return Input.GetKey(KeyCode.LeftShift); // Change this to the key you want to use for sprinting
    }
    public bool sprint => GetSprintInput();

    //public bool IsCrouchKeyPressed()
    //{
    //    return playerControls.Player.Crouch.triggered;
    //}
}