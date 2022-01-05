using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Class <c>GorillaController</c> is a Unity component script used to manage the general gorilla boss behaviour.
/// </summary>
public class GorillaController : MonoBehaviour
{
    #region Fields / Properties

    /// <summary>
    /// Instance field <c>jointRbs</c> is a list of Unity <c>Rigidbody2D</c> components representing the rope joint rigidbodies.
    /// </summary>
    private List<Rigidbody2D> _jointRbs;

    /// <summary>
    /// Instance field <c>gorillaAnimator</c> is a Unity <c>Animator</c> component representing the gorilla animations manager.
    /// </summary>
    private Animator _animator;

    /// <summary>
    /// Instance field <c>spriteRenderer</c> is a Unity <c>SpriteRenderer</c> component representing the gorilla sprite renderer.
    /// </summary>
    private SpriteRenderer _spriteRenderer;

    /// <summary>
    /// Instance field <c>audioSource</c> is a Unity <c>AudioSource</c> component representing the gorilla audio source for SFX playing.
    /// </summary>
    private AudioSource _audioSource;
    
    /// <summary>
    /// Instance field <c>smashClip</c> is a Unity <c>AudioClip</c> structure representing the gorilla smash audio SFX.
    /// </summary>
    [SerializeField] private AudioClip smashClip;
    
    /// <summary>
    /// Instance field <c>hurtClip</c> is a Unity <c>AudioClip</c> structure representing the gorilla hurt audio SFX.
    /// </summary>
    [SerializeField] private AudioClip hurtClip;
    
    /// <summary>
    /// Instance field <c>enrageClip</c> is a Unity <c>AudioClip</c> structure representing the gorilla enrage audio SFX.
    /// </summary>
    [SerializeField] private AudioClip enrageClip;

    /// <summary>
    /// Instance field <c>BigSmashHash</c> represents the integer identifier of the string message "big_smash" for the enemy animator.
    /// </summary>
    private static readonly int BigSmashHash = Animator.StringToHash("big_smash");
    
    /// <summary>
    /// Instance field <c>InstantSmashHash</c> represents the integer identifier of the string message "instant_smash" for the enemy animator.
    /// </summary>
    private static readonly int InstantSmashHash = Animator.StringToHash("instant_smash");
    
    /// <summary>
    /// Instance field <c>SmashHash</c> represents the integer identifier of the string message "smash" for the enemy animator.
    /// </summary>
    private static readonly int SmashHash = Animator.StringToHash("smash");
    
    /// <summary>
    /// Instance field <c>maxLives</c> represents the number of maximum lives of the gorilla enemy.
    /// </summary>
    [Header("General parameters")]
    [SerializeField] private int maxLives = 25;
    
    /// <summary>
    /// Instance field <c>_currentLife</c> represents the current number of lives of the gorilla enemy.
    /// </summary>
    private int _currentLife;
    
    /// <summary>
    /// Instance field <c>hasEnrage</c> represents the enrage status of the gorilla enemy.
    /// </summary>
    private bool _hasEnrage;
    
    /// <summary>
    /// Instance field <c>invincibility</c> represents the invincibility status of the gorilla enemy.
    /// </summary>
    private bool _invincibility;
    
    /// <summary>
    /// Instance field <c>isDead</c> represents the dead status of the gorilla enemy.
    /// </summary>
    private bool _isDead;
    
    /// <summary>
    /// Instance field <c>deathAnimationDuration</c> represents the duration value of the death animation of the gorilla animation.
    /// </summary>
    [SerializeField] private float deathAnimationDuration = 3.0f;

    /// <summary>
    /// Instance field <c>scoreValue</c> represents the score value received by the player killing the gorilla enemy game object.
    /// </summary>
    public int scoreValue = 300;
    
    /// <summary>
    /// Instance field <c>maxLeftPosition</c> represents the maximum left x-coordinate position value of the gorilla enemy to spawn.
    /// </summary>
    [Header("Spawn parameters")]
    [SerializeField] private float maxLeftPosition;
    
    /// <summary>
    /// Instance field <c>maxRightPosition</c> represents the maximum right x-coordinate position value of the gorilla enemy to spawn.
    /// </summary>
    [SerializeField] private float maxRightPosition;
    
