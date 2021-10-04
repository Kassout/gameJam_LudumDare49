using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GorillaController : MonoBehaviour
{
    public int scoreValue = 300;
    
    [HideInInspector] public bool isDead = false;
    
    [SerializeField] private float deathAnimationDuration = 3.0f;
    [SerializeField] private float maxLeftPosition;
    [SerializeField] private float maxRightPosition;
    [SerializeField] private int maxLives = 25;
    [SerializeField] private float startTimeBtwAttack = 10.0f;
    [SerializeField] private float impactForceMagnitude = 300.0f;
    
    public Transform leftRopeAnchor;
    public Transform rightRopeAnchor;
    public GameObject SFX;
    [SerializeField] private int SFXNumber = 10;

    private List<Rigidbody2D> _jointRbs;
    private SpriteRenderer _gorillaSprite;
    
    private int _life;
    private float _timeBtwAttack = 0.0f;
    private bool _isSmashing = false;
    private bool _hasEnrage = false;
    private bool _invincibility = false;

    private Animator _gorillaAnimator;
    private AudioSource _audioSource;

    public AudioClip smashClip;
    public AudioClip hurtClip;
    public AudioClip enrageClip;

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
    
    private void PlaySmashSound()
    {
        _audioSource.volume = 1.3f;
        _audioSource.pitch = 1f;
        _audioSource.clip = smashClip;
        _audioSource.loop = false;
        _audioSource.Play();
    }

    private void PlayEnrageSound()
    {
        _audioSource.volume = 0.8f;
        _audioSource.pitch = 1f;
        _audioSource.clip = enrageClip;
        _audioSource.loop = true;
        _audioSource.Play();
    }

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

    IEnumerator Smash()
    {
        _isSmashing = true;
        _gorillaAnimator.SetTrigger("smash");
        PlayEnrageSound();
        yield return null;
        _isSmashing = false;
    }

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
