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

    private float myTimer = 0;                  // Timer for energy regeneration
	private float pwTimer = 0;				// Timer for current invincibility duration
	private readonly float pwTotalTime = 10f;// Total time for invincibility to last
    private int pulseCost = 10;                 // Cost of a pulse
    private float energyRegenRate = 0.5f;       // The rate at which the energy regenerates. Lower is faster.
	private int superPulseCount = 0;				// Counter for the amount of invincible pulses
	private readonly int superPulseTotal = 3;		// Total amount of invincible pulses a player can send
	
	private MeshRenderer[] players;
	private Color playerColor;
    #endregion

    #region Functions
    void Start() {
        // Resets player stats at the start of a level
		players = gameObject.GetComponentsInChildren<MeshRenderer>();
		playerColor = new Color(1,1,1,1);
		
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
			}

            // Regenerate energy. 1 energy every 2 seconds.
            myTimer += Time.deltaTime;
            if (myTimer > energyRegenRate && energy < maxEnergy) {
                energy++;
                myTimer = 0;
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
	
	public void onPeakTrigger(int channel) {
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