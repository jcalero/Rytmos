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
	public UILabel scoreValueLabel;
	public UILabel totalScoreLabel;
	public UIInput nameInput;
	public UIButton submitButton;
	public UILabel submittedLabel;
	public UILabel errorLabel;

	private static Win instance;
	private Regex nameRegEx = new Regex("^[a-zA-Z0-9]*$");
	#endregion

	#region Functions
	void Start() {
		instance = this;
		scoreValueLabel.text = Player.score.ToString();
		totalScoreLabel.text = "[AADDAA]" + CalculatedScore;
		if (Game.Cheated) {
			HideSubmitBox();
			errorLabel.transform.localPosition = new Vector3(errorLabel.transform.localPosition.x, errorLabel.transform.localPosition.y + 30, errorLabel.transform.localPosition.z);
			errorLabel.text = "[FF2222]You cheater!";
		}
		if (PlayerPrefs.GetString("playername") != null)
			nameInput.text = PlayerPrefs.GetString("playername");
	}
	/// <summary>
	/// Returns the final calculated score after the end of the game.
	/// </summary>
	/// <value>Calculated score (Health * Score * 0.1)</value>
	int CalculatedScore {
		get {
				return Player.score;
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
	/// Sets the text of the information box while submitting to a specified
	/// string.
	/// </summary>
	/// <param name="text">The string to set the text to</param>
	public static void SetSubmitText(string text) {
		instance.submittedLabel.text = text;
	}

	/// <summary>
	/// Sets the text of the error box while submitting to a specified
	/// string.
	/// </summary>
	/// <param name="text">The string to set the text to</param>
	public static void SetErrorText(string text) {
		instance.errorLabel.text = text;
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
		if (!nameRegEx.IsMatch(playerName)) {
			nameInput.text = "";
			errorLabel.text = "[F87431]Invalid. Only letters & numbers allowed.";
			return;
		}
		// Store the new player name
		PlayerPrefs.SetString("playername", playerName);
		// Store the song high score
		string localMD5 = HSController.CalculateTableName(AudioManager.artist, AudioManager.title, Game.GameMode);
		if (PlayerPrefs.GetInt(localMD5) < CalculatedScore) PlayerPrefs.SetInt(localMD5, CalculatedScore);
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

		HideSubmitBox();                                        // Hide the submit box and button

		// Submit the highscore
		StartCoroutine(HSController.PostScores(playerName, CalculatedScore, AudioManager.artist, AudioManager.title, Game.GameMode));
	}


	public static string RemovePipeChar(string str) {
		string tmpStr = str.Replace("|", "/");
		return tmpStr.TrimEnd("\0".ToCharArray());
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
