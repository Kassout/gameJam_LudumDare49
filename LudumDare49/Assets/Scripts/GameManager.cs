using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject startingUI;
    
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
            Time.timeScale = 1;
            startingUI.SetActive(false);
        }
    }
}
