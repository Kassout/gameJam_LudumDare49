using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class <c>PlayerMovement</c> is a Unity component script used to manage the player movement behaviour.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    #region Fields / Properties

    /// <summary>
    /// Instance field <c>playerController</c> is a Unity <c>PlayerController</c> component script representing the general player behavior manager.
    /// </summary>
    private PlayerController _playerController;
    
    /// <summary>
    /// Instance field <c>rigidbody</c> is a Unity <c>Rigidbody2D</c> component representing the player game object rigidbody.
    /// </summary>
    private Rigidbody2D _rigidbody;
    
    /// <summary>
    /// Instance field <c>animator</c> is a Unity <c>Animator</c> component representing the player animations manager.
    /// </summary>
    private Animator _animator;
    
    /// <summary>
    /// Instance field <c>IsWalkingHash</c> represents the integer identifier of the string message "isWalking" for the player animator.
    /// </summary>
    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");

    /// <summary>
    /// Instance field <c>audioSource</c> is a Unity <c>AudioSource</c> component representing the player audio source for SFX playing.
    /// </summary>
    private AudioSource _audioSource;
    
    /// <summary>
    /// Instance field <c>walkClip</c> is a Unity <c>AudioClip</c> object representing the player walk audio sound.
    /// </summary>
    [SerializeField] private AudioClip walkClip;
    
    /// <summary>
    /// Instance field <c>jumpClip</c> is a Unity <c>AudioClip</c> object representing the player jump audio sound.
    /// </summary>
    [SerializeField] private AudioClip jumpClip;
    
    /// <summary>
    /// Instance field <c>movementSpeed</c> represents the speed value of the player movement.
    /// </summary>
    [Header("Movement parameters")]
    [SerializeField] private float movementSpeed = 40f;
    
    /// <summary>
    /// Instance field <c>movementSmoothing</c> represents the smoothing magnitude of the player movement.
    /// </summary>
    [Range(0, .3f)] 
    [SerializeField] private float movementSmoothing = .05f;
    
    /// <summary>
    /// Instance field <c>horizontalMove</c> represents the horizontal movement value of the player.
    /// </summary>
    private float _horizontalMove;
    
    /// <summary>
    /// Instance field <c>facingRight</c> represents the facing right status of the player game object.
    /// </summary>
    private bool _facingRight = true;
    
    /// <summary>
    /// Instance field <c>jumpForce</c> represents the force magnitude of the player jump.
    /// </summary>
    [Header("Jump parameters")]
    [SerializeField] private float jumpForce = 400.0f;
    
    /// <summary>
    /// Instance field <c>jumpBoost</c> represents the boost value of the player jump while in air.
    /// </summary>
    [SerializeField] private float jumpBoost = 0.5f;
    
    /// <summary>
    /// Instance field <c>jumpTime</c> represents the maximum duration value of a player jump.
    /// </summary>
    [SerializeField] private float jumpTime = 0.2f;
    
    /// <summary>
    /// Instance field <c>airControl</c> represents the air control status of the player.
    /// </summary>
    [SerializeField] private bool airControl = true;

    /// <summary>
    /// Instance field <c>jumpTimeCounter</c> represents the current air time value the player spent of.
    /// </summary>
    private float _jumpTimeCounter;
    
    /// <summary>
    /// Instance field <c>isJumping</c> represents jumping status of the player.
    /// </summary>
    private bool _isJumping;

    #endregion

    #region MonoBehavior

    /// <summary>
    /// This function is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
        _audioSource = GetComponent<AudioSource>();
    }

    private bool keyspaceIsPressed;
    
    /// <summary>
    /// This function is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        _horizontalMove = InputHandler.movementInput * movementSpeed;

        if (_playerController.isGrounded)
        {
            if ((Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) || (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))
            {
                _playerController.isGrounded = false;
                _rigidbody.AddForce(Vector2.up * jumpForce);
                PlayJumpSound();
                _isJumping = true;
            }
            else if ((Keyboard.current != null && !Keyboard.current.spaceKey.isPressed) || (Gamepad.current != null && !Gamepad.current.buttonSouth.isPressed))
            {
                _isJumping = false;
                _jumpTimeCounter = jumpTime;
            }
        }
        
        if (_horizontalMove != 0 && _playerController.isGrounded)
        {
            _animator.SetBool(IsWalkingHash, true);
            if (!_audioSource.isPlaying)
            {
                PlayWalkSound();
            }
        }
        else if (!_playerController.isGrounded || _horizontalMove == 0 || _isJumping)
        {
            _animator.SetBool(IsWalkingHash, false);
        }

        if ((Keyboard.current != null && !Keyboard.current.spaceKey.isPressed) || (Gamepad.current != null && !Gamepad.current.buttonSouth.isPressed))
        {
            keyspaceIsPressed = true;
        }
        else
        {
            keyspaceIsPressed = false;
        }
    }

    /// <summary>
    /// This function is called every fixed frame-rate frame.
    /// </summary>
    private void FixedUpdate()
    {
        Move(_horizontalMove * Time.fixedDeltaTime);
    }
    
    #endregion

    #region Private

    /// <summary>
    /// This function is responsible for playing the player walk SFX when called.
    /// </summary>
    private void PlayWalkSound()
    {
        _audioSource.volume = 0.2f;
        _audioSource.pitch = 0.4f;
        _audioSource.clip = walkClip;
        _audioSource.Play();
    }

    /// <summary>
    /// This function is responsible for playing the player jump SFX when called.
    /// </summary>
    private void PlayJumpSound()
    {
        _audioSource.volume = 1.0f;
        _audioSource.pitch = 1.0f;
        _audioSource.clip = jumpClip;
        _audioSource.Play();
    }

    /// <summary>
    /// This function is responsible for flipping the player game object sprite.
    /// </summary>
	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		_facingRight = !_facingRight;

		// Multiply the player's x local scale by -1.
        Transform playerTransform = transform;
        Vector3 theScale = playerTransform.localScale;
		theScale.x *= -1;
		playerTransform.localScale = theScale;
	}

    #endregion

    #region Public

    /// <summary>
    /// This function is responsible for moving the player game object.
    /// </summary>
    /// <param name="move">A float value representing the amplitude of horizontal movement.</param>
    public void Move(float move)
    {
        //only control the player if grounded or airControl is turned on
        if (_playerController.isGrounded || airControl)
        {
            // Move the character by finding the target velocity
            Vector2 velocity = _rigidbody.velocity;
            Vector3 currentVelocity = Vector3.zero;
            Vector3 targetVelocity = new Vector2(move * 10f, velocity.y);
            // And then smoothing it out and applying it to the character
            _rigidbody.velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref currentVelocity, movementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !_facingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && _facingRight)
            {
                // ... flip the player.
                Flip();
            }
        }

        // If the player should continue to jump...
        if (_isJumping)
        {
            if (keyspaceIsPressed)
            {
                if (_jumpTimeCounter > 0)
                {
                    _rigidbody.velocity += Vector2.up * jumpBoost;
                    _jumpTimeCounter -= Time.deltaTime;
                }
                else
                {
                    _isJumping = false;
                }
            }
        }
    }

    #endregion
}
