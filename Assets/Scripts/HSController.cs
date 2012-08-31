using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class HSController : MonoBehaviour {
	private string secretKey = "goldenck";   // Edit this value and make sure it's the same as the one stored on the server
	//private bool highscoresLoaded = false;   // Sets to true if the highscores were loaded once.
	private string userSecretKey = "r2d2imahorse";
	private string emailSecretKey = "r2d2uweboll";

	private string addScoreURL = "http://rytmos-game.com/addscorenew.php?"; //be sure to add a ? to your url
	private string top5URL = "http://rytmos-game.com/displaytop5.php?";
	private string close5URL = "http://rytmos-game.com/displayclose5.php?";
	private string addUserURL = "http://rytmos-game.com/adduser.php?";
	private string checkUserURL = "http://rytmos-game.com/checkuser.php?";
	private string checkEmailURL = "http://rytmos-game.com/checkemail.php?";
	private string sendEmailURL = "http://rytmos-game.com/sendemail.php?";

	public string[][] hsTimeAttack = new string[10][];
	public string[][] hsSurvival = new string[10][];

	private string[][] top5List;
	private string[][] close5List;

	private List<string> songList = new List<string>();
	private string[][] separatedSongList;
	private static int currentlyShowingHS = 0;
	private static int currentlyShowingHSOffline = 0;
	private static bool noSongsLoaded;

	public UILabel[] top5names;
	public UILabel[] top5scores;
	public UILabel[] close5names;
	public UILabel[] close5scores;
	public UILabel[] offlineTop5names;
	public UILabel[] offlineTop5scores;
	public UILabel[] offlineClose5names;
	public UILabel[] offlineClose5scores;
	public UILabel ScoresModeLabel;
	public UILabel ScoresSongLabel;
	public UILabel OfflineSongModeLabel;
	public UILabel OfflineScoresSongLabel;
	public UIButton NextSongButton;
	public UIButton PrevSongButton;
	public UIButton OfflineNextSongButton;
	public UIButton OfflinePrevSongButton;
	public UIButton NextModeButton;
	public UIButton PrevModeButton;

	public Camera cameraSize;

	public static string FetchError;

	private static HSController instance;

	void Awake() {
		instance = this;
		//cameraSize.orthographicSize = Game.cameraScaleFactor;
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

		string post_url = instance.addScoreURL + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&table=" + table + "&hash=" + cheatHash
			+ "&artist=" + RemoveSpecialCharacters(artist).ToLower() + "&song=" + RemoveSpecialCharacters(song).ToLower() + "&mode=" + gameMode.GetHashCode();

		// Post the URL to the site and create a download object to get the result.
		WWW hs_post = new WWW(post_url);
		yield return hs_post; // Wait until the download is done

		if (hs_post.error != null) {
			Debug.LogWarning("There was an error posting the high score: " + hs_post.error);
			if(Application.loadedLevelName == "Win")
				Win.OpenErrorPanel();			
			else if (Application.loadedLevelName == "MainMenu")
				MainMenu.OpenErrorFetchingScoresPanel();
		}
	}

	public static string RemoveSpecialCharacters(string str) {
		return Regex.Replace(str, "[^a-zA-Z0-9]+", "");
	}

	public static string CalculateTableName(string artist, string song, Game.Mode gameMode) {
		string tmpArtist = RemoveSpecialCharacters(artist).ToLower();
		string tmpSong = RemoveSpecialCharacters(song).ToLower();
		return "hs_" + MD5Utils.MD5FromString(tmpArtist + tmpSong + gameMode.GetHashCode());
	}

	public static IEnumerator GetTop5Scores(string artist, string song, Game.Mode gameMode) {

		string table = CalculateTableName(artist, song, gameMode);
		string hs_url = instance.top5URL + "table=" + table;
		WWW hs_get = new WWW(hs_url);
		yield return hs_get;
		if (hs_get.error != null) {
			Debug.LogWarning("There was an error getting the high score: " + hs_get.error);
			FetchError = hs_get.error;
			string errorText = "Error fetching the scores!";
			Top5List = new string[1][];
			Top5List[0] = new string[errorText.Length];
			Top5List[0][0] = errorText;
		} else if (hs_get.text.ToLower().StartsWith("query failed")) {
			Debug.LogWarning("There was an error getting the high score: " + hs_get.text);
			FetchError = hs_get.text;
			string errorText = "Error fetching the scores!";
			Top5List = new string[1][];
			Top5List[0] = new string[errorText.Length];
			Top5List[0][0] = errorText;
		} else {
			Top5List = parseScores(hs_get.text);
		}

		//instance.highscoresLoaded = true;
	}

	public static IEnumerator GetClose5Scores(string artist, string song, string username, Game.Mode gameMode, int score) {

		string table = CalculateTableName(artist, song, gameMode);
		string hs_url = instance.close5URL + "table=" + table + "&score=" + score + "&username=" + username;
		WWW hs_get = new WWW(hs_url);
		yield return hs_get;
		if (hs_get.error != null) {
			Debug.LogWarning("There was an error getting the high score: " + hs_get.error);
			FetchError = hs_get.error;
		} else if (hs_get.text.ToLower().StartsWith("query failed")) {
			Debug.LogWarning("There was an error getting the high score: " + hs_get.text);
			FetchError = hs_get.text;
			string errorText = "Error fetching the scores!";
			Close5List = new string[1][];
			Close5List[0] = new string[errorText.Length];
			Close5List[0][0] = errorText;
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

	public static void InitHSDisplay() {
		if (!instance.LoadSongList() && Game.OnlineMode) {
			UITools.SetActiveState(instance.NextSongButton, false);
			UITools.SetActiveState(instance.PrevSongButton, false);
			UITools.SetActiveState(instance.NextModeButton, false);
			UITools.SetActiveState(instance.PrevModeButton, false);
			instance.ScoresSongLabel.maxLineCount = 2;
			instance.ScoresSongLabel.text = "No songs have been played online yet! Play some first.";
			instance.ScoresModeLabel.text = "";
			int labelcnt = 0;
			while (labelcnt < 5) {
				instance.top5names[labelcnt].text = "";
				instance.close5names[labelcnt].text = "";
				instance.top5scores[labelcnt].text = "";
				instance.close5scores[labelcnt].text = "";
				labelcnt++;
			}
		} else if (!instance.LoadSongList() && !Game.OnlineMode) {
			UITools.SetActiveState(instance.OfflineNextSongButton, false);
			UITools.SetActiveState(instance.OfflinePrevSongButton, false);
			instance.OfflineScoresSongLabel.maxLineCount = 2;
			instance.OfflineScoresSongLabel.text = "No songs have been played offline yet! Play some first.";
			instance.OfflineSongModeLabel.text = "";
			int labelcnt = 0;
			while (labelcnt < 10) {
				instance.offlineClose5names[labelcnt].text = "";
				instance.offlineClose5scores[labelcnt].text = "";
				labelcnt++;
			}
		} else {
			instance.ScoresSongLabel.maxLineCount = 1;
			string artist = instance.separatedSongList[0][0];
			string song = instance.separatedSongList[0][1];
			Game.Mode gameMode = (Game.Mode)Enum.Parse(typeof(Game.Mode), instance.separatedSongList[0][2]);
			if (Game.OnlineMode)
				instance.StartCoroutine(instance.fetchScores(artist, song, gameMode));
			else
				LoadSongOffline(artist, song, gameMode);
		}
	}

	private IEnumerator fetchScores(int songListID) {
		string artist = separatedSongList[songListID][0];
		string song = separatedSongList[songListID][1];
		Game.Mode gameMode = (Game.Mode)Enum.Parse(typeof(Game.Mode), separatedSongList[songListID][2]);
		yield return StartCoroutine(fetchScores(artist, song, gameMode));
		MainMenu.EnableReloadButton();
	}
	private IEnumerator fetchScores(string artist, string song, Game.Mode gameMode) {
		Debug.Log("Fetching scores");
		string artistDisplay = artist;
		string songDisplay = song;
		//string md5Artist = HSController.RemoveSpecialCharacters(artist).ToLower();
		//string md5Song = HSController.RemoveSpecialCharacters(song).ToLower();
		//string entryMD5 = MD5Utils.MD5FromString(md5Artist + md5Song + gameMode);
		string localMD5 = CalculateTableName(artist, song, gameMode);
		int topScore = PlayerPrefs.GetInt(localMD5);

		//Debug.Log("This is what the data looks like before calculating fetch-MD5: " + md5Artist + " - " + md5Song + " - " + gameMode.ToString());

		if (artist.Length > 1) artistDisplay = char.ToUpper(artist[0]) + artist.Substring(1);
		if (song.Length > 1) songDisplay = char.ToUpper(song[0]) + song.Substring(1);

		ScoresSongLabel.text = artistDisplay + " - " + songDisplay;
		string color = "[7783DF]";
		if (gameMode == Game.Mode.Arcade)
			color = "[7783DF]";
		else if (gameMode == Game.Mode.Casual)
			color = "[65FAFF]";
		else if (gameMode == Game.Mode.Tutorial)
			color = "[DAFE3D]";
		else
			color = "";
		ScoresModeLabel.text = color + gameMode.ToString();
		top5names[0].text = "Loading Top Scores...";
		close5names[0].text = "Loading Close Scores...";
		top5scores[0].text = "";
		close5scores[0].text = "";
		for (int labelcnt = 1; labelcnt < top5names.Length; labelcnt++) {
			top5names[labelcnt].text = "";
			top5scores[labelcnt].text = "";
		}
		for (int labelcnt = 1; labelcnt < close5names.Length; labelcnt++) {
			close5names[labelcnt].text = "";
			close5scores[labelcnt].text = "";
		}

		yield return StartCoroutine(GetTop5Scores(artist, song, gameMode));
		if (FetchError == null) {
			for (int cnt = 0; cnt < Top5List.Length; cnt++) {
				if (cnt < top5names.Length) {
					top5names[cnt].text = (cnt + 1) + ". " + Top5List[cnt][0];
					top5scores[cnt].text = UITools.FormatNumber(Top5List[cnt][1]);
				}
				//Debug.Log(HSController.ScoresList[cnt][0] + " :: " + HSController.ScoresList[cnt][1]);
			}
		} else {
			if(Application.loadedLevelName == "Win")
				Win.OpenErrorPanel();
			else if (Application.loadedLevelName == "MainMenu")
				MainMenu.OpenErrorFetchingScoresPanel();
			FetchError = null;
		}
		if(Application.loadedLevelName == "Win") topScore = Player.score;		
		yield return StartCoroutine(GetClose5Scores(artist, song, Game.PlayerName, gameMode, topScore));
		if (FetchError == null) {
			bool formattedOwnRow = false;
			Sort<string>(Close5List, 2);
			for (int cnt = 0; cnt < Close5List.Length; cnt++) {
				if (cnt < close5names.Length) {
					string nr = Close5List[cnt][2];
					close5names[cnt].text = nr + ". " + Close5List[cnt][0];
					close5scores[cnt].text = UITools.FormatNumber(Close5List[cnt][1]);
					close5names[cnt].transform.localScale = new Vector3(26, 26, 1);
					close5scores[cnt].GetComponent<TweenScale>().enabled = false;
					if (Close5List[cnt][1] == topScore.ToString() &&
						Close5List[cnt][0] == Game.PlayerName &&
						!formattedOwnRow) {
						close5names[cnt].text = "[FDD017]" + close5names[cnt].text;
						close5scores[cnt].text = "[FDD017]" + close5scores[cnt].text;
						//close5names[cnt].GetComponent<TweenScale>().enabled = true;
						close5names[cnt].transform.localScale = new Vector3(28, 28, 1);
						close5scores[cnt].GetComponent<TweenScale>().enabled = true;
						formattedOwnRow = true;
					}
				}
				//Debug.Log(HSController.ScoresList[cnt][0] + " :: " + HSController.ScoresList[cnt][1]);
			}
		} else {
			if(Application.loadedLevelName == "Win")
				Win.OpenErrorPanel();
			else if (Application.loadedLevelName == "MainMenu")
				MainMenu.OpenErrorFetchingScoresPanel();
			FetchError = null;
		}
	}

	public static IEnumerator AddUser(string user, string password, string email) {
		string hashedPassword = MD5Utils.MD5FromString(password + instance.userSecretKey);
		string cheatHash = MD5Utils.MD5FromString(user + hashedPassword + instance.userSecretKey);

		string post_url = instance.addUserURL + "name=" + WWW.EscapeURL(user) + "&password=" + hashedPassword + "&email=" + email + "&hash=" + cheatHash;
		MainMenu.SetCreateButtonLabel("Submitting...");
		// Post the URL to the site and create a download object to get the result.
		WWW hs_post = new WWW(post_url);
		yield return hs_post; // Wait until the upload is done

		if (hs_post.error != null) {
			Debug.LogWarning("There was an error adding the user: " + hs_post.error);
			MainMenu.SetCreateErrorLabel("Error. Please try again");
			MainMenu.SetCreateButtonLabel("Create");
		} else if (hs_post.text.StartsWith("Table exists, adding user...<br>ERROR: Username already exists.")) {
			Debug.Log(hs_post.text);
			MainMenu.SetCreateErrorLabel("Error. Username already exists");
			MainMenu.SetCreateButtonLabel("Create");
			MainMenu.UserExists = true;
		} else {
			Debug.Log(hs_post.text);
			Debug.Log("User created: " + user);
			MainMenu.SetCreateErrorLabel("[44DD44]User created: " + user);
			MainMenu.SetCreateButtonLabel("Create");
			MainMenu.UserCreated = true;
		}
	}

	public static IEnumerator AddUserWin(string user, string password, string email) {
		string hashedPassword = MD5Utils.MD5FromString(password + instance.userSecretKey);
		string cheatHash = MD5Utils.MD5FromString(user + hashedPassword + instance.userSecretKey);

		string post_url = instance.addUserURL + "name=" + WWW.EscapeURL(user) + "&password=" + hashedPassword + "&email=" + email + "&hash=" + cheatHash;

		Win.SetCreateButtonLabel("Submitting...");
		// Post the URL to the site and create a download object to get the result.
		WWW hs_post = new WWW(post_url);
		yield return hs_post; // Wait until the upload is done

		if (hs_post.error != null) {
			Debug.LogWarning("There was an error adding the user: " + hs_post.error);
			Win.SetCreateErrorLabel("Error. Please try again");
			Win.SetCreateButtonLabel("Create");
		} else if (hs_post.text.StartsWith("Table exists, adding user...<br>ERROR: Username already exists.")) {
			Debug.Log(hs_post.text);
			Win.SetCreateErrorLabel("Error. Username already exists");
			Win.SetCreateButtonLabel("Create");
		} else {
			Debug.Log(hs_post.text);
			Debug.Log("User created: " + user);
			Win.SetCreateErrorLabel("[44DD44]User created: " + user);
			Win.SetCreateButtonLabel("Create");
			Win.UserCreated = true;
		}
	}

	public static IEnumerator CheckUser(string user, string password) {
		string hashedPassword = MD5Utils.MD5FromString(password + instance.userSecretKey);
		string cheatHash = MD5Utils.MD5FromString(user + hashedPassword + instance.userSecretKey);

		string get_url = instance.checkUserURL + "name=" + WWW.EscapeURL(user) + "&password=" + hashedPassword + "&hash=" + cheatHash;

		//MainMenu.SetCreateButtonLabel("Submitting...");
		// Post the URL to the site and create a download object to get the result.
		WWW hs_get = new WWW(get_url);
		yield return hs_get; // Wait until the check is done

		if (hs_get.error != null) {
			MainMenu.SetLoginErrorLabel("[F87431]Error. Please try again");
			Debug.LogWarning("There was an error logging in: " + hs_get.error);
		} else if (hs_get.text.StartsWith("ERROR: No user")) {
			Debug.Log(hs_get.text);
			MainMenu.SetLoginErrorLabel("[F87431]Username not found. Create an account?");
		} else if (hs_get.text.StartsWith("ERROR: Wrong")) {
			Debug.Log(hs_get.text);
			MainMenu.SetLoginErrorLabel("[F87431]Wrong password, try again");
		} else if (hs_get.text.StartsWith("SUCCESS")) {
			Debug.Log(hs_get.text);
			MainMenu.SetLoginErrorLabel("");
			MainMenu.LoginSuccess = true;
		} else if (hs_get.text.StartsWith("Query failed: Table")) {
			MainMenu.SetLoginErrorLabel("[F87431]Username not found. Create an account?");
			Debug.Log(hs_get.text);
		} else {
			MainMenu.SetLoginErrorLabel("[F87431]Error. Please try again");
			Debug.Log(hs_get.text);
		}
	}

	public static IEnumerator CheckUserWin(string user, string password) {
		string hashedPassword = MD5Utils.MD5FromString(password + instance.userSecretKey);
		string cheatHash = MD5Utils.MD5FromString(user + hashedPassword + instance.userSecretKey);

		string get_url = instance.checkUserURL + "name=" + WWW.EscapeURL(user) + "&password=" + hashedPassword + "&hash=" + cheatHash;

		//MainMenu.SetCreateButtonLabel("Submitting...");
		// Post the URL to the site and create a download object to get the result.
		WWW hs_get = new WWW(get_url);
		yield return hs_get; // Wait until the check is done

		if (hs_get.error != null) {
			Win.SetLoginErrorLabel("[F87431]Error. Please try again");
			Debug.LogWarning("There was an error logging in: " + hs_get.error);
		} else if (hs_get.text.StartsWith("ERROR: No user")) {
			Debug.Log(hs_get.text);
			Win.SetLoginErrorLabel("[F87431]Username not found. Create an account?");
		} else if (hs_get.text.StartsWith("ERROR: Wrong")) {
			Debug.Log(hs_get.text);
			Win.SetLoginErrorLabel("[F87431]Wrong password, try again");
		} else if (hs_get.text.StartsWith("SUCCESS")) {
			Debug.Log(hs_get.text);
			Win.SetLoginErrorLabel("");
			Win.LoginSuccess = true;
		} else if (hs_get.text.StartsWith("Query failed: Table")) {
			Win.SetLoginErrorLabel("[F87431]Username not found. Create an account?");
			Debug.Log(hs_get.text);
		} else {
			Win.SetLoginErrorLabel("[F87431]Error. Please try again");
			Debug.Log(hs_get.text);
		}
	}

	public static IEnumerator CheckEmail(string name, string email) {
		string cheatHash = MD5Utils.MD5FromString(name + email + instance.emailSecretKey);

		string get_url = instance.checkEmailURL + "user=" + WWW.EscapeURL(name) + "&email=" + email + "&hash=" + cheatHash;

		//MainMenu.SetCreateButtonLabel("Submitting...");
		// Post the URL to the site and create a download object to get the result.
		WWW hs_get = new WWW(get_url);
		yield return hs_get; // Wait until the check is done

		if (hs_get.error != null) {
			MainMenu.SetForgotErrorLabel("[F87431]Error. Please try again");
			Debug.LogWarning("There was an error checking email: " + hs_get.error);
		} else if (hs_get.text.StartsWith("ERROR: No user")) {
			Debug.Log(hs_get.text);
			MainMenu.SetForgotErrorLabel("[F87431]Username not found. Try again.");
		} else if (hs_get.text.StartsWith("ERROR: Wrong")) {
			Debug.Log(hs_get.text);
			MainMenu.SetForgotErrorLabel("[F87431]Email not registered with username.");
		} else if (hs_get.text.StartsWith("SUCCESS")) {
			Debug.Log(hs_get.text);
			MainMenu.SetForgotErrorLabel("");
			MainMenu.CorrectEmail = true;
		} else if (hs_get.text.StartsWith("Query failed: Table")) {
			MainMenu.SetForgotErrorLabel("[F87431]Username not found. Try again.");
			Debug.Log(hs_get.text);
		} else {
			MainMenu.SetForgotErrorLabel("[F87431]Error. Please try again");
			Debug.Log(hs_get.text);
		}
	}

	public static IEnumerator CheckEmailWin(string name, string email) {
		string cheatHash = MD5Utils.MD5FromString(name + email + instance.emailSecretKey);

		string get_url = instance.checkEmailURL + "user=" + WWW.EscapeURL(name) + "&email=" + email + "&hash=" + cheatHash;

		//MainMenu.SetCreateButtonLabel("Submitting...");
		// Post the URL to the site and create a download object to get the result.
		WWW hs_get = new WWW(get_url);
		yield return hs_get; // Wait until the check is done

		if (hs_get.error != null) {
			Win.SetForgotErrorLabel("[F87431]Error. Please try again");
			Debug.LogWarning("There was an error checking email: " + hs_get.error);
		} else if (hs_get.text.StartsWith("ERROR: No user")) {
			Debug.Log(hs_get.text);
			Win.SetForgotErrorLabel("[F87431]Username not found. Try again.");
		} else if (hs_get.text.StartsWith("ERROR: Wrong")) {
			Debug.Log(hs_get.text);
			Win.SetForgotErrorLabel("[F87431]Email not registered with username.");
		} else if (hs_get.text.StartsWith("SUCCESS")) {
			Debug.Log(hs_get.text);
			Win.SetForgotErrorLabel("");
			Win.CorrectEmail = true;
		} else if (hs_get.text.StartsWith("Query failed: Table")) {
			Win.SetForgotErrorLabel("[F87431]Username not found. Try again.");
			Debug.Log(hs_get.text);
		} else {
			Win.SetForgotErrorLabel("[F87431]Error. Please try again");
			Debug.Log(hs_get.text);
		}
	}

	public static IEnumerator SendEmail(string user) {
		string key = MD5Utils.MD5FromString(user + DateTime.Now.ToString()).Substring(0, 10);
		string cheatHash = MD5Utils.MD5FromString(user + key + instance.emailSecretKey);

		string get_url = instance.sendEmailURL + "user=" + WWW.EscapeURL(user) + "&key=" + key + "&hash=" + cheatHash;

		//MainMenu.SetCreateButtonLabel("Submitting...");
		// Post the URL to the site and create a download object to get the result.
		WWW hs_get = new WWW(get_url);
		yield return hs_get; // Wait until the check is done

		if (hs_get.error != null) {
			MainMenu.SetForgotErrorLabel("[F87431]Error. Please try again");
			Debug.LogWarning("There was an error checking email: " + hs_get.error);
		} else if (hs_get.text.StartsWith("ERROR: No user")) {
			Debug.Log(hs_get.text);
			MainMenu.SetForgotErrorLabel("[F87431]Username not found. Try again.");
		} else if (hs_get.text.StartsWith("ERROR: Wrong")) {
			Debug.Log(hs_get.text);
			MainMenu.SetForgotErrorLabel("[F87431]Email not registered with username.");
		} else if (hs_get.text.StartsWith("<p>Message sent")) {
			Debug.Log(hs_get.text);
			MainMenu.SetForgotErrorLabel("");
			MainMenu.EmailSent = true;
		} else if (hs_get.text.StartsWith("Query failed: Table")) {
			MainMenu.SetForgotErrorLabel("[F87431]Username not found. Try again.");
			Debug.Log(hs_get.text);
		} else {
			MainMenu.SetForgotErrorLabel("[F87431]Error. Please try again");
			Debug.Log(hs_get.text);
		}
	}

	public static IEnumerator SendEmailWin(string user) {
		string key = MD5Utils.MD5FromString(user + DateTime.Now.ToString()).Substring(0, 10);
		string cheatHash = MD5Utils.MD5FromString(user + key + instance.emailSecretKey);

		string get_url = instance.sendEmailURL + "user=" + WWW.EscapeURL(user) + "&key=" + key + "&hash=" + cheatHash;

		//MainMenu.SetCreateButtonLabel("Submitting...");
		// Post the URL to the site and create a download object to get the result.
		WWW hs_get = new WWW(get_url);
		yield return hs_get; // Wait until the check is done

		if (hs_get.error != null) {
			Win.SetForgotErrorLabel("[F87431]Error. Please try again");
			Debug.LogWarning("There was an error checking email: " + hs_get.error);
		} else if (hs_get.text.StartsWith("ERROR: No user")) {
			Debug.Log(hs_get.text);
			Win.SetForgotErrorLabel("[F87431]Username not found. Try again.");
		} else if (hs_get.text.StartsWith("ERROR: Wrong")) {
			Debug.Log(hs_get.text);
			Win.SetForgotErrorLabel("[F87431]Email not registered with username.");
		} else if (hs_get.text.StartsWith("<p>Message sent")) {
			Debug.Log(hs_get.text);
			Win.SetForgotErrorLabel("");
			Win.EmailSent = true;
		} else if (hs_get.text.StartsWith("Query failed: Table")) {
			Win.SetForgotErrorLabel("[F87431]Username not found. Try again.");
			Debug.Log(hs_get.text);
		} else {
			Win.SetForgotErrorLabel("[F87431]Error. Please try again");
			Debug.Log(hs_get.text);
		}
	}

	private bool LoadSongList() {
		Debug.Log("Loading song list");
		songList.Clear();
		// Load song list
		string filename = Game.OnlineMode ? "songlist.r" : "songlist_offline.r";
		string path = "";
		if (Application.platform == RuntimePlatform.Android)
			path = (Application.persistentDataPath + "/" + filename);
		else if (Application.platform == RuntimePlatform.WindowsPlayer
			|| Application.platform == RuntimePlatform.WindowsEditor)
			path = (Application.persistentDataPath + "\\" + filename);
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
			Debug.LogWarning(e.Message);
			return false;
		}
	}

	public static void LoadNextSong() {
		currentlyShowingHS = (currentlyShowingHS + 1) % instance.separatedSongList.Length;
		instance.StartCoroutine(instance.fetchScores(currentlyShowingHS));
	}

	public static void LoadPrevSong() {
		currentlyShowingHS = ((currentlyShowingHS - 1) % instance.separatedSongList.Length + instance.separatedSongList.Length) % instance.separatedSongList.Length;
		instance.StartCoroutine(instance.fetchScores(currentlyShowingHS));

	}

	public static void LoadNextSongOffline() {
		currentlyShowingHSOffline = (currentlyShowingHSOffline + 1) % instance.separatedSongList.Length;
		LoadSongOffline(currentlyShowingHSOffline);
	}

	public static void LoadPrevSongOffline() {
		currentlyShowingHSOffline = ((currentlyShowingHSOffline - 1) % instance.separatedSongList.Length + instance.separatedSongList.Length) % instance.separatedSongList.Length;
		LoadSongOffline(currentlyShowingHSOffline);
	}

	public static void LoadNextMode() {

	}

	public static void LoadPrevMode() {

	}

	public static void LoadSong(string artist, string song, Game.Mode mode) {
		instance.StartCoroutine(instance.fetchScores(artist, song, mode));
	}

	private static void LoadSongOffline(int songListID) {
		string artist = instance.separatedSongList[songListID][0];
		string song = instance.separatedSongList[songListID][1];
		Game.Mode gameMode = (Game.Mode)Enum.Parse(typeof(Game.Mode), instance.separatedSongList[songListID][2]);
		LoadSongOffline(artist, song, gameMode);
	}

	public static void LoadSongOffline(string artist, string song, Game.Mode mode) {
		// Scores list.
		List<int> scores = new List<int>();

		// Title for display
		if (Application.loadedLevelName == "MainMenu") {
			instance.OfflineScoresSongLabel.text = artist + " - " + song;
			instance.OfflineSongModeLabel.text = mode.ToString();
		}

		// Fetch scores from file
		string fileName = "off_" + HSController.CalculateTableName(artist, song, mode) + ".rhs";
		string hspath = "";
		if (Application.platform == RuntimePlatform.Android)
			hspath = (Application.persistentDataPath + "/" + fileName);
		else if (Application.platform == RuntimePlatform.WindowsPlayer
			|| Application.platform == RuntimePlatform.WindowsEditor)
			hspath = (Application.persistentDataPath + "\\" + fileName);
		else {
			Debug.Log("PLATFORM NOT SUPPORTED YET");
			hspath = "";
		}
		string line;
		try {
			using (StreamReader sr = new StreamReader(hspath)) {
				while ((line = sr.ReadLine()) != null) {
					scores.Add(Int32.Parse(line));
				}
				sr.Close();
			}
		} catch (Exception) {
		}

		// Sort scores
		scores.Sort();
		scores.Reverse();

		int topScore = scores[0];
		int yourScoreIndex = 0;
		bool yourScoreIndexFound = false;
		for (int i = 0; i < scores.Count; i++) {
			if (scores[i] == Player.score && !yourScoreIndexFound) {
				yourScoreIndex = i;
				yourScoreIndexFound = true;
			}
		}

		// Show top score
		if (Application.loadedLevelName == "Win") {
			instance.offlineTop5names[0].text = "1.";
			instance.offlineTop5scores[0].text = topScore.ToString();
			if (topScore == Player.score) {
				instance.offlineTop5names[0].text = "[FDD017]" + instance.offlineTop5names[0].text;
				instance.offlineTop5names[0].transform.localScale = new Vector2(29, 29);
				instance.offlineTop5scores[0].text = "[FDD017]" + instance.offlineTop5scores[0].text;
				instance.offlineTop5scores[0].transform.localScale = new Vector2(29, 29);
				instance.offlineTop5scores[0].GetComponent<TweenScale>().enabled = true;
			}
		}

		// Show close 5 scores
		int scoresToShow = Application.loadedLevelName == "Win" ? 5 : 10;
		//int scoresAfter = (scores.Count - 1) - yourScoreIndex;
		int yourIndex = 0;
		if (scores.Count < scoresToShow) yourIndex = yourScoreIndex;
		for (int i = 0; i < scoresToShow; i++) {
			if (i > scores.Count - 1) {
				instance.offlineClose5names[i].text = "";
				instance.offlineClose5scores[i].text = "";
			} else {
				instance.offlineClose5names[i].text = (yourScoreIndex + i - yourIndex + 1).ToString() + ".";
				instance.offlineClose5scores[i].text = scores[yourScoreIndex + i - yourIndex].ToString();
			}
			if (i == yourIndex && Application.loadedLevelName == "Win") {
				instance.offlineClose5names[yourIndex].text = "[FDD017]" + instance.offlineClose5names[yourIndex].text;
				instance.offlineClose5names[yourIndex].transform.localScale = new Vector2(29, 29);
				instance.offlineClose5scores[yourIndex].text = "[FDD017]" + instance.offlineClose5scores[yourIndex].text;
				instance.offlineClose5scores[yourIndex].transform.localScale = new Vector2(29, 29);
				instance.offlineClose5scores[yourIndex].GetComponent<TweenScale>().enabled = true;
			}
		}
	}

	private static void Sort<T>(T[][] data, int col) {
		StringAsIntComparer comparer = new StringAsIntComparer();
		Array.Sort(data, (x, y) => comparer.Compare(x[col], y[col]));
	}
}
public class StringAsIntComparer : IComparer {
	public int Compare(object l, object r) {
		int left = Int32.Parse((string)l);
		int right = Int32.Parse((string)r);
		return left.CompareTo(right);
	}
}