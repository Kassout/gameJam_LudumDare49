using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    [SerializeField] private float scoreDelay = 10.0f;
    [SerializeField] private int scoreIncrement = 10;
    [SerializeField] private float timeUntilScoreIncrementIncrease = 30.0f;

    private Text _scoreUI;
    private int _score = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _scoreUI = GetComponent<Text>();

        StartCoroutine(ComputeScore(scoreDelay));
        StartCoroutine(ScoreUpdate());
    }

    IEnumerator ComputeScore(float scoreIncreaseDelay)
    {
        float scoreIncrementCountdown = timeUntilScoreIncrementIncrease;
        float scoreCountDown = scoreIncreaseDelay;
        while (true)
        {
            yield return null;

            scoreIncrementCountdown -= Time.deltaTime;
            scoreCountDown -= Time.deltaTime;
            
            // Should the score increase ?
            if (scoreCountDown < 0)
            {
                scoreCountDown += scoreIncreaseDelay;
                _score += scoreIncrement;
            }
            
            // Should the score increment increase ?
            if (scoreIncrementCountdown < 0 && scoreIncreaseDelay > 1)
            {
                scoreIncrementCountdown += timeUntilScoreIncrementIncrease;
                scoreIncrement *= 2;
            }
        }
    }

    IEnumerator ScoreUpdate()
    {
        int displayScore = int.Parse(_scoreUI.text.Split(':')[1]);
        while (true)
        {
            if (displayScore != _score)
            {
                displayScore++;
                _scoreUI.text = "Score : " + displayScore.ToString("D7");
            }
            yield return new WaitForSeconds(0.05f);
        }
    }
}
