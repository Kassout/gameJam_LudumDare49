using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Class <c>EnemyController</c> is a Unity component script used to manage the general enemy behaviour.
/// </summary>
public class EnemyController : MonoBehaviour
{
    #region Fields / Properties

    /// <summary>
    /// Instance field <c>player</c> is a Unity <c>GameObject</c> representing the player game object.
    /// </summary>
    private GameObject _player;
    
    /// <summary>
    /// Instance field <c>hitBoxInstance</c> is a Unity <c>GameObject</c> representing the hit box instance of the enemy.
    /// </summary>
    private GameObject _hitBoxInstance;
    
    /// <summary>
    /// Instance field <c>enemyRigidbody</c> is a Unity <c>Rigidbody2D</c> component representing the enemy game object rigidbody.
    /// </summary>
    private Rigidbody2D _enemyRigidbody;
    
    /// <summary>
    /// Instance field <c>enemyColliders</c> is a list of Unity <c>Collider2D</c> components representing the different enemy colliders.
    /// </summary>
    private List<Collider2D> _enemyColliders;

    /// <summary>
    /// Instance field <c>animator</c> is a Unity <c>Animator</c> component representing the enemy animations manager.
    /// </summary>
    private Animator _animator;
    
    /// <summary>
    /// Instance field <c>audioSource</c> is a Unity <c>AudioSource</c> component representing the enemy audio source for SFX playing.
    /// </summary>
    private AudioSource _audioSource;
    
    /// <summary>
    /// Instance field <c>attackClip</c> is a Unity <c>AudioClip</c> object representing the enemy attack audio sound.
    /// </summary>
    [SerializeField] private AudioClip attackClip;
    
    /// <summary>
    /// Instance field <c>hurtClip</c> is a Unity <c>AudioClip</c> object representing the enemy hurt audio sound.
    /// </summary>
    [SerializeField] private AudioClip hurtClip;
    
    /// <summary>
    /// Instance field <c>IsGroundedHash</c> represents the integer identifier of the string message "isGrounded" for the enemy animator.
    /// </summary>
    private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
    
    /// <summary>
    /// Instance field <c>IsWalkingHash</c> represents the integer identifier of the string message "isWalking" for the enemy animator.
    /// </summary>
    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    
    /// <summary>
    /// Instance field <c>AttackHash</c> represents the integer identifier of the string message "attack" for the enemy animator.
    /// </summary>
    private static readonly int AttackHash = Animator.StringToHash("attack");
    
    /// <summary>
    /// Instance field <c>DeathHash</c> represents the integer identifier of the string message "death" for the enemy animator.
    /// </summary>
    private static readonly int DeathHash = Animator.StringToHash("death");

    /// <summary>
    /// Instance field <c>groundCheckTransform</c> is a Unity <c>Transform</c> component representing the position, rotation and scale of the point checking the enemy grounded status.
    /// </summary>
    [Header("Ground check parameters")]
    [SerializeField] private Transform groundCheckTransform;
    
    /// <summary>
    /// Instance field <c>groundCheckRadius</c> represents the radius size value of the enemy ground check zone.
    /// </summary>
    [SerializeField] private float groundCheckRadius = 0.2f;
    
    /// <summary>
    /// Instance field <c>whatIsGround</c> is a Unity <c>LayerMask</c> structure representing the ground layer in the scene.
    /// </summary>
    [SerializeField] private LayerMask whatIsGround;
    
    /// <summary>
    /// Instance field <c>isGrounded</c> represents the grounded status of the enemy game object.
    /// </summary>
    private bool _isGrounded;
    
    /// <summary>
    /// Instance field <c>movementSpeed</c> represents the speed value of the enemy movement.
    /// </summary>
    [Header("Enemy movement parameters")]
    [SerializeField] private float movementSpeed = 3.0f;

    /// <summary>
    /// Instance field <c>facingRight</c> represents the facing right status of the enemy.
    /// </summary>
    private bool _facingRight;
    
    /// <summary>
    /// Instance field <c>collidingWithAura</c> represents the colliding with player aura status of the enemy game object.
    /// </summary>
    private bool _collidingWithAura;
    
