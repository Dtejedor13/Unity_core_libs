namespace Assets
{
    public interface IAnimationHandler
    {
        public void HandleMovementSpeed(float movementSpeed);
        public void HandleJump();
        public void HandleCrouch(bool crouchKeyIsPressed);
        public void HandleIsGrounded(bool isGrounded);
    }
}
