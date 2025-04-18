using UnityEngine;
using UnityEngine.Windows;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed = 4.0f;
    [SerializeField]
    private float sprintSpeed = 8.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float smoothInputSpeed = .2f;

    private SurvivalManager _survivalManager;
    private CharacterController controller;
    private InputManager _inputManager;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
   
    private Transform _cameraTransform;

    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity;

    private void Start()
    {
        _survivalManager = GetComponent<SurvivalManager>();
        controller = GetComponent<CharacterController>();
        _inputManager = InputManager.Instance;
        _cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;
    }



    void Update()
    {
        if (_inputManager.IsInteractKeyPressed())
        {
            PlayerInteract();
            Debug.Log("Interact Key Pressed");
        }

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Get movement input from input manager
        Vector2 movement = _inputManager.GetMovementInput();
        // Smooth the input
        currentInputVector = Vector2.SmoothDamp(currentInputVector, movement, ref smoothInputVelocity, smoothInputSpeed);
        // Move the player based on the input vector
        Vector3 move = new Vector3(currentInputVector.x, 0f, currentInputVector.y);

        // Project the camera's forward direction onto the horizontal plane
        Vector3 cameraForward = _cameraTransform.forward;
        cameraForward.y = 0f; // Remove the vertical component
        cameraForward.Normalize(); // Normalize to ensure consistent movement speed

        Vector3 cameraRight = _cameraTransform.right;
        cameraRight.y = 0f; // Remove the vertical component
        cameraRight.Normalize();

        // Move the player relative to the camera direction
        move = cameraForward * move.z + cameraRight * move.x;

        // Check for sprint input and adjust speed
        float currentSpeed = _inputManager.GetSprintInput() && _survivalManager.HasStamina() ? sprintSpeed : playerSpeed;

        // Move the player based on the input and speed 
        controller.Move(move * Time.deltaTime * currentSpeed);

        playerVelocity.y += gravityValue * Time.deltaTime;
        // Move the player based on the player velocity
        controller.Move(playerVelocity * Time.deltaTime);
    }


    public void PlayerInteract()
    {
        var layermask0 = 1 << 0; // Default
        var layermask3 = 1 << 3; // Interactable
        var finalmask = layermask0 | layermask3;

        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out hit, 4f, finalmask)) // Range
        {
            Interact interactScript = hit.transform.GetComponent<Interact>();
            if (interactScript) interactScript.CallInteract(this);          
        }
    }

}