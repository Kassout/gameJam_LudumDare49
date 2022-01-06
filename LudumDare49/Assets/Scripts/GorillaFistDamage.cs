using UnityEngine;

/// <summary>
/// Class <c>GorillaFistDamage</c> is a Unity component script used to manage the gorilla enemy fist behaviour.
/// </summary>
public class GorillaFistDamage : MonoBehaviour
{
    #region Fields / Properties

    /// <summary>
    /// Instance field <c>damage</c> represents the quantity of damage received by the player on hit.
    /// </summary>
    [SerializeField] private int damage = 1;
    
    /// <summary>
    /// Instance field <c>forceAmplitude</c> represents the force magnitude value of the gorilla attack smash impact over the player.
    /// </summary>
    [SerializeField] private float forceAmplitude = 150.0f;

    #endregion

    #region MonoBehavior

    /// <summary>
    /// This function is called on trigger enter event, sent when another object entered a trigger collider attached to this object
    /// </summary>
    /// <param name="collision">A Unity <c>Collider2D</c> component of the other game object involved in this collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<PlayerController>().canTakeDamage)
            {
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * forceAmplitude, ForceMode2D.Impulse);
            }
        }
    }

    #endregion
}
