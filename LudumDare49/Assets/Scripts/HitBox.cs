using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO: comments
/// </summary>
public class HitBox : MonoBehaviour
{
    /// <summary>
    /// TODO: comments
    /// </summary>
    public EnemyController origin;

    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<PlayerController>()._canTakeDamage)
            {
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(1);
                collision.gameObject.GetComponent<PlayerMovement>().Move(origin._directionOfPlayer.x * collision.gameObject.GetComponent<Rigidbody2D>().mass);
            }
        }
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    private void OnDestroy()
    {
        origin.attacking = false;
    }
}
