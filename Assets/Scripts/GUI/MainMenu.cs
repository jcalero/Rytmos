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
	private GameObject cam;
	
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

	// Options menu objects
	public UILabel LoggedOutLabel;

	// Options menu settable objects
	public UISlider EffectsVolumeSlider;
	public UISlider MusicVolumeSlider;
	public UILabel ColorblindSettingLabel;
	public UILabel LowGraphicsSettingLabel;

	// LoggedInBox menu objects
	public UILabel UsernameLabel;

	// Login Menu Objects
	public UIInput UserNameInput;
	public UILabel ErrorLabel;
	public UICheckbox RememberMeCheckbox;
	private Regex nameRegEx = new Regex("^[a-zA-Z0-9]*$");

	// Confirm Message Objects
	public UILabel SongNameLabel;
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
		cam = GameObject.Find("Camera");
		cam.GetComponentInChildren<Camera>().orthographicSize = Game.cameraScaleFactor;
		
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