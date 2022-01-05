using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class <c>EndSceneManager</c> is a Unity component script used to manage the end scene behavior.
/// </summary>
public class EndSceneManager : MonoBehaviour
{
    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        StartCoroutine(EndGame());
    }

    /// <summary>
    /// This function is responsible for loading the main menu scene after the end game displayed for several seconds.
    /// </summary>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(1);
    }
}
