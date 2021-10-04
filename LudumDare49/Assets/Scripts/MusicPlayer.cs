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

    public void Switch()
    {
        if(currentClip == lowIntensityClip)
        {
            StartCoroutine(PlaySound(highIntensityClip,false));
        }
        else if (currentClip == highIntensityClip)
        {
            StartCoroutine(PlaySound(lowIntensityClip,true));
        }
        else
        {
            StartCoroutine(PlaySound(lowIntensityClip,false));
        }
    }

    public void PlayClip(AudioClip toPlay)
    {
        StartCoroutine(PlaySound(toPlay,false));
    }

    public IEnumerator PlaySound(AudioClip toPlay, bool transition)
    {
        currentClip = toPlay;
        
        float startVolume = source.volume;
 
        while (source.volume > 0) {
            source.volume -= startVolume * Time.unscaledDeltaTime / 1.5f;
 
            yield return null;
        }
        source.Stop();
        source.volume = startVolume;
        
        if (transition)
        {
            yield return null;
            //2.Assign current AudioClip to audiosource
            source.clip = transitionClip;

            //3.Play Audio
            source.Play();

            //4.Wait for it to finish playing
            while (source.isPlaying)
            {
                source.loop = false;
                yield return null;
            }
        }

        source.loop = true;
        source.clip = toPlay;
        source.Play();

        yield return null;
    }
}
