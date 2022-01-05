using UnityEngine;

/// <summary>
/// Class <c>InputHandler</c> is a Unity component script used to manage the inputs behaviour.
/// </summary>
public class InputHandler : MonoBehaviour
{
    /// <summary>
    /// Instance field <c>inputActions</c> is a Unity <c>InputSystem</c> component object representing the general input bindings of the game.
    /// </summary>
    private InputController _inputController;
    
    /// <summary>
    /// Instance variable <c>movementInput</c> is a Unity <c>Vector2</c> component object representing the movement input vector of the player.
    /// </summary>
    public static float movementInput;
    
    /// <summary>
    /// Instance variable <c>jumpInput</c> represents the jump hold input status of the game.
    /// </summary>
    public static bool jumpInput;
    
    /// <summary>
    /// Instance variable <c>attackInput</c> represents the attack input status of the game.
    /// </summary>
    public static bool attackInput;
    
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        if (_inputController == null)
        {
            _inputController = new InputController();
            
            _inputController.Player.Movement.performed += _ => movementInput = _.ReadValue<float>();

            _inputController.Player.Jump.performed += _ => jumpInput = true;
            _inputController.Player.Attack.performed += _ => attackInput = true;
        }
        
        _inputController.Enable();
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled.
    /// </summary>
    private void OnDisable()
    {
        _inputController.Disable();
    }
    
    /// <summary>
    /// This function is called after all Update functions have been called.
    /// </summary>
    private void LateUpdate()
    {
        // To avoid calling input related methods twice in a frame.
        jumpInput = false;
        attackInput = false;
    }
}
