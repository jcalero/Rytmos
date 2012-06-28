using UnityEngine;
using System.Collections;
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
    }

    void Start() {
        DontDestroyOnLoad(gameObject);      // Makes sure this object is persistent between all scenes of the game
		//AudioManager.initMusic(@"C:/Users/Samuel/Desktop/trudat.wav");
    }

    void Update() {
        // Key input condition for pausing the game
        if (Input.GetKeyDown(KeyCode.Home) || Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown("escape") || Input.GetKeyDown(KeyCode.Space) && Application.loadedLevelName == "Game") {
            if (!paused)
                Pause();
            else
                Resume();
            return;
        }
        // Key input position for DevMode, only works in the "Game" level
        if (Input.GetKeyDown(KeyCode.BackQuote) && Application.loadedLevelName == "Game") {
            DevMode = !DevMode;
        }
    }

    // Global method for pausing the game.
    public static void Pause() {
        AudioSource audio = Camera.main.GetComponent<AudioSource>();
        if (audio != null)                          // Pause the music if it exists
            audio.Pause();
        Time.timeScale = 0f;                        // Stop game time
        paused = !paused;
        Debug.Log(">> Game paused.");
    }

    public static void Resume() {
        AudioSource audio = Camera.main.GetComponent<AudioSource>();
        if (audio != null)
            audio.Play();                           // Resume the music if it exists
        Time.timeScale = 1f;                        // Restores game time
        paused = !paused;
        Debug.Log(">> Game resumed.");
    }

    /// <summary>
    /// Runs when any new scene was loaded
    /// </summary>
    void OnLevelWasLoaded() {
        Debug.Log(">> Level: \"" + Application.loadedLevelName + "\" was loaded");
    }

    /// <summary>
    /// Getter for DevMode value
    /// </summary>
    public static bool DevMode {
        get { return devMode; }
        private set {
            devMode = value;
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

    // TODO: Make a state engine for game states. For use when checking Pause state and
    // devmode etc..
    //public static bool StateA {

    //    get {
    //        switch(State) }

    //}

    /// <summary>
    /// Not in use at the moment.
    /// </summary>
    public enum State {
        Playing,
        Menu,
        Paused
    }
    #endregion
}
