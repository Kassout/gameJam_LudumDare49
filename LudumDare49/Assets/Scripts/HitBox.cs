using UnityEngine;

/// <summary>
/// Class <c>HitBox</c> is a Unity component script used to manage the enemy hit boxes behavior
/// </summary>
public class HitBox : MonoBehaviour
{
    /// <summary>
    /// Instance field <c>origin</c> is a Unity <c>EnemyController</c> component script representing the enemy manager at the origin of the hit box.
    /// </summary>
    public EnemyController origin;

    /// <summary>
    /// This function is called on trigger enter event, sent when another object entered a trigger collider attached to this object
    /// </summary>
    /// <param name="collision">A Unity <c>Collider2D</c> component of the other game object involved in this collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<PlayerController>()._canTakeDamage)
            {
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(1);
                collision.gameObject.GetComponent<PlayerMovement>().Move(origin.directionOfPlayer.x * collision.gameObject.GetComponent<Rigidbody2D>().mass);
            }
        }
    }

    /// <summary>
    /// This function is called when the game shuts down, switches to another scene or when the related game object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        origin.attacking = false;
    }
}
