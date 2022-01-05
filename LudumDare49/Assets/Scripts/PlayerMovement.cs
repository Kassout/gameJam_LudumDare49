using UnityEngine;

/// <summary>
/// TODO: comments
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float movementSpeed = 40f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float jumpForce = 400f;					        // Amount of force added when the player jumps.
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float jumpBoost = 0.5f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float jumpTime = 0.0f;                            // Amout of time a player is able to jump.
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private bool airControl = false;                           // Whether or not a player can steer while jumping;

    /// <summary>
    /// TODO: comments
    /// </summary>
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;	// How much to smooth out the movement

    /// <summary>
    /// TODO: comments
    /// </summary>
    public AudioClip walkClip;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    public AudioClip jumpClip;
    
    // Jump parameters
    /// <summary>
    /// TODO: comments
    /// </summary>
    private bool _isJumping;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private float _jumpTimeCounter = 0.0f;
    
    // Movement parameters
    /// <summary>
    /// TODO: comments
    /// </summary>
    private float _horizontalMove = 0f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private bool _facingRight = true;  // For determining which way the player is currently facing.
    
    /// <summary>
    /// TODO: comments
    /// </summary>
	private Vector3 _velocity = Vector3.zero;
    
    // Components
    /// <summary>
    /// TODO: comments
    /// </summary>
    private Rigidbody2D _playerRigidbody;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
	private Animator _playerAnimator;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private PlayerController _playerController;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private AudioSource _audioSource;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
	private void Awake()
	{
        _playerRigidbody = GetComponent<Rigidbody2D>();
		_playerAnimator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// This function is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        _horizontalMove = InputHandler.movementInput * movementSpeed;

        if (_playerController.isGrounded)
        {
            if (InputHandler.jumpInput)
            {
                _playerController.isGrounded = false;
                _playerRigidbody.AddForce(Vector2.up * jumpForce);
                PlayJumpSound();
                _isJumping = true;
            }
            else
            {
                _isJumping = false;
                _jumpTimeCounter = jumpTime;
            }
        }
        
        if (_horizontalMove != 0 && _playerController.isGrounded)
        {
            _playerAnimator.SetBool("isWalking", true);
            if (!_audioSource.isPlaying)
            {
                PlayWalkSound();
            }
        }
        else if (!_playerController.isGrounded || _horizontalMove == 0 || _isJumping)
        {
            _playerAnimator.SetBool("isWalking", false);
        }
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    private void PlayWalkSound()
    {
        _audioSource.volume = 0.2f;
        _audioSource.pitch = 0.4f;
        _audioSource.clip = walkClip;
        _audioSource.Play();
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    private void PlayJumpSound()
    {
        _audioSource.volume = 1.0f;
        _audioSource.pitch = 1.0f;
        _audioSource.clip = jumpClip;
        _audioSource.Play();
    }

    /// <summary>
    /// This function is called every fixed frame-rate frame.
    /// </summary>
    private void FixedUpdate()
	{
        Move(_horizontalMove * Time.fixedDeltaTime);
	}
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <param name="move">TODO: comments</param>
    public void Move(float move)
    {
        //only control the player if grounded or airControl is turned on
        if (_playerController.isGrounded || airControl)
        {
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, _playerRigidbody.velocity.y);
            // And then smoothing it out and applying it to the character
            _playerRigidbody.velocity = Vector3.SmoothDamp(_playerRigidbody.velocity, targetVelocity, ref _velocity, movementSmoothing);

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
            if (InputHandler.jumpInput)
            {
                if (_jumpTimeCounter > 0)
                {
                    _playerRigidbody.velocity += Vector2.up * jumpBoost;
                    _jumpTimeCounter -= Time.deltaTime;
                }
                else
                {
                    _isJumping = false;
                }
            }
        }
	}
    
    /// <summary>
    /// TODO: comments
    /// </summary>
	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		_facingRight = !_facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
