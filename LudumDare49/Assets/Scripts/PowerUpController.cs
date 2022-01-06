using System.Collections;
using UnityEngine;

/// <summary>
/// Class <c>PowerUpController</c> is a Unity component script used to manage the power up behavior.
/// </summary>
public class PowerUpController : MonoBehaviour
{
    #region Fields / Properties

    /// <summary>
    /// Instance field <c>audioSource</c> is a Unity <c>AudioSource</c> component representing the power up audio source for SFX playing.
    /// </summary>
    private AudioSource _audioSource;

    #endregion

    #region MonoBehavior

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

    #endregion

    #region Private

    /// <summary>
    /// This function is responsible for managing the collect power up behavior.
    /// </summary>
    /// <param name="player">A Unity <c>GameObject</c> representing the player game object.</param>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
    private IEnumerator CollectPowerUp(GameObject player)
    {
        _audioSource.Play();
        player.GetComponent<PlayerController>().TakePowerUp(1);
        GameObject powerUpGameObject;
        (powerUpGameObject = gameObject).GetComponent<SpriteRenderer>().sprite = null;

        yield return new WaitForSeconds(0.15f);
        
        Destroy(powerUpGameObject);
    }

    #endregion
}
