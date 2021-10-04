using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Whether or not the player is grounded.
    [HideInInspector] public bool isGrounded;

    [SerializeField] public int life = 3;
    [SerializeField] private int maxLife = 5;
    [SerializeField] private float invicibilityDuration = 1.0f;
    [SerializeField] private GameObject lifeUI;
    
    // Radius of the overlap circle to determine if grounded
    [SerializeField] private float groundedRadius = .2f; 
    
    // A mask determining what is ground to the character
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Image screenBackground;
    
    // A position marking where to check if the player is grounded.
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private GameObject lifePrefab;
    
    public bool _canTakeDamage = true;
    public AudioClip hurtClip;
    public AudioClip deathClip;
    
    private Animator _playerAnimator;
    private SpriteRenderer _playerSprite;
    private AudioSource _audioSource;

    // Cached property index
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");

    private void Start()
    {
        _playerAnimator = GetComponent<Animator>();
        _playerSprite = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
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
            if (life - damage > 0)
            {
                life -= damage;
                StartCoroutine(Invincibility());
                for (int i = 0; i < damage; i++)
                {
                    if (lifeUI.transform.GetChild((lifeUI.transform.childCount - 1) - i))
                    {
                        Destroy(lifeUI.transform.GetChild((lifeUI.transform.childCount - 1) - i).gameObject);
                    }
                }
            }
            else
            {
                StartCoroutine(Death());
            }
        }
    }
    
    private void PlayHurtSound()
    {
        _audioSource.volume = 0.7f;
        _audioSource.pitch = 1f;
        _audioSource.clip = hurtClip;
        _audioSource.Play();
    }
    
    private void PlayDeathSound()
    {
        _audioSource.volume = 0.7f;
        _audioSource.pitch = 1f;
        _audioSource.clip = deathClip;
        _audioSource.Play();
    }

    IEnumerator Death()
    {
        PlayDeathSound();
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * 25, ForceMode2D.Impulse);
        Time.timeScale = 0.2f;
        screenBackground.color = new Color(1, 0, 0, 0.4f);
        
        MusicPlayer.Instance.PlayClip(MusicPlayer.Instance.startScreenClip);
        yield return new WaitForSecondsRealtime(1.5f);
        
        screenBackground.color = Color.clear;
        Time.timeScale = 1.0f;
        Destroy(this);
    }

    public void TakePowerUp(int heal)
    {
        if (life + heal <= maxLife)
        {
            life += heal;
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

    IEnumerator Invincibility()
    {
        _canTakeDamage = false;
        PlayHurtSound();
        Color normalColor = Color.white;
        Color hitColor = Color.clear;

        for (int i = 0; i < invicibilityDuration / 0.2f; i++)
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
        SceneManager.LoadScene(3);
    }
}
