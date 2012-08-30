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
	public UIPanel MainMenuScoresOfflinePanel;
	public UIPanel MainMenuExtrasPanel;
	public UIPanel MainMenuFileBrowserPanel;
	public UIPanel MainMenuLoggedInBoxPanel;
	public UIPanel MainMenuLogInPanel;
	public UIPanel MainMenuCreatePanel;
	public UIPanel MainMenuChoicePanel;
	public UIPanel MainMenuSongNotFoundPanel;
	public UIPanel MainMenuCreditsPanel;
	public UIPanel MainMenuCredits3DPanel;
	public UIPanel MainMenuForgotPanel;
	public UIPanel MainMenuForgotMessagePanel;
	public UIPanel MainMenuFirstPlayPanel;
	public UIPanel MainMenuAnalysisNotePanel;
	public UIPanel MainMenuGoOnlinePanel;

	// File browser
	public GameObject FileBrowser;

	// Current state of the menu.
	public MenuLevel CurrentMenuLevel;

	// Play menu objects
	public UIButton OptionsButton;
	public UILabel FPSLabel;
	public UIButton CreditsButton;

	// Credits menu objects
	public UIButton CreditsBackButton;
	public GameObject CreditsPage1;
	public UILabel CreditsTestersTitleLabel;
	public UILabel CreditsTestersLabel;

	// Options menu objects
	public UILabel LoggedOutLabel;
	public UIButton OptionsBackButton;

	// Options menu settable objects
	public UISlider EffectsVolumeSlider;
	public UISlider MusicVolumeSlider;
	public UILabel ColorblindSettingLabel;
	public UILabel OnlineModeSettingLabel;

	// LoggedInBox menu objects
	public UILabel UsernameLabel;

	// Mode menu objects
	public UIButton ModeBackButton;

	// Login Menu Objects
	public UIButton LoginBackButton;
	public UIInput UserNameInput;
	public UIInput LoginPassInput;
	public UILabel ErrorLabel;
	public UICheckbox RememberMeCheckbox;
	private Regex nameRegEx = new Regex("^[a-zA-Z0-9]*$");
	public static bool LoginSuccess;

	// Create menu objects
	public UILabel CreateErrorLabel;
	public UILabel CreateButtonLabel;
	public UIInput CreateNameInput;
	public UIInput CreatePassInput;
	public UIInput CreatePass2Input;
	public UIInput CreateEmailInput;
	private string passHashKey = "r2d2imahorse";
	public static bool UserCreated;
	public static bool UserExists;

	// Forgot menu objects
	public UILabel ForgotErrorLabel;
	public UIInput ForgotNameInput;
	public UIInput ForgotEmailInput;
	public UILabel ForgotMessageLabel;
	public UILabel ForgotButtonLabel;
	public UIButton ForgotButton;
	public UIButton ForgotBackButton;
	public static bool CorrectEmail;
	public static bool EmailSent;
	private string globalEmail;

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

	// Credits menu objects
	public Animation SliderAnimation;
	public Animation[] PageAnimations;
	private int creditsCurrentPage;

	// Camera - For Setting Camera Size
	public Camera cameraSize;

	public static MainMenu instance;
	#endregion

	#region Functions

	void Awake() {
		instance = this;
		Game.GameState = Game.State.Menu;
		ChangeMenu(MenuLevel.Base);

#if UNITY_WEBPLAYER
		Game.Song = "";
#endif
	}

	void Start() {
		//cameraSize.orthographicSize = Game.GetCameraScaleFactor();

		float aspect = Camera.mainCamera.aspect;

		// Set up the menu for different aspect ratios
		// Aspect ratio: 1.333..
		if (aspect <= 640.0f / 480.0f) {
			Debug.Log("Setting up for aspect: " + aspect);
			// Play menu
			OptionsButton.transform.localPosition = new Vector2(OptionsButton.transform.localPosition.x + 80,
				OptionsButton.transform.localPosition.y);
			FPSLabel.transform.localPosition = new Vector2(FPSLabel.transform.localPosition.x - 85,
				FPSLabel.transform.localPosition.y);
			CreditsButton.transform.localPosition = new Vector2(CreditsButton.transform.localPosition.x - 85,
				CreditsButton.transform.localPosition.y);
			// Credits menu
			CreditsBackButton.transform.localPosition = new Vector2(CreditsBackButton.transform.localPosition.x + 80,
				CreditsBackButton.transform.localPosition.y);
			CreditsPage1.transform.localScale = new Vector2(0.95f, 0.95f);
			CreditsTestersTitleLabel.transform.localPosition = new Vector2(CreditsTestersTitleLabel.transform.localPosition.x,
				CreditsTestersTitleLabel.transform.localPosition.y + 40);
			CreditsTestersLabel.lineWidth = 900;
			CreditsTestersLabel.transform.localPosition = new Vector2(CreditsTestersLabel.transform.localPosition.x,
				CreditsTestersLabel.transform.localPosition.y - 20);
			// Options menu
			OptionsBackButton.transform.localPosition = new Vector2(OptionsBackButton.transform.localPosition.x + 80,
				OptionsBackButton.transform.localPosition.y);
			// Mode menu
			ModeBackButton.transform.localPosition = new Vector2(ModeBackButton.transform.localPosition.x + 80,
				ModeBackButton.transform.localPosition.y);
			// Login menu
			LoginBackButton.transform.localPosition = new Vector2(-210, 154);
			// Forgot password menu
			ForgotBackButton.transform.localPosition = new Vector2(-220, 154);
			ForgotBackButton.transform.localScale = new Vector2(0.8f, 0.8f);
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
			CreditsButton.transform.localPosition = new Vector2(CreditsButton.transform.localPosition.x - 40,
				CreditsButton.transform.localPosition.y);
			// Credits menu
			CreditsBackButton.transform.localPosition = new Vector2(CreditsBackButton.transform.localPosition.x + 45,
				CreditsBackButton.transform.localPosition.y);
			CreditsPage1.transform.localScale = new Vector2(0.98f, 0.98f);
			CreditsTestersLabel.lineWidth = 1100;
			CreditsTestersLabel.transform.localPosition = new Vector2(CreditsTestersLabel.transform.localPosition.x,
				CreditsTestersLabel.transform.localPosition.y - 20);
			// Options menu
			OptionsBackButton.transform.localPosition = new Vector2(OptionsBackButton.transform.localPosition.x + 45,
				OptionsBackButton.transform.localPosition.y);
			// Mode menu
			ModeBackButton.transform.localPosition = new Vector2(ModeBackButton.transform.localPosition.x + 45,
				ModeBackButton.transform.localPosition.y);
			// Login menu
			LoginBackButton.transform.localPosition = new Vector2(LoginBackButton.transform.localPosition.x + 35,
				LoginBackButton.transform.localPosition.y);
			// Forgot pass menu
			ForgotBackButton.transform.localPosition = new Vector2(ForgotBackButton.transform.localPosition.x + 35,
				ForgotBackButton.transform.localPosition.y);
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
			CreditsButton.transform.localPosition = new Vector2(CreditsButton.transform.localPosition.x - 15,
				CreditsButton.transform.localPosition.y);
			// Options menu
			OptionsBackButton.transform.localPosition = new Vector2(OptionsBackButton.transform.localPosition.x + 20,
				OptionsBackButton.transform.localPosition.y);
			// Credits menu
			CreditsBackButton.transform.localPosition = new Vector2(CreditsBackButton.transform.localPosition.x + 20,
				CreditsBackButton.transform.localPosition.y);
			// Mode menu
			ModeBackButton.transform.localPosition = new Vector2(ModeBackButton.transform.localPosition.x + 20,
				ModeBackButton.transform.localPosition.y);
			// Login menu
			LoginBackButton.transform.localPosition = new Vector2(LoginBackButton.transform.localPosition.x + 10,
				LoginBackButton.transform.localPosition.y);
			// Forgot back menu
			ForgotBackButton.transform.localPosition = new Vector2(ForgotBackButton.transform.localPosition.x + 10,
				ForgotBackButton.transform.localPosition.y);
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
			else if (CurrentMenuLevel == MenuLevel.Credits) OnCreditsBackClicked();
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
		if (MainMenuScoresOfflinePanel.enabled) UITools.SetActiveState(MainMenuScoresOfflinePanel, false);
		if (MainMenuLoggedInBoxPanel.enabled) UITools.SetActiveState(MainMenuLoggedInBoxPanel, false);
		if (MainMenuLogInPanel.enabled) UITools.SetActiveState(MainMenuLogInPanel, false);
		if (MainMenuFileBrowserPanel.enabled) UITools.SetActiveState(MainMenuFileBrowserPanel, false);
		if (MainMenuBasePanel.enabled) UITools.SetActiveState(MainMenuBasePanel, false);
		if (MainMenuPlayPanel.enabled) UITools.SetActiveState(MainMenuPlayPanel, false);
		if (MainMenuExtrasPanel.enabled) UITools.SetActiveState(MainMenuExtrasPanel, false);
		if (MainMenuChoicePanel.enabled) UITools.SetActiveState(MainMenuChoicePanel, false);
		if (MainMenuSongNotFoundPanel.enabled) UITools.SetActiveState(MainMenuSongNotFoundPanel, false);
		if (MainMenuCreatePanel.enabled) UITools.SetActiveState(MainMenuCreatePanel, false);
		if (MainMenuCreditsPanel.enabled) UITools.SetActiveState(MainMenuCreditsPanel, false);
		if (MainMenuCredits3DPanel.enabled) UITools.SetActiveState(MainMenuCredits3DPanel, false);
		if (MainMenuForgotPanel.enabled) UITools.SetActiveState(MainMenuForgotPanel, false);
		if (MainMenuForgotMessagePanel.enabled) UITools.SetActiveState(MainMenuForgotMessagePanel, false);
		if (MainMenuAnalysisNotePanel.enabled) UITools.SetActiveState(MainMenuAnalysisNotePanel, false);
		if (MainMenuFirstPlayPanel.enabled) UITools.SetActiveState(MainMenuFirstPlayPanel, false);
		if (MainMenuGoOnlinePanel.enabled) UITools.SetActiveState(MainMenuGoOnlinePanel, false);
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
				if (MainMenuScoresOfflinePanel.enabled) UITools.SetActiveState(MainMenuScoresOfflinePanel, false);
				if (MainMenuLoggedInBoxPanel.enabled) UITools.SetActiveState(MainMenuLoggedInBoxPanel, false);
				if (MainMenuLogInPanel.enabled) UITools.SetActiveState(MainMenuLogInPanel, false);
				if (MainMenuFileBrowserPanel.enabled) UITools.SetActiveState(MainMenuFileBrowserPanel, false);
				if (MainMenuChoicePanel.enabled) UITools.SetActiveState(MainMenuChoicePanel, false);
				if (MainMenuCreatePanel.enabled) UITools.SetActiveState(MainMenuCreatePanel, false);
				if (MainMenuCreditsPanel.enabled) UITools.SetActiveState(MainMenuCreditsPanel, false);
				if (MainMenuCredits3DPanel.enabled) UITools.SetActiveState(MainMenuCredits3DPanel, false);
				if (MainMenuForgotPanel.enabled) UITools.SetActiveState(MainMenuForgotPanel, false);
				if (MainMenuForgotMessagePanel.enabled) UITools.SetActiveState(MainMenuForgotMessagePanel, false);
				if (MainMenuAnalysisNotePanel.enabled) UITools.SetActiveState(MainMenuAnalysisNotePanel, false);
				if (MainMenuFirstPlayPanel.enabled) UITools.SetActiveState(MainMenuFirstPlayPanel, false);
				if (MainMenuGoOnlinePanel.enabled) UITools.SetActiveState(MainMenuGoOnlinePanel, false);
				UITools.SetActiveState(MainMenuExtrasPanel, true);
				UITools.SetActiveState(MainMenuBasePanel, true);
				UITools.SetActiveState(MainMenuPlayPanel, true);
				break;
			case MenuLevel.Mode:
				if (PlayerPrefs.HasKey("FirstPlay")) {
					if (PlayerPrefs.GetInt("FirstPlay") == 1)
						UITools.SetActiveState(MainMenuFirstPlayPanel, true);
				}
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
				if (Game.IsLoggedIn && Game.OnlineMode) UITools.SetActiveState(MainMenuLoggedInBoxPanel, true);
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
				UITools.SetActiveState(MainMenuGoOnlinePanel, false);
				UITools.SetActiveState(MainMenuScoresPanel, true);
				HSController.InitHSDisplay();
				break;
			case MenuLevel.ScoresOffline:
				UITools.SetActiveState(MainMenuPlayPanel, false);
				UITools.SetActiveState(MainMenuBasePanel, false);
				UITools.SetActiveState(MainMenuGoOnlinePanel, false);
				UITools.SetActiveState(MainMenuScoresOfflinePanel, true);
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
				UITools.SetActiveState(MainMenuCreatePanel, false);
				UITools.SetActiveState(MainMenuPlayPanel, false);
				UITools.SetActiveState(MainMenuBasePanel, false);
				UITools.SetActiveState(MainMenuForgotPanel, false);
				UITools.SetActiveState(MainMenuGoOnlinePanel, false);
				UITools.SetActiveState(MainMenuLogInPanel, true);
				if (PlayerPrefs.GetString("playername") != null)
					UserNameInput.text = PlayerPrefs.GetString("playername");
				RememberMeCheckbox.isChecked = Game.RememberLogin;
				if (ErrorLabel.text != "[44DD44]User created, please log in!") ErrorLabel.text = ""; // Clear error text if any
				LoginPassInput.text = ""; // Clear password field
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
				foreach (UIButton c in MainMenuChoicePanel.GetComponentsInChildren<UIButton>()) StartCoroutine(DelayButton(c, 0.1f));
				break;
			case MenuLevel.SongNotFound:
				if (MainMenuBasePanel.enabled) UITools.SetActiveState(MainMenuBasePanel, false);
				if (MainMenuFileBrowserPanel.enabled) UITools.SetActiveState(MainMenuFileBrowserPanel, false);
				if (MainMenuModePanel.enabled) UITools.SetActiveState(MainMenuModePanel, false);
				if (MainMenuExtrasPanel.enabled) UITools.SetActiveState(MainMenuExtrasPanel, false);
				UITools.SetActiveState(MainMenuSongNotFoundPanel, true);
				UITools.SetActiveState(MainMenuExtrasPanel, true);
				foreach (UIButton c in MainMenuChoicePanel.GetComponentsInChildren<UIButton>()) StartCoroutine(DelayButton(c, 0.1f));
				break;
			case MenuLevel.Create:
				UITools.SetActiveState(MainMenuLogInPanel, false);
				UITools.SetActiveState(MainMenuCreatePanel, true);
				CreateErrorLabel.text = "";
				break;
			case MenuLevel.Credits:
				UITools.SetActiveState(MainMenuPlayPanel, false);
				UITools.SetActiveState(MainMenuCreditsPanel, true);
				UITools.SetActiveState(MainMenuCredits3DPanel, true);
				StartCredits();
				break;
			case MenuLevel.Forgot:
				UITools.SetActiveState(MainMenuLogInPanel, false);
				UITools.SetActiveState(MainMenuForgotPanel, true);
				ForgotErrorLabel.text = "";
				break;
			case MenuLevel.ForgotMessage:
				UITools.SetActiveState(MainMenuForgotPanel, false);
				UITools.SetActiveState(MainMenuForgotMessagePanel, true);
				ForgotMessageLabel.text = "An email was sent to [44CCBB]" + globalEmail + " [FFFFFF]with instructions on how to reset your password!";
				break;
			case MenuLevel.GoOnlineWindow:
				OptionsButton.isEnabled = false;
				CreditsButton.isEnabled = false;
				UITools.SetActiveState(MainMenuGoOnlinePanel, true);
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
		if (Game.AskOnlineMode)
			ChangeMenu(MenuLevel.GoOnlineWindow);
		else if (((!Game.RememberLogin && !Game.IsLoggedIn) || Game.PlayerName.Length < 1) && Game.OnlineMode)
			ChangeMenu(MenuLevel.LogIn);
		else if (!Game.OnlineMode)
			ChangeMenu(MenuLevel.ScoresOffline);
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

	void OnCreditsClicked() {
		ChangeMenu(MenuLevel.Credits);
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
		if (PlayerPrefs.HasKey("FirstNote")) {
			if (PlayerPrefs.GetInt("FirstNote") == 1) {
				UITools.SetActiveState(MainMenuChoicePanel, false);
				UITools.SetActiveState(MainMenuAnalysisNotePanel, true);
			} else {
				ClearMenu();
				Application.LoadLevel("LoadScreen");
			}
		} else {
			ClearMenu();
			Application.LoadLevel("LoadScreen");
		}
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
			if (Application.platform == RuntimePlatform.WindowsEditor) {
				if (!string.IsNullOrEmpty(Game.Path)) FileBrowser.SendMessage("OpenFileWindow", PlayerPrefs.GetString("filePath"));
				else FileBrowser.SendMessage("OpenFileWindow", "");
			} else if (Application.platform == RuntimePlatform.Android) {
				FileBrowser.SendMessage("FetchLetters");
			}
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
			if (Application.platform == RuntimePlatform.WindowsEditor) {
				if (!string.IsNullOrEmpty(Game.Path)) FileBrowser.SendMessage("OpenFileWindow", PlayerPrefs.GetString("filePath"));
				else FileBrowser.SendMessage("OpenFileWindow", "");
			} else if (Application.platform == RuntimePlatform.Android) {
				FileBrowser.SendMessage("FetchLetters");
			}
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

	//private void OnLowGraphicsButtonClicked() {
	//    bool tempValue = Game.LowGraphicsMode;
	//    Game.LowGraphicsMode = !tempValue;
	//    LowGraphicsSettingLabel.text = !tempValue ? "[44ff44]On" : "[FF4444]Off";
	//}

	private void OnOnlineModeButtonClicked() {
		bool tempValue = Game.OnlineMode;
		Game.OnlineMode = !tempValue;
		OnlineModeSettingLabel.text = !tempValue ? "[44ff44]On" : "[FF4444]Off";
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
		string password = LoginPassInput.text;
		if (playerName.Length < 2) {                            // Error if input text is too short
			UserNameInput.text = "";
			ErrorLabel.text = "[F87431]Name too short. Try again";
			return;
		} else if (!nameRegEx.IsMatch(playerName)) {
			UserNameInput.text = "";
			ErrorLabel.text = "[F87431]Invalid. Only letters & numbers allowed.";
			return;
		}
		if (password.Length < 5) {
			LoginPassInput.text = "";
			ErrorLabel.text = "[F87431]Password too short. Try again";
			return;
		}
		password = MD5Utils.MD5FromString(password + passHashKey);
		StartCoroutine(startLogin(playerName, password));
	}
	void OnLoginCreateClicked() {
		ChangeMenu(MenuLevel.Create);
	}
	void OnLoginForgotClicked() {
		ChangeMenu(MenuLevel.Forgot);
	}
	#endregion

	#region Login menu functions
	IEnumerator startLogin(string user, string password) {
		yield return StartCoroutine(HSController.CheckUser(user, password));
		if (LoginSuccess) {
			Game.IsLoggedIn = true;
			// Store the new player name
			PlayerPrefs.SetString("playername", user);
			// Change the menu
			ChangeMenu(MenuLevel.Scores);
			LoginSuccess = false;
		}
	}

	public static void SetLoginErrorLabel(string text) {
		instance.ErrorLabel.text = text;
	}
	#endregion

	#region FirstPlay buttons
	void OnFirstYesClicked() {
		PlayerPrefs.SetInt("FirstPlay", 0);
		OnTutorialButtonClicked();
		UITools.SetActiveState(MainMenuFirstPlayPanel, false);
	}

	void OnFirstNoClicked() {
		UITools.SetActiveState(MainMenuFirstPlayPanel, false);
		PlayerPrefs.SetInt("FirstPlay", 0);
	}
	#endregion

	#region AnaylsisNote buttons
	void OnAnalysisOkayClicked() {
		PlayerPrefs.SetInt("FirstNote", 0);
		ClearMenu();
		Application.LoadLevel("LoadScreen");
	}
	#endregion

	#region Forgot Pass buttons
	void OnForgotBackClicked() {
		ChangeMenu(MenuLevel.LogIn);
	}
	void OnForgotSubmitClicked() {
		string name = ForgotNameInput.text;
		string email = ForgotEmailInput.text;
		if (name.Length < 2) {
			ForgotNameInput.text = "";
			ForgotErrorLabel.text = "Username too short";
		} else if (email.Length < 1) {
			ForgotEmailInput.text = "";
			ForgotErrorLabel.text = "Email field can't be empty";
		} else {
			ForgotButton.isEnabled = false;
			ForgotButtonLabel.text = "Loading...";
			StartCoroutine(forgotPasswordCheck(name, email));
		}
	}
	void OnForgotOkClicked() {
		ChangeMenu(MenuLevel.Base);
	}
	#endregion

	#region Forgot Pass menu functions
	IEnumerator forgotPasswordCheck(string name, string email) {
		yield return StartCoroutine(HSController.CheckEmail(name, email));
		if (CorrectEmail) {
			CorrectEmail = false;
			yield return StartCoroutine(HSController.SendEmail(name));
		} else {
			ForgotButton.isEnabled = true;
			ForgotButtonLabel.text = "Request new password";
			Debug.Log("Wrong email");
		}
		ForgotButton.isEnabled = true;
		ForgotButtonLabel.text = "Request new password";
		if (EmailSent) {
			EmailSent = false;
			globalEmail = email;
			ChangeMenu(MenuLevel.ForgotMessage);
		} else {
			Debug.Log("Error sending email");
		}
	}
	public static void SetForgotErrorLabel(string text) {
		instance.ForgotErrorLabel.text = text;
	}
	#endregion

	#region Create Menu Buttons
	void OnCreateClicked() {
		string name = CreateNameInput.text;
		string password = CreatePassInput.text;
		string email = CreateEmailInput.text;
		if (name.Length < 2) {
			CreateNameInput.text = "";
			CreateErrorLabel.text = "Username too short";
			return;
		} else if (!nameRegEx.IsMatch(name)) {
			CreateNameInput.text = "";
			CreateErrorLabel.text = "Invalid username, use letters & numbers!";
			return;
		} else if (CreatePassInput.text != CreatePass2Input.text) {
			CreatePassInput.text = "";
			CreatePass2Input.text = "";
			CreateErrorLabel.text = "Passwords don't match";
			return;
		} else if (CreatePassInput.text.Length < 5) {
			CreatePassInput.text = "";
			CreatePass2Input.text = "";
			CreateErrorLabel.text = "Password is too short!";
			return;
		} else if (CreateEmailInput.text.Length < 1) {
			CreateEmailInput.text = "";
			CreateErrorLabel.text = "Insert e-mail for password recovery!";
		} else {
			password = MD5Utils.MD5FromString(password + passHashKey);	// Encrypt password
			StartCoroutine(addNewUser(name, password, email));	// Add user
		}
	}
	void OnCreateBackClicked() {
		ChangeMenu(MenuLevel.LogIn);
	}
	#endregion

	#region Create menu functions
	IEnumerator addNewUser(string name, string password, string email) {
		yield return StartCoroutine(HSController.AddUser(name, password, email));
		if (UserCreated) {
			PlayerPrefs.SetString("playername", name);
			ErrorLabel.text = "[44DD44]User created, please log in!";
			ChangeMenu(MenuLevel.LogIn);
			UserCreated = false;
		}
	}
	public static void SetCreateErrorLabel(string text) {
		instance.CreateErrorLabel.text = text;
	}
	public static void SetCreateButtonLabel(string text) {
		instance.CreateButtonLabel.text = text;
	}
	#endregion

	#region Highscore buttons
	void OnNextSongClicked() {
		if (Game.OnlineMode)
			HSController.LoadNextSong();
		else
			HSController.LoadNextSongOffline();
	}

	void OnPrevSongClicked() {
		if (Game.OnlineMode)
			HSController.LoadPrevSong();
		else
			HSController.LoadPrevSongOffline();
	}

	void OnPrevModeClicked() {
		HSController.LoadPrevMode();
	}

	void OnNextModeClicked() {
		HSController.LoadNextMode();
	}
	#endregion

	#region Credit menu buttons
	void OnCreditsBackClicked() {
		StopCredits();
		ChangeMenu(MenuLevel.Base);
	}
	void OnCreditsScreenClicked() {
		StopCoroutine("PageTimer");
		ChangeCreditsPage(((creditsCurrentPage) % 5) + 1);
	}
	#endregion

	#region Credit menu functions
	void StartCredits() {
		StopCoroutine("PageTimer");
		creditsCurrentPage = 1;
		for (int i = 0; i < PageAnimations.Length; i++)
			PageAnimations[i].Blend("Page" + (i + 1) + "-Float");
		StartCoroutine("PageTimer");
	}
	void StopCredits() {
		StopCoroutine("PageTimer");
		SliderAnimation.gameObject.transform.localPosition = Vector3.zero;
		for (int i = 0; i < PageAnimations.Length; i++)
			PageAnimations[i].Stop("Page" + (i + 1) + "-Float");
	}
	IEnumerator PageTimer() {
		yield return new WaitForSeconds(5f);
		switch (creditsCurrentPage) {
			case 1:
				ChangeCreditsPage(2);
				break;
			case 2:
				ChangeCreditsPage(3);
				break;
			case 3:
				ChangeCreditsPage(4);
				break;
			case 4:
				ChangeCreditsPage(5);
				break;
			case 5:
				ChangeCreditsPage(1);
				break;
			default:
				break;
		}
	}
	void ChangeCreditsPage(int pageNr) {
		if (pageNr > 1 && pageNr < 6) {
			SliderAnimation.Blend("Slide" + (pageNr - 1) + "-" + pageNr);
		} else if (pageNr == 1)
			SliderAnimation.Blend("Slide5-1");
		creditsCurrentPage = pageNr;
		StartCoroutine("PageTimer");
	}
	#endregion

	#region Go Online Window buttons
	void OnGoOnlineYesClicked() {
		Game.OnlineMode = true;
		OptionsButton.isEnabled = true;
		CreditsButton.isEnabled = true;
		if ((!Game.RememberLogin && !Game.IsLoggedIn) || Game.PlayerName.Length < 1)
			ChangeMenu(MenuLevel.LogIn);
		else
			ChangeMenu(MenuLevel.Scores);
	}

	void OnGoOnlineNoClicked() {
		OptionsButton.isEnabled = true;
		CreditsButton.isEnabled = true;
		Game.OnlineMode = false;
		ChangeMenu(MenuLevel.ScoresOffline);
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
		OnlineModeSettingLabel.text = Game.OnlineMode ? "[44ff44]On" : "[FF4444]Off";
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
	ScoresOffline,
	Options,
	FileBrowser,
	LogIn,
	ConfirmChoice,
	SongNotFound,
	Create,
	Credits,
	Forgot,
	ForgotMessage,
	GoOnlineWindow
}