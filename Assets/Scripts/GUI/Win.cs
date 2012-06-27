using UnityEngine;
using System.Collections;
/// <summary>
/// Win.cs
/// 2012-06-27
/// 
/// Win menu screen manager. Handles the display of score and text on the "Win" scene.
/// </summary>
public class Win : MonoBehaviour {

    #region Fields
    // These public game objects are set in the Editor/Inspector. 
    // Location: UI Root (Win Menu)
    public UILabel scoreValueLabel;
    public UILabel healthValueLabel;
    public UILabel totalScoreLabel;
    #endregion

    #region Functions
    void Start() {
        scoreValueLabel.text = Player.score.ToString();
        healthValueLabel.text = Player.health.ToString();
        totalScoreLabel.text = "[AADDAA]" + CalculatedScore;
    }
    /// <summary>
    /// Returns the final calculated score after the end of the game.
    /// </summary>
    /// <value>Calculated score (Health * Score * 0.1)</value>
    int CalculatedScore {
        get {
            if (Player.health < 1)
                return Player.score;
            else
                return (int)(Player.health * 0.1 * Player.score);
        }
    }

    /// <summary>
    /// Button handler for the "Play Again!" button
    /// </summary>
    void OnPlayAgainClicked() {
        Application.LoadLevel("Game");
    }

    /// <summary>
    /// Button handler for the "Back to the Menu" button
    /// </summary>
    void OnMainMenuClicked() {
        Application.LoadLevel("MainMenu");
    }
    #endregion
}
