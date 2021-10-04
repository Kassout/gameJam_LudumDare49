using System;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damage = 1;
    
    [SerializeField] private float attackRangeX = 1.0f;
    [SerializeField] private float attackRangeY = 1.0f;
    [SerializeField] private Transform attackPos;
    [SerializeField] private float startTimeBtwAttack;
    [SerializeField] private LayerMask whatIsEnemies;
    [SerializeField] private GameObject scoreUI;
    
    private float _timeBtwAttack = 0.0f;
    private PlayerController _playerController;
    private Animator _playerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_timeBtwAttack <= 0)
        {
            // Then you can attack
            if (Input.GetButtonDown("Fire1") && _playerController.isGrounded)
            { 
                _playerAnimator.SetTrigger("attack");
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
                
                _timeBtwAttack = startTimeBtwAttack;
            }
        }
        else
        {
            _timeBtwAttack -= Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos.position, new Vector3(attackRangeX, attackRangeY, 1));
    }
}