using UnityEngine;
using System.Collections;
using System;
using System.Globalization;
/// <summary>
/// Level.cs
/// 
/// Level manager. Handles all the level feedback. Main class for potentially different level types that
/// inherit from this class.
/// </summary>
public class Level : MonoBehaviour {

    #region Fields
    private static Level Instance;                                  // The Instance of this class for self reference

    public static Color purple = new Color(.5f, 0, .5f, 1f);

    private LinkedSpriteManager spriteManagerScript;
    private Sprite touchSprite;                                     // The SpriteManager created sprite

    public GameObject[] particlesFeedback = new GameObject[6];      // The six feedback particles. Inspector reference. Location: LevelManager
    public GameObject[] linePrefab = new GameObject[6];             // The six feedback screen lines. Inspector reference. Location: LevelManager
    public GameObject spriteManager;                                // Reference to the SpriteManager. Inspector reference. Location: LevelManager
    public GameObject touchPrefab;                                  // The touch sprite. Inspector reference. Location: LevelManager
    public UILabel timerLabel;                                      // The timer label. Inspector reference. Location: LevelManager
    public UILabel timeUpLabel;

    public float levelTimer;                            // Timer showing the time left on this level
    private float startTimer = 30;                      // Time to play on this level
    private string timerString;                         // String to parse the time to
    private string levelSeconds;                        // Seconds left as string
    private string levelHundredths;                     // Hundredths seconds left as string
    private bool displayTimer = true;                   // Used for the blinking of the timer
    private bool levelFinished;                         // Flag for whether the timer has expired or not
    #endregion

    #region Functions
    void Awake() {
        // Local static reference to this class.
        Instance = this;
        spriteManagerScript = spriteManager.GetComponent<LinkedSpriteManager>();
    }

    void Start() {
        levelTimer = startTimer;

        // Create and hide the touch sprite
        touchSprite = spriteManagerScript.AddSprite(touchPrefab, 0.25f, 0.25f, new Vector2(0f, 0.365f), new Vector2(0.63f, 0.63f), false);
        touchSprite.hidden = true;

        // Set up level feedback stuff
        SetUpBorderLineFeedback();
        SetUpParticlesFeedback();
    }

    void Update() {
        // Decrease the level timer and grab the seconds and hundredths
        if (levelTimer > 0) {
            levelTimer -= Time.deltaTime;
            timerString = String.Format(levelTimer.ToString("00.00", CultureInfo.InvariantCulture));
            levelSeconds = timerString.Split('.')[0];
            levelHundredths = timerString.Split('.')[1];
        }

        // Update the timer label
        if (displayTimer) {
            timerLabel.text = levelSeconds + ":" + levelHundredths;

            if (levelTimer < 10) {
                timerLabel.text = "[FF2222]" + levelSeconds + ":" + levelHundredths;
            }
        } else {
            timerLabel.text = "";
        }

        // If the player survives for 30 seconds, go to the "Win" level
        if (levelTimer < 0 && !levelFinished) {
            //Application.LoadLevel("Win");
            levelFinished = true;
            timeUpLabel.text = "[FF2222]Time's Up!";
            Time.timeScale = 0f;
            StartCoroutine(TimerEnd());
        }

        // If the score is 100 or more, go to the "Win" level
        //if (Player.score >= 100)
        //    Application.LoadLevel("Win");
    }

    IEnumerator TimerEnd() {
        yield return StartCoroutine(TimerBlink());
        Time.timeScale = 1f;
        Application.LoadLevel("Win");
    }
    
