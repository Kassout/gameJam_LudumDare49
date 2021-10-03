using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Whether or not the player is grounded.
    [HideInInspector] public bool isGrounded;
    
    // Radius of the overlap circle to determine if grounded
    [SerializeField] private float groundedRadius = .2f; 
    
    // A mask determining what is ground to the character
    [SerializeField] private LayerMask whatIsGround;
    
    // A position marking where to check if the player is grounded.
    [SerializeField] private Transform groundCheckTransform;

    private Animator _playerAnimator;
    
    // Cached property index
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");

    private void Start()
    {
        _playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = false;
        _playerAnimator.SetBool(IsGrounded, isGrounded);
        
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckTransform.position, groundedRadius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                isGrounded = true;
                _playerAnimator.SetBool(IsGrounded, isGrounded);
            }
        }
    }
}
