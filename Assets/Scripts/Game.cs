using UnityEngine;
using System.Collections;
using System.IO;
/// <summary>
/// Game.cs
/// 
/// The main game class. Holds constants and consistent methods over the entire state of the game.
/// </summary>
public class Game : MonoBehaviour {

	#region Fields
	// Static screen side coordinates, gets set at launch of game.
	public static float screenLeft;
	public static float screenBottom;
	public static float screenTop;
	public static float screenRight;
	public static float screenMiddle;

	public bool isTemp;                         // Debug value for when the game manager is a temporary instance.

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
	#endregion

	#region Functions
	void Awake() {
		// Stops phone screen from shutting down on timeout
		Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;

		// Define screen edges based on screen resolution (requires camera to be placed at XY origin, i.e. Vector3 (0, 0, _) )
		screenLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10)).x;
		screenBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10)).y;
		screenRight = -screenLeft;
		screenTop = -screenBottom;
		screenMiddle = 0f;
		//if(Application.platform == RuntimePlatform.WindowsEditor && filePath == null) 
		//    filePath = ""; //"C:\\Users\\Scott\\Desktop\\test.mp3";

	}

	void Start() {
		if (PlayerPrefs.GetString("playername") != null) PlayerName = PlayerPrefs.GetString("playername");
		DontDestroyOnLoad(gameObject);      // Makes sure this object is persistent between all scenes of the game
		//AudioManager.initMusic(@"peppers.wav");
	}

	void Update() {
		// Key input condition for pausing the game
		if ((Input.GetKeyDown(KeyCode.Home) || Input.GetKeyDown(KeyCode.Escape) ||
			Input.GetKeyDown("escape") || Input.GetKeyDown(KeyCode.Space)) && GameState == State.Playing) {
			if (!paused)
				Pause();
			else
				Resume();
			return;
		}
		// Key input position for DevMode, only works in the "Game" level
		if (Input.GetKeyDown(KeyCode.BackQuote) && GameState == State.Playing) {
			DevMode = !DevMode;
		}

		//if (Application.loadedLevelName == "Game" || Application.loadedLevelName == "DeathMatch") {
		//    switch (mode) {
		//        case Mode.TimeAttack:
		//            break;
		//        case Mode.DeathMatch:
		//            break;
		//        default:
		//            break;
		//    }
		//}
	}

	void OnApplicationPause() {
		if (GameState == State.Playing && !Game.Paused)
			Pause();
	}

	// Global method for pausing the game.
	public static void Pause() {
		//        AudioSource audio = Camera.main.GetComponent<AudioSource>();
		//        if (audio != null)	// Pause the music if it exists
		if (GameState == State.Playing) {
			AudioPlayer.pause();
			PauseMenu.Show();
		}
		Time.timeScale = 0f;                        // Stop game time
		paused = true;
		Debug.Log(">> Game paused.");
	}

	public static void Resume() {
		//        AudioSource audio = Camera.main.GetComponent<AudioSource>();
		//        if (audio != null)
		if (GameState == State.Playing) {
			AudioPlayer.resume();                           // Resume the music if it exists
			PauseMenu.Hide();
		}
		Time.timeScale = 1f;                        // Restores game time
		paused = false;
		Debug.Log(">> Game resumed.");
	}

	/// <summary>
	/// Runs when any new scene was loaded
	/// </summary>
	void OnLevelWasLoaded(int level) {
		Debug.Log(">> Level: \"" + Application.loadedLevelName + "\" was loaded");
		if (level == (int)LevelNames.Game) {
			GameState = State.Playing;
		} else {
			GameState = State.Menu;
			Resume();
		}
		DevMode = false;
		Debug.Log(">> Current game mode: " + GameMode);
	}

	/// <summary>
	/// Getter for DevMode value
	/// </summary>
	public static bool DevMode {
		get { return devMode; }
		private set {
			devMode = value;
			if (value) cheated = true;
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
				filePath = PlayerPrefs.GetString("lastSongPlayed");
			return filePath; 
		}
		set {
			filePath = value; 
			PlayerPrefs.SetString("lastSongPlayed", filePath);
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
				return new DirectoryInfo(filePath).Parent.FullName;
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
		get { return PlayerPrefs.GetString("playername"); }
		set { PlayerPrefs.SetString("playername", value); }
	}

	public static float EffectsVolume {
		get {
			if (PlayerPrefs.HasKey("effectsVolume")) {
				float tempVolume = PlayerPrefs.GetFloat("effectsVolume");
				effectsVolume = Mathf.Clamp(tempVolume, 0f, 1f);
				return effectsVolume;
			} else {
				PlayerPrefs.SetFloat("effectsVolume", effectsVolumeDefault);
				return effectsVolumeDefault;
			}
		}
		set {
			float tempVolume = Mathf.Clamp(value, 0f, 1f);
			effectsVolume = Mathf.Clamp(tempVolume, 0f, 1f);
			PlayerPrefs.SetFloat("effectsVolume", tempVolume);
		}
	}

	public static float MusicVolume {
		get {
			if (PlayerPrefs.HasKey("musicVolume")) {
				float tempVolume = PlayerPrefs.GetFloat("musicVolume");
				musicVolume = Mathf.Clamp(tempVolume, 0f, 1f);
				return musicVolume;
			} else {
				PlayerPrefs.SetFloat("musicVolume", musicVolumeDefault);
				return musicVolumeDefault;
			}
		}
		set {
			float tempVolume = Mathf.Clamp(value, 0f, 1f);
			musicVolume = Mathf.Clamp(tempVolume, 0f, 1f);
			PlayerPrefs.SetFloat("musicVolume", tempVolume);
		}
	}

	public static bool ColorBlindMode {
		get {
			if (PlayerPrefs.HasKey("colorBlindMode")) {
				bool tempValue = PlayerPrefs.GetInt("colorBlindMode") > 0 ? true : colorBlindModeDefault;
				colorBlindMode = tempValue;
				return colorBlindMode;
			} else {
				PlayerPrefs.SetInt("colorBlindMode", colorBlindModeDefault ? 1 : 0);
				return colorBlindModeDefault;
			}
		}
		set {
			colorBlindMode = value;
			int tempValue = colorBlindMode ? 1 : 0;
			PlayerPrefs.SetInt("colorBlindMode", tempValue);
		}
	}

	public static bool LowGraphicsMode {
		get {
			if (PlayerPrefs.HasKey("lowGraphicsMode")) {
				bool tempValue = PlayerPrefs.GetInt("lowGraphicsMode") > 0 ? true : lowGraphicsModeDefault;
				lowGraphicsMode = tempValue;
				return lowGraphicsMode;
			} else {
				PlayerPrefs.SetInt("lowGraphicsMode", lowGraphicsModeDefault ? 1 : 0);
				return lowGraphicsModeDefault;
			}
		}
		set {
			lowGraphicsMode = value;
			int tempValue = lowGraphicsMode ? 1 : 0;
			PlayerPrefs.SetInt("lowGraphicsMode", tempValue);
		}
	}

	/// <summary>
	/// Level order number given by the build settings. WARNING: Needs to be kept up to date manually if adding/removing scenes!
	/// There is no way to automatically get the right order for the scenes so it might not be accurate if the order has changed
	/// and this was not updated!
	/// </summary>
	public enum LevelNames {
		SplashScreen,
		MainMenu,
		Game,
		Win
	}

	/// <summary>
	/// Not in use at the moment.
	/// </summary>
	public enum State {
		Menu,
		Playing,
		Paused
	}

	public enum Mode {
		Arcade,
		Casual,
		Story,
		Challenge
	}

	public enum NumOfColors {
		Six,
		Four
	}

	public enum Powerups {
		None,
		TimeSlow,
		MassivePulse,				//Massive Pulse - No Energy to Send, Destroys All Enemies, Score Does not increase
		ChainReaction,
		Invincible,
		ChangeColor
	}


	#endregion
}

