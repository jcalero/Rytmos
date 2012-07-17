using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
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
	//private bool loadedTimeAttack = true;
	//private string survival = "rytmos_hs_dm";
	//private string timeAttack = "rytmos_hs_30sec";
	private List<string> songList = new List<string>();
	private string[][] separatedSongList;
	private int currentlyShowingHS = 0;

	private static MainMenu instance;

	public static string ScoresString;
	public static string FetchError;
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
		if (!LoadSongList()) {
			top5names[0].text = "Top 5 Scores...";
			close5names[0].text = "Scores close to you...";
			highscoresTypeLabel.text = "No songs have been played yet! Play some first.";
		} else {
			string artist = separatedSongList[0][0];
			string song = separatedSongList[0][1];
			Game.Mode gameMode = (Game.Mode)Enum.Parse(typeof(Game.Mode), separatedSongList[0][2]);

			StartCoroutine(fetchScores(artist, song, gameMode));
		}
	}

	void Update() {
		// When the player presses "Escape" or "Back" on Android, returns to main menu screen.
		if (Input.GetKey(KeyCode.Escape))
			OnBackClicked();
	}
	private IEnumerator fetchScores(int songListID) {
		string artist = separatedSongList[songListID][0];
		string song = separatedSongList[songListID][1];
		Game.Mode gameMode = (Game.Mode)Enum.Parse(typeof(Game.Mode), separatedSongList[songListID][2]);
		yield return StartCoroutine(fetchScores(artist, song, gameMode));
		EnableReloadButton();
	}
	private IEnumerator fetchScores(string artist, string song, Game.Mode gameMode) {
		string artistDisplay = artist;
		string songDisplay = song;
		//string md5Artist = HSController.RemoveSpecialCharacters(artist).ToLower();
		//string md5Song = HSController.RemoveSpecialCharacters(song).ToLower();
		//string entryMD5 = MD5Utils.MD5FromString(md5Artist + md5Song + gameMode);
		string localMD5 = HSController.CalculateTableName(artist, song, gameMode);
		int topScore = PlayerPrefs.GetInt(localMD5);

		//Debug.Log("This is what the data looks like before calculating fetch-MD5: " + md5Artist + " - " + md5Song + " - " + gameMode.ToString());

		if (artist.Length > 1) artistDisplay = char.ToUpper(artist[0]) + artist.Substring(1);
		if (song.Length > 1) songDisplay = char.ToUpper(song[0]) + song.Substring(1);

		highscoresTypeLabel.text = artistDisplay + " - " + songDisplay + " (" + gameMode + ") ";
		top5names[0].text = "Loading Top Scores...";
		close5names[0].text = "Loading Close Scores...";
		top5scores[0].text = "";
		close5scores[0].text = "";
		int labelcnt = 1;
		while (labelcnt < 5) {
			top5names[labelcnt].text = "";
			close5names[labelcnt].text = "";
			top5scores[labelcnt].text = "";
			close5scores[labelcnt].text = "";
			labelcnt++;
		}

		yield return StartCoroutine(HSController.GetTop5Scores(artist, song, gameMode));
		if (FetchError == null) {
			for (int cnt = 0; cnt < HSController.Top5List.Length; cnt++) {
				if (cnt < 5) {
					top5names[cnt].text = (cnt + 1) + ". " + HSController.Top5List[cnt][0];
					top5scores[cnt].text = HSController.Top5List[cnt][1];
				}
				//Debug.Log(HSController.ScoresList[cnt][0] + " :: " + HSController.ScoresList[cnt][1]);
			}
		} else {
			top5names[0].text = FetchError;
		}

		yield return StartCoroutine(HSController.GetClose5Scores(artist, song, gameMode, topScore));
		if (FetchError == null) {
			bool formattedOwnRow = false;
			Sort<string>(HSController.Close5List, 2);
			for (int cnt = 0; cnt < HSController.Close5List.Length; cnt++) {
				if (cnt < 5) {
					close5names[cnt].text = HSController.Close5List[cnt][2] + ". " + HSController.Close5List[cnt][0];
					close5scores[cnt].text = HSController.Close5List[cnt][1];
					if (HSController.Close5List[cnt][1] == topScore.ToString() &&
						HSController.Close5List[cnt][0] == Game.PlayerName &&
						!formattedOwnRow) {
						close5names[cnt].text = "[FDD017]" + close5names[cnt].text;
						close5scores[cnt].text = "[FDD017]" + close5scores[cnt].text;
						formattedOwnRow = true;
					}
				}
				//Debug.Log(HSController.ScoresList[cnt][0] + " :: " + HSController.ScoresList[cnt][1]);
			}
		} else {
			close5names[0].text = FetchError;
		}
	}

	private bool LoadSongList() {
		// Load song list
		string path = "";
		if (Application.platform == RuntimePlatform.Android)
			path = (Application.persistentDataPath + "/songlist.r");
		else if (Application.platform == RuntimePlatform.WindowsPlayer
			|| Application.platform == RuntimePlatform.WindowsEditor)
			path = (Application.persistentDataPath + "\\songlist.r");
		else {
			Debug.Log("PLATFORM NOT SUPPORTED YET");
			path = "";
		}
		string line;
		int counter = 0;
		try {
			using (StreamReader sr = new StreamReader(path)) {
				while ((line = sr.ReadLine()) != null) {
					songList.Add(line);
				}
				sr.Close();
				separatedSongList = new string[songList.Count][];

				while (counter < songList.Count) {
					string[] tempString = songList[counter].Split('|');
					separatedSongList[counter] = new string[3];
					separatedSongList[counter][0] = tempString[0];
					separatedSongList[counter][1] = tempString[1];
					separatedSongList[counter][2] = tempString[2];
					counter++;
				}
				if (songList.Count < 1) return false;
				else return true;
			}
		} catch (Exception e) {
			Debug.LogError(e.Message);
			return false;
		}
	}
	private static void Sort<T>(T[][] data, int col) {
		Comparer<T> comparer = Comparer<T>.Default;
		Array.Sort<T[]>(data, (x, y) => comparer.Compare(x[col], y[col]));
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
		// Store the new player name
		//Game.PlayerName = "Jakob";
		// Store the song high score
		//string entryMD5 = MD5Utils.MD5FromString(Game.Artist + Game.SongName + Game.Mode.DeathMatch);
		//if (PlayerPrefs.GetInt(entryMD5) < 10500) PlayerPrefs.SetInt(entryMD5, 10500);
		//StartCoroutine(HSController.PostScoresNew(Game.PlayerName, 10500, Game.Artist, Game.SongName, Game.Mode.DeathMatch));
	}

	/// <summary>
	/// Button handler for "Highscores" button
	/// </summary>
	void OnHighScoresClicked() {
		// Stop any current momentum to allow for Spring to begin.
		panel.currentMomentum = Vector3.zero;
		// Begin spring motion
		SpringPanel.Begin(panel.gameObject, highScoresMenu, 13f);
		//Debug.Log(Game.Artist + Game.SongName + Game.Mode.DeathMatch);
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
		DisableReloadButton();
		currentlyShowingHS = (currentlyShowingHS + 1) % separatedSongList.Length;
		StartCoroutine(fetchScores(currentlyShowingHS));
	}

	void OnPrevHighscoreClicked() {
		DisableReloadButton();
		currentlyShowingHS = ((currentlyShowingHS - 1) % separatedSongList.Length + separatedSongList.Length) % separatedSongList.Length;
		StartCoroutine(fetchScores(currentlyShowingHS));
	}

	void OnReloadClicked() {
		DisableReloadButton();
		StartCoroutine(fetchScores(currentlyShowingHS));
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
