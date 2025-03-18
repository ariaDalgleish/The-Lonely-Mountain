using UnityEngine;

[RequireComponent(typeof(CharacterController))] 
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed = 5.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private InputManager _inputManager;
    private Transform _cameraTransform;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        _inputManager = InputManager.Instance;
        _cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movement = _inputManager.GetMovementInput();
        Vector3 move = new Vector3(movement.x, 0f, movement.y);
        move = _cameraTransform.forward * move.z + _cameraTransform.right * move.x;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        //if (move != Vector3.zero)
        //{
        //   gameObject.transform.forward = move;
        //}

        // Makes the player jump
        if (_inputManager.IsJumpKeyPressed() && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        //if(_inputManager.IsSprintKeyPressed())
        //{
        //    playerSpeed = 10.0f;
        //}
        //else
        //{
        //    playerSpeed = 5.0f;
        //}

        //if (_inputManager.IsCrouchKeyPressed())
        //{
        //    controller.height = 1.0f;
        //}
        //else
        //{
        //    controller.height = 2.0f;
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
