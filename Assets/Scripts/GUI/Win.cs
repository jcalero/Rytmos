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
	// Base Menu Objects
	public UILabel SongLabel;
	public UILabel HitsValueLabel;
	public UILabel MissesValueLabel;
	public UILabel ScoreValueLabel;
	public UILabel RankValueLabel;
	public UIButton HighscoreButton;
	// Login Menu Objects
	public UIInput UserNameInput;
	public UILabel ErrorLabel;
	public UICheckbox RememberMeCheckbox;
	// Create Menu Objects
	// ??

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
		SongLabel.text = AudioManager.artist + " - " + AudioManager.title;
		HitsValueLabel.text = Player.TotalKills.ToString();
		MissesValueLabel.text = (EnemySpawnScript.spawnCount - Player.TotalKills).ToString();
		ScoreValueLabel.text = Player.score.ToString();
		RankValueLabel.text = CalculatedRank;
		if (Game.Cheated) {
			HighscoreButton.enabled = false;
			HighscoreButton.GetComponentInChildren<UILabel>().text = "[FF2222]You cheater!";
		}
	}
	/// <summary>
	/// Returns the final calculated score after the end of the game.
	/// </summary>
	/// <value>Calculated rank (total hits / total spawns)</value>
	string CalculatedRank {
		get {
			if (Player.TotalKills > 0) {
				int value = (int)((Player.TotalKills / (float)EnemySpawnScript.spawnCount) * 100);
				Debug.Log(Player.TotalKills + " / " + (float)EnemySpawnScript.spawnCount + " = " + (Player.TotalKills / (float)EnemySpawnScript.spawnCount));
				return "[33FF33]" + value + "%";
			} else
				return "[FF3333]0%";
		}
	}

	private void ChangeMenu(WinMenuLevel menuLevel) {
		currentMenuLevel = menuLevel;
		switch (currentMenuLevel) {
			case WinMenuLevel.Base:
				if (CreatePanel.enabled) UITools.SetActiveState(CreatePanel, false);
				if (LoginPanel.enabled) UITools.SetActiveState(LoginPanel, false);
				if (ScoresPanel.enabled) UITools.SetActiveState(ScoresPanel, false);
				UITools.SetActiveState(TitlePanel, true);
				UITools.SetActiveState(BasePanel, true);
				break;
			case WinMenuLevel.Create:
				UITools.SetActiveState(LoginPanel, false);
				UITools.SetActiveState(CreatePanel, true);
				break;
			case WinMenuLevel.Login:
				UITools.SetActiveState(TitlePanel, false);
				UITools.SetActiveState(BasePanel, false);
				UITools.SetActiveState(LoginPanel, true);
				if (PlayerPrefs.GetString("playername") != null)
					UserNameInput.text = PlayerPrefs.GetString("playername");
				RememberMeCheckbox.isChecked = Game.RememberLogin;
				ErrorLabel.text = "";                                   // Clear error text if any
				break;
			case WinMenuLevel.Scores:
				if (BasePanel.enabled) UITools.SetActiveState(BasePanel, false);
				if (CreatePanel.enabled) UITools.SetActiveState(CreatePanel, false);
				if (LoginPanel.enabled) UITools.SetActiveState(LoginPanel, false);
				if (TitlePanel.enabled) UITools.SetActiveState(TitlePanel, false);
				UITools.SetActiveState(ScoresPanel, true);
				break;
		}
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
		if ((!Game.RememberLogin && !Game.IsLoggedIn) || Game.PlayerName.Length < 1)
			ChangeMenu(WinMenuLevel.Login);
		else {
			SubmitScores(Game.PlayerName);
			ChangeMenu(WinMenuLevel.Scores);
			HSController.LoadSong(AudioManager.artist, AudioManager.title, Game.GameMode);
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
		string playerName = UserNameInput.text;                     // Set input box text to playerName
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
		SubmitScores(playerName);
		ChangeMenu(WinMenuLevel.Scores);
		HSController.LoadSong(AudioManager.artist, AudioManager.title, Game.GameMode);
	}
	#endregion

	/// <summary>
	/// Sets the text of the information box while submitting to a specified
	/// string.
	/// </summary>
	/// <param name="text">The string to set the text to</param>
	public static void SetSubmitText(string text) {
		Debug.LogWarning(text);
		//instance.submittedLabel.text = text;
	}

	/// <summary>
	/// Sets the text of the error box while submitting to a specified
	/// string.
	/// </summary>
	/// <param name="text">The string to set the text to</param>
	public static void SetErrorText(string text) {
		Debug.LogWarning(text);
		//instance.errorLabel.text = text;
	}

	private void SubmitScores(string playerName) {
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
		StartCoroutine(HSController.PostScores(playerName, Player.score, AudioManager.artist, AudioManager.title, Game.GameMode));

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
	Scores
}