using UnityEngine;
using System.Collections;
/// <summary>
/// MainMenu.cs
/// 
/// Handles the display of the main menu. 
/// </summary>
public class MainMenu : MonoBehaviour {

    #region Fields
    public UIDraggablePanel panel; // Inspector instance. Location: MainMenuManager.
    public UIButton reloadButton;
    public UIButton nextButton;
    public UIButton prevButton;
    public UILabel highscoresTypeLabel;
    public bool skipMenu;           // Allows you to skip directly from the Main Menu to the Arcade game mode

    // Values for menu slider locations.
    private Vector3 quitMenu;
    private Vector3 mainMenu;
    private Vector3 modeMenu;
    private Vector3 highScoresMenu;
    private Vector3 optionsMenu;
    private bool highscoresLoaded;

    private bool loadedSurvival;
    private bool loadedTimeAttack = true;
    private string survival = "rytmos_hs_dm";
    private string timeAttack = "rytmos_hs_30sec";

    private static MainMenu instance;
    #endregion

    #region Functions

    void Awake() {
        if (skipMenu) Application.LoadLevel("Game");

        instance = this;
        quitMenu = new Vector3(0f, 0f, 0f);
        mainMenu = new Vector3(-650f, 0f, 0f);
        modeMenu = new Vector3(-650f * 2, 0f, 0f);
        highScoresMenu = new Vector3(-650f * 3, 0f, 0f);
        optionsMenu = new Vector3(-650f * 4, 0f, 0f);

        Game.GameState = Game.State.Menu;
    }

    void Start() {
        // Fetch high-scores from server
        if (!highscoresLoaded) {
            if (loadedSurvival) {
                StartCoroutine(HSController.GetScores(survival, true));
                StartCoroutine(HSController.GetScores(timeAttack, false));
            }
            if (loadedTimeAttack) {
                StartCoroutine(HSController.GetScores(survival, false));
                StartCoroutine(HSController.GetScores(timeAttack, true));
            }
            highscoresLoaded = true;
            DisableReloadButton();
        }
    }

    void Update() {
        // When the player presses "Escape" or "Back" on Android, returns to main menu screen.
        if (Input.GetKey(KeyCode.Escape))
            OnBackClicked();
    }

    /// <summary>
    /// Button handler for "Play" button
    /// </summary>
    void OnPlayClicked() {
        // Stop any current momentum to allow for Spring to begin.
        panel.currentMomentum = Vector3.zero;
        // Begin spring motion
        SpringPanel.Begin(panel.gameObject, modeMenu, 13f);
    }

    /// <summary>
    /// Button handler for "Highscores" button
    /// </summary>
    void OnHighScoresClicked() {
        // Stop any current momentum to allow for Spring to begin.
        panel.currentMomentum = Vector3.zero;
        // Begin spring motion
        SpringPanel.Begin(panel.gameObject, highScoresMenu, 13f);
    }

    /// <summary>
    /// Button handler for "Options" button
    /// </summary>
    void OnOptionsClicked() {
        // Stop any current momentum to allow for Spring to begin.
        panel.currentMomentum = Vector3.zero;
        // Begin spring motion
        SpringPanel.Begin(panel.gameObject, optionsMenu, 13f);
    }

    /// <summary>
    /// Button handler for "Quit Game" button
    /// </summary>
    void OnQuitClicked() {
        // Stop any current momentum to allow for Spring to begin.
        panel.currentMomentum = new Vector3(0f, 0f, 0f);
        // Begin spring motion
        SpringPanel.Begin(panel.gameObject, quitMenu, 13f);
    }

    /// <summary>
    /// Button handler or "Back" button
    /// </summary>
    void OnBackClicked() {
        // Stop any current momentum to allow for Spring to begin.
        panel.currentMomentum = Vector3.zero;
        // Begin spring motion
        SpringPanel.Begin(panel.gameObject, mainMenu, 13f);
    }

    /// <summary>
    /// Button handler for "Yes, Quit Game" button
    /// </summary>
    void OnQuitConfirmedClicked() {
        Application.Quit();
    }

    /// <summary>
    /// Button handler for "Arcade" button
    /// </summary>
    void OnArcadeModeClicked() {
        Application.LoadLevel("Game");
    }

    void OnChallengeModeClicked() {
        Application.LoadLevel("DeathMatch");
    }

    void OnNextHighscoreClicked() {
        if (loadedSurvival) {
            highscoresTypeLabel.text = "Time Attack";
            loadedTimeAttack = true;
            loadedSurvival = false;
            HSController.ShowScores(timeAttack);
        } else {
            highscoresTypeLabel.text = "Survival";
            loadedSurvival = true;
            loadedTimeAttack = false;
            HSController.ShowScores(survival);
        }
    }

    void OnPrevHighscoreClicked() {
        if (loadedSurvival) {
            highscoresTypeLabel.text = "Time Attack";
            loadedTimeAttack = true;
            loadedSurvival = false;
            HSController.ShowScores(timeAttack);
        } else {
            highscoresTypeLabel.text = "Survival";
            loadedSurvival = true;
            loadedTimeAttack = false;
            HSController.ShowScores(survival);
        }
    }

    void OnReloadClicked() {
        DisableReloadButton();
        if (loadedSurvival)
            StartCoroutine(HSController.GetScores(survival, true));
        if (loadedTimeAttack)
            StartCoroutine(HSController.GetScores(timeAttack, true));
    }

    public static void DisableReloadButton() {
        instance.reloadButton.isEnabled = false;
        instance.nextButton.isEnabled = false;
        instance.prevButton.isEnabled = false;
    }

    public static void EnableReloadButton() {
        instance.reloadButton.isEnabled = true;
        instance.nextButton.isEnabled = true;
        instance.prevButton.isEnabled = true;
    }
    #endregion
}