    /// <summary>
    /// Instance field <c>directionOfPlayer</c> is a Unity <c>Vector3</c> structure representing the direction vector to reach the player game object from the actual enemy position.
    /// </summary>
    [HideInInspector] public Vector3 directionOfPlayer;
    
    /// <summary>
    /// Instance field <c>hitBoxPrefab</c> is a Unity <c>GameObject</c> representing the enemy attack hit box prefabricated game object.
    /// </summary>
    [Header("Enemy Attack parameters")]
    [SerializeField] private GameObject hitBoxPrefab;
    
    /// <summary>
    /// Instance field <c>attackPosition</c> is a Unity <c>Transform</c> component representing the position, rotation and scale of the point of the hit box prefabricated game object instantiation.
    /// </summary>
    [SerializeField] private Transform attackPosition;
    
    /// <summary>
    /// Instance field <c>attackWindowDuration</c> represents the duration value of the open window of enemy attack.
    /// </summary>
    [SerializeField] private float attackWindowDuration = 0.1f;
    
    /// <summary>
    /// Instance field <c>attackWindowDelay</c> represents the duration value at target contact before the enemy attack window opens.
    /// </summary>
    [SerializeField] private float attackWindowDelay = 0.1f; // Time between collision with player trigger and attack hit box trigger
    
    /// <summary>
    /// Instance field <c>attackCooldownTime</c> represents the duration value between two enemy attacks.
    /// </summary>
    [SerializeField] private float attackCooldownTime = 2.0f;
    
    /// <summary>
    /// Instance field <c>attackCooldownTimeValue</c> represents the time before the last enemy attack.
    /// </summary>
    private float _attackCooldownTimeValue;
    
    /// <summary>
    /// Instance field <c>attacking</c> represents the attacking status of the enemy game object.
    /// </summary>
    [HideInInspector] public bool attacking;
    
    /// <summary>
    /// Instance field <c>life</c> represents the number of lives of the enemy.
    /// </summary>
    [Header("General parameters")]
    [SerializeField] private int life = 1;
    
    /// <summary>
    /// Instance field <c>isDead</c> represents the dead status of the enemy.
    /// </summary>
    private bool _isDead;
    
    /// <summary>
    /// Instance field <c>scoreValue</c> represents the score value received by the player killing the enemy game object.
    /// </summary>
    public int scoreValue = 50;

    #endregion

