using UnityEngine;
using System.Collections;
using System.IO;

/// <summary>
/// Game.cs
/// 
/// The main game class. Holds constants and consistent methods over the entire state of the game.
/// </summary>
public class Game : MonoBehaviour
{

	#region Fields
	// Static screen side coordinates, gets set at launch of game.
	public static float screenLeft;
	public static float screenBottom;
	public static float screenTop;
	public static float screenRight;
	public static float screenMiddle;
	public bool isTemp;                         // Debug value for when the game manager is a temporary instance.
	public static bool sendSuper;				// Lame variable to indicate to everything about if you want to send a superpulse

	private static bool devMode = false;        // True when devMode/debug Mode is enabled. Gets checked by DevScript.
	private static bool paused = false;         // True when the game is paused
	private static bool cheated = false;        // True when devMode was on at any point of the game
	private static bool syncMode = false;		// True on 'NigelMode Option' Selected
	private static Mode mode;                    // Defines current/last game mode.
	private static State state;                   // Defines the current game state.
	private static string filePath;             // File path to the selected song to be loaded.
	private static string songName = "Unknown";
	private static string artistName = "Unknown";
	private static NumOfColors colors;			//Defines the amount of colors used in the game
	private static Powerups powerups;			//Defines the currently active powerup
	public static bool IsLoggedIn = false;
	// Settings
	private static float effectsVolume;
	private static float effectsVolumeDefault = 0.75f;
	private static float musicVolume;
	private static float musicVolumeDefault = 1f;
	private static bool colorBlindMode;
	private static bool colorBlindModeDefault = false;
	private static bool lowGraphicsMode;
	private static bool lowGraphicsModeDefault = false;
	private static bool rememberLogin;
	private static bool rememberLoginDefault = false;
	public static bool disablePause = false;
	private static bool isOnline = false;
	private static bool askOnlineMode = true;
	
	public static float cameraScaleFactor;

	// Progression settings
	public static bool ArcadeUnlocked = true;
	
	// Trial/Full version related settings
	private static readonly bool isFullVersion = false;
	public static bool isUnlockedVersion;
	#endregion

