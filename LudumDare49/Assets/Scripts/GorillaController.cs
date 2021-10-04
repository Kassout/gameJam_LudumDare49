using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GorillaController : MonoBehaviour
{
    public int scoreValue = 300;
    [SerializeField] private float deathAnimationDuration = 3.0f;
    [SerializeField] private float maxLeft;
    [SerializeField] private float maxRight;
    [HideInInspector] public bool isDead = false;
    
    [SerializeField] private int maxLives;

    private int _life;

    private Animator _gorillaAnimator;

    private void Start()
    {
        _gorillaAnimator = GetComponent<Animator>();
        
        _life = maxLives;
        SetGorillaPosition();
    }

    private void Update()
    {

    }

    private void SetGorillaPosition()
    {
        float targetX = Random.Range(transform.position.x - 5, transform.position.x + 5);

        transform.Translate(new Vector3(targetX, 0, 0));

        if(transform.position.x > maxRight)
        {
            transform.position = new Vector3(maxRight, 0, 0);
        }
        if (transform.position.x < maxLeft)
        {
            transform.position = new Vector3(maxLeft, 0, 0);
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
