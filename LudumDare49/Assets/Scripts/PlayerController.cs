using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Whether or not the player is grounded.
    [HideInInspector] public bool isGrounded;
    
    [SerializeField] private int life = 3;

    [SerializeField] private int maxLife = 5;

    [SerializeField] private float invicibilityDuration = 1.0f;

    [SerializeField] private GameObject lifeUI;
    
    // Radius of the overlap circle to determine if grounded
    [SerializeField] private float groundedRadius = .2f; 
    
    // A mask determining what is ground to the character
    [SerializeField] private LayerMask whatIsGround;
    
    // A position marking where to check if the player is grounded.
    [SerializeField] private Transform groundCheckTransform;

    [SerializeField] private GameObject lifePrefab;

    private Animator _playerAnimator;
    private SpriteRenderer _playerSprite;

    private bool _canTakeDamage = true;

    // Cached property index
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");

    private void Start()
    {
        _playerAnimator = GetComponent<Animator>();
        _playerSprite = GetComponent<SpriteRenderer>();
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
    
    public void TakeDamage(int damage)
    {
        if (_canTakeDamage)
        {
            life -= damage;
            if (life > 0)
            {
                StartCoroutine(Invincibility(invicibilityDuration));
                for (int i = 0; i < damage; i++)
                {
                    if (lifeUI.transform.GetChild(i))
                    {
                        Destroy(lifeUI.transform.GetChild(i).gameObject);
                    }
                }
            }
            else
            {
                Destroy(this);
            }
        }
    }

    public void TakePowerUp(int heal)
    {
        life += heal;
        if (life < maxLife)
        {
            for (int i = 0; i < heal; i++)
            {
                StartCoroutine(Heal()); 
            }
        }
    }

    IEnumerator Heal()
    {
        Transform lastChild = lifeUI.transform.GetChild(lifeUI.transform.childCount - 1);
        GameObject lifeObject = Instantiate(lifePrefab, lifePrefab.transform.position, Quaternion.identity);
        lifeObject.transform.SetParent(lifeUI.transform);
        RectTransform UIPosition = lifeObject.GetComponent<RectTransform>(); 
        UIPosition.localPosition = Vector3.zero;
        UIPosition.anchoredPosition =
            new Vector2(lastChild.GetComponent<RectTransform>().anchoredPosition.x - 80.0f, 0);
        yield return null;
    }

    IEnumerator Invincibility(float duration)
    {
        _canTakeDamage = false;
        Color normalColor = Color.white;
        Color hitColor = Color.clear;

        for (int i = 0; i < duration / 0.2f; i++)
        {
            _playerSprite.material.color = hitColor;
            yield return new WaitForSeconds(0.1f);
            _playerSprite.material.color = normalColor;
            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
        _playerSprite.material.color = normalColor;
        _canTakeDamage = true;
    }

    private void OnDestroy()
    {
        SceneManager.LoadScene(0);
    }
}
