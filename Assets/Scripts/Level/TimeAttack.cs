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
public class TimeAttack : Level {

    #region Fields
    private static TimeAttack Instance;                                  // The Instance of this class for self reference

    public UILabel timerLabel;                                      // The timer label. Inspector reference. Location: LevelManager
    public UILabel timeUpLabel;

    public float levelTimer;                            // Timer showing the time left on this level
    private float startTimer = 30;                      // Time to play on this level in seconds
    private string timerString;                         // String to parse the time to
    private string levelSeconds;                        // Seconds left as string
    private string levelHundredths;                     // Hundredths seconds left as string
    private bool displayTimer = true;                   // Used for the blinking of the timer
    private bool levelFinished;                         // Flag for whether the timer has expired or not

    #endregion

    #region Functions
    protected override void Awake() {
        // Local static reference to this class.
        //Instance = this;
        base.Awake();
    }

    protected override void Start() {
        Game.GameMode = Game.Mode.TimeAttack;
        Game.GameState = Game.State.Playing;
        base.Start();
        levelTimer = startTimer;
        Player.maxEnergy = 50;
        Player.ResetStats();
        EnemyScript.energyReturn = 2;
        enemySpawner.SpawnRate = 2f;
    }

    void Update() {
        // Decrease the level timer and grab the seconds and hundredths
        if (levelTimer > 0 && !levelFinished) {
            levelTimer -= Time.deltaTime;
            timerString = String.Format(levelTimer.ToString("00.00", CultureInfo.InvariantCulture));
            levelSeconds = timerString.Split('.')[0];
            levelHundredths = timerString.Split('.')[1];
        }

        if (Game.Paused) {
            timerLabel.text = "";
        } else {
            // Update the timer label
            if (displayTimer) {
                if (levelTimer < 10) {
                    timerLabel.text = "[FF2222]" + levelSeconds + ":" + levelHundredths;
                } else {
                    timerLabel.text = levelSeconds + ":" + levelHundredths;
                }
            } else {
                timerLabel.text = "";
            }
        }
        // If the player survives for 30 seconds, go to the "Win" level
        if (levelTimer < 0 && !levelFinished) {
            levelFinished = true;
            timeUpLabel.text = "[FF2222]Time's Up!";
            levelSeconds = "00";        // Reset the timer for "in-between-updates" values that might slip in
            levelHundredths = "00";     // Reset the timer for "in-between-updates" values that might slip in
            Time.timeScale = 0f;        // Pause the enemies
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
    #endregion

}
