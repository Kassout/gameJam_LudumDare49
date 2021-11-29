using UnityEngine;

/// <summary>
/// TODO: comments
/// </summary>
public class DestroyObjects : MonoBehaviour
{
    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <param name="other">TODO: comments</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        Destroy(other.gameObject);
    }
}
