using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    [SerializeField] private float scoreDelay = 10.0f;
    [SerializeField] private float scoreIncrement = 10.0f;

    private Text scoreUI;
    private int _score = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        scoreUI = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
        
        scoreUI.text = _score.ToString();
    }
}