	#region Functions
	void Awake ()
	{
		// Stops phone screen from shutting down on timeout
		Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;
		//Application.targetFrameRate = 30;

		// Define screen edges based on screen resolution (requires camera to be placed at XY origin, i.e. Vector3 (0, 0, _) )
		screenLeft = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, 10)).x;
		screenBottom = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, 10)).y;
		screenRight = -screenLeft;
		screenTop = -screenBottom;
		screenMiddle = 0f;
		
		isUnlockedVersion = CheckForUnlockedVersion();
		//cameraScaleFactor = GetCameraScaleFactor();

		//if (PlayerPrefs.GetInt("casualplayed") > 4)
		//    ArcadeUnlocked = true;
	}

	void Start ()
	{		
		if (PlayerPrefs.GetString ("playername") != null)
			PlayerName = PlayerPrefs.GetString ("playername");
		IsLoggedIn = RememberLogin;

		DontDestroyOnLoad (gameObject);      // Makes sure this object is persistent between all scenes of the game
	}

	void Update ()
	{		
		// Key input condition for pausing the game
		if ((Input.GetKeyDown (KeyCode.Home) || Input.GetKeyDown (KeyCode.Escape) ||
			Input.GetKeyDown ("escape") || Input.GetKeyDown (KeyCode.Space)) && GameState == State.Playing) {
			if (!paused)
				Pause ();
			else if(PauseMenu.InOptions)
				PauseMenu.LeaveOptions();
			else
				Resume ();
			return;
		}
		// Key input position for DevMode, only works in the "Game" level
//		if (Input.GetKeyDown (KeyCode.BackQuote) && GameState == State.Playing) {
//			DevMode = !DevMode;
//		}
	}

	void OnApplicationPause ()
	{
		if (GameState == State.Playing && !Game.Paused)
			Pause ();
	}

	// Global method for pausing the game.
	public static void Pause ()
	{
		if(disablePause) return;
		if (GameState == State.Playing) 
			PauseMenu.Show();
		CommonPauseOperation();
		Debug.Log (">> Game paused.");
	}
	
	public static void CommonPauseOperation() {
		Time.timeScale = 0f;                        // Stop game time
		paused = true;
		if (GameState == State.Playing) 
			AudioPlayer.pause ();
	}
	
	public static void Resume(bool isTutorial) {
		if(isTutorial) {
			if (GameState == State.Playing) {
				disablePause = false;
				AudioPlayer.resume();	
				TutorialMenu.Hide();
			}
			Time.timeScale = 1f;                        // Restores game time
			paused = false;
		}
	}
	
	public static void CommonResumeOperation() {
		Time.timeScale = 1f;
		paused = false;
		if(GameState == State.Playing) 
			AudioPlayer.resume ();
	}

	public static void Resume ()
	{
		if(disablePause) return;
		if (GameState == State.Playing) 
			PauseMenu.Hide ();
		CommonResumeOperation();
		
		Debug.Log (">> Game resumed.");
	}

	/// <summary>
	/// Runs when any new scene was loaded
	/// </summary>
	void OnLevelWasLoaded (int level)
	{
		Debug.Log (">> Level: \"" + Application.loadedLevelName + "\" was loaded");
		if (level == (int)LevelNames.Game) {
			GameState = State.Playing;
		} else {
			GameState = State.Menu;
			Resume ();
		}
		DevMode = false;
		Debug.Log (">> Current game mode: " + GameMode);
	}
	
	public static float GetCameraScaleFactor() {
		return 1f;
		//// Magic starts here
		//float aspectRatioMultiplier = 1f;
		//if((float)Screen.height/(float)Screen.width < 480f/800f) aspectRatioMultiplier = (480f/800f) / ((float)Screen.height/(float)Screen.width);
		//else if((float)Screen.height/(float)Screen.width > 480f/800f) aspectRatioMultiplier =  ((float)Screen.height/(float)Screen.width)/ (480f/800f);
		
		//if(Screen.width >= 800f) {
		//    if(800f/Screen.width > 480f/Screen.height)
		//        return aspectRatioMultiplier* 800f/Screen.width;
		//    else
		//        return aspectRatioMultiplier* 480f/Screen.height;
		//}
		//else {
		//    if(800f/Screen.width < 480f/Screen.height)
		//        return aspectRatioMultiplier* 800f/Screen.width;
		//    else
		//        return aspectRatioMultiplier* 480f/Screen.height;
		//}
		//// Magic ends here
	}

	/// <summary>
	/// Getter for DevMode value
	/// </summary>
	public static bool DevMode {
		get { return devMode; }
		private set {
			devMode = value;
			if (value)
				cheated = true;
		}
	}

	/// <summary>
	/// Getter and Setter for the pause state (might not be necessary given the 
	/// Pause() and Resume() method but will keep it here for now)
	/// </summary>
	public static bool Paused {
		get { return paused; }
		set { paused = value; }
	}

	public static bool Cheated {
		get { return cheated; }
		set { cheated = value; }
	}

	public static string Song {
		get {
			if (filePath == "" || filePath == null)
				filePath = PlayerPrefs.GetString ("lastSongPlayed");
			return filePath; 
		}
		set {
			filePath = value; 
			PlayerPrefs.SetString ("lastSongPlayed", filePath);
		}
	}

	public static string SongName {
		get { return songName; }
		set { songName = value; }
	}

	public static string Artist {
		get { return artistName; }
		set { artistName = value; }
	}

	public static string Path {
		get {
			if (filePath != null) {
				return new DirectoryInfo (filePath).Parent.FullName;
			} else {
				return null;
			}
		}
	}

	public static State GameState {
		get { return state; }
		set { state = value; }
	}

	public static Mode GameMode {
		get { return mode; }
		set { mode = value; }
	}

	public static NumOfColors ColorMode {
		get { return colors; }
		set { colors = value; }
	}

	public static bool SyncMode {
		get { return syncMode; }
		set { syncMode = value; }
	}

	public static Powerups PowerupActive {
		get { return powerups; }
		set { powerups = value; }
	}

	public static string PlayerName {
		get {
			if (PlayerPrefs.HasKey ("playername"))
				return PlayerPrefs.GetString ("playername");
			else
				return null;
		}
		set { PlayerPrefs.SetString ("playername", value); }
	}

	public static float EffectsVolume {
		get {
			if (PlayerPrefs.HasKey ("effectsVolume")) {
				float tempVolume = PlayerPrefs.GetFloat ("effectsVolume");
				effectsVolume = Mathf.Clamp (tempVolume, 0f, 1f);
				return effectsVolume;
			} else {
				PlayerPrefs.SetFloat ("effectsVolume", effectsVolumeDefault);
				return effectsVolumeDefault;
			}
		}
		set {
			float tempVolume = Mathf.Clamp (value, 0f, 1f);
			effectsVolume = Mathf.Clamp (tempVolume, 0f, 1f);
			PlayerPrefs.SetFloat ("effectsVolume", tempVolume);
		}
	}

	public static float MusicVolume {
		get {
			if (PlayerPrefs.HasKey ("musicVolume")) {
				float tempVolume = PlayerPrefs.GetFloat ("musicVolume");
				musicVolume = Mathf.Clamp (tempVolume, 0f, 1f);
				return musicVolume;
			} else {
				PlayerPrefs.SetFloat ("musicVolume", musicVolumeDefault);
				return musicVolumeDefault;
			}
		}
		set {
			float tempVolume = Mathf.Clamp (value, 0f, 1f);
			musicVolume = Mathf.Clamp (tempVolume, 0f, 1f);
			PlayerPrefs.SetFloat ("musicVolume", tempVolume);
		}
	}

	public static bool ColorBlindMode {
		get {
			if (PlayerPrefs.HasKey ("colorBlindMode")) {
				bool tempValue = PlayerPrefs.GetInt ("colorBlindMode") > 0 ? true : colorBlindModeDefault;
				colorBlindMode = tempValue;
				return colorBlindMode;
			} else {
				PlayerPrefs.SetInt ("colorBlindMode", colorBlindModeDefault ? 1 : 0);
				return colorBlindModeDefault;
			}
		}
		set {
			colorBlindMode = value;
			int tempValue = colorBlindMode ? 1 : 0;
			PlayerPrefs.SetInt ("colorBlindMode", tempValue);
		}
	}

	public static bool LowGraphicsMode {
		get {
			if (PlayerPrefs.HasKey ("lowGraphicsMode")) {
				bool tempValue = PlayerPrefs.GetInt ("lowGraphicsMode") > 0 ? true : lowGraphicsModeDefault;
				lowGraphicsMode = tempValue;
				return lowGraphicsMode;
			} else {
				PlayerPrefs.SetInt ("lowGraphicsMode", lowGraphicsModeDefault ? 1 : 0);
				return lowGraphicsModeDefault;
			}
		}
		set {
			lowGraphicsMode = value;
			int tempValue = lowGraphicsMode ? 1 : 0;
			PlayerPrefs.SetInt ("lowGraphicsMode", tempValue);
		}
	}

	public static bool RememberLogin {
		get {
			if (PlayerPrefs.HasKey ("rememberLogin")) {
				bool tempValue = PlayerPrefs.GetInt ("rememberLogin") > 0 ? true : rememberLoginDefault;
				rememberLogin = tempValue;
				return rememberLogin;
			} else {
				PlayerPrefs.SetInt ("rememberLogin", rememberLoginDefault ? 1 : 0);
				return rememberLoginDefault;
			}
		}
		set {
			rememberLogin = value;
			int tempValue = rememberLogin ? 1 : 0;
			PlayerPrefs.SetInt ("rememberLogin", tempValue);
		}
	}

	public static bool OnlineMode {
		get {
			return isOnline;
		}
		set {
			isOnline = value;
			if (askOnlineMode) askOnlineMode = false;
		}
	}

	public static bool AskOnlineMode {
		get {
			return askOnlineMode;
		}
	}

	/// <summary>
	/// Level order number given by the build settings. WARNING: Needs to be kept up to date manually if adding/removing scenes!
	/// There is no way to automatically get the right order for the scenes so it might not be accurate if the order has changed
	/// and this was not updated!
	/// </summary>
	public enum LevelNames
	{
		SplashScreen,
		MainMenu,
		Game,
		Win
	}

	/// <summary>
	/// Not in use at the moment.
	/// </summary>
	public enum State
	{
		Menu,
		Playing,
		Paused
	}

	public enum Mode
	{
		Arcade,
		Casual,
		Tutorial,
		Challenge
	}

	public enum NumOfColors
	{
		Six,
		Four
	}

	public enum Powerups
	{
		None,
		MassivePulse,				//Massive Pulse - No Energy to Send, Destroys All Enemies, Score Does not increase
		ChainReaction,
		Invincible
	}

	private static bool CheckForUnlockedVersion() {
		if(PlayerPrefs.HasKey("unlockedVersion")) {
			return (PlayerPrefs.GetInt ("unlockedVersion") > 0 ? true : false) || isFullVersion;
		}
		else {
			PlayerPrefs.SetInt("unlockedVersion",0);
			return false || isFullVersion;
		}
	}
	#endregion
}

