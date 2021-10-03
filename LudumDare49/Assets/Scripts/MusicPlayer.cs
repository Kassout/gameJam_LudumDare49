using System.Collections;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource source;

    public AudioClip currentClip;

    public AudioClip highIntensityClip;
    public AudioClip lowIntensityClip;
    public AudioClip transitionClip;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Switch()
    {
        if(currentClip == lowIntensityClip)
        {
            StartCoroutine(PlaySound(highIntensityClip,false));
            currentClip = highIntensityClip;
        }
        else if (currentClip == highIntensityClip)
        {
            StartCoroutine(PlaySound(lowIntensityClip,true));
            highIntensityClip = lowIntensityClip;
        }
        else
        {
            StartCoroutine(PlaySound(lowIntensityClip,false));
            currentClip = lowIntensityClip;
        }
    }

    public IEnumerator PlaySound(AudioClip toPlay, bool transition)
    {
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
