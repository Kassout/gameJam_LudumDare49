using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class <c>ScoreController</c> is a Unity component script used to manage the score computation behavior.
/// </summary>
public class ScoreController : MonoBehaviour
{
    #region Fields / Properties

    /// <summary>
    /// Instance field <c>scoreUI</c> is a Unity <c>Text</c> component representing the score UI game object.
    /// </summary>
    private Text _scoreUI;
    
    /// <summary>
    /// Instance field <c>naturalScoreGrowthTickDelay</c> represents the duration between two natural score growth.
    /// </summary>
    [SerializeField] private float naturalScoreGrowthTickDelay = 5.0f;
    
    /// <summary>
    /// Instance field <c>naturalScoreGrowthIncrement</c> represents the natural increment value of the score at each score delay tick.
    /// </summary>
    [SerializeField] private int naturalScoreGrowthIncrement = 10;
    
    /// <summary>
    /// Instance field <c>timeUntilScoreIncrementIncrease</c> represents the duration value between two score growth increment increase.
    /// </summary>
    [SerializeField] private float timeUntilScoreIncrementIncrease = 30.0f;

    /// <summary>
    /// Instance field <c>score</c> represents the actual player score.
    /// </summary>
    private int _score;

    #endregion

    #region MonoBehavior

    /// <summary>
    /// This function is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    /// </summary>
    private void Start()
    {
        _scoreUI = GetComponent<Text>();

        StartCoroutine(ComputeScore(naturalScoreGrowthTickDelay));
        StartCoroutine(ScoreUpdate());
    }

    #endregion

    #region Private

    /// <summary>
    /// This function is responsible for managing the score computation behavior.
    /// </summary>
    /// <param name="scoreIncreaseDelay">A float value representing the duration between to score increment computation.</param>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
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
                _score += naturalScoreGrowthIncrement;
            }
            
            // Should the score increment increase ?
            if (scoreIncrementCountdown < 0 && scoreIncreaseDelay > 1)
            {
                scoreIncrementCountdown += timeUntilScoreIncrementIncrease;
                naturalScoreGrowthIncrement *= 2;
            }
        }
    }

    /// <summary>
    /// This function is responsible for manager the score updating behavior on display.
    /// </summary>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
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

    #endregion

    #region Public

    /// <summary>
    /// This function is responsible for managing the adding score behavior.
    /// </summary>
    /// <param name="amount">An integer value representing the point quantity to add to the current score.</param>
    /// <returns>A <c>IEnumerator</c> interface representing a list of controls regarding the iteration of the list of current running/called coroutine functions.</returns>
    public IEnumerator AddScore(int amount)
    {
        _score += amount;
        yield return null;
    }

    #endregion
}
