using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class <c>GameManager</c> is a Unity component script used to manage the general game behaviour.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Fields / Properties

    /// <summary>
    /// Instance field <c>startingUI</c> is a Unity <c>GameObject</c> representing the start screen UI.
    /// </summary>
    [SerializeField] private GameObject startingUI;
    
    /// <summary>
    /// Instance field <c>transitionTime</c> represents the duration value of the transition between two scenes.
    /// </summary>
    [SerializeField] private float transitionTime = 0.3f;

    /// <summary>
    /// Instance field <c>audioSource</c> is a Unity <c>AudioSource</c> component representing the game manager audio source.
    /// </summary>
    private AudioSource _audioSource;

    /// <summary>
    /// Instance field <c>isStarted</c> represents the started status of the game party.
    /// </summary>
    private bool isStarted;

    #endregion

    #region MonoBehavior

    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        Time.timeScale = 0;
    }

    /// <summary>
    /// This function is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        if (InputHandler.startInput && !isStarted)
        {
            isStarted = true;
            _audioSource.Play();
            StartCoroutine(StartGame());
        }
    }

    #endregion

    #region Private

    /// <summary>
    /// This function is responsible for managing the tasks related to the game starting.
    /// </summary>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
    private IEnumerator StartGame()
    {
        startingUI.GetComponentInChildren<Animator>().SetTrigger("startGame");
        MusicPlayer.Instance.Switch();
        yield return new WaitForSecondsRealtime(transitionTime);
        Time.timeScale = 1;
        SceneManager.LoadScene(2);
    }

    #endregion
}
