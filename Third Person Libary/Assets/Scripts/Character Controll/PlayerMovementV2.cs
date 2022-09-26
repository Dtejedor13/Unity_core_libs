using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementV2 : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed = 7f;
    public float sprintSpeed = 10f;
    public float groundDrag = 5f;
    public Transform orientation;

    [Header("Groundcheck")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;

    [Header("Jump")]
    public bool JumpIsPressed;
    public float jumpForce = 12f;
    public float jumpCooldown = .25f;
    public float airMultiplier = .4f;

    public bool grounded;
    private bool redayToJump = true;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private TP_InputManager inputs;

    private void Awake()
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
        
        ReadPlayerInputs();
        SpeedControl();

        // apply and handle drag
        if (grounded) rb.drag = groundDrag;
        else rb.drag = 0;
    }

    private void LateUpdate()
    {
        MovePlayer();
    }

    private void ReadPlayerInputs()
    {
        horizontalInput = inputs.MovementInput.x;
        verticalInput = inputs.MovementInput.y;

        if (inputs.JumpAction && redayToJump && grounded)
        {
            redayToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // crouching and sprinting
        if (inputs.IsCrouching)
        {
            moveSpeed = walkSpeed / 2f;
        }
        else if (inputs.IsSprinting)
        {
            moveSpeed = sprintSpeed;
        }
        else
            moveSpeed = walkSpeed;


        // walk in the direction player is looking
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
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
}
