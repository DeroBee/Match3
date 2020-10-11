using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    #region Public Fields

    public static GuiManager instance;
    public GameObject gameOverPanel;
    public Text highScoreText;
    public Text moveCounterText;
    public Text scoreText;
    public Text yourScoreText;

    #endregion Public Fields

    #region Private Fields

    private int moveCounter;
    private int score;

    #endregion Private Fields

    #region Public Properties

    public int MoveCounter
    {
        get
        {
            return moveCounter;
        }
        set
        {
            moveCounter = value;
            if (moveCounter <= 0)
            {
                moveCounter = 0;
                StartCoroutine(WaitForShifting());
            }
            moveCounterText.text = moveCounter.ToString();
        }
    }

    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            scoreText.text = score.ToString();
        }
    }

    #endregion Public Properties

    #region Private Methods

    // Start is called before the first frame update
    private void Awake()
    {
        instance = GetComponent<GuiManager>();

        MoveCounter = 60;
        Score = 0;
    }

    private void GameOver()
    {
        gameOverPanel.SetActive(true);
        if (Score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", Score);
            PlayerPrefs.Save();
            highScoreText.text = "New best: " + PlayerPrefs.GetInt("HighScore").ToString();
        }
        else
        {
            highScoreText.text = "Best: " + PlayerPrefs.GetInt("HighScore").ToString();
        }
        yourScoreText.text = Score.ToString();
    }

    private IEnumerator WaitForShifting()
    {
        yield return new WaitUntil(() => !BoardManager.instance.IsShifting);
        yield return new WaitForSeconds(.25f);
        GameOver();
    }

    #endregion Private Methods
}