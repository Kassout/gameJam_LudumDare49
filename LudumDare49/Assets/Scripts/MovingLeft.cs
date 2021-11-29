using System;
using UnityEngine;

/// <summary>
/// TODO: comments
/// </summary>
public class MovingLeft : MonoBehaviour
{
    /// <summary>
    /// TODO: comments
    /// </summary>
    public Transform groundCheck;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float objectSpeed = 3.0f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private bool checkGround = true;

    /// <summary>
    /// TODO: comments
    /// </summary>
    private Rigidbody2D objectRigidBody;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private Animator objectAnimator;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private bool isGrounded = false;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    const float groundedRadius = .2f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    void Start()
    {
        objectRigidBody = GetComponent<Rigidbody2D>();
        objectAnimator = GetComponent<Animator>();
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
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
            // The enemy is grounded if a circlecast to the groundcheck position hits anything designated as ground
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
