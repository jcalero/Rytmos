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
	public static bool hasPowerup = false;		// Current status of the player's powerup
	public static bool takenPowerup = false;	// If the player takes the powerup from the screne

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
	private int superPulseCount = 4;			// Counter for the amount of invincible pulses
		
	
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
			if (Input.GetMouseButtonDown(0)) 
            	clickOnScreen();
			

			
			//If you have the invincibility, singleColor or chainPulse powerups, increment its personal timer
			if(Game.PowerupActive == Game.Powerups.Invincible ||
			   Game.PowerupActive == Game.Powerups.ChainReaction ||
			   Game.PowerupActive == Game.Powerups.ChangeColor) {
				pwTimer += Time.deltaTime;
				if(pwTimer > pwTotalTime) {
					Debug.Log ("Powerup: "+Game.PowerupActive+" deactivated");
					Game.PowerupActive = Game.Powerups.None;
					hasPowerup = false;	
					pwTimer = 0;
				}
			} else pwTimer = 0;

            // Regenerate energy. 1 energy every 2 seconds.
            energyTimer += Time.deltaTime;
            if (energyTimer > energyRegenRate && energy < maxEnergy) {
                energy++;
                energyTimer = 0;
            }
			
			if(Game.PowerupActive == Game.Powerups.ChainReaction) {
				meshRenders[0].material.SetColor("_Color", Color.white);
				meshRenders[1].material.SetColor("_Color", new Color(1,1,1,0f));
				meshRenders[2].material.SetColor ("_Color", Color.white);
			} else if (Game.PowerupActive == Game.Powerups.MassivePulse && superPulseCount > 1) {
				meshRenders[0].material.SetColor("_Color", Color.white);
				meshRenders[1].material.SetColor("_Color", new Color(1,1,1,0.3f));
				if(superPulseCount > 2) {
					meshRenders[1].material.SetColor("_Color", Color.white);
					meshRenders[2].material.SetColor("_Color", new Color(1,1,1,0.3f));
				}
				if(superPulseCount > 3) meshRenders[2].material.SetColor("_Color", Color.white);
			} 
			
			else if(Game.PowerupActive == Game.Powerups.ChangeColor) {
				Color selected = Level.singleColourSelect(EnemySpawnScript.currentlySelectedEnemy);
				if(sentPulse) sentPulse = animRing(true, true, .1f, .1f, .3f, .3f, .3f, .3f, 1f, selected);
				else if(sentPulseTwo) sentPulseTwo = animRing (false, false, .1f, .1f, .3f, .3f, .3f, 1f, .3f, selected);
				
				//TODO: Show what happens when you don't set glowAnimIn to true
				if(!sentPulse && !sentPulseTwo) {
					if(glowAnimOut) glowAnimOut = animRing(true, false, .1f, .1f, .3f, .3f, .3f, .3f, 1f, selected);
					else if(glowAnimIn) glowAnimIn = animRing(false, false, .1f, .1f, .3f, .3f, .3f, 1f, .3f, selected);
				}
			}	//Used for animating the ring when the player sends a pulse		
			else {
				if(sentPulse) sentPulse = animRing(true, true, .1f, .1f, .3f, .3f, 1f);
				else if(sentPulseTwo) sentPulseTwo = animRing (false, false, .1f, .1f, .3f, 1f, .3f);
				
				//TODO: Show what happens when you don't set glowAnimIn to true
				if(!sentPulse && !sentPulseTwo) {
					if(glowAnimOut) glowAnimOut = animRing(true, false, .1f, .1f, .3f, .3f, 1f);
					else if(glowAnimIn) glowAnimIn = animRing(false, false, .1f, .1f, .3f, 1f, .3f);
				}
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
	
	public void clickOnScreen() {		
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
   		RaycastHit hit = new RaycastHit();
		if(Physics.Raycast(new Vector3(ray.origin.x, ray.origin.y, -10), new Vector3(0,0,1), out hit, 10.0f)) {
				if(hit.collider.name == "PlayerTouchFeedback") {
					if(hasPowerup) {
						int choice = Random.Range(0,4);
						switch(choice) {
						case 0:
							Debug.Log ("Massive Pulse Activated!");
							Game.PowerupActive = Game.Powerups.MassivePulse;
							hasPowerup = false;
							break;
						case 1:
							Debug.Log ("Invicibility Activated!");
							pwTimer = 0;
							Game.PowerupActive = Game.Powerups.Invincible;
							hasPowerup = false;
							break;
						case 2:
							Debug.Log ("Single Colour Enemies Activated!");
							pwTimer = 0;
							Game.PowerupActive = Game.Powerups.ChangeColor;
							hasPowerup = false;
							break;
						case 3:
							Debug.Log ("Chain Reaction Activated!");
							pwTimer = 0;
							Game.PowerupActive = Game.Powerups.ChainReaction;
							hasPowerup = false;
							break;
						default:
							Debug.Log ("Powerup Failed...");
							Game.PowerupActive = Game.Powerups.None;
							hasPowerup = false;
							break;
						}
					}
				} else if(hit.collider.name == "Powerup(Clone)") {
					hasPowerup = true;
					takenPowerup = true;
				} else if(energy - pulseCost >= 0) {
					// Show the touch sprite at the mouse location.
	               	Level.ShowTouchSprite(new Vector3(ray.origin.x, ray.origin.y, 0));
	                // Create a pulse and trigger animation
	                Instantiate(pulsePrefab, Vector3.zero, pulsePrefab.transform.localRotation);
					sentPulse = true;
					resetGlowTimers();
	                // Reduce the player energy if not a superpulse
					if(Game.PowerupActive != Game.Powerups.MassivePulse) {
						energy -= pulseCost;
						superPulseCount = 4;
					} else {
						//Only allow 3 superpulses
						superPulseCount--;
						if(superPulseCount == 0) {
							Game.PowerupActive = Game.Powerups.None;
							hasPowerup = false;
							energy -= pulseCost;
							superPulseCount = 4;
						}
				}
			}
		}
	}
	
	/// <summary>
	/// Controls the lighting up of the ring. Only allows brightness values between 0 and 1
	/// <param name="ring">The index of which ring you want to light up (0-2)</param>
	/// <param name="timer">Where the ring is currently along its lightup phase</param>
	/// <param name="totalTimer">The total time that the ring requires to achieve end Brightness</param>
	/// <param name="startBrightness">The start brightness level of the ring</param>
	/// <param name="endBrightness">The final brightness level of the ring</param>
	/// </summary>
	private void lightRing(int ring, float timer, float totalTimer, float startBrightness, float finalBrightness) {	
		if(finalBrightness > 1 || finalBrightness < 0) finalBrightness = 1;
		if(startBrightness > 1 || startBrightness < 0) startBrightness = 0;
		float brightness = ((timer/totalTimer)*(finalBrightness-startBrightness)) + startBrightness;
		if(brightness > finalBrightness) playerColor[ring].a = finalBrightness;	
		else playerColor[ring].a = brightness;
		meshRenders[ring].material.SetColor("_Color", playerColor[ring]);
	}
	
	/// <summary>
	/// Controls the darkening of the ring. Only allows brightness values between 0 and 1
	/// <param name="ring">The index of which ring you want to darken (0-2)</param>
	/// <param name="timer">Where the ring is currently along its darkening phase</param>
	/// <param name="totalTimer">The total time that the ring requires to achieve end Darkness</param>
	/// <param name="startDarkness">The start brightness level of the ring</param>
	/// <param name="endDarkness">The final brightness level of the ring</param>
	/// </summary>
	private void darkenRing(int ring, float timer, float totalTimer, float startDarkness, float finalDarkness) {
		if(finalDarkness > 1 || finalDarkness < 0) finalDarkness = 0;
		if(startDarkness > 1 || startDarkness < 0) startDarkness = 1;
		float brightness = startDarkness - ((startDarkness-finalDarkness) * (timer/totalTimer));
		if(brightness < finalDarkness) playerColor[ring].a = finalDarkness;	
		else playerColor[ring].a = brightness;
		meshRenders[ring].material.SetColor("_Color", playerColor[ring]);
	}
	
	private void lightRing(int ring, float timer, float totalTimer, float startBrightness, float finalBrightness, Color c) {	
		if(finalBrightness > 1 || finalBrightness < 0) finalBrightness = 1;
		if(startBrightness > 1 || startBrightness < 0) startBrightness = 0;
		float brightness = ((timer/totalTimer)*(finalBrightness-startBrightness)) + startBrightness;
		if(brightness > finalBrightness) c.a = finalBrightness;	
		else c.a = brightness;
		meshRenders[ring].material.SetColor("_Color", c);
	}
	
	private void darkenRing(int ring, float timer, float totalTimer, float startDarkness, float finalDarkness, Color c) {
		if(finalDarkness > 1 || finalDarkness < 0) finalDarkness = 0;
		if(startDarkness > 1 || startDarkness < 0) startDarkness = 1;
		float brightness = startDarkness - ((startDarkness-finalDarkness) * (timer/totalTimer));
		if(brightness < finalDarkness) c.a = finalDarkness;	
		else c.a = brightness;
		meshRenders[ring].material.SetColor("_Color", c);
	}
	
	/// <summary>
	/// Controls the darkening of the ring. Only allows brightness values between 0 and 1
	/// <param name="firstHalf">A boolean that indicates if you are lighting up the ring</param>
	/// <param name="pulse">A boolean that indicates if this lightup is a result of a pulse being sent</param>
	/// <param name="middleStartTime">The time after the innerGlowTimer has started that you want the middle ring to start lighting up/darkening</param>
	/// <param name="outerStartTime">The time after the middleGlowTimer has started that you want the outer ring to start lighting up/darkening</param>
	/// <param name="innerDuration">How long you want the inner ring to take to achieve full brightness</param>
	/// <param name="middleDuration">The length of time you want the middle ring to achieve full brightness</param>
	/// <param name="outerDuration">The length of time you want the outer ring to achieve full brightness</param>
	/// <param name="startBrightness">The start brightness level of the ring</param>
	/// <param name="endBrightness">The final brightness level of the ring</param>
	/// <returns>A boolean that returns true if you still need to animate, or false if done</returns>
	/// </summary>
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
	
	public bool animRing(bool firstHalf, bool pulse, float middleStartTime, float outerStartTime, float innerDuration, float middleDuration, float outerDuration, float startBrightness, float endBrightness, Color c) {
		//Total time for animation sequence. Either sum of start times with final ring duration, or largest duration
		float totalRingTime = middleStartTime+outerStartTime+outerDuration;
		totalRingTime = Mathf.Max (Mathf.Max(innerDuration, middleDuration), totalRingTime);
			
		//Inner ring - starts initially
		if(firstHalf) lightRing(0, glowTimers[0], innerDuration, startBrightness, endBrightness, c);
		else darkenRing(0,glowTimers[0], innerDuration, startBrightness, endBrightness, c);
		incrementGlowTimers(0);
			
		//Middle ring - only starts after the inner timer has passed the middle start time
		if(glowTimers[0] > middleStartTime) {
			if(firstHalf) lightRing (1, glowTimers[1], middleDuration, startBrightness, endBrightness, c);
			else darkenRing (1, glowTimers[1], middleDuration, startBrightness, endBrightness, c);
			incrementGlowTimers(1);
		}
		
		//Outer ring - only starts after the middle timer has passed the outer start time
		if(glowTimers[1] > outerStartTime) {
			if(firstHalf) lightRing (2, glowTimers[2], outerDuration, startBrightness, endBrightness,c );
			else darkenRing(2, glowTimers[2], outerDuration, startBrightness, endBrightness,c);
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
	
	public void setLoudFlag(int flag) {}

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