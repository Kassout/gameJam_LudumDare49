using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public EnemyController origin;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<PlayerController>()._canTakeDamage == true)
            {
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(1);
                collision.gameObject.GetComponent<PlayerMovement>().Move(origin._directionOfPlayer.x * collision.gameObject.GetComponent<Rigidbody2D>().mass);
            }
        }
    }

    private void OnDestroy()
    {
        origin.attacking = false;
    }
}
