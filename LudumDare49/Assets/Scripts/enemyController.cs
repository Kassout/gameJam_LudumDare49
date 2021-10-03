using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyController : MonoBehaviour
{
    public Transform groundCheck;
    private GameObject Player;

    [SerializeField] private float objectSpeed = 3.0f;
    [SerializeField] private bool checkGround = true;

    private Rigidbody2D objectRigidBody;
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

    // Start is called before the first frame update
    void Start()
    {
        objectRigidBody = GetComponent<Rigidbody2D>();
        Player = FindObjectOfType<PlayerController>().gameObject;
        atkCooldown = atkCooldownValue + 2;
    }

    private void Update()
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

    void FixedUpdate()
    {
        if (checkGround && isGrounded)
        {
            if (!CollidingWithAura)
            {
                objectRigidBody.AddForce(-DirectionOfPlayer * objectSpeed * objectRigidBody.mass);
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
        instance.GetComponent<hitBox>().origin = this;
        Destroy(instance, atkTimer);
    }
}
