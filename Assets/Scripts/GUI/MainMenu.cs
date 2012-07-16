using UnityEngine;
using System.Collections;
using System.IO;
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
	public UIButton ArcadeButton;
	public UIButton SurvivalButton;
	public UIButton StoryButton;
	public UIButton BackModeButton;
	public UIPanel FileBrowserPanel;
	public UILabel highscoresTypeLabel;
	public UILabel selectModeLabel;

	public UILabel[] top5names;
	public UILabel[] top5scores;
	public UILabel[] close5names;
	public UILabel[] close5scores;

	// Values for menu slider locations.
	private Vector3 quitMenu;
	private Vector3 mainMenu;
	private Vector3 modeMenu;
	private Vector3 highScoresMenu;
	private Vector3 optionsMenu;
	private GameObject fb;

	private bool highscoresLoaded;
	private bool loadedSurvival;
	private bool loadedTimeAttack = true;
	private string survival = "rytmos_hs_dm";
	private string timeAttack = "rytmos_hs_30sec";

	private static MainMenu instance;

	public static string ScoresString;
	#endregion

	#region Functions

	void Awake() {
		instance = this;
		quitMenu = new Vector3(0f, 0f, 0f);
		mainMenu = new Vector3(-650f, 0f, 0f);
		modeMenu = new Vector3(-650f * 2, 0f, 0f);
		highScoresMenu = new Vector3(-650f * 3, 0f, 0f);
		optionsMenu = new Vector3(-650f * 4, 0f, 0f);
		fb = GameObject.Find("FileBrowser");

		Game.GameState = Game.State.Menu;
	}

	void Start() {
		// Fetch high-scores from server
		//if (!highscoresLoaded) {
		//    if (loadedSurvival) {
		//        StartCoroutine(HSController.GetScores(survival, true));
		//        StartCoroutine(HSController.GetScores(timeAttack, false));
		//    }
		//    if (loadedTimeAttack) {
		//        StartCoroutine(HSController.GetScores(survival, false));
		//        StartCoroutine(HSController.GetScores(timeAttack, true));
		//    }
		//    highscoresLoaded = true;
		//    DisableReloadButton();
		//}
	}

	void Update() {
		// When the player presses "Escape" or "Back" on Android, returns to main menu screen.
		if (Input.GetKey(KeyCode.Escape))
			OnBackClicked();
	}

	private IEnumerator fetchScores(string artist, string song, Game.Mode gameMode) {
		yield return StartCoroutine(HSController.GetScoresNew(artist, song, gameMode));
		foreach (string[] row in HSController.ScoresList) {
			//Debug.Log(row[0] + " :: " + row[1]);
		}
	}

	#region Main Menu buttons
	/// <summary>
	/// Button handler for "Play" button
	/// </summary>
	void OnPlayClicked() {
		// Stop any current momentum to allow for Spring to begin.
		panel.currentMomentum = Vector3.zero;
		// Begin spring motion
		SpringPanel.Begin(panel.gameObject, modeMenu, 13f);
		//StartCoroutine(fetchScores("artista", "song", Game.Mode.DeathMatch));
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

		//StartCoroutine(HSController.PostScoresNew("Masddasda", 2222, "artista", "song", Game.Mode.DeathMatch));
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
	#endregion

	#region Other menu buttons (Back, Quit-confirm)
	/// <summary>
	/// Button handler or "Back" button
	/// </summary>
	void OnBackClicked() {
		fb.SendMessage("CloseFileWindow");
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
	#endregion

	#region Select Mode buttons (Shows file browser)
	/// <summary>
	/// Button handler for "Arcade" button
	/// </summary>
	void OnArcadeModeClicked() {
		Game.GameMode = Game.Mode.DeathMatch;
		ToggleModeMenu(false);
		ToggleBackModeButton(false);
		ToggleFileBrowserPanel(true);
		if (!string.IsNullOrEmpty(Game.Path)) fb.SendMessage("OpenFileWindow", PlayerPrefs.GetString("filePath"));
		else fb.SendMessage("OpenFileWindow", "");

		//Application.LoadLevel("Game");
	}

	/// <summary>
	/// Button handler for "Survival" button
	/// </summary>
	void OnChallengeModeClicked() {
		//FileBrowser.LoadFileBrowser("DeathMatch");
		//Application.LoadLevel("DeathMatch");
	}
	#endregion

	#region Highscore buttons
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
	#endregion

	#region Enable/Disable regions of the UI
	public static void ToggleModeMenu(bool state) {
		string labelText = "Choose Game Mode";
		Vector3 labelPosition = new Vector3(0f, 134f, 0f);
		Vector3 labelScale = new Vector3(40, 40, 1);
		instance.ArcadeButton.isEnabled = state;
		instance.ArcadeButton.transform.GetChild(0).gameObject.active = state;   // Show/Hide Background
		instance.ArcadeButton.transform.GetChild(1).gameObject.active = state;   // Show/Hide Label
		instance.SurvivalButton.isEnabled = state;
		instance.SurvivalButton.transform.GetChild(0).gameObject.active = state;   // Show/Hide Background
		instance.SurvivalButton.transform.GetChild(1).gameObject.active = state;   // Show/Hide Label
		instance.StoryButton.isEnabled = state;
		instance.StoryButton.transform.GetChild(0).gameObject.active = state;   // Show/Hide Background
		instance.StoryButton.transform.GetChild(1).gameObject.active = state;   // Show/Hide Label

		if (!state) {
			labelText = "Select Song";
			labelPosition = new Vector3(0f, 159f, 0f);
			labelScale = new Vector3(30, 30, 1);
		}

		instance.selectModeLabel.text = labelText;
		instance.selectModeLabel.transform.localPosition = labelPosition;
		instance.selectModeLabel.transform.localScale = labelScale;
	}

	public static void ToggleFileBrowserPanel(bool state) {
		Game.SetActiveState(instance.FileBrowserPanel, state);
	}

	public static void ToggleBackModeButton(bool state) {
		instance.BackModeButton.isEnabled = state;
		instance.BackModeButton.transform.GetChild(0).gameObject.active = state;   // Show/Hide Background
		instance.BackModeButton.transform.GetChild(1).gameObject.active = state;   // Show/Hide Label
	}

	public static void DisableReloadButton() {
		instance.reloadButton.isEnabled = false;
		instance.reloadButton.transform.GetChild(0).gameObject.active = false;
		instance.reloadButton.transform.GetChild(1).gameObject.active = false;
		instance.nextButton.isEnabled = false;
		instance.prevButton.isEnabled = false;
	}

	public static void EnableReloadButton() {
		instance.reloadButton.isEnabled = true;
		instance.reloadButton.transform.GetChild(0).gameObject.active = true;
		instance.reloadButton.transform.GetChild(1).gameObject.active = true;
		instance.nextButton.isEnabled = true;
		instance.prevButton.isEnabled = true;
	}
	#endregion

	#endregion
}
