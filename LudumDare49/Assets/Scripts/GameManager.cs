using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject startingUI;
    public float transitionTime = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(StartGame());

        }
    }

    IEnumerator StartGame()
    {
        startingUI.GetComponentInChildren<Animator>().SetTrigger("startGame");
        yield return new WaitForSecondsRealtime(transitionTime);
        Time.timeScale = 1;
        startingUI.SetActive(false); 
    }
}
