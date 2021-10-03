using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    bool FacingRight;

    public Transform groundCheck;
    private GameObject Player;

    [SerializeField] private float objectSpeed = 3.0f;
    [SerializeField] private bool checkGround = true;

    private Rigidbody2D _objectRigidBody;
    private Collider2D _objectCollider;
    public Animator objectAnimator;
    private bool isGrounded = false;

    public float groundedRadius = .2f;
    public LayerMask groundedMask;

    public bool CollidingWithAura;

    public Vector3 DirectionOfPlayer;

    public GameObject hitBox;
    public Transform atkPos;
    public float atkTimer;
    [System.NonSerialized] public float atkCooldown;
    public float atkCooldownValue;
    
    public bool attacking;

    [SerializeField] private int life = 1;
    private bool _isDead = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _objectRigidBody = GetComponent<Rigidbody2D>();
        _objectCollider = GetComponent<Collider2D>();
        Player = FindObjectOfType<PlayerController>().gameObject;
        atkCooldown = atkCooldownValue + 2;
    }

    private void Update()
    {
        if (!_isDead)
        {
            DirectionOfPlayer = (transform.position - Player.transform.position).normalized;

            if (checkGround)
            {
                // The enemy is grounded if a circlecast to the groundcheck position hits anything designated as ground
                // This can be done using layers instead but Sample Assets will not overwrite your project settings.
                Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, groundedMask);
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != gameObject)
                    {
                        isGrounded = true;
                        objectAnimator.SetBool("isGrounded", isGrounded);
                        break;
                    }
                    else
                    {
                        isGrounded = false;
                        objectAnimator.SetBool("isGrounded", isGrounded);
                    }
                }
            }

            atkCooldown -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (!_isDead)
        {
            if (checkGround && isGrounded)
            {
                if (!CollidingWithAura)
                {
                    _objectRigidBody.AddForce(-DirectionOfPlayer * objectSpeed * _objectRigidBody.mass);
                }
                else
                {
                    if (!attacking && atkCooldown <= 0)
                    {
                        Attack();
                        attacking = true;
                        atkCooldown = atkCooldownValue;
                    }
                }
            }

            // If the input is moving the enemy right and the player is facing left...
            if (DirectionOfPlayer.x < 0 && !FacingRight)
            {
                // ... flip
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (DirectionOfPlayer.x > 0 && FacingRight)
            {
                // ... flip
                Flip();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Aura")
        {
            CollidingWithAura = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Aura")
        {
            CollidingWithAura = false;
        }
    }

    void Attack()
    {
        GameObject instance = Instantiate(hitBox, atkPos.position, Quaternion.identity, transform);
        instance.GetComponent<HitBox>().origin = this;
        Destroy(instance, atkTimer);
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        FacingRight = !FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void TakeDamage(int damage)
    {
        life -= damage;
        if (life <= 0)
        {
            _isDead = true;
            _objectCollider.enabled = false;
            Vector2 deathDirection = Random.Range(0, 1) == 0 ? Vector2.left : Vector2.right;
            float deathForceMagnitude = Random.Range(50, 100);
            _objectRigidBody.AddForce((Vector2.up + deathDirection) * deathForceMagnitude, ForceMode2D.Impulse);
            objectAnimator.SetTrigger("death");
        }
    }
}
