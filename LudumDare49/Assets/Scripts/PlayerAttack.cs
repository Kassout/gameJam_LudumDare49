using System.Collections;
using UnityEngine;

/// <summary>
/// TODO: comments
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    /// <summary>
    /// TODO: comments
    /// </summary>
    public int damage = 1;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float attackRangeX = 1.0f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float attackRangeY = 1.0f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private Transform attackPos;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float startTimeBtwAttack;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private LayerMask whatIsEnemies;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private GameObject scoreUI;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    public AudioClip attackClip;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private float _timeBtwAttack = 0.0f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private PlayerController _playerController;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private Animator _playerAnimator;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private AudioSource _audioSource;

    /// <summary>
    /// TODO: comments
    /// </summary>
    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _playerAnimator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    void Update()
    {
        if (_timeBtwAttack <= 0)
        {
            // Then you can attack
            if (Input.GetButtonDown("Fire1") && _playerController.isGrounded)
            {
                StartCoroutine(Attack());
                
                _timeBtwAttack = startTimeBtwAttack;
            }
        }
        else
        {
            _timeBtwAttack -= Time.deltaTime;
        }
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <returns>TODO: comments</returns>
    IEnumerator Attack()
    {
        _playerAnimator.SetTrigger("attack");
        PlayAttackSound();

        yield return new WaitForSeconds(0.15f);
        Collider2D[] enemiesToDamage =
            Physics2D.OverlapBoxAll(attackPos.position, new Vector2(attackRangeX, attackRangeY), whatIsEnemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            if (enemiesToDamage[i].GetComponent<EnemyController>())
            {
                EnemyController enemy = enemiesToDamage[i].GetComponent<EnemyController>();
                bool isDead = enemy.TakeDamage(damage);
                if (isDead)
                {
                    StartCoroutine(scoreUI.GetComponent<ScoreController>().AddScore(enemy.scoreValue));
                }
            }

            if (enemiesToDamage[i].GetComponent<GorillaController>())
            {
                GorillaController gorillaController = enemiesToDamage[i].GetComponent<GorillaController>();
                bool isDead = gorillaController.TakeDamage(damage);
                if (isDead)
                {
                    StartCoroutine(scoreUI.GetComponent<ScoreController>().AddScore(gorillaController.scoreValue));
                }
            }
        }
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    private void PlayAttackSound()
    {
        _audioSource.volume = 0.7f;
        _audioSource.pitch = 1f;
        _audioSource.clip = attackClip;
        _audioSource.Play();
    }
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos.position, new Vector3(attackRangeX, attackRangeY, 1));
    }
}