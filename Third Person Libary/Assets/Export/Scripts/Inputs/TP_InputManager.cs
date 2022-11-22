using UnityEngine;

public class TP_InputManager : MonoBehaviour
{
    public bool JumpIsPressed { get; private set; } = false;
    public bool CrouchIsPressed { get; private set; } = false;
    public bool SprintIsPressed { get; private set; } = false;
    public bool InteractionIsPressed { get; private set; } = false;
    public Vector2 MovementInput { get; private set; } = Vector2.zero;

    // combat
    public bool PrimaryAttackIsPressed { get; private set; } = false;
    public bool Action1IsPressed { get; private set; } = false;
    public bool Action2IsPressed { get; private set; } = false;
    public bool Action3IsPressed { get; private set; } = false;
    public bool Action4IsPressed { get; private set; } = false;

    public float VerticalInput => MovementInput.y;
    public float HorizontalInput => MovementInput.x;

    PlayerInputs playerInputs;

    private void OnEnable()
    {
        if (playerInputs == null)
        {
            playerInputs = new PlayerInputs();
            playerInputs.Movement.Movement.performed += x => MovementInput = x.ReadValue<Vector2>();
            playerInputs.Movement.Jump.performed += x => JumpIsPressed = true;
            playerInputs.Movement.Jump.canceled += x => JumpIsPressed = false;
            playerInputs.Movement.Sprint.performed += x => SprintIsPressed = true;
            playerInputs.Movement.Sprint.canceled += x => SprintIsPressed = false;
            playerInputs.Movement.Crouch.performed += x => CrouchIsPressed = x.performed;
            playerInputs.Movement.Interaction.performed += x => InteractionIsPressed = x.performed;
            playerInputs.Movement.Interaction.canceled += x => InteractionIsPressed = false;
            playerInputs.Combat.PrimaryAttack.performed += x => PrimaryAttackIsPressed = x.performed;
            playerInputs.Combat.PrimaryAttack.canceled += x => PrimaryAttackIsPressed = false;
            playerInputs.Combat.Action1.performed += x => Action1IsPressed = x.performed;
            playerInputs.Combat.Action1.canceled += x => Action1IsPressed = false;
            playerInputs.Combat.Action2.performed += x => Action2IsPressed = x.performed;
            playerInputs.Combat.Action2.canceled += x => Action2IsPressed = false;
            playerInputs.Combat.Action3.performed += x => Action3IsPressed = x.performed;
            playerInputs.Combat.Action3.canceled += x => Action3IsPressed = false;
            playerInputs.Combat.Action4.performed += x => Action4IsPressed = x.performed;
            playerInputs.Combat.Action4.canceled += x => Action4IsPressed = false;
        }

        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
    }

}
