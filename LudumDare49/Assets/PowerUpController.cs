using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakePowerUp(1);
            Destroy(gameObject);
        }
    }
}
