using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class <c>MusicPlayer</c> is a Unity component script used to manage the general music plays behavior.
/// </summary>
public class MusicPlayer : MonoBehaviour
{
    #region Fields / Properties

    /// <summary>
    /// Singleton instance property.
    /// </summary>
    public static MusicPlayer Instance { get; private set; }

    /// <summary>
    /// Instance field <c>audioSource</c> is a Unity <c>AudioSource</c> component representing the music player audio source.
    /// </summary>
    private AudioSource _audioSource;

    /// <summary>
    /// Instance field <c>currentClip</c> is a Unity <c>AudioClip</c> structure representing the currently playing audio clip on the music player.
    /// </summary>
    [SerializeField] private AudioClip currentClip;

    /// <summary>
    /// Instance field <c>highIntensityClip</c> is a Unity <c>AudioClip</c> structure representing the high intensity theme clip.
    /// </summary>
    [SerializeField] private AudioClip highIntensityClip;
    
    /// <summary>
    /// Instance field <c>lowIntensityClip</c> is a Unity <c>AudioClip</c> structure representing the low intensity theme clip.
    /// </summary>
    [SerializeField] private AudioClip lowIntensityClip;
    
    /// <summary>
    /// Instance field <c>transitionClip</c> is a Unity <c>AudioClip</c> structure representing the transition sound clip.
    /// </summary>
    [SerializeField] private AudioClip transitionClip;
    
    /// <summary>
    /// Instance field <c>startScreenClip</c> is a Unity <c>AudioClip</c> structure representing the start screen sound clip.
    /// </summary>
    public AudioClip startScreenClip;

    #endregion

    #region MonoBehavior

    /// <summary>
    /// This function is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        // Singleton
        if (null == Instance)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Warning: multiple " + this + " in scene!");
        }

        _audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(1);
    }

    #endregion

    #region Private

        /// <summary>
    /// This function is responsible for playing a given audio clip with or without transition audio clip and with or without volume transition.
    /// </summary>
    /// <param name="toPlay">A Unity <c>AudioClip</c> structure representing the audio clip to play by the music player.</param>
    /// <param name="transition">A boolean value representing the transition status of the audio clip.</param>
    /// <param name="volumeTransition">A boolean value representing the volume transition status of the audio clip.</param>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
    private IEnumerator PlaySound(AudioClip toPlay, bool transition, bool volumeTransition = true)
    {
        currentClip = toPlay;
        
        if (volumeTransition)
        {
            float startVolume = _audioSource.volume;
 
            while (_audioSource.volume > 0) {
                _audioSource.volume -= startVolume * Time.unscaledDeltaTime / 1.5f;
 
                yield return null;
            }
            _audioSource.Stop();
            _audioSource.volume = startVolume;
        }

        if (transition)
        {
            yield return null;
            //2.Assign current AudioClip to audiosource
            _audioSource.clip = transitionClip;

            //3.Play Audio
            _audioSource.Play();

            bool isTransitioning = false;
            //4.Wait for it to finish playing
            while (_audioSource.isPlaying && _audioSource.time < 6.0f)
            {
                _audioSource.loop = false;
                yield return null;
                
                if (_audioSource.time > 4.0f && !isTransitioning)
                {
                    isTransitioning = true;
                    float startVolume = _audioSource.volume;
 
                    while (_audioSource.volume > 0) {
                        _audioSource.volume -= startVolume * Time.unscaledDeltaTime / 2.0f;
 
                        yield return null;
                    }
                    _audioSource.Stop();
                    _audioSource.volume = startVolume;
                }
            }
        }

        _audioSource.loop = true;
        _audioSource.clip = toPlay;
        _audioSource.Play();

        yield return null;
    }

    #endregion

    #region Public

    /// <summary>
    /// This function is responsible for switching audio clip currently playing.
    /// </summary>
    /// <param name="volumeTransition">A boolean value representing the volume transition status of the switching process of the music player.</param>
    public void Switch(bool volumeTransition = true)
    {
        if(currentClip == lowIntensityClip)
        {
            StartCoroutine(PlaySound(highIntensityClip,false, volumeTransition));
        }
        else if (currentClip == highIntensityClip)
        {
            StartCoroutine(PlaySound(lowIntensityClip,true, volumeTransition));
        }
        else
        {
            StartCoroutine(PlaySound(lowIntensityClip,false, volumeTransition));
        }
    }

    /// <summary>
    /// This function is responsible for playing a given audio clip.
    /// </summary>
    /// <param name="toPlay">A Unity <c>AudioClip</c> structure representing the audio clip to play by the music player.</param>
    public void PlayClip(AudioClip toPlay)
    {
        StartCoroutine(PlaySound(toPlay,false));
    }

    #endregion
}