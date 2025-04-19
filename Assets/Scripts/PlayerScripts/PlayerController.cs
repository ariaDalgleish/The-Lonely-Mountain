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
    [SerializeField]
    private bool useFootSteps = true;

    private SurvivalManager _survivalManager;
    private CharacterController controller;
    private InputManager _inputManager;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    

    private Transform _cameraTransform;

    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity;

    /*[Header("Footsteps")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float sprintStepMultipler = 0.6f;
    [SerializeField] private AudioSource footstepAudioSource = default;
    [SerializeField] private AudioClip[] snowClips = default;
    [SerializeField] private AudioClip[] grassClips = default;
    [SerializeField] private AudioClip[] rockClips = default;
    private float footstepTimer = 0f;
    */

    private bool IsSprinting => _inputManager.GetSprintInput() && _survivalManager.HasStamina();
    //private float GetCurrentOffset => IsSprinting ? baseStepSpeed * sprintStepMultipler : baseStepSpeed;

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
        //if (useFootSteps) HandleFootsteps();
        
            
        

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
        float currentSpeed = IsSprinting ? sprintSpeed : playerSpeed;
        //float currentSpeed = _inputManager.GetSprintInput() && _survivalManager.HasStamina() ? sprintSpeed : playerSpeed;

        // Move the player based on the input and speed 
        controller.Move(move * Time.deltaTime * currentSpeed);

        // Move the player based on the player velocity      
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
    /*private void HandleFootsteps()
    {
        if (!controller.isGrounded) return;

        if (currentInputVector == Vector2.zero) return;

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            if (Physics.Raycast(_cameraTransform.position, Vector3.down, out RaycastHit hit, 3))
            {
                switch (hit.collider.tag)
                {
                    case "Footsteps/Snow":
                        footstepAudioSource.PlayOneShot(snowClips[Random.Range(0, snowClips.Length -1)]);
                        break;
                    case "Footsteps/Grass":
                        footstepAudioSource.PlayOneShot(grassClips[Random.Range(0, grassClips.Length -1)]);
                        break;
                    case "Footsteps/Rock":
                        footstepAudioSource.PlayOneShot(rockClips[Random.Range(0, rockClips.Length -1)]);
                        break;
                    default:
                        footstepAudioSource.PlayOneShot(snowClips[Random.Range(0, snowClips.Length - 1)]);
                        break;
                }
            }

            footstepTimer = GetCurrentOffset;
        }
    }
    */

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