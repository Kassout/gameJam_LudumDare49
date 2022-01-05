using UnityEngine;

/// <summary>
/// Class <c>EffectsController</c> is a Unity component script used to manage the general enemy gorilla death effects behavior.
/// </summary>
public class EffectsController : MonoBehaviour
{
    /// <summary>
    /// This function is responsible for destroying the effects when called.
    /// </summary>
    private void DestroyEffects()
    {
        Destroy(gameObject);
    }
}
