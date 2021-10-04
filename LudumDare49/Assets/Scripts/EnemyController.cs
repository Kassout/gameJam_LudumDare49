using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 3.0f;
    [SerializeField] private bool checkGround = true;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundedRadius = .2f;
    [SerializeField] private LayerMask whatIsGround;
    
    private bool _facingRight;
    public Vector3 _directionOfPlayer;
    private bool _isGrounded = false;
    
    private GameObject _player;
    private Rigidbody2D _enemyRigidbody;
    private List<Collider2D> _enemyColliders;
    public Animator animator;
    
    public bool collidingWithAura;

    public GameObject hitBox;
    public Transform atkPos;
    public float atkTimer;
    [SerializeField] private float attackWindowDelay = 0.3f; // Time between collision with player trigger and attack hit box trigger
    [System.NonSerialized] public float atkCooldown = 0.0f;
    public float atkCooldownValue;
    
    public bool attacking;

    [SerializeField] private int life = 1;
    public int scoreValue = 50;
    private bool _isDead = false;

    private AudioSource _audioSource;

    public AudioClip attackClip;
    public AudioClip hurtClip;
    
    // Start is called before the first frame update
    void Start()
    {
        _enemyRigidbody = GetComponent<Rigidbody2D>();
        _enemyColliders = GetComponents<Collider2D>().ToList();
        _audioSource = GetComponent<AudioSource>();
        _player = FindObjectOfType<PlayerController>().gameObject;
        atkCooldown = atkCooldownValue;
    }

    private void Update()
    {
        if (!_isDead)
        {
            _directionOfPlayer = (transform.position - _player.transform.position).normalized;

            if (checkGround)
            {
                _isGrounded = false;
                animator.SetBool("isGrounded", _isGrounded);
                
                // The enemy is grounded if a circlecast to the groundcheck position hits anything designated as ground
                // This can be done using layers instead but Sample Assets will not overwrite your project settings.
                Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != gameObject)
                    {
                        _isGrounded = true;
                        animator.SetBool("isGrounded", _isGrounded);
                    }
                }
            }

            atkCooldown -= Time.deltaTime;
        }
    }
    
    private Vector3 _velocity = Vector3.zero;
    
    private void PlayHurtSound()
    {
        _audioSource.volume = 0.7f;
        _audioSource.pitch = 1f;
        _audioSource.clip = hurtClip;
        _audioSource.Play();
    }
    
    private void PlayAttackSound()
    {
        _audioSource.volume = 0.7f;
        _audioSource.pitch = 1f;
        _audioSource.clip = attackClip;
        _audioSource.Play();
    }

    void FixedUpdate()
    {
        if (!_isDead)
        {
            if (checkGround && _isGrounded)
            {
                if (!collidingWithAura)
                {
                    // Move the character by finding the target velocity
                    Vector3 targetVelocity = -_directionOfPlayer * new Vector2( movementSpeed * Time.fixedDeltaTime, _enemyRigidbody.velocity.y);
                    // And then smoothing it out and applying it to the character
                    _enemyRigidbody.velocity = Vector3.SmoothDamp(_enemyRigidbody.velocity, targetVelocity, ref _velocity, 0.05f);

                    //_enemyRigidbody.MovePosition(-_directionOfPlayer * movementSpeed * Time.fixedDeltaTime);
                    animator.SetBool("isWalking", true);
                }
                else
                {
                    animator.SetBool("isGrounded", true);
                    if (!attacking && atkCooldown <= 0)
                    {
                        StartCoroutine(Attack());
                    }
                }
            }

            // If the input is moving the enemy right and the player is facing left...
            if (_directionOfPlayer.x < 0 && !_facingRight)
            {
                // ... flip
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (_directionOfPlayer.x > 0 && _facingRight)
            {
                // ... flip
                Flip();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Aura"))
        {
            collidingWithAura = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Aura"))
        {
            collidingWithAura = false;
        }
    }

    IEnumerator Attack()
    {
        attacking = true;

        yield return new WaitForSeconds(attackWindowDelay);

        if (!_isDead)
        {
            PlayAttackSound();
            animator.SetTrigger("attack");

            yield return new WaitForSeconds(0.30f);
            GameObject instance = Instantiate(hitBox, atkPos.position, Quaternion.identity, transform);
            instance.GetComponent<HitBox>().origin = this;
            Destroy(instance, atkTimer);
        
            atkCooldown = atkCooldownValue;
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

    public bool TakeDamage(int damage)
    {
        life -= damage;
        if (life <= 0)
        {
            _isDead = true;
            foreach (var collider2D in _enemyColliders)
            {
                collider2D.isTrigger = true;
            }
            float deathForceMagnitude = Random.Range(50, 150);
            _enemyRigidbody.AddForce(Vector2.up * deathForceMagnitude, ForceMode2D.Impulse);
            PlayHurtSound();
            animator.SetTrigger("death");
        }

        return _isDead;
    }
}
