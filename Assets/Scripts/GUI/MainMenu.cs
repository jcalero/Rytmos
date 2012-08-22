using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
/// <summary>
/// MainMenu.cs
/// 
/// Handles the display of the main menu. 
/// </summary>
public class MainMenu : MonoBehaviour {

	#region Fields
	// Menu panels (Inspector instances, location: MainMenuManager)
	public UIPanel MainMenuBasePanel;
	public UIPanel MainMenuPlayPanel;
	public UIPanel MainMenuQuitPanel;
	public UIPanel MainMenuModePanel;
	public UIPanel MainMenuOptionsPanel;
	public UIPanel MainMenuScoresPanel;
	public UIPanel MainMenuExtrasPanel;
	public UIPanel MainMenuFileBrowserPanel;
	public UIPanel MainMenuLoggedInBoxPanel;
	public UIPanel MainMenuLogInPanel;
	public UIPanel MainMenuChoicePanel;
	public UIPanel MainMenuSongNotFoundPanel;

	// File browser
	public GameObject FileBrowser;

	// Current state of the menu.
	public MenuLevel CurrentMenuLevel;

	// Play menu objects
	public UIButton OptionsButton;
	public UILabel FPSLabel;

	// Options menu objects
	public UILabel LoggedOutLabel;
	public UIButton OptionsBackButton;

	// Options menu settable objects
	public UISlider EffectsVolumeSlider;
	public UISlider MusicVolumeSlider;
	public UILabel ColorblindSettingLabel;
	public UILabel LowGraphicsSettingLabel;

	// LoggedInBox menu objects
	public UILabel UsernameLabel;

	// Mode menu objects
	public UIButton ModeBackButton;

	// Login Menu Objects
	public UIButton LoginBackButton;
	public UIInput UserNameInput;
	public UILabel ErrorLabel;
	public UICheckbox RememberMeCheckbox;
	private Regex nameRegEx = new Regex("^[a-zA-Z0-9]*$");

	// Scores menu objects
	public UIButton ScoresBackButton;
	public UIButton NextSongButton;
	public UIButton PrevSongButton;
	public GameObject VerticalDivider;
	public GameObject HorizontalDivider;
	public GameObject TSNames;
	public GameObject YSScores;
	public UILabel TopScoresLabel;
	public UILabel ScoresSongNameLabel;
	public UILabel ScoresSongModeLabel;

	// Confirm Message Objects
	public UILabel SongNameLabel;

	// File browser menu objects
	public UIButton FileBrowserBackButton;
	public UILabel PathLabel;
	public UILabel RecentlyPlayedLabel;
	public UILabel SongFromPhoneLabel;
	public UISlicedSprite BrowserBox;
	public UISlicedSprite PathBox;
	public UIButton RecentlyPlayedTab;
	public UIButton SongFromPhoneTab;
	public UIButton UpButton;
	
	// Camera - For Setting Camera Size
	public Camera cameraSize;
	#endregion

	#region Functions

	void Awake() {
		
		Game.GameState = Game.State.Menu;
		ChangeMenu(MenuLevel.Base);
		
#if UNITY_WEBPLAYER
		Game.Song = "";
#endif
	}
	
