using UnityEngine;
using System.Collections;
/// <summary>
/// Player.cs
/// 
/// Player manager. Handles player values and control.
/// </summary>
public class Player : MonoBehaviour,PeakListener {
    #region Fields
    public static int startScore = 0;           // Start score
    public static int startHealth = 100;        // Starting health of the player
    public static int maxHealth = 100;          // Maximum health of the player
    public static int startEnergy = 50;         // Starting energy of the player
    public static int maxEnergy = 50;           // Maximum energy of the player
    public static bool GodMode = false;         // God mode

    public static int energy;                   // Current energy of the player
    public static int health;                   // Current health of the player
    public static int score;                    // Current score of the player

    public GameObject pulsePrefab;              // The pulse. Inspector reference. Location: Player

    private float energyTimer = 0;              // Timer for energy regeneration
	private float pwTimer = 0;					// Timer for current invincibility duration
	private readonly float pwTotalTime = 10f;	// Total time for invincibility to last
	
	private bool glowAnimOut = false;			// Flag for starting glow animation
	private bool glowAnimIn = false;			// Flag for second half of glow animation
	private float[] glowTimers = new float[3];	// Timers for each ring
	private bool sentPulse = false;				// Flag for if you have sent a pulse
	private bool sentPulseTwo = false;			// Second half of the pulse animation
	
    private int pulseCost = 10;                 // Cost of a pulse
    private float energyRegenRate = 0.5f;       // The rate at which the energy regenerates. Lower is faster.
	private int superPulseCount = 0;			// Counter for the amount of invincible pulses
	private readonly int superPulseTotal = 3;	// Total amount of invincible pulses a player can send
	
	public GameObject[] players = new GameObject[3];
	private MeshRenderer[] meshRenders = new MeshRenderer[3];
	private Color[] playerColor;
    #endregion

    #region Functions
    void Start() {
        // Resets player stats at the start of a level
		for(int i=0; i<players.Length; i++) {
			meshRenders[i] = players[i].GetComponent<MeshRenderer>();
		}
		//TODO: Make this relative to how many rings in the player we have
		playerColor = new Color[]{new Color(1,1,1,0), new Color(1,1,1,0), new Color(1,1,1,0)};
		
		PeakTriggerManager.addSelfToListenerList(this);
		
        ResetStats();
    }

    void Update() {
        if (!Game.Paused) {
            // If the player clicks, and has enough energy, sends out a pulse
            if (Input.GetMouseButtonDown(0) && energy - pulseCost >= 0) {
                // Calculates screen location based on mouse position
                Vector3 tempPos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0f);
                // Show the touch sprite at the mouse location.
                Level.ShowTouchSprite(tempPos);
                // Create a pulse and trigger animation
                Instantiate(pulsePrefab, new Vector3(0, 0, 0), pulsePrefab.transform.localRotation);
				sentPulse = true;
				resetGlowTimers();
                // Reduce the player energy if not a superpulse
				if(Game.PowerupActive != Game.Powerups.MassivePulse) {
					energy -= pulseCost;
					superPulseCount = 0;
				} else {
					//Only allow 3 superpulses
					superPulseCount++;
					if(superPulseCount > superPulseTotal) {
						Game.PowerupActive = Game.Powerups.None;
						energy -= pulseCost;
						superPulseCount = 0;
					}
				}
				
            }
			
			//Used for animating the ring when the player sends a pulse
			if(sentPulse) sentPulse = animRing(true, true, .1f, .1f, .3f, .3f, 1f);
			else if(sentPulseTwo) sentPulseTwo = animRing (false, false, .1f, .1f, .3f, 1f, .3f);
			
			//If you have the invincibility, singleColor or chainPulse powerups, increment its personal timer
			if(Game.PowerupActive == Game.Powerups.Invincible ||
			   Game.PowerupActive == Game.Powerups.ChainReaction ||
			   Game.PowerupActive == Game.Powerups.ChangeColor) {
				pwTimer += Time.deltaTime;
				if(pwTimer > pwTotalTime) {
					Debug.Log ("Powerup: "+Game.PowerupActive+" deactivated");
					Game.PowerupActive = Game.Powerups.None;
					pwTimer = 0;
				}
			} else pwTimer = 0;

            // Regenerate energy. 1 energy every 2 seconds.
            energyTimer += Time.deltaTime;
            if (energyTimer > energyRegenRate && energy < maxEnergy) {
                energy++;
                energyTimer = 0;
            }
			
			//TODO: Show what happens when you don't set glowAnimIn to true
			if(!sentPulse && !sentPulseTwo) {
				if(glowAnimOut) glowAnimOut = animRing(true, false, .1f, .1f, .3f, .3f, 1f);
				else if(glowAnimIn) glowAnimIn = animRing(false, false, .1f, .1f, .3f, 1f, .3f);
			}
        }

