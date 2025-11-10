using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    private Transform _cameraTransform;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity;

    // Sprint handling
    private bool wasSprinting = false;
    private float sprintSoundTimer = 0f;
    private const float sprintSoundInterval = 2f;
    private bool IsSprinting =>
        _inputManager.GetSprintInput() &&
        _survivalManager.HasStamina() &&
        currentInputVector.magnitude > 0.1f; // Only sprint if moving

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
        HandleMovement();
        HandleSprintSound();
        //if (useFootSteps) HandleFootsteps();
    }

    private void HandleSprintSound()
    {
        bool isCurrentlySprinting = IsSprinting;
        if (isCurrentlySprinting)
        {
            sprintSoundTimer += Time.deltaTime;
            if (sprintSoundTimer >= sprintSoundInterval)
            {
                SoundManager.PlaySound(SoundType.SPRINT, .2f);
                sprintSoundTimer = 0f;
            }
        }
        else
        {
            sprintSoundTimer = sprintSoundInterval; // Reset so it plays immediately next time
        }
    }

    private void HandleMovement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Get movement input from input manager
        // Smooth the input
        // Move the player based on the input vector
        Vector2 movement = _inputManager.GetMovementInput();
        currentInputVector = Vector2.SmoothDamp(currentInputVector, movement, ref smoothInputVelocity, smoothInputSpeed);
        Vector3 move = new Vector3(currentInputVector.x, 0f, currentInputVector.y);


        // Project the camera's forward direction onto the horizontal plane
        Vector3 cameraForward = _cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = _cameraTransform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();


        // Move the player relative to the camera direction
        move = cameraForward * move.z + cameraRight * move.x;

        // Check for sprint input and adjust speed
        _survivalManager.SetSprinting(IsSprinting);
        float currentSpeed = IsSprinting ? sprintSpeed : playerSpeed;
        //float currentSpeed = _inputManager.GetSprintInput() && _survivalManager.HasStamina() ? sprintSpeed : playerSpeed;

        // Move the player based on the input and speed 
        controller.Move(move * Time.deltaTime * currentSpeed);

        // Move the player based on the player velocity      
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
  
}