	void Start() {
		//cameraSize.orthographicSize = Game.GetCameraScaleFactor();

		float aspect = Camera.mainCamera.aspect;
		Debug.Log(aspect);

		// Set up the menu for different aspect ratios
		// Aspect ratio: 1.333..
		if (aspect <= 640.0f / 480.0f) {
			Debug.Log("Setting up for aspect: " + aspect);
			// Play menu
			OptionsButton.transform.localPosition = new Vector2(OptionsButton.transform.localPosition.x + 80,
				OptionsButton.transform.localPosition.y);
			FPSLabel.transform.localPosition = new Vector2(FPSLabel.transform.localPosition.x - 85,
				FPSLabel.transform.localPosition.y);
			// Options menu
			OptionsBackButton.transform.localPosition = new Vector2(OptionsBackButton.transform.localPosition.x + 80,
				OptionsBackButton.transform.localPosition.y);
			// Mode menu
			ModeBackButton.transform.localPosition = new Vector2(ModeBackButton.transform.localPosition.x + 80,
				ModeBackButton.transform.localPosition.y);
			// Login menu
			LoginBackButton.transform.localPosition = new Vector2(-210, 154);
			// Scores menu
			ScoresSongNameLabel.lineWidth = 460;
			ScoresSongModeLabel.transform.localPosition = new Vector2(ScoresSongModeLabel.transform.localPosition.x,
				ScoresSongModeLabel.transform.localPosition.y + 60);
			ScoresSongNameLabel.transform.localPosition = new Vector2(ScoresSongNameLabel.transform.localPosition.x,
				ScoresSongNameLabel.transform.localPosition.y - 60);
			ScoresBackButton.transform.localPosition = new Vector2(ScoresBackButton.transform.localPosition.x + 70,
				ScoresBackButton.transform.localPosition.y);
			PrevSongButton.transform.localPosition = new Vector2(PrevSongButton.transform.localPosition.x + 30,
				PrevSongButton.transform.localPosition.y - 60);
			NextSongButton.transform.localPosition = new Vector2(NextSongButton.transform.localPosition.x - 30,
				NextSongButton.transform.localPosition.y - 60);
			TSNames.transform.localPosition = new Vector2(TSNames.transform.localPosition.x + 70,
				TSNames.transform.localPosition.y);
			YSScores.transform.localPosition = new Vector2(YSScores.transform.localPosition.x - 70,
				YSScores.transform.localPosition.y);
			HorizontalDivider.transform.localScale = new Vector2(HorizontalDivider.transform.localScale.x - 140,
				HorizontalDivider.transform.localScale.y);
			TopScoresLabel.transform.localPosition = new Vector2(TopScoresLabel.transform.localPosition.x + 60,
				TopScoresLabel.transform.localPosition.y);
			// File browser menu
			FileBrowserBackButton.transform.localPosition = new Vector2(FileBrowserBackButton.transform.localPosition.x + 80,
				FileBrowserBackButton.transform.localPosition.y);
			BrowserBox.transform.localScale = new Vector2(BrowserBox.transform.localScale.x - 160,
				BrowserBox.transform.localScale.y);
			PathBox.transform.localScale = new Vector2(PathBox.transform.localScale.x - 160,
				PathBox.transform.localScale.y);
			BrowserBox.transform.localPosition = new Vector2(BrowserBox.transform.localPosition.x + 80,
				BrowserBox.transform.localPosition.y);
			PathBox.transform.localPosition = new Vector2(PathBox.transform.localPosition.x + 80,
				PathBox.transform.localPosition.y);
			PathLabel.transform.localPosition = new Vector2(PathLabel.transform.localPosition.x + 80,
				PathLabel.transform.localPosition.y);
			RecentlyPlayedTab.transform.localPosition = new Vector2(RecentlyPlayedTab.transform.localPosition.x - 20,
				RecentlyPlayedTab.transform.localPosition.y - 3);
			SongFromPhoneTab.transform.localPosition = new Vector2(SongFromPhoneTab.transform.localPosition.x - 47,
				SongFromPhoneTab.transform.localPosition.y - 3);
			RecentlyPlayedTab.transform.localScale = new Vector2(0.9f, 0.9f);
			SongFromPhoneTab.transform.localScale = new Vector2(0.9f, 0.9f);
			UpButton.transform.localScale = new Vector2(0.9f, 0.9f);
			UpButton.transform.localPosition = new Vector2(UpButton.transform.localPosition.x, UpButton.transform.localPosition.y - 3);
		}
			// Aspect ratio: 1.5
		else if (aspect == (480.0f / 320.0f)) {
			Debug.Log("Setting up for aspect: " + aspect);
			// Play menu
			OptionsButton.transform.localPosition = new Vector2(OptionsButton.transform.localPosition.x + 45,
				OptionsButton.transform.localPosition.y);
			FPSLabel.transform.localPosition = new Vector2(FPSLabel.transform.localPosition.x - 40,
				FPSLabel.transform.localPosition.y);
			// Options menu
			OptionsBackButton.transform.localPosition = new Vector2(OptionsBackButton.transform.localPosition.x + 45,
				OptionsBackButton.transform.localPosition.y);
			// Mode menu
			ModeBackButton.transform.localPosition = new Vector2(ModeBackButton.transform.localPosition.x + 45,
				ModeBackButton.transform.localPosition.y);
			// Login menu
			LoginBackButton.transform.localPosition = new Vector2(LoginBackButton.transform.localPosition.x + 35,
				LoginBackButton.transform.localPosition.y);
			// Scores menu
			ScoresSongNameLabel.lineWidth = 460;
			ScoresBackButton.transform.localPosition = new Vector2(ScoresBackButton.transform.localPosition.x + 30,
				ScoresBackButton.transform.localPosition.y);
			PrevSongButton.transform.localPosition = new Vector2(PrevSongButton.transform.localPosition.x + 25,
				PrevSongButton.transform.localPosition.y);
			NextSongButton.transform.localPosition = new Vector2(NextSongButton.transform.localPosition.x - 25,
				NextSongButton.transform.localPosition.y);
			TSNames.transform.localPosition = new Vector2(TSNames.transform.localPosition.x + 30,
				TSNames.transform.localPosition.y);
			YSScores.transform.localPosition = new Vector2(YSScores.transform.localPosition.x - 30,
				YSScores.transform.localPosition.y);
			HorizontalDivider.transform.localScale = new Vector2(HorizontalDivider.transform.localScale.x - 60,
				HorizontalDivider.transform.localScale.y);
			TopScoresLabel.transform.localPosition = new Vector2(TopScoresLabel.transform.localPosition.x + 20,
				TopScoresLabel.transform.localPosition.y);
			// File browser menu
			FileBrowserBackButton.transform.localPosition = new Vector2(FileBrowserBackButton.transform.localPosition.x + 40,
				FileBrowserBackButton.transform.localPosition.y);
			BrowserBox.transform.localScale = new Vector2(BrowserBox.transform.localScale.x - 80,
				BrowserBox.transform.localScale.y);
			PathBox.transform.localScale = new Vector2(PathBox.transform.localScale.x - 80,
				PathBox.transform.localScale.y);
			BrowserBox.transform.localPosition = new Vector2(BrowserBox.transform.localPosition.x + 40,
				BrowserBox.transform.localPosition.y);
			PathBox.transform.localPosition = new Vector2(PathBox.transform.localPosition.x + 40,
				PathBox.transform.localPosition.y);
			PathLabel.transform.localPosition = new Vector2(PathLabel.transform.localPosition.x + 40,
				PathLabel.transform.localPosition.y);
			RecentlyPlayedTab.transform.localPosition = new Vector2(RecentlyPlayedTab.transform.localPosition.x - 25,
				RecentlyPlayedTab.transform.localPosition.y);
			SongFromPhoneTab.transform.localPosition = new Vector2(SongFromPhoneTab.transform.localPosition.x - 25,
				SongFromPhoneTab.transform.localPosition.y);
		}
			// Aspect ratio: 1.6
		else if (aspect == (1280.0f / 800.0f)) {
			Debug.Log("Setting up for aspect: " + aspect);
			// Play menu
			OptionsButton.transform.localPosition = new Vector2(OptionsButton.transform.localPosition.x + 20,
				OptionsButton.transform.localPosition.y);
			FPSLabel.transform.localPosition = new Vector2(FPSLabel.transform.localPosition.x - 15,
				FPSLabel.transform.localPosition.y);
			// Options menu
			OptionsBackButton.transform.localPosition = new Vector2(OptionsBackButton.transform.localPosition.x + 20,
				OptionsBackButton.transform.localPosition.y);
			// Mode menu
			ModeBackButton.transform.localPosition = new Vector2(ModeBackButton.transform.localPosition.x + 20,
				ModeBackButton.transform.localPosition.y);
			// Login menu
			LoginBackButton.transform.localPosition = new Vector2(LoginBackButton.transform.localPosition.x + 10,
				LoginBackButton.transform.localPosition.y);
			// Scores menu
			ScoresBackButton.transform.localPosition = new Vector2(ScoresBackButton.transform.localPosition.x + 10,
				ScoresBackButton.transform.localPosition.y);
			TSNames.transform.localPosition = new Vector2(TSNames.transform.localPosition.x + 10,
				TSNames.transform.localPosition.y);
			YSScores.transform.localPosition = new Vector2(YSScores.transform.localPosition.x - 10,
				YSScores.transform.localPosition.y);
			// File browser menu
			FileBrowserBackButton.transform.localPosition = new Vector2(FileBrowserBackButton.transform.localPosition.x + 20,
				FileBrowserBackButton.transform.localPosition.y);
			BrowserBox.transform.localScale = new Vector2(BrowserBox.transform.localScale.x - 40,
				BrowserBox.transform.localScale.y);
			PathBox.transform.localScale = new Vector2(PathBox.transform.localScale.x - 40,
				PathBox.transform.localScale.y);
			BrowserBox.transform.localPosition = new Vector2(BrowserBox.transform.localPosition.x + 20,
				BrowserBox.transform.localPosition.y);
			PathBox.transform.localPosition = new Vector2(PathBox.transform.localPosition.x + 20,
				PathBox.transform.localPosition.y);
			PathLabel.transform.localPosition = new Vector2(PathLabel.transform.localPosition.x + 20,
				PathLabel.transform.localPosition.y);
		}
			// Aspect ratio: 1.666.. (Default android)
		else if (aspect == (800.0f / 480.0f)) {
			// Default. Doesn't need to change.
		}
			// Aspect ratio: 1.7066..
		else if (aspect == (1024.0f / 600.0f)) {
			// Using the same as 1.666.. (Default)
		}
			// Aspect ratio: 1.779166..
		else if (aspect == (854.0f / 480.0f)) {
			// Using the same as 1.666.. (Default)
		}
			// Aspect ratio: 1.8
		else if (aspect == (432.0f / 240.0f)) {
			// Using the same as 1.666.. (Default)
		}
	}

