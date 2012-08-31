using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using System;
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
	// Panels
	public UIPanel TitlePanel;
	public UIPanel BasePanel;
	public UIPanel LoginPanel;
	public UIPanel CreatePanel;
	public UIPanel ScoresPanel;
	public UIPanel ScoresOfflinePanel;
	public UIPanel ScoresTitlePanel;
	public UIPanel ForgotPassPanel;
	public UIPanel ForgotPassMessagePanel;
	public UIPanel GoOnlinePanel;
	public UIPanel RetrySubmitPanel;
	// Base Menu Objects
	public UILabel SongLabel;
	public UILabel HitsValueLabel;
	public UILabel MissesValueLabel;
	public UILabel ScoreValueLabel;
	public UILabel RankValueLabel;
	public UIButton HighscoreButton;
	public UIButton ReplaySongButton;
	public UIButton MainMenuButton;
	// Retry Menu Objects
	public UILabel RetryScoreValueLabel;
	public UILabel RetryRankValueLabel;
//	public UIButton RetryHighscoreButton;
//	public UIButton RetryReplaySongButton;
//	public UIButton RetryMainMenuButton;
	// Login Menu Objects
	public UIInput UserNameInput;
	public UIInput PasswordInput;
	public UILabel ErrorLabel;
	public UICheckbox RememberMeCheckbox;
	public UIButton LoginBackButton;
	private string passHashKey = "r2d2imahorse";
	public static bool LoginSuccess;
	// Create Menu Objects
	public UILabel CreateErrorLabel;
	public UILabel CreateButtonLabel;
	public UIButton CreateBackButton;
	public UIInput CreateNameInput;
	public UIInput CreatePass1Input;
	public UIInput CreatePass2Input;
	public UIInput CreateEmailInput;
	public static bool UserCreated;
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
	// Scores Menu Objects
	public UILabel SongNameLabel;
	public UILabel SongModeLabel;
	public UILabel TopScoreLabel;
	public UILabel YourScoreLabel;
	public UIButton ScoresMainMenuButton;
	public UIButton ScoresReplayButton;
	public GameObject YSNames;
	public GameObject TSNames;
	public GameObject YSScores;
	public GameObject TSScores;
	public GameObject VerticalDivider;
	public GameObject HorizontalDivider;
	public GameObject TopYoursDivider;
	// Scores Loading Objects
	public UILabel SubmittingScoreLabel;

	private WinMenuLevel currentMenuLevel;

	private static Win instance;
	private Regex nameRegEx = new Regex("^[a-zA-Z0-9]*$");
	#endregion

	#region Functions
	void Awake() {
		Game.GameState = Game.State.Menu;
		ChangeMenu(WinMenuLevel.Base);
	}

	void Start() {
		instance = this;

		float aspect = Camera.mainCamera.aspect;
		//Debug.Log(aspect);

		// Set up the menu for different aspect ratios
		// Aspect ratio: 1.333..
		if (aspect <= 640.0f / 480.0f) {
			//Debug.Log("Setting up for aspect: " + aspect);
			// Base menu
			HighscoreButton.transform.localScale = new Vector2(0.7f, 0.7f);
			MainMenuButton.transform.localScale = new Vector2(0.9f, 0.9f);
			ReplaySongButton.transform.localScale = new Vector2(0.9f, 0.9f);
			MainMenuButton.transform.localPosition = new Vector2(MainMenuButton.transform.localPosition.x + 80,
																 MainMenuButton.transform.localPosition.y);
			ReplaySongButton.transform.localPosition = new Vector2(ReplaySongButton.transform.localPosition.x - 80,
																 ReplaySongButton.transform.localPosition.y);
			SongLabel.lineWidth = 600;
			// Login menu
			LoginBackButton.transform.localPosition = new Vector2(-209, 154);
			// Create menu
			CreateBackButton.transform.localPosition = new Vector2(-209, 184);
			// Forgot password menu
			ForgotBackButton.transform.localPosition = new Vector2(-220, 154);
			ForgotBackButton.transform.localScale = new Vector2(0.8f, 0.8f);
			// Scores menu
			SongNameLabel.lineWidth = 520;
			TSNames.transform.localPosition = new Vector2(30, 0);
			YSNames.transform.localPosition = new Vector2(30, 0);
			TSScores.transform.localPosition = new Vector2(-30, 0);
			YSScores.transform.localPosition = new Vector2(-30, 0);
			VerticalDivider.transform.localPosition = new Vector3(VerticalDivider.transform.localPosition.x + 30,
				VerticalDivider.transform.localPosition.y);
			HorizontalDivider.transform.localScale = new Vector2(HorizontalDivider.transform.localScale.x - 60,
				HorizontalDivider.transform.localScale.y);
			TopYoursDivider.transform.localScale = new Vector2(TopYoursDivider.transform.localScale.x - 60,
				TopYoursDivider.transform.localScale.y);
			TopScoreLabel.transform.localPosition = new Vector2(-204, 55);
			YourScoreLabel.transform.localPosition = new Vector2(-204, 7);
			TopScoreLabel.transform.localScale = new Vector2(22, 24);
			YourScoreLabel.transform.localScale = new Vector2(22, 24);
			ScoresMainMenuButton.transform.localScale = new Vector2(0.8f, 0.8f);
			ScoresMainMenuButton.transform.localPosition = new Vector2(ScoresMainMenuButton.transform.localPosition.x + 60,
				ScoresMainMenuButton.transform.localPosition.y);
			ScoresReplayButton.transform.localScale = new Vector2(0.8f, 0.8f);
			ScoresReplayButton.transform.localPosition = new Vector2(ScoresReplayButton.transform.localPosition.x - 60,
				ScoresReplayButton.transform.localPosition.y);
		}
			// Aspect ratio: 1.5
		else if (aspect == (480.0f / 320.0f)) {
			//Debug.Log("Setting up for aspect: " + aspect);
			// Base menu
			HighscoreButton.transform.localScale = new Vector2(0.8f, 0.8f);
			MainMenuButton.transform.localPosition = new Vector2(MainMenuButton.transform.localPosition.x + 40,
																 MainMenuButton.transform.localPosition.y);
			ReplaySongButton.transform.localPosition = new Vector2(ReplaySongButton.transform.localPosition.x - 40,
																 ReplaySongButton.transform.localPosition.y);
			SongLabel.lineWidth = 650;
			// Login menu
			LoginBackButton.transform.localPosition = new Vector2(LoginBackButton.transform.localPosition.x + 35,
																  LoginBackButton.transform.localPosition.y);
			// Create menu
			CreateBackButton.transform.localPosition = new Vector2(CreateBackButton.transform.localPosition.x + 35,
																  CreateBackButton.transform.localPosition.y);
			// Forgot pass menu
			ForgotBackButton.transform.localPosition = new Vector2(ForgotBackButton.transform.localPosition.x + 35,
				ForgotBackButton.transform.localPosition.y);
			// Scores menu
			SongNameLabel.lineWidth = 600;
			TopScoreLabel.transform.localScale = new Vector2(24, 24);
			YourScoreLabel.transform.localScale = new Vector2(24, 24);
			ScoresMainMenuButton.transform.localScale = new Vector2(0.9f, 0.9f);
			ScoresMainMenuButton.transform.localPosition = new Vector2(ScoresMainMenuButton.transform.localPosition.x + 25,
				ScoresMainMenuButton.transform.localPosition.y);
			ScoresReplayButton.transform.localScale = new Vector2(0.9f, 0.9f);
			ScoresReplayButton.transform.localPosition = new Vector2(ScoresReplayButton.transform.localPosition.x - 25,
				ScoresReplayButton.transform.localPosition.y);
		}
			// Aspect ratio: 1.6
		else if (aspect == (1280.0f / 800.0f)) {
			//Debug.Log("Setting up for aspect: " + aspect);
			// Base menu
			HighscoreButton.transform.localScale = new Vector2(0.9f, 0.9f);
			MainMenuButton.transform.localPosition = new Vector2(MainMenuButton.transform.localPosition.x + 25,
																 MainMenuButton.transform.localPosition.y);
			ReplaySongButton.transform.localPosition = new Vector2(ReplaySongButton.transform.localPosition.x - 25,
																 ReplaySongButton.transform.localPosition.y);
			// Login menu
			LoginBackButton.transform.localPosition = new Vector2(LoginBackButton.transform.localPosition.x + 25,
																  LoginBackButton.transform.localPosition.y);
			// Create menu
			CreateBackButton.transform.localPosition = new Vector2(CreateBackButton.transform.localPosition.x + 25,
																  CreateBackButton.transform.localPosition.y);
			// Forgot back menu
			ForgotBackButton.transform.localPosition = new Vector2(ForgotBackButton.transform.localPosition.x + 25,
				ForgotBackButton.transform.localPosition.y);
			// Scores menu
			ScoresMainMenuButton.transform.localScale = new Vector2(0.9f, 0.9f);
			ScoresMainMenuButton.transform.localPosition = new Vector2(ScoresMainMenuButton.transform.localPosition.x + 10,
				ScoresMainMenuButton.transform.localPosition.y);
			ScoresReplayButton.transform.localScale = new Vector2(0.9f, 0.9f);
			ScoresReplayButton.transform.localPosition = new Vector2(ScoresReplayButton.transform.localPosition.x - 10,
				ScoresReplayButton.transform.localPosition.y);
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
		
		SongLabel.text = AudioManager.artist + " - " + AudioManager.title;
		CalculateSongLabelSize();
		HitsValueLabel.text = Player.TotalKills.ToString();
		MissesValueLabel.text = (EnemySpawnScript.spawnCount - Player.TotalKills).ToString();
		ScoreValueLabel.text = Player.score.ToString();
		RetryScoreValueLabel.text = ScoreValueLabel.text;
		RankValueLabel.text = CalculatedRank;
		RetryRankValueLabel.text = RankValueLabel.text;
		if (Game.Cheated) {
			HighscoreButton.enabled = false;
			HighscoreButton.GetComponentInChildren<UILabel>().text = "[FF2222]You cheater!";
		}
		UITools.SetActiveState(RetrySubmitPanel, false);
	}
	/// <summary>
	/// Returns the final calculated score after the end of the game.
	/// </summary>
	/// <value>Calculated rank (total hits / total spawns)</value>
	string CalculatedRank {
		get {
			if (Player.TotalKills > 0) {
				float valueOneWeight = 0.5f;
				float valueTwoWeight = 0.5f;
				int valueOne = (int)((Player.TotalKills / (float)EnemySpawnScript.spawnCount) * 100);
				//Debug.Log(Player.TotalKills + " / " + (float)EnemySpawnScript.spawnCount + " = " + (Player.TotalKills / (float)EnemySpawnScript.spawnCount));
				int valueTwo = 0;
				if (EnemySpawnScript.spawnCount > (Player.MultiplierKillDivisor * Player.MaxMultiplier)) {
					valueTwo = (EnemySpawnScript.spawnCount * Player.MaxMultiplier * 10);
					for (int cnt = 1; cnt <= Player.MaxMultiplier - 1; cnt++)
						valueTwo -= Player.MultiplierKillDivisor * cnt * 10;
					valueTwo = (int)((Player.score / (float)valueTwo) * 100);
				} else {
					int localMaxMultiplier = (EnemySpawnScript.spawnCount / Player.MultiplierKillDivisor) + 1;
					for (int cnt = 1; cnt <= localMaxMultiplier - 1; cnt++)
						valueTwo += Player.MultiplierKillDivisor * cnt * 10;
					valueTwo = valueTwo + ((EnemySpawnScript.spawnCount - ((localMaxMultiplier - 1) * 6)) * localMaxMultiplier * 10);
					valueTwo = (int)((Player.score / (float)valueTwo) * 100);
				}
				Debug.Log("High Rank: " + valueOne + " : Low Rank: " + valueTwo);
				int value = (int)(valueOne * valueOneWeight + valueTwo * valueTwoWeight);
				return "[33FF33]" + value + "%";
			} else
				return "[FF3333]0%";
		}
	}

	void Update() {
		// When the player presses "Escape" or "Back" on Android, returns to main menu screen
		// or goes to the quit menu if on the main menu already.
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (currentMenuLevel == WinMenuLevel.GoOnlineWindow) ChangeMenu(WinMenuLevel.Base);
			else OnMainMenuClicked();
		}
	}

	private void ChangeMenu(WinMenuLevel menuLevel) {
		currentMenuLevel = menuLevel;
		switch (currentMenuLevel) {
			case WinMenuLevel.Base:
				if (RetrySubmitPanel.enabled) UITools.SetActiveState(RetrySubmitPanel, false);
				if (CreatePanel.enabled) UITools.SetActiveState(CreatePanel, false);
				if (LoginPanel.enabled) UITools.SetActiveState(LoginPanel, false);
				if (ScoresPanel.enabled) UITools.SetActiveState(ScoresPanel, false);
				if (ScoresOfflinePanel.enabled) UITools.SetActiveState(ScoresOfflinePanel, false);
				if (ScoresTitlePanel.enabled) UITools.SetActiveState(ScoresTitlePanel, false);
				if (ForgotPassPanel.enabled) UITools.SetActiveState(ForgotPassPanel, false);
				if (ForgotPassMessagePanel.enabled) UITools.SetActiveState(ForgotPassMessagePanel, false);
				if (GoOnlinePanel.enabled) UITools.SetActiveState(GoOnlinePanel, false);
				UITools.SetActiveState(TitlePanel, true);
				UITools.SetActiveState(BasePanel, true);
				break;
			case WinMenuLevel.Create:
				if(RetrySubmitPanel.enabled) UITools.SetActiveState(RetrySubmitPanel, false);
				UITools.SetActiveState(LoginPanel, false);
				UITools.SetActiveState(CreatePanel, true);
				CreateErrorLabel.text = "";
				break;
			case WinMenuLevel.Login:
				if(RetrySubmitPanel.enabled) UITools.SetActiveState(RetrySubmitPanel, false);
				UITools.SetActiveState(CreatePanel, false);
				UITools.SetActiveState(TitlePanel, false);
				UITools.SetActiveState(ForgotPassPanel, false);
				UITools.SetActiveState(BasePanel, false);
				UITools.SetActiveState(GoOnlinePanel, false);
				UITools.SetActiveState(LoginPanel, true);
				if (PlayerPrefs.GetString("playername") != null)
					UserNameInput.text = PlayerPrefs.GetString("playername");
				RememberMeCheckbox.isChecked = Game.RememberLogin;
				if (ErrorLabel.text != "[44DD44]User created, please log in!") ErrorLabel.text = ""; // Clear error text any
				PasswordInput.text = ""; // Clear password field
				break;
			case WinMenuLevel.Scores:
				if(RetrySubmitPanel.enabled) UITools.SetActiveState(RetrySubmitPanel, false);
				if (BasePanel.enabled) UITools.SetActiveState(BasePanel, false);
				if (CreatePanel.enabled) UITools.SetActiveState(CreatePanel, false);
				if (LoginPanel.enabled) UITools.SetActiveState(LoginPanel, false);
				if (TitlePanel.enabled) UITools.SetActiveState(TitlePanel, false);
				UITools.SetActiveState(GoOnlinePanel, false);
				UITools.SetActiveState(ScoresPanel, true);
				break;
			case WinMenuLevel.ScoresOffline:
				if(RetrySubmitPanel.enabled) UITools.SetActiveState(RetrySubmitPanel, false);
				if (BasePanel.enabled) UITools.SetActiveState(BasePanel, false);
				if (CreatePanel.enabled) UITools.SetActiveState(CreatePanel, false);
				if (TitlePanel.enabled) UITools.SetActiveState(TitlePanel, false);
				UITools.SetActiveState(GoOnlinePanel, false);
				SongNameLabel.text = AudioManager.artist + " - " + AudioManager.title;
				SongModeLabel.text = Game.GameMode.ToString();
				UITools.SetActiveState(ScoresTitlePanel, true);
				UITools.SetActiveState(ScoresOfflinePanel, true);
				break;
			case WinMenuLevel.LoadingScores:
				if(RetrySubmitPanel.enabled) UITools.SetActiveState(RetrySubmitPanel, false);
				if (BasePanel.enabled) UITools.SetActiveState(BasePanel, false);
				if (CreatePanel.enabled) UITools.SetActiveState(CreatePanel, false);
				if (LoginPanel.enabled) UITools.SetActiveState(LoginPanel, false);
				if (TitlePanel.enabled) UITools.SetActiveState(TitlePanel, false);
				UITools.SetActiveState(GoOnlinePanel, false);
				SongNameLabel.text = AudioManager.artist + " - " + AudioManager.title;
				SongModeLabel.text = Game.GameMode.ToString();
				UITools.SetActiveState(ScoresTitlePanel, true);
				break;
			case WinMenuLevel.Forgot:				
				if(RetrySubmitPanel.enabled) UITools.SetActiveState(RetrySubmitPanel, false);
				UITools.SetActiveState(LoginPanel, false);
				UITools.SetActiveState(ForgotPassPanel, true);
				ForgotErrorLabel.text = "";
				break;
			case WinMenuLevel.ForgotMessage:
				if(RetrySubmitPanel.enabled) UITools.SetActiveState(RetrySubmitPanel, false);
				UITools.SetActiveState(ForgotPassPanel, false);
				UITools.SetActiveState(ForgotPassMessagePanel, true);
				ForgotMessageLabel.text = "An email was sent to [44CCBB]" + globalEmail + " [FFFFFF]with instructions on how to reset your password!";
				break;
			case WinMenuLevel.GoOnlineWindow:
				if(RetrySubmitPanel.enabled) UITools.SetActiveState(RetrySubmitPanel, false);
				HighscoreButton.isEnabled = false;
				MainMenuButton.isEnabled = false;
				ReplaySongButton.isEnabled = false;
				UITools.SetActiveState(GoOnlinePanel, true);
				break;
			case WinMenuLevel.SubmitError:
				if (CreatePanel.enabled) UITools.SetActiveState(CreatePanel, false);
				if (LoginPanel.enabled) UITools.SetActiveState(LoginPanel, false);
				if (ScoresPanel.enabled) UITools.SetActiveState(ScoresPanel, false);
				if (ScoresOfflinePanel.enabled) UITools.SetActiveState(ScoresOfflinePanel, false);
				if (ScoresTitlePanel.enabled) UITools.SetActiveState(ScoresTitlePanel, false);
				if (ForgotPassPanel.enabled) UITools.SetActiveState(ForgotPassPanel, false);
				if (ForgotPassMessagePanel.enabled) UITools.SetActiveState(ForgotPassMessagePanel, false);
				if (GoOnlinePanel.enabled) UITools.SetActiveState(GoOnlinePanel, false);
				UITools.SetActiveState(TitlePanel, true);
				UITools.SetActiveState(RetrySubmitPanel, true);
				break;
		}
	}
	
	public static void OpenErrorPanel() {
		instance.ChangeMenu(WinMenuLevel.SubmitError);	
	}

	#region Base Menu Buttons
	/// <summary>
	/// Button handler for the "Play Again!" button
	/// </summary>
	void OnReplaySongClicked() {
		Application.LoadLevel("Game");
	}

	/// <summary>
	/// Button handler for the "Back to the Menu" button
	/// </summary>
	void OnMainMenuClicked() {
		Application.LoadLevel("MainMenu");
	}

	void OnSubmitHighscoreClicked() {
		if (Game.AskOnlineMode)
			ChangeMenu(WinMenuLevel.GoOnlineWindow);
		else if (((!Game.RememberLogin && !Game.IsLoggedIn) || Game.PlayerName.Length < 1) && Game.OnlineMode)
			ChangeMenu(WinMenuLevel.Login);
		else if (!Game.OnlineMode) {
			ChangeMenu(WinMenuLevel.ScoresOffline);
			SubmitScoresOffline();
		} else {
			ChangeMenu(WinMenuLevel.LoadingScores);
			StartCoroutine(SubmitScores(Game.PlayerName));
		}
	}
	#endregion

	#region Login Menu Buttons
	void OnBackClicked() {
		ChangeMenu(WinMenuLevel.Base);
	}
	void OnRememberMeActivate() {
		Game.RememberLogin = RememberMeCheckbox.isChecked;
	}
	/// <summary>
	/// Button and input text handler for submitting highscore.
	/// </summary>
	void OnLoginSubmit() {
		string playerName = UserNameInput.text;
		string password = PasswordInput.text;
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
			PasswordInput.text = "";
			ErrorLabel.text = "[F87431]Password too short. Try again";
			return;
		}
		password = MD5Utils.MD5FromString(password + passHashKey);
		StartCoroutine(startLogin(playerName, password));
	}

	void OnCreateClicked() {
		ChangeMenu(WinMenuLevel.Create);
	}

	void OnLoginForgotClicked() {
		ChangeMenu(WinMenuLevel.Forgot);
	}
	#endregion

	#region Login Menu functions
	IEnumerator startLogin(string user, string password) {
		yield return StartCoroutine(HSController.CheckUserWin(user, password));
		if (LoginSuccess) {
			Game.IsLoggedIn = true;
			// Store the new player name
			PlayerPrefs.SetString("playername", user);
			// Change the menu
			ChangeMenu(WinMenuLevel.LoadingScores);
			// Submit and load the scores
			StartCoroutine(SubmitScores(user));
			LoginSuccess = false;
		}
	}

	public static void SetLoginErrorLabel(string text) {
		instance.ErrorLabel.text = text;
	}
	#endregion

	#region Create Menu Buttons
	void OnCreateSubmit() {
		string name = CreateNameInput.text;
		string password = CreatePass1Input.text;
		string email = CreateEmailInput.text;
		if (name.Length < 2) {
			CreateNameInput.text = "";
			CreateErrorLabel.text = "Username too short";
			return;
		} else if (!nameRegEx.IsMatch(name)) {
			CreateNameInput.text = "";
			CreateErrorLabel.text = "Invalid username, use letters & numbers!";
			return;
		} else if (CreatePass1Input.text != CreatePass2Input.text) {
			CreatePass1Input.text = "";
			CreatePass2Input.text = "";
			CreateErrorLabel.text = "Passwords don't match";
			return;
		} else if (CreatePass1Input.text.Length < 5) {
			CreatePass1Input.text = "";
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
		ChangeMenu(WinMenuLevel.Login);
	}
	#endregion

	#region Create menu functions
	IEnumerator addNewUser(string name, string password, string email) {
		yield return StartCoroutine(HSController.AddUserWin(name, password, email));
		if (UserCreated) {
			PlayerPrefs.SetString("playername", name);
			ErrorLabel.text = "[44DD44]User created, please log in!";
			ChangeMenu(WinMenuLevel.Login);
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

	#region Forgot Pass buttons
	void OnForgotBackClicked() {
		ChangeMenu(WinMenuLevel.Login);
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
		ChangeMenu(WinMenuLevel.Base);
	}
	#endregion

	#region Forgot Pass menu functions
	IEnumerator forgotPasswordCheck(string name, string email) {
		yield return StartCoroutine(HSController.CheckEmailWin(name, email));
		if (CorrectEmail) {
			CorrectEmail = false;
			yield return StartCoroutine(HSController.SendEmailWin(name));
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
			ChangeMenu(WinMenuLevel.ForgotMessage);
		} else {
			Debug.Log("Error sending email");
		}
	}
	public static void SetForgotErrorLabel(string text) {
		instance.ForgotErrorLabel.text = text;
	}
	#endregion

	#region Go Online Window buttons
	void OnGoOnlineYesClicked() {
		Game.OnlineMode = true;
		HighscoreButton.isEnabled = true;
		MainMenuButton.isEnabled = true;
		ReplaySongButton.isEnabled = true;
		if ((!Game.RememberLogin && !Game.IsLoggedIn) || Game.PlayerName.Length < 1)
			ChangeMenu(WinMenuLevel.Login);
		else {
			ChangeMenu(WinMenuLevel.LoadingScores);
			StartCoroutine(SubmitScores(Game.PlayerName));
		}
	}

	void OnGoOnlineNoClicked() {
		Game.OnlineMode = false;
		HighscoreButton.isEnabled = true;
		MainMenuButton.isEnabled = true;
		ReplaySongButton.isEnabled = true;
		ChangeMenu(WinMenuLevel.ScoresOffline);
		SubmitScoresOffline();
	}
	#endregion

	void CalculateSongLabelSize() {
		if (SongLabel.text.Length > 38) {
			SongLabel.transform.localScale = new Vector2(38, 38);
		} else if (SongLabel.text.Length > 28) {
			SongLabel.transform.localScale = new Vector2(50, 50);
		}
	}

	private void SubmitScoresOffline() {
		// Store the song in song list
		string songRow = RemovePipeChar(AudioManager.artist).Trim() + "|" + RemovePipeChar(AudioManager.title).Trim() + "|" + Game.GameMode.GetHashCode();
		string path = "";
		bool songExists = false;
		if (Application.platform == RuntimePlatform.Android)
			path = (Application.persistentDataPath + "/songlist_offline.r");
		else if (Application.platform == RuntimePlatform.WindowsPlayer
			|| Application.platform == RuntimePlatform.WindowsEditor)
			path = (Application.persistentDataPath + "\\songlist_offline.r");
		else {
			Debug.Log("PLATFORM NOT SUPPORTED YET");
			path = "";
		}
		string line;
		try {
			using (StreamReader sr = new StreamReader(path)) {
				while ((line = sr.ReadLine()) != null) {
					string lineClean = HSController.RemoveSpecialCharacters(line).ToLower();
					string songRowClean = HSController.RemoveSpecialCharacters(songRow).ToLower();
					if (lineClean == songRowClean)
						songExists = true;
				}
				sr.Close();
			}
		} catch (Exception) {
		}
		if (!songExists) {
			StreamWriter sw = new StreamWriter(path, true);
			sw.WriteLine(songRow);
			sw.Close();
		}

		// Add score to file
		string fileName = "off_" + HSController.CalculateTableName(AudioManager.artist, AudioManager.title, Game.GameMode) + ".rhs";
		string hspath = "";
		//bool fileExists = false;
		if (Application.platform == RuntimePlatform.Android)
			hspath = (Application.persistentDataPath + "/" + fileName);
		else if (Application.platform == RuntimePlatform.WindowsPlayer
			|| Application.platform == RuntimePlatform.WindowsEditor)
			hspath = (Application.persistentDataPath + "\\" + fileName);
		else {
			Debug.Log("PLATFORM NOT SUPPORTED YET");
			hspath = "";
		}

		StreamWriter swSong = new StreamWriter(hspath, true);
		swSong.WriteLine(Player.score);
		swSong.Close();

		SubmittingScoreLabel.text = "";
		ChangeMenu(WinMenuLevel.ScoresOffline);
		HSController.LoadSongOffline(AudioManager.artist, AudioManager.title, Game.GameMode);
	}

	private IEnumerator SubmitScores(string playerName) {
		// Store the new player name
		PlayerPrefs.SetString("playername", playerName);
		// Store the song high score
		string localMD5 = HSController.CalculateTableName(AudioManager.artist, AudioManager.title, Game.GameMode);
		if (PlayerPrefs.GetInt(localMD5) < Player.score) PlayerPrefs.SetInt(localMD5, Player.score);
		// Fix song name and artist


		// Store the song in song list
		string songRow = RemovePipeChar(AudioManager.artist).Trim() + "|" + RemovePipeChar(AudioManager.title).Trim() + "|" + Game.GameMode.GetHashCode();
		string path = "";
		bool songExists = false;
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
		try {
			using (StreamReader sr = new StreamReader(path)) {
				while ((line = sr.ReadLine()) != null) {
					string lineClean = HSController.RemoveSpecialCharacters(line).ToLower();
					string songRowClean = HSController.RemoveSpecialCharacters(songRow).ToLower();
					if (lineClean == songRowClean)
						songExists = true;
				}
				sr.Close();
			}
		} catch (Exception) {
		}
		if (!songExists) {
			StreamWriter sw = new StreamWriter(path, true);
			sw.WriteLine(songRow);
			sw.Close();
		}

		//HideSubmitBox();                                        // Hide the submit box and button

		// Submit the highscore
		yield return StartCoroutine(HSController.PostScores(playerName, Player.score, AudioManager.artist, AudioManager.title, Game.GameMode));
		SubmittingScoreLabel.text = "";
		ChangeMenu(WinMenuLevel.Scores);
		HSController.LoadSong(AudioManager.artist, AudioManager.title, Game.GameMode);
	}


	public static string RemovePipeChar(string str) {
		string tmpStr = str.Replace("|", "/");
		return tmpStr.TrimEnd("\0".ToCharArray());
	}

	void HideSubmitBox() {
		HighscoreButton.isEnabled = false;
		HighscoreButton.enabled = false;
		HighscoreButton.GetComponentInChildren<UISlicedSprite>().enabled = false;
		HighscoreButton.GetComponentInChildren<UILabel>().enabled = false;
		//nameInput.GetComponentInChildren<UISlicedSprite>().enabled = false;
		//nameInput.GetComponentsInChildren<UILabel>()[1].enabled = false;
	}

	public static void ShowSubmitBox() {
		instance.HighscoreButton.isEnabled = true;
		instance.HighscoreButton.enabled = true;
		instance.HighscoreButton.GetComponentInChildren<UISlicedSprite>().enabled = true;
		instance.HighscoreButton.GetComponentInChildren<UILabel>().enabled = true;
		//instance.nameInput.GetComponentInChildren<UISlicedSprite>().enabled = true;
		//instance.nameInput.GetComponentsInChildren<UILabel>()[1].enabled = true;
	}
	#endregion
}
public enum WinMenuLevel {
	Base,
	Login,
	Create,
	LoadingScores,
	Scores,
	ScoresOffline,
	Forgot,
	ForgotMessage,
	GoOnlineWindow,
	SubmitError
}