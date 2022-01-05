using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// TODO: comments
/// </summary>
public class ScoreController : MonoBehaviour
{
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float scoreDelay = 10.0f;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private int scoreIncrement = 10;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    [SerializeField] private float timeUntilScoreIncrementIncrease = 30.0f;

    /// <summary>
    /// TODO: comments
    /// </summary>
    private Text _scoreUI;
    
    /// <summary>
    /// TODO: comments
    /// </summary>
    private int _score = 0;
    
    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        _scoreUI = GetComponent<Text>();

        StartCoroutine(ComputeScore(scoreDelay));
        StartCoroutine(ScoreUpdate());
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <param name="scoreIncreaseDelay">TODO: comments</param>
    /// <returns>TODO: comments</returns>
    private IEnumerator ComputeScore(float scoreIncreaseDelay)
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

    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <returns>TODO: comments</returns>
    private IEnumerator ScoreUpdate()
    {
        int displayScore = int.Parse(_scoreUI.text.Split(':')[1]);
        while (true)
        {
            if (displayScore != _score)
            {
                if (displayScore > 100 && displayScore + 10 < _score)
                {
                    displayScore += 10;
                }
                else
                {
                    displayScore++;
                }
                
                _scoreUI.text = "Score : " + displayScore.ToString("D7");
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    /// <summary>
    /// TODO: comments
    /// </summary>
    /// <param name="amount">TODO: comments</param>
    /// <returns>TODO: comments</returns>
    public IEnumerator AddScore(int amount)
    {
        _score += amount;
        yield return null;
    }
}
