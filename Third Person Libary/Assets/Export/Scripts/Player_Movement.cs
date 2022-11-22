using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TP_InputManager))] // Note: if not needed remove it
[RequireComponent(typeof(CharacterController))]
public class Player_Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] float movementSpeed = 10f;

    [Header("Jumping")]
    [SerializeField] float gravityScale = 1.5f;
    [SerializeField] private float jumpHeight = 10f;

    public bool EnableMovement = true;
    public bool IsGrounded;
    private float turnSmoothVelocity;
    private CharacterController controller;
    private Vector3 jumpVelocity;
    private float gravityValue = -9.8f; // just a magic number

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.visible = false;
    }

    private void Update()
    {
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit _, 1.5f, groundLayerMask);
        HandleMovement();
    }

    private void HandleMovement()
    {
        // inputs
        TP_InputManager inputs = GetComponent<TP_InputManager>();
        float horizontal = inputs.HorizontalInput;
        float vertilcal = inputs.VerticalInput;
        bool jumpIsPressed = inputs.JumpIsPressed;

        if (!EnableMovement) {
            horizontal = 0f;
            vertilcal = 0f;
            jumpIsPressed = false;
        }

        Vector3 direction = new Vector3(horizontal, 0f, vertilcal).normalized;

        // movement
        if (direction.magnitude >= 0.1f)
        {

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 0.1f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            controller.Move(moveDir.normalized * movementSpeed * Time.deltaTime);
        }

        // jumping 
        if (jumpIsPressed && IsGrounded)
        {
            // handle jump anim here
            jumpVelocity.y = jumpHeight;
        }
        else
        {
            jumpVelocity.y += (gravityValue * gravityScale) * Time.deltaTime;
        }

        controller.Move(jumpVelocity * Time.deltaTime);
    }
}
