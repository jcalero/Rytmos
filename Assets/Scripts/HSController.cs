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

	private string addScoreURL = "http://rytmos-game.com/addscorenew.php?"; //be sure to add a ? to your url
	private string top5URL = "http://rytmos-game.com/displaytop5.php?";
	private string close5URL = "http://rytmos-game.com/displayclose5.php?";


	public string[][] hsTimeAttack = new string[10][];
	public string[][] hsSurvival = new string[10][];

	private string[][] top5List;
	private string[][] close5List;

	private List<string> songList = new List<string>();
	private string[][] separatedSongList;
	private static int currentlyShowingHS = 0;
	private static bool noSongsLoaded;

	public UILabel[] top5names;
	public UILabel[] top5scores;
	public UILabel[] close5names;
	public UILabel[] close5scores;
	public UILabel ScoresModeLabel;
	public UILabel ScoresSongLabel;
	public UIButton NextSongButton;
	public UIButton PrevSongButton;
	public UIButton NextModeButton;
	public UIButton PrevModeButton;

	public static string FetchError;

	private static HSController instance;

	void Awake() {
		instance = this;

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

		Win.SetSubmitText("Submitting...");
		//instance.submittedLabel.text = "Submitting...";
		// Post the URL to the site and create a download object to get the result.
		WWW hs_post = new WWW(post_url);
		yield return hs_post; // Wait until the download is done

		if (hs_post.error != null) {
			Debug.LogWarning("There was an error posting the high score: " + hs_post.error);
			//instance.submittedLabel.text = "";
			Win.SetSubmitText("");
			//instance.errorLabel.text = "[FF2222]Error submitting, please try again.";
			Win.SetErrorText("[FF2222]Error submitting, please try again.");
			Win.ShowSubmitBox();
		} else {
			//instance.submittedLabel.text = "Highscore submitted";
			Win.SetSubmitText("Highscore submitted");
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

	public static IEnumerator GetClose5Scores(string artist, string song, Game.Mode gameMode, int score) {

		string table = CalculateTableName(artist, song, gameMode);
		string hs_url = instance.close5URL + "table=" + table + "&score=" + score;
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
		if (!instance.LoadSongList()) {
			UITools.SetActiveState(instance.NextSongButton, false);
			UITools.SetActiveState(instance.PrevSongButton, false);
			UITools.SetActiveState(instance.NextModeButton, false);
			UITools.SetActiveState(instance.PrevModeButton, false);
			instance.ScoresSongLabel.maxLineCount = 2;
			instance.ScoresSongLabel.text = "No songs have been played yet! Play some first.";
			instance.ScoresModeLabel.text = "";
			int labelcnt = 0;
			while (labelcnt < 5) {
				instance.top5names[labelcnt].text = "";
				instance.close5names[labelcnt].text = "";
				instance.top5scores[labelcnt].text = "";
				instance.close5scores[labelcnt].text = "";
				labelcnt++;
			}

		} else {
			instance.ScoresSongLabel.maxLineCount = 1;
			string artist = instance.separatedSongList[0][0];
			string song =   instance.separatedSongList[0][1];
			Game.Mode gameMode = (Game.Mode)Enum.Parse(typeof(Game.Mode), instance.separatedSongList[0][2]);

			instance.StartCoroutine(instance.fetchScores(artist, song, gameMode));
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
		ScoresModeLabel.text = gameMode.ToString();
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

		yield return StartCoroutine(GetTop5Scores(artist, song, gameMode));
		if (FetchError == null) {
			for (int cnt = 0; cnt < Top5List.Length; cnt++) {
				if (cnt < 5) {
					top5names[cnt].text = (cnt + 1) + ". " + Top5List[cnt][0];
					top5scores[cnt].text = UITools.FormatNumber(Top5List[cnt][1]);
				}
				//Debug.Log(HSController.ScoresList[cnt][0] + " :: " + HSController.ScoresList[cnt][1]);
			}
		} else {
			top5names[0].text = FetchError;
		}

		yield return StartCoroutine(GetClose5Scores(artist, song, gameMode, topScore));
		if (FetchError == null) {
			bool formattedOwnRow = false;
			Sort<string>(Close5List, 2);
			for (int cnt = 0; cnt < Close5List.Length; cnt++) {
				if (cnt < 5) {
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

	public static void LoadNextMode() {

	}

	public static void LoadPrevMode() {

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