        // If the player health is lower than 0, load the "Lose" level
        if (Player.health <= 0) {
            Player.health = 0;
            if (Game.GameMode.Equals(Game.Mode.DeathMatch)) {
                Application.LoadLevel("Win");
            } else {
                Application.LoadLevel("Lose");
            }
        }

    }
	
	private void lightRing(int ring, float timer, float totalTimer, float startBrightness, float finalBrightness) {	
		if(finalBrightness > 1 || finalBrightness < 0) finalBrightness = 1;
		if(startBrightness > 1 || startBrightness < 0) startBrightness = 0;
		float brightness = ((timer/totalTimer)*(finalBrightness-startBrightness)) + startBrightness;
		if(brightness > finalBrightness) playerColor[ring].a = finalBrightness;	
		else playerColor[ring].a = brightness;
		meshRenders[ring].material.SetColor("_Color", playerColor[ring]);
	}
	
	private void darkenRing(int ring, float timer, float totalTimer, float startDarkness, float finalDarkness) {
		if(finalDarkness > 1 || finalDarkness < 0) finalDarkness = 0;
		if(startDarkness > 1 || startDarkness < 0) startDarkness = 1;
		float brightness = startDarkness - ((startDarkness-finalDarkness) * (timer/totalTimer));
		if(brightness < finalDarkness) playerColor[ring].a = finalDarkness;	
		else playerColor[ring].a = brightness;
		meshRenders[ring].material.SetColor("_Color", playerColor[ring]);
	}
	
	public bool animRing(bool firstHalf, bool pulse, float middleStartTime, float outerStartTime, float innerDuration, float middleDuration, float outerDuration, float startBrightness, float endBrightness) {
		//Total time for animation sequence. Either sum of start times with final ring duration, or largest duration
		float totalRingTime = middleStartTime+outerStartTime+outerDuration;
		totalRingTime = Mathf.Max (Mathf.Max(innerDuration, middleDuration), totalRingTime);
			
		//Inner ring - starts initially
		if(firstHalf) lightRing(0, glowTimers[0], innerDuration, startBrightness, endBrightness);
		else darkenRing(0,glowTimers[0], innerDuration, startBrightness, endBrightness);
		incrementGlowTimers(0);
			
		//Middle ring - only starts after the inner timer has passed the middle start time
		if(glowTimers[0] > middleStartTime) {
			if(firstHalf) lightRing (1, glowTimers[1], middleDuration, startBrightness, endBrightness);
			else darkenRing (1, glowTimers[1], middleDuration, startBrightness, endBrightness);
			incrementGlowTimers(1);
		}
		
		//Outer ring - only starts after the middle timer has passed the outer start time
		if(glowTimers[1] > outerStartTime) {
			if(firstHalf) lightRing (2, glowTimers[2], outerDuration, startBrightness, endBrightness);
			else darkenRing(2, glowTimers[2], outerDuration, startBrightness, endBrightness);
			incrementGlowTimers(2);
		}
		
		//Once we've exceeded the total time to illuminate/dim the lights, reset times
		if(glowTimers[0] > totalRingTime) {
			resetGlowTimers();
			if(!pulse && firstHalf) glowAnimIn = true;
			if(pulse) sentPulseTwo = true;
			return false;
		}
		return true;
	}
	
	public bool animRing(bool firstHalf, bool pulse, float middleStartTime, float outerStartTime, float duration, float startBrightness, float endBrightness) {
		return animRing (firstHalf, pulse, middleStartTime, outerStartTime, duration, duration, duration, startBrightness, endBrightness);
	}
				
	private void incrementGlowTimers(int timer) {
		glowTimers[timer] += Time.deltaTime;
	}
	
	private void resetGlowTimers() {
		for(int i=0; i<glowTimers.Length; i++) glowTimers[i] = 0;
	}
	
	private void resetGlowTimers(int timer) {
		glowTimers[timer] = 0;
	}
	
	public void onPeakTrigger(int channel) {
		if(!glowAnimOut && !glowAnimIn) glowAnimOut = true;
		
	}
	
	public void setLoudFlag(bool flag) {}

    /// <summary>
    /// Reset the player stats to default values
    /// </summary>
    public static void ResetStats() {
        score = startScore;
        health = startHealth = maxHealth;
        energy = startEnergy = maxEnergy;
    }
    #endregion
}