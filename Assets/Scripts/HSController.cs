using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

public class HSController : MonoBehaviour {
	private string secretKey = "goldenck";   // Edit this value and make sure it's the same as the one stored on the server
	private bool highscoresLoaded = false;   // Sets to true if the highscores were loaded once.

	private string addScoreURL = "http://rytmos-game.com/addscorenew.php?"; //be sure to add a ? to your url
	private string highscoreURL = "http://rytmos-game.com/displaynew.php?";
	public UILabel[] highscores;
	public UILabel[] names;
	public UILabel submittedLabel;
	public UILabel errorLabel;

	public string[][] hsTimeAttack = new string[10][];
	public string[][] hsSurvival = new string[10][];

	private string[][] scoresList;

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

	public static string[][] ScoresList {
		get { return instance.scoresList; }
		private set { instance.scoresList = value; }
	}

	// remember to use StartCoroutine when calling this function!
	public static IEnumerator PostScores(string name, int score, string table) {
		//This connects to a server side php script that will add the name and score to a MySQL DB.
		// Supply it with a string representing the players name and the players score.
		string hash = MD5Utils.MD5FromString(name + score + instance.secretKey);

		string post_url = instance.addScoreURL + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&table=" + table + "&hash=" + hash;

		instance.submittedLabel.text = "Submitting...";
		// Post the URL to the site and create a download object to get the result.
		WWW hs_post = new WWW(post_url);
		yield return hs_post; // Wait until the download is done

		if (hs_post.error != null) {
			print("There was an error posting the high score: " + hs_post.error);
			instance.submittedLabel.text = "";
			instance.errorLabel.text = "[FF2222]Error submitting, please try again.";
			Win.ShowSubmitBox();
		} else {
			instance.submittedLabel.text = "Highscore submitted";
		}
	}

	// remember to use StartCoroutine when calling this function!
	public static IEnumerator PostScoresNew(string name, int score, string artist, string song, Game.Mode gameMode) {
		//This connects to a server side php script that will add the name and score to a MySQL DB.
		// Supply it with a string representing the players name and the players score.
		string hash = MD5Utils.MD5FromString(name + score + instance.secretKey);
		string table = CalculateTableName(artist, song, gameMode);

		string post_url = instance.addScoreURL + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&table=" + table + "&hash=" + hash;

		// Post the URL to the site and create a download object to get the result.
		WWW hs_post = new WWW(post_url);
		yield return hs_post; // Wait until the download is done

		if (hs_post.error != null) {
			Debug.LogWarning("There was an error posting the high score: " + hs_post.error);
		} else {
			Debug.Log("Highscore submitted");
		}
	}

	public static string CalculateTableName(string artist, string song, Game.Mode gameMode) {
		return "hs_" + MD5Utils.MD5FromString(artist + song + gameMode.ToString());
	}

	// Get the scores from the MySQL DB to display in a UILabel.
	// remember to use StartCoroutine when calling this function!
	public static IEnumerator GetScores(string table, bool show) {
		if (instance.highscoresLoaded && show) {
			for (int cnt = 0; cnt < instance.names.Length; cnt++) {
				instance.highscores[cnt].text = "";
				instance.names[cnt].text = "";
			}
		}
		for (int cnt = 0; cnt < instance.names.Length; cnt++) {
			if (show) {
				instance.highscores[cnt].text = "Loading Scores";
				instance.names[cnt].text = "Loading Scores";
			}
			string hs_name_url = instance.highscoreURL + "?position=" + cnt + "&field=1" + "&table=" + table;
			string hs_score_url = instance.highscoreURL + "?position=" + cnt + "&field=2" + "&table=" + table;
			WWW hs_get = new WWW(hs_name_url);
			WWW hs_get_score = new WWW(hs_score_url);
			yield return hs_get;
			yield return hs_get_score;

			if (hs_get.error != null) {
				print("There was an error getting the high score: " + hs_get.error);
			} else if (hs_get_score.error != null) {
				print("There was an error getting the high score: " + hs_get_score.error);
			} else {
				string name = (cnt + 1) + ". " + hs_get.text;
				string score = hs_get_score.text;
				if (table.Equals("rytmos_hs_dm")) {
					instance.hsSurvival[cnt][0] = name;
					instance.hsSurvival[cnt][1] = score;
				}
				if (table.Equals("rytmos_hs_30sec")) {
					instance.hsTimeAttack[cnt][0] = name;
					instance.hsTimeAttack[cnt][1] = score;
				}
				if (show) {
					instance.names[cnt].text = name;
					instance.highscores[cnt].text = score; // this is a GUIText that will display the scores in game.
				}
			}
		}
		instance.highscoresLoaded = true;
		MainMenu.EnableReloadButton();
	}

	public static IEnumerator GetScoresNew(string artist, string song, Game.Mode gameMode) {

		string table = CalculateTableName(artist, song, gameMode);
		string hs_url = instance.highscoreURL + "table=" + table;
		WWW hs_get = new WWW(hs_url);
		yield return hs_get;

		if (hs_get.error != null) {
			print("There was an error getting the high score: " + hs_get.error);
		} else {
			ScoresList = parseScores(hs_get.text);
		}

		instance.highscoresLoaded = true;
	}

	private static string[][] parseScores(string scores) {
		string[] tempStrs = scores.Split('|');
		string[][] tempResult = new string[tempStrs.Length - 1][];
		for (int cnt = 0; cnt < tempResult.Length; cnt++) {
			tempResult[cnt] = tempStrs[cnt].Split('-');
		}
		return tempResult;
	}

	public static void ShowScores(string table) {
		for (int cnt = 0; cnt < instance.names.Length; cnt++) {
			if (table.Equals("rytmos_hs_dm")) {
				instance.names[cnt].text = instance.hsSurvival[cnt][0]; ;
				instance.highscores[cnt].text = instance.hsSurvival[cnt][1]; ;
			}
			if (table.Equals("rytmos_hs_30sec")) {
				instance.names[cnt].text = instance.hsTimeAttack[cnt][0]; ;
				instance.highscores[cnt].text = instance.hsTimeAttack[cnt][1]; ;
			}
		}
	}

}