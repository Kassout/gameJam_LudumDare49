using UnityEngine;

/// <summary>
/// TODO: comments
/// </summary>
public class GorillaFistDamage : MonoBehaviour
{
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private int damage = 1;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float forceAmplitude = 150.0f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <param name="collision">TODO: comments</param>
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
