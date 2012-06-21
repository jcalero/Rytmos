using UnityEngine;
using System.Collections;

public class Win : MonoBehaviour
{

    #region Fields
    UILabel scoreLabel;
    UILabel healthLabel;
    UILabel totalScoreLabel;
    #endregion

    #region Functions

    void Awake()
    {
        scoreLabel = GameObject.Find("LevelScoreValue").GetComponent<UILabel>();
        healthLabel = GameObject.Find("HealthValue").GetComponent<UILabel>();
        totalScoreLabel = GameObject.Find("TotalScoreValue").GetComponent<UILabel>();
    }

    void Update()
    {
        scoreLabel.text = Player.score.ToString();
        healthLabel.text = Player.health.ToString();
        totalScoreLabel.text = "[AADDAA]" + CalculatedHealth;
    }

    int CalculatedHealth
    {
        get
        {
            if (Player.health < 1)
                return Player.score;
            else
                return (int)(Player.health * 0.1 * Player.score);
        }
    }

    void OnPlayAgainClicked()
    {
        Application.LoadLevel("Game");
    }

    void OnMainMenuClicked()
    {
        Application.LoadLevel("MainMenu");
    }

    #endregion
}
