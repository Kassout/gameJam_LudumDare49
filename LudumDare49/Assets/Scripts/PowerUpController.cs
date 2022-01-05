using System.Collections;
using UnityEngine;

/// <summary>
/// TODO: comments
/// </summary>
public class PowerUpController : MonoBehaviour
{
    /// <summary>
    /// TODO: comments
    /// </summary>
    private AudioSource _audioSource;

    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// This function is called on trigger enter event, sent when another object entered a trigger collider attached to this object
    /// </summary>
    /// <param name="collision">A Unity <c>Collider2D</c> component of the other game object involved in this collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(CollectPowerUp(collision.gameObject));
        }
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <param name="player">TODO: comments</param>
    /// <returns>TODO: comments</returns>
    private IEnumerator CollectPowerUp(GameObject player)
    {
        _audioSource.Play();
        player.GetComponent<PlayerController>().TakePowerUp(1);
        gameObject.GetComponent<SpriteRenderer>().sprite = null;

        yield return new WaitForSeconds(0.15f);
        
        Destroy(gameObject);
    }
}
