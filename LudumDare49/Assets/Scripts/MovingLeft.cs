using System;
using UnityEngine;

public class MovingLeft : MonoBehaviour
{
    public Transform groundCheck;
    
    [SerializeField] private float objectSpeed = 3.0f;
    [SerializeField] private bool checkGround = true;

    private Rigidbody2D objectRigidBody;
    private Animator objectAnimator;
    private bool isGrounded = false;
    
    const float groundedRadius = .2f;
    
    // Start is called before the first frame update
    void Start()
    {
        objectRigidBody = GetComponent<Rigidbody2D>();
        objectAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (checkGround && isGrounded)
        {
            objectRigidBody.MovePosition(transform.position + Vector3.left * objectSpeed * Time.fixedDeltaTime);
        }
        else if (!checkGround)
        {
            objectRigidBody.MovePosition(transform.position + Vector3.left * objectSpeed * Time.fixedDeltaTime);
        }
        
        if (checkGround)
        {
            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius);
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
    }
}
