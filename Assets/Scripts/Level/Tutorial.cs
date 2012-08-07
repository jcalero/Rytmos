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
		
		firstSpawn = false;
		secondSpawn = false;
		thirdSpawn = false;
		done = false;
		selectedPowerup = false;
		spawnPowerupsNormal = false;
		activatedTime = 1000000;
		activatedPowerup = false;
		secondChain = false;
		
		Game.GameState = Game.State.Playing;
		base.Start();
		Player.maxEnergy = 100;
		Player.ResetStats();
		EnemyScript.energyReturn = 5;
		Debug.Log("GameMode: " + Game.GameMode);
		audioTimer = 0;
		waitTimer = 0;
		
	}
	
	void showMessage(int scene, float duration, ref bool message) {
		waitTimer += Time.deltaTime;
		if(waitTimer > duration) {
			Game.Pause(true);
			sceneNumber = scene;
			waitTimer = 0;
			message = true;
		}
	}
	
	void showMessage(int scene, ref bool message) {
		Game.Pause (true);
		sceneNumber = scene;
		message = true;
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
			showMessage(1, .75f, ref firstEnemyMessage);
			if(firstEnemyMessage) Debug.Log ("Your first enemy is here. Cyan... Destroy it by clicking on the cyan area here");	
		//Second enemy spawn message
		} else if(secondSpawn && !secondEnemyMessage) {
			showMessage (2, .85f, ref secondEnemyMessage);
			if(secondEnemyMessage) Debug.Log("Different enemy this time - yellow. But you can destroy him the same way. Press here");
		//Third enemy spawn message
		} else if(thirdSpawn && !thirdEnemyMessage) {
			showMessage (3, .75f, ref thirdEnemyMessage);
			if(thirdEnemyMessage) Debug.Log("Third enemy - green. You know what to do. Click here. ");
		}
		
		//PLayer multiplier has increased
		if(Player.multiplier >= 2 && !comboMessage) {
			showMessage (4, .2f, ref comboMessage);
			if(comboMessage) Debug.Log("You're multiplier has increased. Use it to increase your score");
		}
		
		//Advanced play message
		if(EnemySpawnScript.spawnCount/2 >= 5 && !slideMessage) {
			showMessage (5, ref slideMessage);
			Debug.Log("You're doing well. You've got two enemies coming up that are close together. Instead of two pulses, try sliding.");
		}
		
		//INcreased spawners message
		if(EnemySpawnScript.spawnerCounter >= 3 && !increasedSpawnersMessage) {
			showMessage(6, .2f, ref increasedSpawnersMessage);
			if(increasedSpawnersMessage) Debug.Log("You're doing well. More enemies will be attacking. This will last until you get hit.");
		}
		
		//Spawn powerup
		if(audioTimer > 78f && !shieldPowerupMessage) {
			showMessage (7, ref shieldPowerupMessage);
			powerupScript.GetComponent<PowerupScript>().spawnPowerupOnScreen(1, new Vector3(3,3,0));
			Debug.Log("There's a powerup to help you. These will disappear, so click it quickly.");
		}
		
		//Activate powerup
		if(selectedPowerup && !useShieldMessage) {
			showMessage(8, .2f, ref useShieldMessage);
			if(useShieldMessage) Debug.Log("You've got a shield. You can see it in the center. Use it by clicking.");
		}
		
		//Shield powerup demo
		if(activatedPowerup && !demoShieldMessage) {
			activatedTime = audioTimer;
			showMessage(9, .2f, ref demoShieldMessage);
			if(demoShieldMessage) Debug.Log("This is a shield. Use it to keep your combo");
		}
		
		//Spawn a superPulse, with the intention to help the player through the difficult part
		if(!superPulseMessage && audioTimer >=activatedTime + 8.5f) {
			showMessage(10, ref superPulseMessage);
			Debug.Log("Spawn super Pulse to help deal with large wave");
			powerupScript.GetComponent<PowerupScript>().spawnPowerupOnScreen(0, new Vector3(-3,4,0));

		}
		
		//Spawn a chainPulse, to help the player through a difficult part
		if(!chainPulseMessage && audioTimer >=138) {
			showMessage(11, ref chainPulseMessage);
			Debug.Log("Spawn chain pulse to help deal with large wave");
			powerupScript.GetComponent<PowerupScript>().spawnPowerupOnScreen(2, new Vector3(1,1,0));
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
