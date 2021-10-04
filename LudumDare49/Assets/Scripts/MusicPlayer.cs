using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }

    public AudioSource source;

    public AudioClip currentClip;

    public AudioClip highIntensityClip;
    public AudioClip lowIntensityClip;
    public AudioClip transitionClip;
    public AudioClip startScreenClip;
    
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

    public void PlayClip(AudioClip toPlay)
    {
        StartCoroutine(PlaySound(toPlay,false));
    }

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