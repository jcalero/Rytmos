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
    public UIInput nameInput;
    public UIButton submitButton;
    public UILabel submittedLabel;
    public UILabel errorLabel;

    private static Win instance;
    #endregion

    #region Functions
    void Start() {
        instance = this;
        scoreValueLabel.text = Player.score.ToString();
        healthValueLabel.text = Player.health.ToString();
        totalScoreLabel.text = "[AADDAA]" + CalculatedScore;
        if (Game.Cheated || Player.maxHealth > 100) {
            HideSubmitBox();
            errorLabel.transform.localPosition = new Vector3(errorLabel.transform.localPosition.x, errorLabel.transform.localPosition.y + 30, errorLabel.transform.localPosition.z);
            errorLabel.text = "[FF2222]You cheater!";
        }
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

    /// <summary>
    /// Button and input text handler for submitting highscore.
    /// </summary>
    void OnSubmit() {
        errorLabel.text = "";                                   // Clear error text if any
        string playerName = nameInput.text;                     // Set input box text to playerName
        if (playerName.Length < 2) {                            // Error if input text is too short
            nameInput.text = "";
            errorLabel.text = "[F87431]Too short. Try again";
            return;
        }
        HideSubmitBox();                                        // Hide the submit box and button
        StartCoroutine(HSController.PostScores(playerName, CalculatedScore));       // Submit the highscore
    }

    void HideSubmitBox() {
        submitButton.isEnabled = false;
        submitButton.enabled = false;
        submitButton.GetComponentInChildren<UISlicedSprite>().enabled = false;
        submitButton.GetComponentInChildren<UILabel>().enabled = false;
        nameInput.GetComponentInChildren<UISlicedSprite>().enabled = false;
        nameInput.GetComponentsInChildren<UILabel>()[1].enabled = false;
    }

    public static void ShowSubmitBox() {
        instance.submitButton.isEnabled = true;
        instance.submitButton.enabled = true;
        instance.submitButton.GetComponentInChildren<UISlicedSprite>().enabled = true;
        instance.submitButton.GetComponentInChildren<UILabel>().enabled = true;
        instance.nameInput.GetComponentInChildren<UISlicedSprite>().enabled = true;
        instance.nameInput.GetComponentsInChildren<UILabel>()[1].enabled = true;
    }
    #endregion
}
