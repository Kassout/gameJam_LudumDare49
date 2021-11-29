using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// TODO: comments
/// </summary>
public class EndSceneManager : MonoBehaviour
{
    /// <summary>
    /// TODO: comments
    /// </summary>
    private void Start()
    {
        StartCoroutine(EndGame());
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <returns>TODO: comments</returns>
    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(1);
    }
}
