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

	public static float audioTimer;
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
	public static bool secondChain;
	
	public static float timeStamp;
	public static int[] spawnPosStore;
	public static int colorStore;
	public static int sceneNumber;
	public static bool numSpawners;
	public static int storeRotation;
	
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
		resendMessage = false;
		
		firstSpawn = false;
		secondSpawn = false;
		thirdSpawn = false;
		done = false;
		selectedPowerup = false;
		spawnPowerupsNormal = false;
		activatedTime = 1000000;
		activatedPowerup = false;
		secondChain = false;
		
		sceneNumber = 0;
		colorStore = 2;
		spawnPosStore = new int[3];
		numSpawners = false;
		storeRotation = 1;
		
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
	
	bool showMessage(int scene, float duration) {
		waitTimer += Time.deltaTime;
		if(waitTimer > duration) {
			Game.Pause();
			sceneNumber = scene;
			waitTimer = 0;
			return true;
		}
		return false;
	}
	
	void showMessage(int scene) {
		Game.Pause ();
		sceneNumber = scene;
		resendMessageTimer = 0;
	}
	
	void Update() {
		if(AudioPlayer.isPlaying)
			audioTimer += Time.deltaTime;
		
		if(Input.GetKeyDown(KeyCode.Alpha2)) 
			Debug.Log ("Time at chain pulse: "+audioTimer);
		
		if(Input.GetKeyDown (KeyCode.Alpha3))
			Debug.Log ("Time at too difficult: "+audioTimer);
		
		if(Input.GetKeyDown(KeyCode.Alpha5)) {
			PeakTriggerManager.seekTo(155);
			AudioPlayer.seekTo(155);
			audioTimer = 155;
		}
		
		//First enemy spawn message
		if(firstSpawn && !firstEnemyMessage) {
			if(showMessage(1, .75f)) {
				Debug.Log ("Your first enemy is here. Cyan... Destroy it by clicking on the cyan area here");
				firstEnemyMessage = true;
			}
			
			
		//Second enemy spawn message
		} else if(secondSpawn && !secondEnemyMessage) {
			if(showMessage (2, .85f)) {
				Debug.Log("Different enemy this time - yellow. But you can destroy him the same way. Press here");
				secondEnemyMessage = true;
			}
		//Third enemy spawn message
		} else if(thirdSpawn && !thirdEnemyMessage) {
			if(showMessage (3, .75f)) {
				Debug.Log("Third enemy - green. You know what to do. Click here. ");
				thirdEnemyMessage = true;
			}
		}
		
		//PLayer multiplier has increased
		if(Player.multiplier >= 2 && !comboMessage) {
			if(showMessage (4, .2f)) {
				Debug.Log("You're multiplier has increased. Use it to increase your score");
				comboMessage = true;	
			}
		}
		
		//Advanced play message
		if(EnemySpawnScript.spawnCount/2 >= 5 && !slideMessage) {
			showMessage (5);
			Debug.Log("You're doing well. You've got two enemies coming up that are close together. Instead of two pulses, try sliding.");
			slideMessage = true;
		}
		
		//INcreased spawners message
		if(EnemySpawnScript.spawnerCounter >= 3 && !increasedSpawnersMessage) {
			if(showMessage(6, .2f)) {
				Debug.Log("You're doing well. More enemies will be attacking. This will last until you get hit.");
				increasedSpawnersMessage = true;
			}
		}
		
		//Spawn powerup
		if(audioTimer > 78f && !shieldPowerupMessage) {
			showMessage (7);
			powerupScript.GetComponent<PowerupScript>().spawnPowerupOnScreen(1, new Vector3(3,3,0));
			Debug.Log("There's a powerup to help you. These will disappear, so click it quickly.");
			shieldPowerupMessage = true;
			resendMessage = true;
		}
		
		//Spawn powerup reset
		if(resendMessage && !useShieldMessage) {
			resendMessageTimer += Time.deltaTime;
			if(resendMessageTimer > 5) {
				Debug.Log("PLease click on the powerup before it disappears");
				//Revert here to scene 7
				resendMessageTimer = 0;
				shieldPowerupMessage = false;
			}
		}
		
		//Activate powerup
		if(selectedPowerup && !useShieldMessage) {
			resendMessage = false;
			resendMessageTimer = 0;
			if(showMessage(8, .2f)) {
				Debug.Log("You've got a shield. You can see it in the center. Use it by clicking.");
				useShieldMessage = true;
				resendMessage = true;
			}
		}
		
		//Activate powerup reset
		if(resendMessage && !demoShieldMessage) {
			resendMessageTimer += Time.deltaTime;
			if(resendMessageTimer > 5) {
				//Revert here to scene 8
				Debug.Log("Please use your shield");
				resendMessageTimer = 0;
				useShieldMessage = false;
			}
		}
		
		//Shield powerup demo
		if(activatedPowerup && !demoShieldMessage) {
			activatedTime = audioTimer;
			resendMessage = false;
			resendMessageTimer = 0;
			if(showMessage(9, .2f)) {
				Debug.Log("This is a shield. Use it to keep your combo");
				demoShieldMessage = true;
			}
		}
		
		//Spawn a superPulse, with the intention to help the player through the difficult part
		if(!superPulseMessage && audioTimer >=activatedTime + 8.5f) {
			showMessage(10);
			Debug.Log("Spawn super Pulse to help deal with large wave");
			powerupScript.GetComponent<PowerupScript>().spawnPowerupOnScreen(0, new Vector3(-3,4,0));
			superPulseMessage = true;
		}
		
		//Spawn a chainPulse, to help the player through a difficult part
		if(!chainPulseMessage && audioTimer >=138) {
			showMessage(11);
			Debug.Log("Spawn chain pulse to help deal with large wave");
			powerupScript.GetComponent<PowerupScript>().spawnPowerupOnScreen(2, new Vector3(1,1,0));
			chainPulseMessage = true;
		}
		
		if(!secondChain && audioTimer >=159) {
			powerupScript.GetComponent<PowerupScript>().spawnPowerupOnScreen(2, new Vector3(1,1,0));
			secondChain = true;
			spawnPowerupsNormal = true;
		}		
	}
	
	public static void SkipTo(float seconds) {
		PeakTriggerManager.seekTo(seconds);
		AudioPlayer.seekTo(seconds);
		audioTimer = seconds;
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
