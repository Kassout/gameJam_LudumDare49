using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// TODO: comments
/// </summary>
public class MusicPlayer : MonoBehaviour
{
    /// <summary>
    /// TODO: comments
    /// </summary>
    public static MusicPlayer Instance { get; private set; }

    /// <summary>
    /// TODO: comments
    /// </summary>
    public AudioSource source;

    /// <summary>
    /// TODO: comments
    /// </summary>
    public AudioClip currentClip;

    /// <summary>
    /// TODO: comments
    /// </summary>
    public AudioClip highIntensityClip;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    public AudioClip lowIntensityClip;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    public AudioClip transitionClip;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    public AudioClip startScreenClip;
    
    /// <summary>
    /// TODO: comments
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
        
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <param name="volumeTransition">TODO: comments</param>
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
    /// TODO: comments
    /// </summary>
    /// <param name="toPlay">TODO: comments</param>
    public void PlayClip(AudioClip toPlay)
    {
        StartCoroutine(PlaySound(toPlay,false));
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <param name="toPlay">TODO: comments</param>
    /// <param name="transition">TODO: comments</param>
    /// <param name="volumeTransition">TODO: comments</param>
    /// <returns>TODO: comments</returns>
    public IEnumerator PlaySound(AudioClip toPlay, bool transition, bool volumeTransition = true)
    {
        currentClip = toPlay;
        
        if (volumeTransition)
        {
            float startVolume = source.volume;
 
            while (source.volume > 0) {
                source.volume -= startVolume * Time.unscaledDeltaTime / 1.5f;
 
                yield return null;
            }
            source.Stop();
            source.volume = startVolume;
        }

        if (transition)
        {
            yield return null;
            //2.Assign current AudioClip to audiosource
            source.clip = transitionClip;

            //3.Play Audio
            source.Play();

            bool isTransitioning = false;
            //4.Wait for it to finish playing
            while (source.isPlaying && source.time < 6.0f)
            {
                source.loop = false;
                yield return null;
                
                if (source.time > 4.0f && !isTransitioning)
                {
                    isTransitioning = true;
                    float startVolume = source.volume;
 
                    while (source.volume > 0) {
                        source.volume -= startVolume * Time.unscaledDeltaTime / 2.0f;
 
                        yield return null;
                    }
                    source.Stop();
                    source.volume = startVolume;
                }
            }
        }

        source.loop = true;
        source.clip = toPlay;
        source.Play();

        yield return null;
    }
}