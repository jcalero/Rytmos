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
	
	private float glowTimer = 0;				// Timer for glow
	private readonly float glowOutTotalTime = 3f;	// Total time for the glow animation to last
	private bool glowAnimOut = false;				// Flag for starting glow animation
	private bool glowAnimIn = false;				// Flag for second half of glow animation
	
	private float[] glowTimers = new float[3];
	private bool tempB = false;
	
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
                // Create a pulse
                Instantiate(pulsePrefab, new Vector3(0, 0, 0), pulsePrefab.transform.localRotation);
                // Reduce the player energy if not a superpulse
				if(Game.PowerupActive != Game.Powerups.MassivePulse) energy -= pulseCost;
				else {
					//Only allow 3 superpulses
					superPulseCount++;
					if(superPulseCount > superPulseTotal) {
						Game.PowerupActive = Game.Powerups.None;
						superPulseCount = 0;
					}
				}
				
            }
			
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
			
			
			if(glowAnimOut) {
				float middleRingStartTime = .1f;
				float outerRingStartTime = .1f;
				float innerRingDuration = .3f;
				float middleRingDuration = .3f;
				float outerRingDuration = .3f;
				float totalRingTime = middleRingStartTime+outerRingStartTime+outerRingDuration;
				
				lightRing(0, glowTimers[0], innerRingDuration, .3f, 1f);
				incrementGlowTimers(0);
				
				if(glowTimers[0] > middleRingStartTime) {
					lightRing (1, glowTimers[1], middleRingDuration, .3f, 1f);
					incrementGlowTimers(1);
				}
				
				if(glowTimers[1] > outerRingStartTime) {
					lightRing (2, glowTimers[2], outerRingDuration, .3f, 1f);
					incrementGlowTimers(2);
				}
				
				if(glowTimers[0] > totalRingTime) {
					resetGlowTimers();		
					glowAnimIn = true;
					glowAnimOut = false;
				}
			}

			if(glowAnimIn) {
				float middleRingStartTime = .1f;
				float outerRingStartTime = .1f;
				float innerRingDuration = .3f;
				float middleRingDuration = .3f;
				float outerRingDuration = .3f;
				float totalRingTime = middleRingStartTime+outerRingStartTime+outerRingDuration;
				
				darkenRing(0, glowTimers[0], innerRingDuration, 1f, .3f);
				incrementGlowTimers(0);
			
			
				if(glowTimers[0] > middleRingStartTime) {
					darkenRing (1, glowTimers[1], middleRingDuration, 1f, .3f);
					incrementGlowTimers(1);
				}
			
				if(glowTimers[1] > outerRingStartTime) {
					darkenRing (2, glowTimers[2], outerRingDuration, 1f, .3f);
					incrementGlowTimers(2);
				}
				
				if(glowTimers[0] > totalRingTime) {
					resetGlowTimers();		
					glowAnimIn = false;
					glowAnimOut = false;
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
				
	private void incrementGlowTimers() {
		for(int i=0; i<glowTimers.Length; i++) glowTimers[i] += Time.deltaTime;
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
		if(!glowAnimOut && !glowAnimIn) {
			glowAnimOut = true;
		}
		
		/*
		for(int i = 0; i < players.Length; i++) {
			
			if(players[i] == null) continue;
		
			
			switch(channel) {
			case 1:
				playerColor.a = 0.8f + 0.2f*Random.Range(0,101)/100f;
				players[i].material.SetColor("_Color",playerColor);
				break;
				
			case 2:
				playerColor.r = Random.Range(0,101)/100f;
				players[i].material.SetColor("_Color",playerColor);
				break;
			case 3:				
				playerColor.g = Random.Range(0,101)/100f;
				players[i].material.SetColor("_Color",playerColor);
				break;
			case 4:
				playerColor.b = Random.Range(0,101)/100f;
				players[i].material.SetColor("_Color",playerColor);
				break;
			}
			
		}
		*/
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