using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// TODO: comments
/// </summary>
public class GorillaController : MonoBehaviour
{
    /// <summary>
    /// TODO: comments
    /// </summary>
    public int scoreValue = 300;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [HideInInspector] public bool isDead = false;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float deathAnimationDuration = 3.0f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float maxLeftPosition;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float maxRightPosition;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private int maxLives = 25;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float startTimeBtwAttack = 10.0f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float impactForceMagnitude = 300.0f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    public Transform leftRopeAnchor;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    public Transform rightRopeAnchor;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    public GameObject SFX;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private int SFXNumber = 10;

    /// <summary>
    /// TODO: comments
    /// </summary>
    private List<Rigidbody2D> _jointRbs;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private SpriteRenderer _gorillaSprite;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private int _life;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private float _timeBtwAttack = 0.0f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private bool _isSmashing = false;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private bool _hasEnrage = false;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private bool _invincibility = false;

    /// <summary>
    /// TODO: comments
    /// </summary>
    private Animator _gorillaAnimator;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private AudioSource _audioSource;

    /// <summary>
    /// TODO: comments
    /// </summary>
    public AudioClip smashClip;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    public AudioClip hurtClip;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    public AudioClip enrageClip;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private void Start()
    {
        _gorillaAnimator = GetComponent<Animator>();
        _gorillaSprite = GetComponentInChildren<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        
        leftRopeAnchor = GameObject.FindGameObjectWithTag("LeftAnchor").transform;
        rightRopeAnchor = GameObject.FindGameObjectWithTag("RightAnchor").transform;
        
        _jointRbs = leftRopeAnchor.gameObject.GetComponentsInChildren<Rigidbody2D>().ToList();

        _life = maxLives;
        _timeBtwAttack = 5.0f;
        SetGorillaPosition();
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    private void Update()
    {
        if (!isDead && !_isSmashing)
        {
            if (_timeBtwAttack <= 0)
            {
                StartCoroutine(Smash());
                _timeBtwAttack = startTimeBtwAttack;
            }
            else
            {
                _timeBtwAttack -= Time.deltaTime;
            }
        }

        if (_life == 10 && !_hasEnrage)
        {
            _hasEnrage = true;
            StartCoroutine(BigSmash());
        }
    }
    
    /// <summary>
    /// TODO: comments
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
    /// TODO: comments
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
    /// TODO: comments
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
    /// TODO: comments
    /// </summary>
    /// <returns>TODO: comments</returns>
    IEnumerator BigSmash()
    {
        _invincibility = true;
        _isSmashing = true;
        _gorillaAnimator.SetTrigger("big_smash");
        PlayEnrageSound();
        yield return new WaitForSeconds(2.1f);
        
        _gorillaAnimator.SetTrigger("instant_smash");
        yield return new WaitForSeconds(1f);
        
        _gorillaAnimator.SetTrigger("instant_smash");
        yield return new WaitForSeconds(1f);
        
        _gorillaAnimator.SetTrigger("instant_smash");
        yield return new WaitForSeconds(1f);

        startTimeBtwAttack /= 2;
        _timeBtwAttack = startTimeBtwAttack;
        _isSmashing = false;
        _invincibility = false;
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    private void ApplyForce()
    {
        foreach (var rigidbody in _jointRbs)
        {
            if (Vector2.Distance(rigidbody.position, leftRopeAnchor.position) <=
                Vector2.Distance(rigidbody.position, rightRopeAnchor.position))
            {
                rigidbody.AddForce(Vector2.up * impactForceMagnitude / (Vector2.Distance(rigidbody.position, leftRopeAnchor.position) / 2), ForceMode2D.Impulse);
            }
            else
            {
                rigidbody.AddForce(Vector2.up * impactForceMagnitude / (Vector2.Distance(rigidbody.position, rightRopeAnchor.position) / 2), ForceMode2D.Impulse);
            }
        }
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <returns>TODO: comments</returns>
    IEnumerator Smash()
    {
        _isSmashing = true;
        _gorillaAnimator.SetTrigger("smash");
        PlayEnrageSound();
        yield return null;
        _isSmashing = false;
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    private void SetGorillaPosition()
    {
        float targetX = Random.Range(transform.position.x - 5, transform.position.x + 5);

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
    /// TODO: comments
    /// </summary>
    /// <param name="damage">TODO: comments</param>
    /// <returns>TODO: comments</returns>
    public bool TakeDamage(int damage)
    {
        if (!_invincibility && !isDead)
        {
            _life -= damage;
            if (_life <= 0)
            {
                StartCoroutine(Die());
            }
            else
            {
                StartCoroutine(Damaged());
            }

        }

        return isDead;
    }
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <returns>TODO: comments</returns>
    IEnumerator Damaged()
    {
        PlayHurtSound();
        Color normalColor = Color.white;
        Color hitColor = Color.clear;

        for (int i = 0; i < 2; i++)
        {
            _gorillaSprite.material.color = hitColor;
            yield return new WaitForSeconds(0.05f);
            _gorillaSprite.material.color = normalColor;
            yield return new WaitForSeconds(0.05f);
        }

        yield return null;
        _gorillaSprite.material.color = normalColor;
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <returns>TODO: comments</returns>
    IEnumerator Die()
    {
        isDead = true;
        MusicPlayer.Instance.Switch(false);
        
        for (int i = 0; i < SFXNumber; i++)
        {
            Vector2 sfxPosition = (Vector2) transform.position + new Vector2(Random.Range(_gorillaSprite.bounds.min.x + 7.5f, _gorillaSprite.bounds.max.x - 7.5f),
                Random.Range(_gorillaSprite.bounds.min.y, _gorillaSprite.bounds.max.y - 5));
            float sfxScale = Random.Range(1, 4);
            SFX.transform.localScale = new Vector3(sfxScale, sfxScale, 0);
            Instantiate(SFX, sfxPosition, quaternion.identity);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(deathAnimationDuration);
        
        SpawnerController.stopSpawn = false;
        Destroy(gameObject);
    }
}
