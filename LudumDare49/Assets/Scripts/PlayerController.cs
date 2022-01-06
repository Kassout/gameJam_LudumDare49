using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Class <c>PlayerController</c> is a Unity component script used to manage the general player behaviour.
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region Fields / Properties

    /// <summary>
    /// Instance field <c>animator</c> is a Unity <c>Animator</c> component representing the player animations manager.
    /// </summary>
    private Animator _animator;
    
    /// <summary>
    /// Instance field <c>spriteRenderer</c> is a Unity <c>SpriteRenderer</c> component representing the player sprite renderer.
    /// </summary>
    private SpriteRenderer _spriteRenderer;
    
    /// <summary>
    /// Instance field <c>audioSource</c> is a Unity <c>AudioSource</c> component representing the player audio source for SFX playing.
    /// </summary>
    private AudioSource _audioSource;
    
    /// <summary>
    /// Instance field <c>hurtClip</c> is a Unity <c>AudioClip</c> object representing the player hurt audio sound.
    /// </summary>
    [SerializeField] private AudioClip hurtClip;
    
    /// <summary>
    /// Instance field <c>deathClip</c> is a Unity <c>AudioClip</c> object representing the player death audio sound.
    /// </summary>
    [SerializeField] private AudioClip deathClip;

    /// <summary>
    /// Instance field <c>IsGroundedHash</c> represents the integer identifier of the string message "isGrounded" for the player animator.
    /// </summary>
    private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
    
    /// <summary>
    /// Instance field <c>maxLife</c> represents the maximum number of lives of the player.
    /// </summary>
    [Header("Player life parameters")]
    [SerializeField] private int maxLife = 5;
    
    /// <summary>
    /// Instance field <c>life</c> represents the number of lives of the player.
    /// </summary>
    [SerializeField] private int life = 3;
    
    /// <summary>
    /// Instance field <c>lifePrefab</c> is a Unity <c>GameObject</c> representing the prefabricated life game object.
    /// </summary>
    [SerializeField] private GameObject lifePrefab;
    
    /// <summary>
    /// Instance field <c>lifeUI</c> is a Unity <c>GameObject</c> representing the game life UI.
    /// </summary>
    [SerializeField] private GameObject lifeUI;
    
    /// <summary>
    /// Instance field <c>screenBackground</c> is a Unity <c>Image</c> component representing the player death screen background.
    /// </summary>
    [SerializeField] private Image screenBackground;
    
    /// <summary>
    /// Instance field <c>invincibilityDuration</c> represents the duration value of the player invincibility on hurt.
    /// </summary>
    [SerializeField] private float invincibilityDuration = 1.0f;
    
    /// <summary>
    /// Instance field <c>canTakeDamage</c> represents the can take damage status of the player.
    /// </summary>
    [HideInInspector] public bool canTakeDamage = true;
    
    /// <summary>
    /// Instance field <c>groundCheckTransform</c> is a Unity <c>Transform</c> component representing the position, rotation and scale of the player game object ground check point.
    /// </summary>
    [Header("Player ground check parameters")]
    [SerializeField] private Transform groundCheckTransform;
    
    /// <summary>
    /// Instance field <c>whatIsGround</c> is a Unity <c>LayerMask</c> structure representing the ground layer in the scene.
    /// </summary>
    [SerializeField] private LayerMask whatIsGround;
    
    /// <summary>
    /// Instance field <c>groundCheckRadius</c> represents the radius size value of the player ground check zone.
    /// </summary>
    [SerializeField] private float groundCheckRadius = 0.2f; 
    
    /// <summary>
    /// Instance field <c>isGrounded</c> represents the grounded status of the player game object.
    /// </summary>
    [HideInInspector] public bool isGrounded;

    #endregion

    #region MonoBehavior

    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// This function is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        isGrounded = false;
        _animator.SetBool(IsGroundedHash, isGrounded);
        
        // The player is grounded if a circle cast to the ground check position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        if (Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, whatIsGround))
        {
            isGrounded = true;
            _animator.SetBool(IsGroundedHash, isGrounded);
        }
    }
    
    /// <summary>
    /// This function is called when the game shuts down, switches to another scene or when the related game object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        SceneManager.LoadScene(3);
    }

    #endregion

    #region Private

    /// <summary>
    /// This function is responsible for playing the player hurt SFX when called.
    /// </summary>
    private void PlayHurtSound()
    {
        _audioSource.volume = 0.7f;
        _audioSource.pitch = 1f;
        _audioSource.clip = hurtClip;
        _audioSource.Play();
    }
    
    /// <summary>
    /// This function is responsible for playing the player death SFX when called.
    /// </summary>
    private void PlayDeathSound()
    {
        _audioSource.volume = 0.7f;
        _audioSource.pitch = 1f;
        _audioSource.clip = deathClip;
        _audioSource.Play();
    }

    /// <summary>
    /// This function is responsible for managing the death behavior of the player.
    /// </summary>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
    private IEnumerator Death()
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

    /// <summary>
    /// This function is responsible for managing the healing behavior of the player.
    /// </summary>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
    private IEnumerator Heal()
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

    /// <summary>
    /// This function is responsible for managing the invincibility behavior of the player.
    /// </summary>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
    private IEnumerator Invincibility()
    {
        canTakeDamage = false;
        PlayHurtSound();
        Color normalColor = Color.white;
        Color hitColor = Color.clear;

        for (int i = 0; i < invincibilityDuration / 0.2f; i++)
        {
            Material material = _spriteRenderer.material;
            material.color = hitColor;
            yield return new WaitForSeconds(0.1f);
            material.color = normalColor;
            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
        _spriteRenderer.material.color = normalColor;
        canTakeDamage = true;
    }
    
    #endregion

    #region Public

    /// <summary>
    /// This function is responsible for subtracting life of the player when taking damage.
    /// </summary>
    /// <param name="damage">An integer value representing the quantity of damage received by the player.</param>
    public void TakeDamage(int damage)
    {
        if (canTakeDamage)
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
    


    /// <summary>
    /// This function is responsible for adding life to the player when taking power up.
    /// </summary>
    /// <param name="heal">An integer value representing the quantity of heal received by the player.</param>
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

    #endregion
}