	void Update() {
		// When the player presses "Escape" or "Back" on Android, returns to main menu screen
		// or goes to the quit menu if on the main menu already.
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (CurrentMenuLevel == MenuLevel.Base) ChangeMenu(MenuLevel.Quit);
			else if (CurrentMenuLevel == MenuLevel.FileBrowser && !FileBrowserMenu.RecentlyPlayedActive) FileBrowser.SendMessage("OnUpClicked");
			else OnBackClicked();
		}
		gameObject.audio.volume = Game.MusicVolume;
	}

	/// <summary>
	/// Change the menu now to watever the CurrentMenuLevel is set to.
	/// </summary>
	private void ChangeMenu() {
		ChangeMenu(CurrentMenuLevel);
	}

	public void ClearMenu() {
		if (MainMenuQuitPanel.enabled) UITools.SetActiveState(MainMenuQuitPanel, false);
		if (MainMenuModePanel.enabled) UITools.SetActiveState(MainMenuModePanel, false);
		if (MainMenuOptionsPanel.enabled) UITools.SetActiveState(MainMenuOptionsPanel, false);
		if (MainMenuScoresPanel.enabled) UITools.SetActiveState(MainMenuScoresPanel, false);
		if (MainMenuLoggedInBoxPanel.enabled) UITools.SetActiveState(MainMenuLoggedInBoxPanel, false);
		if (MainMenuLogInPanel.enabled) UITools.SetActiveState(MainMenuLogInPanel, false);
		if (MainMenuFileBrowserPanel.enabled) UITools.SetActiveState(MainMenuFileBrowserPanel, false);
		if (MainMenuBasePanel.enabled) UITools.SetActiveState(MainMenuBasePanel, false);
		if (MainMenuPlayPanel.enabled) UITools.SetActiveState(MainMenuPlayPanel, false);
		if (MainMenuExtrasPanel.enabled) UITools.SetActiveState(MainMenuExtrasPanel, false);
		if (MainMenuChoicePanel.enabled) UITools.SetActiveState(MainMenuChoicePanel, false);
		if (MainMenuSongNotFoundPanel.enabled) UITools.SetActiveState(MainMenuSongNotFoundPanel, false);
	}

	/// <summary>
	/// Change the menu to the given menu level. Also set CurrentMenuLevel appropriately.
	/// </summary>
	/// <param name="menu">The MenuLevel enum to change the menu level to.</param>
	public void ChangeMenu(MenuLevel menu) {
		CurrentMenuLevel = menu;
		switch (CurrentMenuLevel) {
			case MenuLevel.Base:
				if (MainMenuQuitPanel.enabled) UITools.SetActiveState(MainMenuQuitPanel, false);
				if (MainMenuModePanel.enabled) UITools.SetActiveState(MainMenuModePanel, false);
				if (MainMenuOptionsPanel.enabled) UITools.SetActiveState(MainMenuOptionsPanel, false);
				if (MainMenuScoresPanel.enabled) UITools.SetActiveState(MainMenuScoresPanel, false);
				if (MainMenuLoggedInBoxPanel.enabled) UITools.SetActiveState(MainMenuLoggedInBoxPanel, false);
				if (MainMenuLogInPanel.enabled) UITools.SetActiveState(MainMenuLogInPanel, false);
				if (MainMenuFileBrowserPanel.enabled) UITools.SetActiveState(MainMenuFileBrowserPanel, false);
				if (MainMenuChoicePanel.enabled) UITools.SetActiveState(MainMenuChoicePanel, false);
				UITools.SetActiveState(MainMenuExtrasPanel, true);
				UITools.SetActiveState(MainMenuBasePanel, true);
				UITools.SetActiveState(MainMenuPlayPanel, true);
				break;
			case MenuLevel.Mode:
				if (MainMenuFileBrowserPanel.enabled) UITools.SetActiveState(MainMenuFileBrowserPanel, false);
				if (MainMenuPlayPanel.enabled) UITools.SetActiveState(MainMenuPlayPanel, false);
				if (MainMenuChoicePanel.enabled) UITools.SetActiveState(MainMenuChoicePanel, false);
				UITools.SetActiveState(MainMenuExtrasPanel, true);
				UITools.SetActiveState(MainMenuBasePanel, true);
				UITools.SetActiveState(MainMenuModePanel, true);
				break;
			case MenuLevel.Options:
				UITools.SetActiveState(MainMenuPlayPanel, false);
				UITools.SetActiveState(MainMenuOptionsPanel, true);
				if (Game.IsLoggedIn) UITools.SetActiveState(MainMenuLoggedInBoxPanel, true);
				UsernameLabel.text = Game.PlayerName;
				break;
			case MenuLevel.Quit:
				UITools.SetActiveState(MainMenuPlayPanel, false);
				UITools.SetActiveState(MainMenuQuitPanel, true);
				break;
			case MenuLevel.Scores:
				UITools.SetActiveState(MainMenuPlayPanel, false);
				UITools.SetActiveState(MainMenuBasePanel, false);
				UITools.SetActiveState(MainMenuLogInPanel, false);
				UITools.SetActiveState(MainMenuScoresPanel, true);
				HSController.InitHSDisplay();
				break;
			case MenuLevel.FileBrowser:
				if (MainMenuChoicePanel.enabled) UITools.SetActiveState(MainMenuChoicePanel, false);
				if (MainMenuSongNotFoundPanel.enabled) UITools.SetActiveState(MainMenuSongNotFoundPanel, false);
				if (MainMenuExtrasPanel.enabled) UITools.SetActiveState(MainMenuExtrasPanel, false);
				UITools.SetActiveState(MainMenuModePanel, false);
				UITools.SetActiveState(MainMenuBasePanel, false);
				UITools.SetActiveState(MainMenuFileBrowserPanel, true);
				break;
			case MenuLevel.LogIn:
				UITools.SetActiveState(MainMenuPlayPanel, false);
				UITools.SetActiveState(MainMenuBasePanel, false);
				UITools.SetActiveState(MainMenuLogInPanel, true);
				if (PlayerPrefs.GetString("playername") != null)
					UserNameInput.text = PlayerPrefs.GetString("playername");
				RememberMeCheckbox.isChecked = Game.RememberLogin;
				ErrorLabel.text = "";                                   // Clear error text if any
				break;
			case MenuLevel.ConfirmChoice:
				if (MainMenuBasePanel.enabled) UITools.SetActiveState(MainMenuBasePanel, false);
				if (MainMenuFileBrowserPanel.enabled) UITools.SetActiveState(MainMenuFileBrowserPanel, false);
				if (MainMenuModePanel.enabled) UITools.SetActiveState(MainMenuModePanel, false);
				if (MainMenuExtrasPanel.enabled) UITools.SetActiveState(MainMenuExtrasPanel, false);
				SongNameLabel.text = GetSongTitleFromFile();
				CalculateSongLabelSize();
				UITools.SetActiveState(MainMenuChoicePanel, true);
				UITools.SetActiveState(MainMenuExtrasPanel, true);
				foreach(UIButton c in MainMenuChoicePanel.GetComponentsInChildren<UIButton>()) StartCoroutine(DelayButton(c,0.1f));
				break;
			case MenuLevel.SongNotFound:
				if (MainMenuBasePanel.enabled) UITools.SetActiveState(MainMenuBasePanel, false);
				if (MainMenuFileBrowserPanel.enabled) UITools.SetActiveState(MainMenuFileBrowserPanel, false);
				if (MainMenuModePanel.enabled) UITools.SetActiveState(MainMenuModePanel, false);
				if (MainMenuExtrasPanel.enabled) UITools.SetActiveState(MainMenuExtrasPanel, false);
				UITools.SetActiveState(MainMenuSongNotFoundPanel, true);
				UITools.SetActiveState(MainMenuExtrasPanel, true);
				foreach(UIButton c in MainMenuChoicePanel.GetComponentsInChildren<UIButton>()) StartCoroutine(DelayButton(c,0.1f));
				break;
				
		}
	}

	#region Main Menu buttons
	/// <summary>
	/// Button handler for "Play" button
	/// </summary>
	void OnPlayClicked() {
		ChangeMenu(MenuLevel.Mode);
	}

	/// <summary>
	/// Button handler for "Highscores" button
	/// </summary>
	void OnScoresClicked() {
		//Debug.Log(Game.RememberLogin + " : " + Game.IsLoggedIn);
		if ((!Game.RememberLogin && !Game.IsLoggedIn) || Game.PlayerName.Length < 1)
			ChangeMenu(MenuLevel.LogIn);
		else
			ChangeMenu(MenuLevel.Scores);
	}

	/// <summary>
	/// Button handler for "Options" button
	/// </summary>
	void OnOptionsClicked() {
		ChangeMenu(MenuLevel.Options);
		LoadOptions();
	}
	#endregion

	#region Other Buttons (Back)
	/// <summary>
	/// Button handler or "Back" button
	/// </summary>
	void OnBackClicked() {
		if (CurrentMenuLevel == MenuLevel.FileBrowser) FileBrowser.SendMessage("CloseFileWindow");
		else ChangeMenu(MenuLevel.Base);
	}
	#endregion

	#region Quit menu buttons
	/// <summary>
	/// Button handler for "Yes, Quit Game" button
	/// </summary>
	void OnQuitConfirmedClicked() {
		Application.Quit();
	}

	/// <summary>
	/// Button handler for "No, Don't Quit Game" button
	/// </summary>
	private void OnQuitCancelClicked() {
		ChangeMenu(MenuLevel.Base);
	}
	#endregion

	#region Confirm Choice Menu
	void OnChoiceConfirmedClicked() {
		ClearMenu();
		Application.LoadLevel("LoadScreen");
	}

	void OnChoiceDeclineClicked() {
		ChangeMenu(MenuLevel.FileBrowser);
		if (!FileBrowserMenu.RecentlyPlayedActive) {
			if (!string.IsNullOrEmpty(Game.Path)) FileBrowser.SendMessage("OpenFileWindow", PlayerPrefs.GetString("filePath"));
			else FileBrowser.SendMessage("OpenFileWindow", "");
		} else {
			FileBrowser.SendMessage("OpenRecentFilesWindow");
		}
	}

	void CalculateSongLabelSize() {
		if (SongNameLabel.text.Length > 38) {
			SongNameLabel.transform.localScale = new Vector2(38, 38);
		} else if (SongNameLabel.text.Length > 28) {
			SongNameLabel.transform.localScale = new Vector2(50, 50);
		}
	}

	string GetSongTitleFromFile() {
		if (Game.Song != null && Game.Song != "") {
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
				return Game.Song.Substring(Game.Song.LastIndexOf('\\') + 1, Game.Song.LastIndexOf('.') - Game.Song.LastIndexOf('\\') - 1);
			else
				return Game.Song.Substring(Game.Song.LastIndexOf('/') + 1, Game.Song.LastIndexOf('.') - Game.Song.LastIndexOf('/') - 1);
		} else return "";
	}
	#endregion

	#region Select Mode buttons (Shows file browser)
	/// <summary>
	/// Button handler for "Arcade" button
	/// </summary>
	private void OnArcadeButtonClicked() {
#if !UNITY_WEBPLAYER
		Game.GameMode = Game.Mode.Arcade;
		ChangeMenu(MenuLevel.FileBrowser);
		if (!FileBrowserMenu.RecentlyPlayedActive) {
			if (!string.IsNullOrEmpty(Game.Path)) FileBrowser.SendMessage("OpenFileWindow", PlayerPrefs.GetString("filePath"));
			else FileBrowser.SendMessage("OpenFileWindow", "");
		} else {
			FileBrowser.SendMessage("OpenRecentFilesWindow");
		}
#else
		if(Game.Song != "Jazz-Fog" && Game.Song != "KnoxCanyon" && Game.Song != "LG-F1" && Game.Song != "YouGotToChange") {
			Game.Song = "Poundcake";
			AudioManager.artist = "GRiZ";
			AudioManager.title = "Poundcake";
		}
		StartCoroutine(AudioManager.initMusic(""));
		while(!AudioManager.isSongLoaded()) {}
		AudioManager.tagDataSet = true;
		Application.LoadLevel("LoadScreen");
#endif
	}

	/// <summary>
	/// Button handler for "Casual" button
	/// </summary>
	private void OnCasualButtonClicked() {
		Game.GameMode = Game.Mode.Casual;
		FadeOutMenu();
		ChangeMenu(MenuLevel.FileBrowser);
		if (!FileBrowserMenu.RecentlyPlayedActive) {
			if (!string.IsNullOrEmpty(Game.Path)) FileBrowser.SendMessage("OpenFileWindow", PlayerPrefs.GetString("filePath"));
			else FileBrowser.SendMessage("OpenFileWindow", "");
		} else {
			FileBrowser.SendMessage("OpenRecentFilesWindow");
		}
	}

	/// <summary>
	/// Button handler for "Challenge" button
	/// </summary>
	private void OnChallengeButtonClicked() {
		OnArcadeButtonClicked();
	}
	/// <summary>
	/// Button handler for "Story" button
	/// </summary>
	private void OnTutorialButtonClicked() {
		Game.Song = "Tutorial";
		AudioManager.artist = "Luke Stark";
		AudioManager.title = "The Price Of Victory";
		Game.GameMode = Game.Mode.Tutorial;
		Application.LoadLevel("LoadScreen");
	}
	#endregion

	#region Option menu functions
	private void OnEffectsSliderChange() {
		Game.EffectsVolume = EffectsVolumeSlider.sliderValue;
	}

	private void OnMusicSliderChange() {
		Game.MusicVolume = MusicVolumeSlider.sliderValue;
	}

	private void OnColorBlindModeButtonClicked() {
		bool tempValue = Game.ColorBlindMode;
		Game.ColorBlindMode = !tempValue;
		ColorblindSettingLabel.text = !tempValue ? "[44ff44]On" : "[FF4444]Off";
	}

	private void OnLowGraphicsButtonClicked() {
		bool tempValue = Game.LowGraphicsMode;
		Game.LowGraphicsMode = !tempValue;
		LowGraphicsSettingLabel.text = !tempValue ? "[44ff44]On" : "[FF4444]Off";
	}

	private void OnLogOutClicked() {
		UITools.SetActiveState(MainMenuLoggedInBoxPanel, false);
		Game.IsLoggedIn = false;
		Game.RememberLogin = false;
		StartCoroutine(DelayLabel(LoggedOutLabel, "Logged out..", 2f));
	}

	private IEnumerator DelayLabel(UILabel label, string text, float time) {
		label.text = text;
		yield return new WaitForSeconds(time);
		label.text = "";
	}
	
	private IEnumerator DelayButton(UIButton button, float time) {
		button.isEnabled = false;
		yield return new WaitForSeconds(time);
		button.isEnabled = true;
	}
	#endregion

	#region Login Menu buttons
	void OnRememberMeActivate() {
		Game.RememberLogin = RememberMeCheckbox.isChecked;
	}
	void OnLoginClicked() {
		string playerName = UserNameInput.text;
		if (playerName.Length < 2) {                            // Error if input text is too short
			UserNameInput.text = "";
			ErrorLabel.text = "[F87431]Too short. Try again";
			return;
		}
		if (!nameRegEx.IsMatch(playerName)) {
			UserNameInput.text = "";
			ErrorLabel.text = "[F87431]Invalid. Only letters & numbers allowed.";
			return;
		}
		Game.IsLoggedIn = true;
		// Store the new player name
		PlayerPrefs.SetString("playername", playerName);
		// Change the menu
		ChangeMenu(MenuLevel.Scores);
	}
	#endregion

	#region Highscore buttons
	void OnNextSongClicked() {
		HSController.LoadNextSong();
	}

	void OnPrevSongClicked() {
		HSController.LoadPrevSong();
	}

	void OnPrevModeClicked() {
		HSController.LoadPrevMode();
	}

	void OnNextModeClicked() {
		HSController.LoadNextMode();
	}
	#endregion

	#region Utilities
	/// <summary>
	/// Loads the specified level after a certain amount of time
	/// </summary>
	/// <param name="level">The level string to load</param>
	/// <param name="time">The amount of seconds to wait before loading</param>
	/// <returns></returns>
	private IEnumerator LoadLevelDelayed(string level, float time) {
		yield return new WaitForSeconds(time);
		Application.LoadLevel(level);
	}

	private void FadeOutMenu() {

	}

	private void LoadOptions() {
		EffectsVolumeSlider.sliderValue = Game.EffectsVolume;
		MusicVolumeSlider.sliderValue = Game.MusicVolume;
		ColorblindSettingLabel.text = Game.ColorBlindMode ? "[44ff44]On" : "[FF4444]Off";
		LowGraphicsSettingLabel.text = Game.LowGraphicsMode ? "[44ff44]On" : "[FF4444]Off";
	}

	public static void EnableReloadButton() {

	}
	#endregion

	#endregion
}

public enum MenuLevel {
	Base,
	Mode,
	Quit,
	Scores,
	Options,
	FileBrowser,
	LogIn,
	ConfirmChoice,
	SongNotFound
}