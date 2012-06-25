using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

    #region Fields
    // Static screen side coordinates, gets set at launch of game.
    public static float screenLeft;
    public static float screenBottom;
    public static float screenTop;
    public static float screenRight;
    public static float screenMiddle;

    public bool isTemp;                         // Debug value for when the game manager is a temporary instance.

    private static bool devMode = false;        // True when devMode/debug Mode is enabled. Get's checked by DevScript.
    private static bool paused = false;
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
        DontDestroyOnLoad(gameObject);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Home) || Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown("escape") || Input.GetKeyDown(KeyCode.Space) && Application.loadedLevelName == "Game") {
                if (!paused)
                    Pause();
                else
                    Resume();
            return;
        }
        if (Input.GetKeyDown(KeyCode.BackQuote) && Application.loadedLevelName == "Game") {
            devMode = !devMode;
        }
    }

    public static void Pause() {
        AudioSource audio = Camera.main.GetComponent<AudioSource>();
        if (audio != null)
            audio.Pause();
        Time.timeScale = 0f;
        paused = !paused;
        Debug.Log(">> Game paused.");
    }

    public static void Resume() {
        AudioSource audio = Camera.main.GetComponent<AudioSource>();
        if (audio != null)
            audio.Play();
        Time.timeScale = 1f;
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
    }

    public static bool Paused {
        get { return paused; }
        set { paused = value; }
    }

    // TODO: Make a state engine for game states. For use when checking Pause state and
    // devmode etc..
    //public static bool StateA {

    //    get {
    //        switch(State) }

    //}

    public enum State {
        Playing,
        Menu,
        Paused
    }
    #endregion
}
