using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputs))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float rotationSpeed = 15f;
    public Transform orientation;
    public Transform freeLookCameraTransform;
    public float groundDrag = 5f;
    
    Vector3 moveDirection;
    Rigidbody rb;
    TP_InputManager inputs;
    Vector3 PreviousFramePosition = Vector3.zero;

    [Header("Groundcheck")]
    public bool grounded;
    public float playerheigt = 2f;
    public LayerMask ground;

    [Header("Air controll")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool redyToJump;

    private void Awake()
    {
        inputs = GetComponent<TP_InputManager>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerheigt * 0.5f + 0.2f, ground);

        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        HandleAirControll();
        HandleMovement();
        HandleRotation();
        MesureMovementSpeed();
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;
        targetDirection = moveDirection;
        targetDirection = targetDirection + freeLookCameraTransform.right * inputs.HorizontalInput;

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = playerRotation;
    }

    private void HandleMovement()
    {
        // walk always in the direction the player is looking
        moveDirection = orientation.forward * inputs.VerticalInput;
        moveDirection += orientation.right * inputs.HorizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;
        moveDirection = moveDirection * moveSpeed;

        Vector3 movementVelocity = moveDirection;
        rb.velocity = movementVelocity;
    }

    private void HandleAirControll()
    {
        if (inputs.JumpButtonIsPressed && redyToJump && grounded)
        {
            redyToJump = false;
            Jump();
            Invoke(nameof(JumpCooldownIsOff), jumpCooldown);
        }
    }

    private void MesureMovementSpeed()
    {
        // mesure current movementspeed
        float movementPerFrame = Vector3.Distance(PreviousFramePosition, transform.position);
        GetComponent<Animator>().SetFloat("movement speed", movementPerFrame / Time.deltaTime);
        PreviousFramePosition = transform.position;
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void JumpCooldownIsOff()
    {
        redyToJump = true;
    }
}
