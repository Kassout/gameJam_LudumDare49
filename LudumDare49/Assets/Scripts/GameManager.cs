using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// TODO: comments
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// TODO: comments
    /// </summary>
    public GameObject startingUI;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    public float transitionTime = 0.3f;

    /// <summary>
    /// TODO: comments
    /// </summary>
    private AudioSource _audioSource;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        Time.timeScale = 0;
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _audioSource.Play();
            StartCoroutine(StartGame());
        }
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <returns>TODO: comments</returns>
    IEnumerator StartGame()
    {
        startingUI.GetComponentInChildren<Animator>().SetTrigger("startGame");
        MusicPlayer.Instance.Switch();
        yield return new WaitForSecondsRealtime(transitionTime);
        Time.timeScale = 1;
        SceneManager.LoadScene(2);
    }
}
