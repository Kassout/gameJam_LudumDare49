using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 40f;
    [SerializeField] private float jumpForce = 400f;					        // Amount of force added when the player jumps.
    [SerializeField] private float jumpBoost = 0.5f;
    [SerializeField] private float jumpTime = 0.0f;                            // Amout of time a player is able to jump.
    [SerializeField] private bool airControl = false;                           // Whether or not a player can steer while jumping;

    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;	// How much to smooth out the movement
    
    // Jump parameters
    private bool _isJumping;
    private float _jumpTimeCounter = 0.0f;
    
    // Movement parameters
    private float _horizontalMove = 0f;
    private bool _facingRight = true;  // For determining which way the player is currently facing.
	private Vector3 _velocity = Vector3.zero;
    
    // Components
    private Rigidbody2D _playerRigidbody;
	private Animator _playerAnimator;
    private PlayerController _playerController;

	private void Awake()
	{
        _playerRigidbody = GetComponent<Rigidbody2D>();
		_playerAnimator = GetComponent<Animator>();
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        _horizontalMove = Input.GetAxisRaw("Horizontal") * movementSpeed;

        if (_playerController.isGrounded)
        {
            if (Input.GetButtonDown("Jump") || Input.GetKey(KeyCode.W))
            {
                _playerController.isGrounded = false;
                _playerRigidbody.AddForce(Vector2.up * jumpForce);
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
        }
        else if (!_playerController.isGrounded || _horizontalMove == 0 || _isJumping)
        {
            _playerAnimator.SetBool("isWalking", false);
        }
    }

    private void FixedUpdate()
	{
        Move(_horizontalMove * Time.fixedDeltaTime);
	}
    
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
            if (Input.GetButton("Jump") || Input.GetKey(KeyCode.W))
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
