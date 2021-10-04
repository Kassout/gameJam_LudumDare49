using UnityEngine;

public class GorillaFistDamage : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float forceAmplitude = 150.0f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<PlayerController>()._canTakeDamage)
            {
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * forceAmplitude, ForceMode2D.Impulse);
            }
        }
    }
}
