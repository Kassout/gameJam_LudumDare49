using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuManager : MonoBehaviour
{
    public int MenuSceneIndex;
    public int GameSceneIndex;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);   
    }

    public void SwitchScene(int Index)
    {
        SceneManager.LoadScene(Index);
    }
}
