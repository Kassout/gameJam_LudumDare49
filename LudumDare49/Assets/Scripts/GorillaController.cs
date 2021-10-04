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

    private List<Rigidbody2D> jointRbs;
    
    private int _life;
    private float _timeBtwAttack = 0.0f;

    private Animator _gorillaAnimator;

    private void Start()
    {
        _gorillaAnimator = GetComponent<Animator>();
        
        leftRopeAnchor = GameObject.FindGameObjectWithTag("LeftAnchor").transform;
        rightRopeAnchor = GameObject.FindGameObjectWithTag("RightAnchor").transform;
        
        jointRbs = leftRopeAnchor.gameObject.GetComponentsInChildren<Rigidbody2D>().ToList();

        _life = maxLives;
        _timeBtwAttack = 5.0f;
        SetGorillaPosition();
    }

    private void Update()
    {
        if (!isDead)
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
    }

    private void ApplyForce()
    {
        foreach (var rigidbody in jointRbs)
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
        _gorillaAnimator.SetTrigger("smash");
        yield return null;
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
        _life -= damage;
        if (_life <= 0)
        {
            StartCoroutine(Die());
        }

        return isDead;
    }

    IEnumerator Die()
    {
        isDead = true;
        _gorillaAnimator.SetTrigger("death");

        yield return new WaitForSeconds(deathAnimationDuration);
        
        Destroy(gameObject);
    }
}
