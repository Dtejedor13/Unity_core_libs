using UnityEngine;

public class TP_InputManager : MonoBehaviour
{
    public bool JumpAction { get; private set; } = false;
    public bool IsCrouching { get; private set; } = false;
    public bool IsSprinting { get; private set; } = false;

    public Vector2 MovementInput { get; private set; } = Vector2.zero;

    public float VerticalInput => MovementInput.y;
    public float HorizontalInput => MovementInput.x;

    PlayerInputs playerInputs;

    private void OnEnable()
    {
        if (playerInputs == null)
        {
            playerInputs = new PlayerInputs();
            playerInputs.Movement.Movement.performed += x => MovementInput = x.ReadValue<Vector2>();
            playerInputs.Movement.Jump.performed += x => JumpAction = true;
            playerInputs.Movement.Jump.canceled += x => JumpAction = false;
            playerInputs.Movement.Sprint.performed += x => IsSprinting = true;
            playerInputs.Movement.Sprint.canceled += x => IsSprinting = false;
            playerInputs.Movement.Crouch.performed += x => IsCrouching = x.performed;
        }

        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
    }

}
