using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicPlayer : MonoBehaviour
{
    public AudioSource Source;

    public AudioClip currentClip;

    public AudioClip HighIntensityClip;
    public AudioClip LowIntensityClip;
    public AudioClip TransitionClip;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Switch()
    {
        if(currentClip == LowIntensityClip)
        {
            StartCoroutine(playSound(HighIntensityClip,false));
            currentClip = HighIntensityClip;
        }
        else if (currentClip == HighIntensityClip)
        {
            StartCoroutine(playSound(LowIntensityClip,true));
            HighIntensityClip = LowIntensityClip;
        }
        else
        {
            StartCoroutine(playSound(LowIntensityClip,false));
            currentClip = LowIntensityClip;
        }
    }

    IEnumerator playSound(AudioClip toPlay,bool transition)
    {
        if (transition)
        {
            yield return null;
            //2.Assign current AudioClip to audiosource
            Source.clip = TransitionClip;

            //3.Play Audio
            Source.Play();

            //4.Wait for it to finish playing
            while (Source.isPlaying)
            {
                Source.loop = false;
                yield return null;
            }
        }

        Source.loop = true;
        Source.clip = toPlay;
        Source.Play();

        yield return null;
    }
}
