using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputs))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerMovementV2 : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float walkSpeed = 7f;
    public float sprintSpeed = 10f;
    public float groundDrag = 5f;
    public Transform orientation;

    [Header("Groundcheck")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;

    [Header("Jumping")]
    public bool JumpIsPressed;
    public float jumpForce = 12f;
    public float jumpCooldown = .25f;
    public float airMultiplier = .4f;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 40;

    public bool grounded;
    private bool redayToJump = true;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Vector3 PreviousFramePosition = Vector3.zero;
    private RaycastHit slopeHit;
    private Rigidbody rb;
    private TP_InputManager inputs;

    private void Start()
    {
        inputs = GetComponent<TP_InputManager>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // groundcheck
        Vector3 raypoint = transform.position + new Vector3(0, .2f, 0);
        grounded = Physics.Raycast(raypoint, Vector3.down, .5f, whatIsGround);
        GetComponent<Animator>().SetBool("isGrounded", grounded);

        ReadPlayerInputs();
        SpeedControl();

        // apply and handle drag
        if (grounded) rb.drag = groundDrag;
        else rb.drag = 0;
    }

    private void LateUpdate()
    {
        MovePlayer();
        MesureMovementSpeed();
    }

    private void ReadPlayerInputs()
    {
        horizontalInput = inputs.MovementInput.x;
        verticalInput = inputs.MovementInput.y;

        // crouching
        if (inputs.JumpAction && redayToJump && grounded)
        {
            redayToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // crouching
        if (inputs.IsCrouching)
            Debug.Log("Crouching");
        GetComponent<Animator>().SetBool("isCrouching", inputs.IsCrouching);
    }

    private void MovePlayer()
    {
        // crouching and sprinting
        if (inputs.IsCrouching) moveSpeed = walkSpeed / 2f;
        else if (inputs.IsSprinting) moveSpeed = sprintSpeed;
        else moveSpeed = walkSpeed;

        // walk in the direction player is looking
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
        }

        if (grounded) 
            rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
        else if (!grounded) // in air
            rb.AddForce(moveDirection.normalized * moveSpeed * airMultiplier, ForceMode.Force);
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        GetComponent<Animator>().SetTrigger("jump");
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, .2f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    private void ResetJump()
    {
        redayToJump = true;
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitVel.x, rb.velocity.y, limitVel.z);
        }
    }

    private void MesureMovementSpeed()
    {
        // mesure current movementspeed
        float movementPerFrame = Vector3.Distance(PreviousFramePosition, transform.position);
        GetComponent<Animator>().SetFloat("movement speed", movementPerFrame / Time.deltaTime);
        PreviousFramePosition = transform.position;
    }
}