    #region MonoBehavior

    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        _enemyRigidbody = GetComponent<Rigidbody2D>();
        _enemyColliders = GetComponents<Collider2D>().ToList();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _player = FindObjectOfType<PlayerController>().gameObject;
        _attackCooldownTimeValue = attackCooldownTime;
        
#if !UNITY_EDITOR
        hitBoxPrefab.GetComponent<SpriteRenderer>().enabled = false;
#endif
    }

    /// <summary>
    /// This function is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        if (!_isDead)
        {
            directionOfPlayer = (transform.position - _player.transform.position).normalized;

            _isGrounded = false;
            _animator.SetBool(IsGroundedHash, _isGrounded);
                
            // The enemy is grounded if a circle cast to the ground check position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            if (Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, whatIsGround))
            {
                _isGrounded = true;
                _animator.SetBool(IsGroundedHash, _isGrounded);
            }
            
            _attackCooldownTimeValue -= Time.deltaTime;
        }
    }
    
    /// <summary>
    /// This function is called every fixed frame-rate frame.
    /// </summary>
    private void FixedUpdate()
    {
        if (!_isDead)
        {
            if (_isGrounded)
            {
                if (!_collidingWithAura)
                {
                    // Move the character by finding the target velocity
                    Vector2 velocity = _enemyRigidbody.velocity;
                    Vector3 targetVelocity = -directionOfPlayer * new Vector2( movementSpeed * Time.fixedDeltaTime, velocity.y);
                    // And then smoothing it out and applying it to the character
                    Vector3 currentVelocity = Vector3.zero;
                    _enemyRigidbody.velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref currentVelocity, 0.05f);

                    //_enemyRigidbody.MovePosition(-_directionOfPlayer * movementSpeed * Time.fixedDeltaTime);
                    _animator.SetBool(IsWalkingHash, true);
                }
                else
                {
                    _animator.SetBool(IsGroundedHash, true);
                    if (!attacking && _attackCooldownTimeValue <= 0)
                    {
                        StartCoroutine(Attack());
                    }
                }
            }

            // If the input is moving the enemy right and the player is facing left...
            if (directionOfPlayer.x < 0 && !_facingRight)
            {
                // ... flip
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (directionOfPlayer.x > 0 && _facingRight)
            {
                // ... flip
                Flip();
            }
        }
    }
    
    /// <summary>
    /// This function is called on trigger enter event, sent when another object entered a trigger collider attached to this object
    /// </summary>
    /// <param name="collision">A Unity <c>Collider2D</c> component of the other game object involved in this collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Aura"))
        {
            _collidingWithAura = true;
        }
    }

    /// <summary>
    /// This function is called on trigger stay event, sent when another object keep colliding a trigger collider attached to this object
    /// </summary>
    /// <param name="collision">A Unity <c>Collider2D</c> component of the other game object involved in this collision.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        _collidingWithAura = collision.gameObject.CompareTag("Aura");
    }

    /// <summary>
    /// This function is called on trigger exit event, sent when another object leaves a trigger collider attached to this object
    /// </summary>
    /// <param name="collision">A Unity <c>Collider2D</c> component of the other game object involved in this collision.</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Aura"))
        {
            _collidingWithAura = false;
        }
    }
    
    #endregion

    #region Privae

    /// <summary>
    /// This function is responsible for playing the enemy hurt SFX when called.
    /// </summary>
    private void PlayHurtSound()
    {
        _audioSource.volume = 0.7f;
        _audioSource.pitch = 1f;
        _audioSource.clip = hurtClip;
        _audioSource.Play();
    }
    
    /// <summary>
    /// This function is responsible for playing the enemy attack SFX when called.
    /// </summary>
    private void PlayAttackSound()
    {
        _audioSource.volume = 0.7f;
        _audioSource.pitch = 1f;
        _audioSource.clip = attackClip;
        _audioSource.Play();
    }
    
    /// <summary>
    /// This function is responsible for managing the general attack behavior of the enemy game object.
    /// </summary>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
    private IEnumerator Attack()
    {
        attacking = true;

        yield return new WaitForSeconds(attackWindowDelay);

        if (!_isDead)
        {
            PlayAttackSound();
            _animator.SetTrigger(AttackHash);

            yield return new WaitForSeconds(0.16f);

            if (!_isDead)
            {
                _hitBoxInstance = Instantiate(hitBoxPrefab, attackPosition.position, Quaternion.identity, transform);
                _hitBoxInstance.GetComponent<HitBox>().origin = this;
                Destroy(_hitBoxInstance, attackWindowDuration);
            }

            _attackCooldownTimeValue = attackCooldownTime;
        }
    }
    
    /// <summary>
    /// This function is responsible for flipping the sprite representing the enemy game object to keep facing the target.
    /// </summary>
    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        _facingRight = !_facingRight;

        // Multiply the player's x local scale by -1.
        Transform objectTransform = transform;
        Vector3 theScale = objectTransform.localScale;
        theScale.x *= -1;
        objectTransform.localScale = theScale;
    }

    #endregion

    #region Public

    /// <summary>
    /// This function is responsible for subtracting life of the enemy when taking damage.
    /// </summary>
    /// <param name="damage">An integer value representing the quantity of damage received by the enemy.</param>
    /// <returns>A boolean value representing the dead status of the enemy.</returns>
    public bool TakeDamage(int damage)
    {
        life -= damage;
        if (life <= 0)
        {
            _isDead = true;
            Destroy(_hitBoxInstance);
            foreach (Collider2D other in _enemyColliders)
            {
                other.isTrigger = true;
            }
            float deathForceMagnitude = Random.Range(50, 150);
            _enemyRigidbody.AddForce(Vector2.up * deathForceMagnitude, ForceMode2D.Impulse);
            PlayHurtSound();
            _animator.SetTrigger(DeathHash);
        }

        return _isDead;
    }

    #endregion
}