    /// <summary>
    /// Instance field <c>attackCooldownTime</c> represents the duration value between two gorilla enemy attacks.
    /// </summary>
    [Header("Attack parameters")]
    [SerializeField] private float attackCooldownTime = 10.0f;
    
    /// <summary>
    /// Instance field <c>attackCooldownTimeValue</c> represents the time before the last enemy attack.
    /// </summary>
    private float _attackCooldownTimeValue;
    
    /// <summary>
    /// Instance field <c>isSmashing</c> represents the smashing status of the gorilla enemy.
    /// </summary>
    private bool _isSmashing;

    /// <summary>
    /// Instance field <c>impactForceMagnitude</c> represents the force magnitude value of the gorilla attack smash impact.
    /// </summary>
    [SerializeField] private float impactForceMagnitude = 300.0f;
    
    /// <summary>
    /// Instance field <c>leftRopeAnchor</c> is a Unity <c>Transform</c> component representing the position, rotation and scale of the left rope anchor.
    /// </summary>
    private Transform _leftRopeAnchor;
    
    /// <summary>
    /// Instance field <c>rightRopeAnchor</c> is a Unity <c>Transform</c> component representing the position, rotation and scale of the right rope anchor.
    /// </summary>
    private Transform _rightRopeAnchor;
    
    /// <summary>
    /// Instance field <c>effectsBox</c> is a Unity <c>GameObject</c> representing the gorilla death effects player manager.
    /// </summary>
    [Header("On death parameters")]
    [SerializeField] private GameObject effectsBox;
    
    /// <summary>
    /// Instance field <c>effectsNumber</c> represents the quantity of effects to play on gorilla's death.
    /// </summary>
    [SerializeField] private int effectsNumber = 10;

    #endregion

    #region MonoBehavior

    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        
        _leftRopeAnchor = GameObject.FindGameObjectWithTag("LeftAnchor").transform;
        _rightRopeAnchor = GameObject.FindGameObjectWithTag("RightAnchor").transform;
        
        _jointRbs = _leftRopeAnchor.gameObject.GetComponentsInChildren<Rigidbody2D>().ToList();

