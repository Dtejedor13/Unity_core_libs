using UnityEngine;

public class TP_InputManager : MonoBehaviour
{
    public bool JumpButtonIsPressed { get; private set; }
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
            playerInputs.Movement.Jump.performed += x => JumpButtonIsPressed = x.ReadValue<bool>();
        }

        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
    }
}
