using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Text.RegularExpressions;

public class HSController : MonoBehaviour {
	private string secretKey = "goldenck";   // Edit this value and make sure it's the same as the one stored on the server
	//private bool highscoresLoaded = false;   // Sets to true if the highscores were loaded once.

	private string addScoreURL = "http://rytmos-game.com/addscorenew.php?"; //be sure to add a ? to your url
	//private string highscoreURL = "http://rytmos-game.com/displaynew.php?";
	private string top5URL = "http://rytmos-game.com/displaytop5.php?";
	private string close5URL = "http://rytmos-game.com/displayclose5.php?";
	public UILabel[] highscores;
	public UILabel[] names;
	public UILabel submittedLabel;
	public UILabel errorLabel;

	public string[][] hsTimeAttack = new string[10][];
	public string[][] hsSurvival = new string[10][];

	private string[][] top5List;
	private string[][] close5List;

	private static HSController instance;

	void Awake() {
		instance = this;

		//float time2 = Time.realtimeSinceStartup;
		//Debug.Log(GetMD5HashFromFile("D:\\looongmp3.mp3"));
		//Debug.Log("Time to generate MD5 using BitConverter: " + (Time.realtimeSinceStartup - time2));

		for (int i = 0; i < hsTimeAttack.Length; i++)
			hsTimeAttack[i] = new string[2];
		for (int i = 0; i < hsSurvival.Length; i++)
			hsSurvival[i] = new string[2];
	}

	public static string[][] Top5List {
		get { return instance.top5List; }
		private set { instance.top5List = value; }
	}

	public static string[][] Close5List {
		get { return instance.close5List; }
		private set { instance.close5List = value; }
	}

	// remember to use StartCoroutine when calling this function!
	public static IEnumerator PostScores(string name, int score, string artist, string song, Game.Mode gameMode) {
		//This connects to a server side php script that will add the name and score to a MySQL DB.

		string cheatHash = MD5Utils.MD5FromString(name + score + instance.secretKey);
		string table = CalculateTableName(artist, song, gameMode);

		string post_url = instance.addScoreURL + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&table=" + table + "&hash=" + cheatHash;

		instance.submittedLabel.text = "Submitting...";
		// Post the URL to the site and create a download object to get the result.
		WWW hs_post = new WWW(post_url);
		yield return hs_post; // Wait until the download is done

		if (hs_post.error != null) {
			Debug.LogWarning("There was an error posting the high score: " + hs_post.error);
			instance.submittedLabel.text = "";
			instance.errorLabel.text = "[FF2222]Error submitting, please try again.";
			Win.ShowSubmitBox();
		} else {
			instance.submittedLabel.text = "Highscore submitted";
		}
	}

	public static string RemoveSpecialCharacters(string str) {
		return Regex.Replace(str, "[^a-zA-Z0-9]+", "");
	}

	public static string CalculateTableName(string artist, string song, Game.Mode gameMode) {
		string tmpArtist = HSController.RemoveSpecialCharacters(artist).ToLower();
		string tmpSong = HSController.RemoveSpecialCharacters(song).ToLower();
		return "hs_" + MD5Utils.MD5FromString(tmpArtist + tmpSong + gameMode.ToString());
	}

	public static IEnumerator GetTop5Scores(string artist, string song, Game.Mode gameMode) {

		string table = CalculateTableName(artist, song, gameMode);
		string hs_url = instance.top5URL + "table=" + table;
		WWW hs_get = new WWW(hs_url);
		yield return hs_get;
		if (hs_get.error != null) {
			Debug.LogWarning("There was an error getting the high score: " + hs_get.error);
			MainMenu.FetchError = hs_get.error;
		} else if (hs_get.text.ToLower().StartsWith("query failed")) {
			Debug.LogWarning("There was an error getting the high score: " + hs_get.text);
			MainMenu.FetchError = hs_get.text;
		} else {
			Top5List = parseScores(hs_get.text);
		}

		//instance.highscoresLoaded = true;
	}

	public static IEnumerator GetClose5Scores(string artist, string song, Game.Mode gameMode, int score) {

		string table = CalculateTableName(artist, song, gameMode);
		string hs_url = instance.close5URL + "table=" + table + "&score=" + score;
		WWW hs_get = new WWW(hs_url);
		yield return hs_get;
		if (hs_get.error != null) {
			Debug.LogWarning("There was an error getting the high score: " + hs_get.error);
			MainMenu.FetchError = hs_get.error;
		} else if (hs_get.text.ToLower().StartsWith("query failed")) {
			Debug.LogWarning("There was an error getting the high score: " + hs_get.text);
			MainMenu.FetchError = hs_get.text;
		} else {
			Close5List = parseScores(hs_get.text);
		}

		//instance.highscoresLoaded = true;
	}

	private static string[][] parseScores(string scores) {
		string[] tempStrs = scores.Split('|');
		string[][] tempResult = new string[tempStrs.Length - 1][];
		for (int cnt = 0; cnt < tempResult.Length; cnt++) {
			tempResult[cnt] = tempStrs[cnt].Split('-');
		}
		return tempResult;
	}

	//public static void ShowScores(string table) {
	//    for (int cnt = 0; cnt < instance.names.Length; cnt++) {
	//        if (table.Equals("rytmos_hs_dm")) {
	//            instance.names[cnt].text = instance.hsSurvival[cnt][0];
	//            instance.highscores[cnt].text = instance.hsSurvival[cnt][1];
	//        }
	//        if (table.Equals("rytmos_hs_30sec")) {
	//            instance.names[cnt].text = instance.hsTimeAttack[cnt][0];
	//            instance.highscores[cnt].text = instance.hsTimeAttack[cnt][1];
	//        }
	//    }
	//}

}