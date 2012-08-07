using UnityEngine;
using System.Collections;
/// <summary>
/// Level.cs
/// 
/// Level manager. Handles all the level feedback. Main class for potentially different level types that
/// inherit from this class.
/// </summary>
public class Tutorial : Level {

	#region Fields
	public GameObject powerupScript;

	
	private static DeathMatch Instance;                                  // The Instance of this class for self reference

	private float audioTimer;
	private float waitTimer;
	private float resendMessageTimer; 
	private bool resendMessage;
	private float activatedTime;
	
	public static bool firstEnemyMessage;
	public static bool secondEnemyMessage;
	public static bool thirdEnemyMessage;
	public static bool comboMessage;
	public static bool shieldPowerupMessage;
	public static bool useShieldMessage;
	public static bool demoShieldMessage;
	public static bool slideMessage;
	public static bool superPulseMessage;
	public static bool increasedSpawnersMessage;
	public static bool chainPulseMessage;
	
	public static bool firstSpawn;
	public static bool secondSpawn;
	public static bool thirdSpawn;
	public static bool done;
	public static bool selectedPowerup;
	public static bool activatedPowerup;
	public static bool spawnPowerupsNormal;
	
	

	public UILabel SurviveLabel;
	public UILabel invincibility;	
	#endregion

	#region Functions
	protected override void Awake() {
		// Local static reference to this class.
		//Instance = this;
		base.Awake();
	}

	protected override void Start() {
		firstEnemyMessage = false;
		secondEnemyMessage = false;
		thirdEnemyMessage = false;
		comboMessage = false;
		shieldPowerupMessage = false;
		useShieldMessage = false;
		demoShieldMessage = false;
		slideMessage = false;
		superPulseMessage = false;
		increasedSpawnersMessage = false;
		chainPulseMessage = false;
		
		firstSpawn = false;
		secondSpawn = false;
		thirdSpawn = false;
		done = false;
		selectedPowerup = false;
		spawnPowerupsNormal = false;
		activatedTime = 1000000;
		
		Game.GameState = Game.State.Playing;
		base.Start();
		Player.maxEnergy = 100;
		Player.ResetStats();
		EnemyScript.energyReturn = 5;
		Debug.Log("GameMode: " + Game.GameMode);
		audioTimer = 0;
		waitTimer = 0;
		resendMessageTimer = 0;
		
	}

	void Update() {
		if(AudioPlayer.isPlaying)
			audioTimer += Time.deltaTime;
		
		//First enemy spawn message
		if(firstSpawn && !firstEnemyMessage) {
			waitTimer += Time.deltaTime;
			if(waitTimer > .75f) {
				Debug.Log("Your first enemy is here. Cyan... Destroy it by clicking on the cyan area here");
				Game.Pause();
				firstEnemyMessage = true;
				waitTimer = 0;
			}
		//Second enemy spawn message
		} else if(secondSpawn && !secondEnemyMessage) {
			waitTimer += Time.deltaTime;
			if(waitTimer >.85f) {
				Debug.Log("Different enemy this time - yellow. But you can destroy him the same way. Press here");
				Game.Pause();
				secondEnemyMessage = true;
				waitTimer = 0;
			}
		//Third enemy spawn message
		} else if(thirdSpawn && !thirdEnemyMessage) {
			waitTimer += Time.deltaTime;
			if(waitTimer > .75f) {
				Debug.Log("Third enemy - green. You know what to do. Click here. ");
				Game.Pause();
				thirdEnemyMessage = true;
				waitTimer = 0;
			}
		}
		
		//PLayer multiplier has increased
		if(Player.multiplier >= 2 && !comboMessage) {
			waitTimer += Time.deltaTime;
			if(waitTimer >.2f) {
				Debug.Log("You're multiplier has increased. Use it to increase your score");
				comboMessage = true;	
				Game.Pause();
				waitTimer = 0;
			}
		}
		
		//Advanced play message
		if(EnemySpawnScript.spawnCount/2 >= 4 && !slideMessage) {
			Debug.Log("You're doing well. You've got two enemies coming up that are close together. Instead of two pulses, try sliding.");
			slideMessage = true;
			Game.Pause();
		}
		
		//INcreased spawners message
		if(EnemySpawnScript.spawnerCounter >= 3 && !increasedSpawnersMessage) {
			waitTimer += Time.deltaTime;
			if(waitTimer>.2f) {
				Debug.Log("You're doing well. More enemies will be attacking. This will last until you get hit.");
				increasedSpawnersMessage = true;
				Game.Pause();
				waitTimer = 0;
			}
		}
		
		//Spawn powerup
		if(audioTimer > 75f && !shieldPowerupMessage) {
			powerupScript.GetComponent<PowerupScript>().spawnPowerupOnScreen(1, new Vector3(3,3,0));
			Game.Pause();
			Debug.Log("There's a powerup to help you. These will disappear, so click it quickly.");
			shieldPowerupMessage = true;
			resendMessageTimer = 0;
			resendMessage = true;
		}
		
		//Spawn powerup reset
		if(resendMessage && !useShieldMessage) {
			resendMessageTimer += Time.deltaTime;
			if(resendMessageTimer > 5) {
				Debug.Log("PLease click on the powerup before it disappears");
				//Revert here
				resendMessageTimer = 0;
				shieldPowerupMessage = false;
			}
		}
		
		//Activate powerup
		if(selectedPowerup && !useShieldMessage) {
			resendMessage = false;
			resendMessageTimer = 0;
			waitTimer += Time.deltaTime;
			if(waitTimer > .2f) {
				Debug.Log("You've got a shield. You can see it in the center. Use it by clicking.");
				useShieldMessage = true;
				Game.Pause();
				waitTimer = 0;
				resendMessage = true;
			}
		}
		
		//Activate powerup reset
		if(resendMessage && !demoShieldMessage) {
			resendMessageTimer += Time.deltaTime;
			if(resendMessageTimer > 5) {
				//Revert here
				Debug.Log("Please use your shield");
				resendMessageTimer = 0;
				useShieldMessage = false;
			}
		}
		
		//Shield powerup demo
		if(activatedPowerup && !demoShieldMessage) {
			activatedTime = audioTimer;
			waitTimer+= Time.deltaTime;
			resendMessage = false;
			resendMessageTimer = 0;
			if(waitTimer > .2f) {
				Debug.Log("This is a shield. Use it to keep your combo");
				Game.Pause();
				demoShieldMessage = true;
				waitTimer = 0;
			}
		}
		
		//Spawn a superPulse, with the intention to help the player through the difficult part
		if(!superPulseMessage && audioTimer >=activatedTime + 8.5f) {
			Debug.Log("Spawn super Pulse to help deal with large wave");
			Game.Pause();
			powerupScript.GetComponent<PowerupScript>().spawnPowerupOnScreen(0, new Vector3(-3,4,0));
			superPulseMessage = true;
		}
		
		//Spawn a chainPulse, to help the player through a difficult part
		//TODO: Time needs to be adjusted to reflect the song
		if(!chainPulseMessage && audioTimer >= 140) {
			Debug.Log("Spawn chain pulse to help deal with large wave");
			Game.Pause();
			powerupScript.GetComponent<PowerupScript>().spawnPowerupOnScreen(0, new Vector3(-3,-5,0));
			chainPulseMessage = true;
			spawnPowerupsNormal = true;
		}
			
		
	}

	IEnumerator DelayLabel() {
		float delayTime = Time.realtimeSinceStartup + 5f;
		while (Time.realtimeSinceStartup < delayTime) {
			yield return 0;
		}
		SurviveLabel.text = "";
	}
	#endregion

}