    IEnumerator TimerBlink() {
        if (displayTimer) timerLabel.text = "[FF2222]00:00";
        float blinkTime = Time.realtimeSinceStartup + 3f;
        while (Time.realtimeSinceStartup < blinkTime) {
            if (blinkTime - Time.realtimeSinceStartup > 2.5f) {
                displayTimer = false;
                yield return 0;
            } else if (blinkTime - Time.realtimeSinceStartup > 2.0f) {
                displayTimer = true;
                yield return 0;
            } else if (blinkTime - Time.realtimeSinceStartup > 1.5f) {
                displayTimer = false;
                yield return 0;
            } else if (blinkTime - Time.realtimeSinceStartup > 1.0f) {
                displayTimer = true;
                yield return 0;
            } else if (blinkTime - Time.realtimeSinceStartup > 0.5f) {
                displayTimer = false;
                yield return 0;
            } else if (blinkTime - Time.realtimeSinceStartup > 0.01f) {
                displayTimer = true;
                yield return 0;
            }
        }
    }
    /// <summary>
    /// Shows the touch sprite at the "pos" location with the respective colour
    /// on that position
    /// </summary>
    /// <param name="pos">The position the sprite should spawn at</param>
    public static void ShowTouchSprite(Vector3 pos) {
        Instance.touchSprite.SetColor(singleColourSelect(Input.mousePosition) + new Color(0.3f, 0.3f, 0.3f));
        Instance.touchPrefab.transform.position = pos;
        Instance.touchSprite.hidden = false;
        // Animate the sprite "bouncing"
        iTween.ScaleFrom(Instance.touchPrefab, iTween.Hash("scale", new Vector3(1.3f, 1.3f, 1.3f), "time", 0.3f, "oncomplete", "HideTouchSprite", "oncompletetarget", Instance.gameObject));
    }

    /// <summary>
    /// Hides the touch sprite
    /// </summary>
    private void HideTouchSprite() {
        touchSprite.hidden = true;
    }

    /// <summary>
    /// Positions the particle feedbacks at the correct location of the screen
    /// </summary>
    private void SetUpParticlesFeedback() {
        particlesFeedback[0].transform.localPosition = new Vector3(Game.screenLeft, Game.screenBottom, 0);
        particlesFeedback[1].transform.localPosition = new Vector3(Game.screenMiddle, Game.screenBottom, 0);
        particlesFeedback[2].transform.localPosition = new Vector3(Game.screenRight, Game.screenBottom, 0);
        particlesFeedback[3].transform.localPosition = new Vector3(Game.screenLeft, Game.screenTop, 0);
        particlesFeedback[4].transform.localPosition = new Vector3(Game.screenMiddle, Game.screenTop, 0);
        particlesFeedback[5].transform.localPosition = new Vector3(Game.screenRight, Game.screenTop, 0);
    }

    private void SetUpBorderLineFeedback() {
        /*
         * Here we will possibly have to reallocate the possible lines if we are dealing with different amounts of colours
         */

        //First line - Green to cyan
        linePrefab[0].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenLeft, Game.screenBottom));
        linePrefab[0].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenMiddle, Game.screenBottom));

        //Second line - Cyan to blue
        linePrefab[1].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenMiddle, Game.screenBottom));
        linePrefab[1].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenRight, Game.screenBottom));

        //Third line - blue to purple
        linePrefab[2].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenRight, Game.screenBottom));
        linePrefab[2].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenRight, Game.screenTop));

        //Fourth line - purple to red
        linePrefab[3].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenRight, Game.screenTop));
        linePrefab[3].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenMiddle, Game.screenTop));

        //Fifth line - red to yellow
        linePrefab[4].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenMiddle, Game.screenTop));
        linePrefab[4].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenLeft, Game.screenTop));

        //Sixth line - yellow to green
        linePrefab[5].GetComponent<LineRenderer>().SetPosition(0, new Vector3(Game.screenLeft, Game.screenTop));
        linePrefab[5].GetComponent<LineRenderer>().SetPosition(1, new Vector3(Game.screenLeft, Game.screenBottom));
    }

    /// <summary>
    /// Helper function to find the colour from a given screen coordinate. I.e. Input.mousePosition
    /// </summary>
    /// <param name="xy">The x,y coordinates of the screen location as Vector2</param>
    /// <returns>The color at the given coordinates</returns>
    public static Color singleColourSelect(Vector2 xy) {
        float normalizedX = xy.x - (Screen.width / 2);
        float normalizedY = xy.y - (Screen.height / 2);
        float angle = (Mathf.Rad2Deg * Mathf.Atan2(normalizedY, normalizedX));

        if (angle > 0) {
            if (angle < 60) {
                //Purple - top right.
                return purple;
            } else {
                if (angle > 120) {
                    //Yellow - Top left
                    return Color.yellow;
                } else {
                    //Red - Top Middle
                    return Color.red;
                }
            }
        } else {
            if (angle > -60) {
                //Blue - Bottom right
                return Color.blue;
            } else {
                if (angle < -120) {
                    //Green - bottom left
                    return Color.green;
                } else {
                    //cyan - bottom middle
                    return Color.cyan;
                }
            }
        }
    }
    #endregion

}
