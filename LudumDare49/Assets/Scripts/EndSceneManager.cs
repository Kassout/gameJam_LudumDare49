using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(MusicPlayer.Instance.PlaySound(MusicPlayer.Instance.startScreenClip, false));
        StartCoroutine(EndGame());
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(1);
    }
}
