using UnityEngine;

[RequireComponent(typeof(CharacterController))] 
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed = 5.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private float smoothInputSpeed = .2f;
    //[SerializeField]
   // private float sprintSpeed = 10.0f;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private InputManager _inputManager;
    private Transform _cameraTransform;

    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        _inputManager = InputManager.Instance;
        _cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movement = _inputManager.GetMovementInput();
        currentInputVector = Vector2.SmoothDamp(currentInputVector, movement, ref smoothInputVelocity, smoothInputSpeed);
        Vector3 move = new Vector3(currentInputVector.x, 0f, currentInputVector.y);
        move = _cameraTransform.forward * move.z + _cameraTransform.right * move.x;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

       

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);


        //if (move != Vector3.zero)
        //{
        //   gameObject.transform.forward = move;
        //}

        // Makes the player jump
        //if (_inputManager.IsJumpKeyPressed() && groundedPlayer)
        //{
        //    playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        //}

        if (_inputManager.IsSprintKeyPressed())
        {
            Debug.Log("SprintPressed");
        }
        //else
        //{
        //    playerSpeed = 5.0f;
        //}
              
    }

    /*[SerializeField]
    private float speed = 5f;

    [SerializeField]
    private Transform _cameraTransform;

    private Rigidbody _rb;
    private Vector2 _moveInput;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
       
     private void FixedUpdate()
    {
        Vector3 move =
            _cameraTransform.forward * _moveInput.y + _cameraTransform.right * _moveInput.x;
        move.y = 0f;
       _rb.AddForce(move.normalized * speed, ForceMode.VelocityChange);
    }

    private void Move(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }
   */
}
