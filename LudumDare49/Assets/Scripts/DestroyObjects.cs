using UnityEngine;

/// <summary>
/// Class <c>DestroyObjects</c> is a Unity component script used to manage the objects destroying behavior of the attached game object.
/// </summary>
public class DestroyObjects : MonoBehaviour
{
    /// <summary>
    /// This function is called on trigger exit event, sent when another object leaves a trigger collider attached to this object
    /// </summary>
    /// <param name="other">A Unity <c>Collider2D</c> component of the other game object involved in this collision.</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(other.GetComponent<PlayerController>().Death());
        }
        else
        {
            Destroy(other.transform.gameObject);
        }
    }
}
