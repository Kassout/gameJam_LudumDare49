using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private List<Rigidbody2D> _jointRbs;
    private SpriteRenderer _gorillaSprite;
    
    private int _life;
    private float _timeBtwAttack = 0.0f;
    private bool _isSmashing = false;
    private bool _hasEnrage = false;
    private bool _invincibility = false;

    private Animator _gorillaAnimator;

    private void Start()
    {
        _gorillaAnimator = GetComponent<Animator>();
        _gorillaSprite = GetComponentInChildren<SpriteRenderer>();
        
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

        if (_life == 15 && !_hasEnrage)
        {
            _hasEnrage = true;
            StartCoroutine(BigSmash());
        }
    }

    IEnumerator BigSmash()
    {
        _invincibility = true;
        _isSmashing = true;
        _gorillaAnimator.SetTrigger("big_smash");
        yield return new WaitForSeconds(2.1f);
        
        _gorillaAnimator.SetTrigger("instant_smash");
        yield return new WaitForSeconds(1f);
        
        _gorillaAnimator.SetTrigger("instant_smash");
        yield return new WaitForSeconds(1f);
        
        _gorillaAnimator.SetTrigger("instant_smash");
        yield return new WaitForSeconds(1f);
        
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
        if (!_invincibility)
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
        _gorillaAnimator.SetTrigger("death");

        yield return new WaitForSeconds(deathAnimationDuration);
        
        SpawnerController.stopSpawn = false;
        Destroy(gameObject);
    }
}