        _currentLife = maxLives;
        _attackCooldownTimeValue = 5.0f;
        SetGorillaPosition();
    }

    /// <summary>
    /// This function is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        if (!_isDead && !_isSmashing)
        {
            if (_attackCooldownTimeValue <= 0)
            {
                StartCoroutine(Smash());
                _attackCooldownTimeValue = attackCooldownTime;
            }
            else
            {
                _attackCooldownTimeValue -= Time.deltaTime;
            }
        }

        if (_currentLife == 10 && !_hasEnrage)
        {
            _hasEnrage = true;
            StartCoroutine(BigSmash());
        }
    }

    #endregion

    #region Private

        /// <summary>
    /// This function is responsible for playing the gorilla enemy hurt SFX when called.
    /// </summary>
    private void PlayHurtSound()
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.volume = 0.7f;
            _audioSource.pitch = 1f;
            _audioSource.clip = hurtClip;
            _audioSource.loop = false;
            _audioSource.Play();
        }
    }
    
    /// <summary>
    /// This function is responsible for playing the gorilla enemy smash SFX when called.
    /// </summary>
    private void PlaySmashSound()
    {
        _audioSource.volume = 1.3f;
        _audioSource.pitch = 1f;
        _audioSource.clip = smashClip;
        _audioSource.loop = false;
        _audioSource.Play();
    }

    /// <summary>
    /// This function is responsible for playing the gorilla enemy enrage SFX when called.
    /// </summary>
    private void PlayEnrageSound()
    {
        _audioSource.volume = 0.8f;
        _audioSource.pitch = 1f;
        _audioSource.clip = enrageClip;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    /// <summary>
    /// This function is responsible for managing the big smash attack behavior of the gorilla enemy.
    /// </summary>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
    private IEnumerator BigSmash()
    {
        _invincibility = true;
        _isSmashing = true;
        _animator.SetTrigger(BigSmashHash);
        PlayEnrageSound();
        yield return new WaitForSeconds(2.1f);
        
        _animator.SetTrigger(InstantSmashHash);
        yield return new WaitForSeconds(1f);
        
        _animator.SetTrigger(InstantSmashHash);
        yield return new WaitForSeconds(1f);
        
        _animator.SetTrigger(InstantSmashHash);
        yield return new WaitForSeconds(1f);

        attackCooldownTime /= 2;
        _attackCooldownTimeValue = attackCooldownTime;
        _isSmashing = false;
        _invincibility = false;
    }

    /// <summary>
    /// This function is responsible for applying force to the rope terrain.
    /// </summary>
    private void ApplyForce()
    {
        foreach (Rigidbody2D jointRigidBody in _jointRbs)
        {
            if (Vector2.Distance(jointRigidBody.position, _leftRopeAnchor.position) <=
                Vector2.Distance(jointRigidBody.position, _rightRopeAnchor.position))
            {
                jointRigidBody.AddForce(Vector2.up * impactForceMagnitude / (Vector2.Distance(jointRigidBody.position, _leftRopeAnchor.position) / 2), ForceMode2D.Impulse);
            }
            else
            {
                jointRigidBody.AddForce(Vector2.up * impactForceMagnitude / (Vector2.Distance(jointRigidBody.position, _rightRopeAnchor.position) / 2), ForceMode2D.Impulse);
            }
        }
    }

    /// <summary>
    /// This function is responsible for managing the smash attack behavior of the gorilla enemy.
    /// </summary>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
    private IEnumerator Smash()
    { 
        _isSmashing = true;
        _animator.SetTrigger(SmashHash);
        PlayEnrageSound();
        yield return null;
        _isSmashing = false;
    }
    
    /// <summary>
    /// This function is responsible for computing and setting up the spawning gorilla position.
    /// </summary>
    private void SetGorillaPosition()
    {
        Vector3 position = transform.position;
        float targetX = Random.Range(position.x - 5, position.x + 5);

        transform.Translate(new Vector3(targetX, 0, 0));

        if(transform.position.x > maxRightPosition)
        {
            transform.position = new Vector3(maxRightPosition, 0, 0);
        }
        if (transform.position.x < maxLeftPosition)
        {
            transform.position = new Vector3(maxLeftPosition, 0, 0);
        }
    }
    
    /// <summary>
    /// This function is responsible for managing the damaged behavior of the gorilla enemy.
    /// </summary>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
    private IEnumerator Damaged()
    {
        PlayHurtSound();
        Color normalColor = Color.white;
        Color hitColor = Color.clear;

        for (int i = 0; i < 2; i++)
        {
            Material material = _spriteRenderer.material;
            material.color = hitColor;
            yield return new WaitForSeconds(0.05f);
            material.color = normalColor;
            yield return new WaitForSeconds(0.05f);
        }

        yield return null;
        _spriteRenderer.material.color = normalColor;
    }

    /// <summary>
    /// This function is responsible for managing the dying behavior of the gorilla enemy.
    /// </summary>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
    private IEnumerator Die()
    {
        _isDead = true;
        MusicPlayer.Instance.Switch(false);
        
        for (int i = 0; i < effectsNumber; i++)
        {
            Bounds bounds = _spriteRenderer.bounds;
            Vector2 sfxPosition = (Vector2) transform.position + new Vector2(Random.Range(bounds.min.x + 7.5f, bounds.max.x - 7.5f),
                Random.Range(bounds.min.y, bounds.max.y - 5));
            float sfxScale = Random.Range(1, 4);
            effectsBox.transform.localScale = new Vector3(sfxScale, sfxScale, 0);
            Instantiate(effectsBox, sfxPosition, quaternion.identity);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(deathAnimationDuration);
        
        SpawnerController.stopSpawn = false;
        Destroy(gameObject);
    }
    
    #endregion

    #region Public

    /// <summary>
    /// This function is responsible for subtracting life of the gorilla enemy when taking damage.
    /// </summary>
    /// <param name="damage">An integer value representing the quantity of damage received by the gorilla enemy.</param>
    /// <returns>A boolean value representing the dead status of the gorilla enemy.</returns>
    public bool TakeDamage(int damage)
    {
        if (!_invincibility && !_isDead)
        {
            _currentLife -= damage;
            StartCoroutine(_currentLife <= 0 ? Die() : Damaged());
        }

        return _isDead;
    }

    #endregion
}
