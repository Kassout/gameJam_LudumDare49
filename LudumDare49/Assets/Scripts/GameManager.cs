using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject startingUI;
    public float transitionTime = 0.3f;

    private AudioSource _audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _audioSource.Play();
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        startingUI.GetComponentInChildren<Animator>().SetTrigger("startGame");
        MusicPlayer.Instance.Switch();
        yield return new WaitForSecondsRealtime(transitionTime);
        Time.timeScale = 1;
        SceneManager.LoadScene(2);
    }
}
