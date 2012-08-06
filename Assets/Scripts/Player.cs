using UnityEngine;

using System.Collections;
/// <summary>
/// Player.cs
/// 
/// Player manager. Handles player values and control.
/// </summary>
public class Player : MonoBehaviour, PeakListener {
	#region Fields
	public static int startScore = 0;           // Start score
	public static int startEnergy = 50;         // Starting energy of the player
	public static int maxEnergy = 50;           // Maximum energy of the player
	public static bool GodMode = false;         // God mode

	private static int energy;                   // Current energy of the player
	public static int score;                    // Current score of the player
	public static int multiplier = 1;			// Current multiplier of the player
	private readonly static int multiplierKillDivisor = 6;	// Amount of enemies needed to kill before multiplier increases.
	private readonly static int maxMultiplier = 20;			// Maximum multiplier reachable
	public static int KillStreakCounter = 0;	// Counts how many enemies the pulse has destroyed (reset when the player is hit)
	public static int TotalKills = 0;			// Kill count
	public static bool hasPowerup = false;		// Current status of the player's powerup
	public static bool takenPowerup = false;	// If the player takes the powerup from the screne
	public static bool shieldFlash = false;

	public GameObject pulsePrefab;              // The pulse. Inspector reference. Location: Player
	public GameObject superPulsePrefab;			// The super pulse powerup. Inspector reference. Location: Player
	public GameObject powerupPrefab; 			// The powerup. Inspector refernce. Location: PLayer
	public GameObject powerupDisplay;			// The sprite to display in the center. Inspector reference. Location: Player
	public UISprite energyBar;					// Energy bar, location: Player

	private float energyTimer = 0;              // Timer for energy regeneration
	private float pwTimer = 0;					// Timer for current invincibility duration
	private readonly float pwTotalTime = 10f;	// Total time for invincibility to last
	private int energyBarSize;

	private bool glowAnimOut = false;			// Flag for starting glow animation
	private bool glowAnimIn = false;			// Flag for second half of glow animation
	private float[] glowTimers = new float[3];	// Timers for each ring
	private bool sentPulse = false;				// Flag for if you have sent a pulse
	private bool sentPulseTwo = false;			// Second half of the pulse animation

	private int pulseCost = 20;                 // Cost of a pulse
	private float energyRegenRate = 0.1f;       // The rate at which the energy regenerates. Lower is faster.
	private readonly int totalSuperpulses = 1;
	private int superPulseCount;	// Counter for the amount of invincible pulses
	private Game.Powerups playerpowerup;

	public GameObject[] players = new GameObject[3];
	private MeshRenderer[] meshRenders = new MeshRenderer[3];
	
	private static AudioSource[] audioSources;

	private static Player instance;
	#endregion

	#region Functions
	void Awake() {
		instance = this;
		energyBarSize = (int)energyBar.transform.localScale.x;
	}

	void Start() {
		audioSources = gameObject.GetComponentsInChildren<AudioSource>();
		foreach(AudioSource AS in audioSources) AS.volume = Game.EffectsVolume;
		superPulseCount = totalSuperpulses;
		// Resets player stats at the start of a level
		for (int i = 0; i < players.Length; i++) {
			meshRenders[i] = players[i].GetComponent<MeshRenderer>();
		}
		//TODO: Make this relative to how many rings in the player we have
		PeakTriggerManager.addSelfToListenerList(this);
		ResetStats();
	}

	void Update() {
		if (!Game.Paused) {
			// If the player clicks, and has enough energy, sends out a pulse
			if (Input.GetMouseButtonDown(0))
				clickOnScreen();

			//If you have the invincibility, singleColor or chainPulse powerups, increment its personal timer
			if (Game.PowerupActive == Game.Powerups.Invincible ||
			   Game.PowerupActive == Game.Powerups.ChainReaction) {
				pwTimer += Time.deltaTime;
				if (pwTimer > pwTotalTime) {
					Game.PowerupActive = Game.Powerups.None;
					if (playerpowerup == Game.Powerups.None) {
						hasPowerup = false;
					}
					pwTimer = 0;
				}
			} else pwTimer = 0;

			// Regenerate energy. 1 energy every 2 seconds.
			energyTimer += Time.deltaTime;
			if (energyTimer > energyRegenRate && Energy < maxEnergy) {
				Energy++;
				energyTimer = 0;
			}

			if (Game.PowerupActive == Game.Powerups.ChainReaction) {
				meshRenders[0].material.SetColor("_Color", Color.white);
				meshRenders[1].material.SetColor("_Color", new Color(1, 1, 1, .3f));
				meshRenders[2].material.SetColor("_Color", Color.white);
			} else
				showAnimRing(Color.white);
		}
	}

	private void UpdateEnergyBar() {
		if (instance.energyBar.gameObject != null)
			instance.energyBar.transform.localScale = new Vector2(energyBarSize * ((float)Energy / (float)maxEnergy), instance.energyBar.transform.localScale.y);
	}

