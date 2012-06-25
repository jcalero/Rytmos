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

    private static bool devMode = false;        // True when devMode/debug Mode is enabled. Get's checked by DevScript.
    #endregion

    #region Functions
    void Awake() {
        // Stops phone screen from shutting down on timeout
        Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;

        // Make sure the game manager stays throughout all scenes.
        DontDestroyOnLoad(gameObject);

        // Define screen edges based on screen resolution (requires camera to be placed at XY origin, i.e. Vector3 (0, 0, _) )
        screenLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10)).x;
        screenBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10)).y;
        screenRight = -screenLeft;
        screenTop = -screenBottom;
        screenMiddle = 0f;
    }

    void Update() {
        //if (Input.GetKeyDown(KeyCode.Home) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("escape"))
        //{
        //    Application.Quit();
        //    return;
        //}
        if (Input.GetKeyDown(KeyCode.BackQuote)) {
            devMode = !devMode;
        }
    }

    /// <summary>
    /// Runs when any new scene was loaded
    /// </summary>
    void OnLevelWasLoaded() {
        Debug.Log("Level: \"" + Application.loadedLevelName + "\" was loaded.");
    }

    /// <summary>
    /// Getter for DevMode value
    /// </summary>
    public static bool DevMode {
        get { return devMode; }
    }

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
