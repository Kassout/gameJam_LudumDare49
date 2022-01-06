using System.Collections;
using UnityEngine;

/// <summary>
/// Class <c>PlayerAttack</c> is a Unity component script used to manage the player attack behaviour.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    #region Fields / Properties

    /// <summary>
    /// Instance field <c>playerController</c> is a Unity <c>PlayerController</c> component script representing the general player behavior manager.
    /// </summary>
    private PlayerController _playerController;
    
    /// <summary>
    /// Instance field <c>playerAnimator</c> is a Unity <c>Animator</c> component representing the player animations manager.
    /// </summary>
    private Animator _playerAnimator;
    
    /// <summary>
    /// Instance field <c>audioSource</c> is a Unity <c>AudioSource</c> component representing the player audio source for SFX playing.
    /// </summary>
    private AudioSource _audioSource;
    
    /// <summary>
    /// Instance field <c>attackClip</c> is a Unity <c>AudioClip</c> object representing the player attack audio sound.
    /// </summary>
    [SerializeField] private AudioClip attackClip;
    
    /// <summary>
    /// Instance field <c>scoreUI</c> is a Unity <c>GameObject</c> representing the game score UI.
    /// </summary>
    [SerializeField] private GameObject scoreUI;
    
    /// <summary>
    /// Instance field <c>damage</c> represents the quantity of damage inflict by the player on hit.
    /// </summary>
    [SerializeField] private int damage = 1;
    
    /// <summary>
    /// Instance field <c>attackRangeX</c> represents the value of the hit box boundary size over the x-axis.
    /// </summary>
    [SerializeField] private float attackRangeX = 1.0f;
    
    /// <summary>
    /// Instance field <c>attackRangeX</c> represents the value of the hit box boundary size over the y-axis.
    /// </summary>
    [SerializeField] private float attackRangeY = 1.0f;
    
    /// <summary>
    /// Instance field <c>attackPosition</c> is a Unity <c>Transform</c> component representing the position, rotation and scale of the player attack hit box.
    /// </summary>
    [SerializeField] private Transform attackPosition;
    
    /// <summary>
    /// Instance field <c>attackCooldownTime</c> represents the duration value between two enemy attacks.
    /// </summary>
    [SerializeField] private float attackCooldownTime = 0.7f;
    
    /// <summary>
    /// Instance field <c>attackCooldownTimeValue</c> represents the time before the last enemy attack.
    /// </summary>
    private float _attackCooldownTimeValue;
    
    /// <summary>
    /// Instance field <c>whatIsEnemies</c> is a Unity <c>LayerMask</c> structure representing the enemy layer in the scene.
    /// </summary>
    [SerializeField] private LayerMask whatIsEnemies;

    /// <summary>
    /// Instance field <c>AttackHash</c> represents the integer identifier of the string message "attack" for the player animator.
    /// </summary>
    private static readonly int AttackHash = Animator.StringToHash("attack");

    #endregion

    #region MonoBehavior

    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _playerAnimator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// This function is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        if (_attackCooldownTimeValue <= 0)
        {
            // Then you can attack
            if (InputHandler.attackInput && _playerController.isGrounded)
            {
                StartCoroutine(Attack());
                
                _attackCooldownTimeValue = attackCooldownTime;
            }
        }
        else
        {
            _attackCooldownTimeValue -= Time.deltaTime;
        }
    }

    #endregion

    #region Private

        /// <summary>
    /// This function is responsible for managing the attack behavior of the player.
    /// </summary>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
    private IEnumerator Attack()
    {
        _playerAnimator.SetTrigger(AttackHash);
        PlayAttackSound();

        yield return new WaitForSeconds(0.15f);
        Collider2D[] enemiesToDamage =
            Physics2D.OverlapBoxAll(attackPosition.position, new Vector2(attackRangeX, attackRangeY), whatIsEnemies);
        foreach (Collider2D other in enemiesToDamage)
        {
            if (other.GetComponent<EnemyController>())
            {
                EnemyController enemy = other.GetComponent<EnemyController>();
                bool isDead = enemy.TakeDamage(damage);
                if (isDead)
                {
                    StartCoroutine(scoreUI.GetComponent<ScoreController>().AddScore(enemy.scoreValue));
                }
            }

            if (other.GetComponent<GorillaController>())
            {
                GorillaController gorillaController = other.GetComponent<GorillaController>();
                bool isDead = gorillaController.TakeDamage(damage);
                if (isDead)
                {
                    StartCoroutine(scoreUI.GetComponent<ScoreController>().AddScore(gorillaController.scoreValue));
                }
            }
        }
    }

    /// <summary>
    /// This function is responsible for playing the player attack SFX when called.
    /// </summary>
    private void PlayAttackSound()
    {
        _audioSource.volume = 0.7f;
        _audioSource.pitch = 1f;
        _audioSource.clip = attackClip;
        _audioSource.Play();
    }
    
    /// <summary>
    /// This function is responsible for drawing a red cube representing the attack hit box, when called.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPosition.position, new Vector3(attackRangeX, attackRangeY, 1));
    }

    #endregion
}