	public void clickOnScreen() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(new Vector3(ray.origin.x, ray.origin.y, -10), new Vector3(0, 0, 1), out hit, 10.0f)) {
			if (hit.collider.name == "PlayerTouchFeedback") {
				if (hasPowerup) {
					Game.PowerupActive = playerpowerup;
					powerupDisplay.GetComponent<CenterPowerupDisplay>().hideSprite();
					hasPowerup = false;
					if(Game.GameMode == Game.Mode.Tutorial && !Tutorial.demoShieldMessage) 
						Tutorial.activatedPowerup = true;
					
					if (playerpowerup == Game.Powerups.MassivePulse) {
						Instantiate(superPulsePrefab, Vector3.zero, superPulsePrefab.transform.localRotation);
						superPulseCount = 0;
						playerpowerup = Game.Powerups.None;
					}
					pwTimer = 0;
				}
			} else if (hit.collider.name == "Powerup") {
				if (hasPowerup) powerupDisplay.GetComponent<CenterPowerupDisplay>().hideSprite();
				hasPowerup = true;
				takenPowerup = true;
				playerpowerup = powerupPrefab.GetComponent<PowerupScript>().Powerup();
				if (hasPowerup && playerpowerup != Game.Powerups.None) 
					powerupDisplay.GetComponent<CenterPowerupDisplay>().changeSprite(playerpowerup);
			} else if (Game.DevMode && Game.sendSuper){
				triggerMassivePulse();
			} else if (energy - pulseCost >= 0 ||  Game.GameMode == Game.Mode.Casual) {
				// Show the touch sprite at the mouse location.
				Level.ShowTouchSprite(new Vector3(ray.origin.x, ray.origin.y, 0));
				// Create a pulse and trigger animation
				Instantiate(pulsePrefab, Vector3.zero, pulsePrefab.transform.localRotation);
				sentPulse = true;
				resetGlowTimers();
				// Reduce the player energy if not a superpulse
				if (Game.PowerupActive != Game.Powerups.MassivePulse) {
					Energy -= pulseCost;
				} else {
					//Only allow 1 superpulses
					if (superPulseCount == 0) {
						Game.PowerupActive = Game.Powerups.None;
						Energy -= pulseCost;
					}
				}
			}
		}
	}
	
	public void triggerMassivePulse() {
		Game.PowerupActive = Game.Powerups.MassivePulse;
		Instantiate(superPulsePrefab, Vector3.zero, superPulsePrefab.transform.localRotation);
		superPulseCount = 0;
		playerpowerup = Game.Powerups.None;
		Debug.Log ("Activated super pulse");
	}

	/// <summary>
	/// Controls the lighting up of the ring. Only allows brightness values between 0 and 1
	/// <param name="ring">The index of which ring you want to light up (0-2)</param>
	/// <param name="timer">Where the ring is currently along its lightup phase</param>
	/// <param name="totalTimer">The total time that the ring requires to achieve end Brightness</param>
	/// <param name="startBrightness">The start brightness level of the ring</param>
	/// <param name="endBrightness">The final brightness level of the ring</param>
	/// </summary>
	private void lightRing(int ring, float timer, float totalTimer, float startBrightness, float finalBrightness, Color c) {
		if (finalBrightness > 1 || finalBrightness < 0) finalBrightness = 1;
		if (startBrightness > 1 || startBrightness < 0) startBrightness = 0;
		float brightness = ((timer / totalTimer) * (finalBrightness - startBrightness)) + startBrightness;
		if (brightness > finalBrightness) c.a = finalBrightness;
		else c.a = brightness;
		meshRenders[ring].material.SetColor("_Color", c);
	}

	/// <summary>
	/// Controls the darkening of the ring. Only allows brightness values between 0 and 1
	/// <param name="ring">The index of which ring you want to darken (0-2)</param>
	/// <param name="timer">Where the ring is currently along its darkening phase</param>
	/// <param name="totalTimer">The total time that the ring requires to achieve end Darkness</param>
	/// <param name="startDarkness">The start brightness level of the ring</param>
	/// <param name="endDarkness">The final brightness level of the ring</param>
	/// </summary>
	private void darkenRing(int ring, float timer, float totalTimer, float startDarkness, float finalDarkness, Color c) {
		if (finalDarkness > 1 || finalDarkness < 0) finalDarkness = 0;
		if (startDarkness > 1 || startDarkness < 0) startDarkness = 1;
		float brightness = startDarkness - ((startDarkness - finalDarkness) * (timer / totalTimer));
		if (brightness < finalDarkness) c.a = finalDarkness;
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
	/// <param name="c">The color of the ring to be displayed (usually white)</param>
	/// <returns>A boolean that returns true if you still need to animate, or false if done</returns>
	/// </summary>

	public bool animRing(bool firstHalf, bool pulse, float middleStartTime, float outerStartTime, float innerDuration, float middleDuration, float outerDuration, float startBrightness, float endBrightness, Color c) {
		//Total time for animation sequence. Either sum of start times with final ring duration, or largest duration
		float totalRingTime = middleStartTime + outerStartTime + outerDuration;
		totalRingTime = Mathf.Max(Mathf.Max(innerDuration, middleDuration), totalRingTime);

		//Inner ring - starts initially
		if (firstHalf) lightRing(0, glowTimers[0], innerDuration, startBrightness, endBrightness, c);
		else darkenRing(0, glowTimers[0], innerDuration, startBrightness, endBrightness, c);
		incrementGlowTimers(0);

		//Middle ring - only starts after the inner timer has passed the middle start time
		if (glowTimers[0] > middleStartTime) {
			if (firstHalf) lightRing(1, glowTimers[1], middleDuration, startBrightness, endBrightness, c);
			else darkenRing(1, glowTimers[1], middleDuration, startBrightness, endBrightness, c);
			incrementGlowTimers(1);
		}

		//Outer ring - only starts after the middle timer has passed the outer start time
		if (glowTimers[1] > outerStartTime) {
			if (firstHalf) lightRing(2, glowTimers[2], outerDuration, startBrightness, endBrightness, c);
			else darkenRing(2, glowTimers[2], outerDuration, startBrightness, endBrightness, c);
			incrementGlowTimers(2);
		}

		//Once we've exceeded the total time to illuminate/dim the lights, reset times
		if (glowTimers[0] > totalRingTime) {
			resetGlowTimers();
			if (!pulse && firstHalf) glowAnimIn = true;
			if (pulse) sentPulseTwo = true;
			return false;
		}
		return true;
	}

	public bool animRing(bool firstHalf, bool pulse, float middleStartTime, float outerStartTime, float duration, float startBrightness, float endBrightness, Color c) {
		return animRing(firstHalf, pulse, middleStartTime, outerStartTime, duration, duration, duration, startBrightness, endBrightness, c);
	}

	public void showAnimRing(Color c) {
		if (sentPulse) sentPulse = animRing(true, true, .05f, .05f, .1f, .3f, 1f, c);
		else if (sentPulseTwo) sentPulseTwo = animRing(false, false, .1f, .1f, .3f, 1f, .3f, c);
		if (!sentPulse && !sentPulseTwo) {
			if (glowAnimOut) glowAnimOut = animRing(true, false, .1f, .1f, .3f, .3f, 1f, c);
			else if (glowAnimIn) glowAnimIn = animRing(false, false, .1f, .1f, .3f, 1f, .3f, c);
		}
	}

	private void incrementGlowTimers(int timer) {
		glowTimers[timer] += Time.deltaTime;
	}

	private void resetGlowTimers() {
		for (int i = 0; i < glowTimers.Length; i++) glowTimers[i] = 0;
	}

	private void resetGlowTimers(int timer) {
		glowTimers[timer] = 0;
	}

	public void onPeakTrigger(int channel, int intensity) {
		if (!glowAnimOut && !glowAnimIn) glowAnimOut = true;
	}

	public void setLoudFlag(int flag) { }
	
	public static void playGetHitSound() {
		if(shieldFlash)
			audioSources[0].Play();
		else
			audioSources[1].Play();
	}

	void OnDisable() {
		PeakTriggerManager.removeSelfFromListenerList(this);
	}

	/// <summary>
	/// Reset the player stats to default values
	/// </summary>
	public static void ResetStats() {
		score = startScore;
		ResetMultiplier();
		TotalKills = 0;
		Energy = startEnergy = maxEnergy;
	}

	public static int IncrementScore() {
		return IncrementScore(10);
	}

	public static int IncrementScore(int value) {
		
		UpdateMultiplier();
		
		int scoreIncrement = value * multiplier;
		score += scoreIncrement;
		HUD.UpdateScore();
		return scoreIncrement;
	}

	public static void UpdateMultiplier() {
		multiplier = (KillStreakCounter / multiplierKillDivisor) + 1;
		if(multiplier > maxMultiplier) multiplier = maxMultiplier;
		HUD.UpdateMultiplier();
	}

	public static void ResetMultiplier() {
		KillStreakCounter = 0;
		multiplier = 1;
		HUD.UpdateMultiplier();
	}
	
	public static void ReduceMultiplier() {		
		multiplier -= 5;
		
		if(multiplier < 1) {
			multiplier = 1;
			KillStreakCounter = 0;
		} else {
			KillStreakCounter = (multiplier-1)*multiplierKillDivisor;
		}
		HUD.UpdateMultiplier();
	}

	public static int MaxMultiplier {
		get { return maxMultiplier; }
	}

	public static int Energy {
		get { return energy; }
		set { 
			energy = value;
			instance.UpdateEnergyBar();
		}
	}

	public static int MultiplierKillDivisor {
		get { return multiplierKillDivisor; }
	}
	#endregion
}