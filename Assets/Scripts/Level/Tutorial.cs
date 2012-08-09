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
	public static bool hasBeenHitMessage;
	
	public static bool firstSpawn;
	public static bool secondSpawn;
	public static bool thirdSpawn;
	public static bool done;
	public static bool selectedPowerup;
	public static bool activatedPowerup;
	public static bool spawnPowerupsNormal;
	public static bool secondChain;
	public static bool showingMessage;
	public static bool hasBeenHit;
	public static bool sentTutorialPulse;
	
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
		hasBeenHitMessage = false;
		
		firstSpawn = false;
		secondSpawn = false;
		thirdSpawn = false;
		done = false;
		selectedPowerup = false;
		spawnPowerupsNormal = false;
		activatedTime = 1000000;
		activatedPowerup = false;
		secondChain = false;
		sceneNumber = 1;
		showingMessage = false;
		hasBeenHit = false;
		
		sentTutorialPulse = false;

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
			showingMessage = true;
			sceneNumber = scene;
			waitTimer = 0;
			message = true;
			Game.Pause(true);
		}
	}
	
	void showMessage(int scene, ref bool message) {
		sceneNumber = scene;
		showingMessage = true;
		message = true;
		Game.Pause (true);
	}
	
	void Update() {
		if(AudioPlayer.isPlaying)
			audioTimer += Time.deltaTime;
		
		if(Input.GetKeyDown(KeyCode.Alpha2)) 
			Debug.Log ("Time at chain pulse: "+audioTimer);
		
		if(Input.GetKeyDown(KeyCode.Alpha5)) {
			PeakTriggerManager.seekTo(155);
			audioTimer = 155;
		}
		
		
		//First enemy spawn message
		if(firstSpawn && !firstEnemyMessage && !showingMessage) 
			showMessage(1, .75f, ref firstEnemyMessage);
		
		//Second enemy spawn message
		else if(secondSpawn && !secondEnemyMessage && !showingMessage) 
			showMessage (2, .85f, ref secondEnemyMessage);
		
		//Third enemy spawn message
		else if(thirdSpawn && !thirdEnemyMessage && !showingMessage) 
			showMessage (3, .75f, ref thirdEnemyMessage);
		
		//PLayer multiplier has increased
		if(Player.multiplier >= 2 && !comboMessage && !showingMessage) 
			showMessage (4, .2f, ref comboMessage);
		
		//Advanced play message
		if(EnemySpawnScript.spawnCount/2 >= 5 && !slideMessage && !showingMessage)
			showMessage (5, ref slideMessage);
		
		//INcreased spawners message
		if(EnemySpawnScript.spawnerCounter >= 3 && !increasedSpawnersMessage && !showingMessage) 
			showMessage(6, .2f, ref increasedSpawnersMessage);
		
		//Spawn powerup
		if(audioTimer > 78f && !shieldPowerupMessage && !showingMessage) {
			showMessage (7, ref shieldPowerupMessage);
			powerupScript.GetComponent<PowerupScript>().spawnPowerupOnScreen(1, new Vector3(3,3,0));
		}
		
		//Shield powerup demo
		if(activatedPowerup && !demoShieldMessage && !showingMessage) {
			activatedTime = audioTimer;
			showMessage(8, .2f, ref demoShieldMessage);
		}
		
		//Spawn a superPulse, with the intention to help the player through the difficult part
		if(!superPulseMessage && audioTimer >=activatedTime + 8.5f && !showingMessage) {
			showMessage(9, ref superPulseMessage);
			powerupScript.GetComponent<PowerupScript>().spawnPowerupOnScreen(0, new Vector3(-4,3,0));

		}
		
		//Spawn a chainPulse, to help the player through a difficult part
		if(!chainPulseMessage && audioTimer >=138 && !showingMessage) {
			showMessage(10, ref chainPulseMessage);
			powerupScript.GetComponent<PowerupScript>().spawnPowerupOnScreen(2, new Vector3(2.4f,1f,0));
		}
		
		if(hasBeenHit && !hasBeenHitMessage) {
			Debug.Log("Is this trigered");
			showMessage(11, ref hasBeenHitMessage);	
		}
		
		
		if(!secondChain && audioTimer >=159 && !showingMessage) {
			powerupScript.GetComponent<PowerupScript>().spawnPowerupOnScreen(2, new Vector3(3,-2,0));
			secondChain = true;
			spawnPowerupsNormal = true;
		}		
	}
	
	public static void SkipTo(float seconds) {
		PeakTriggerManager.seekTo(seconds);
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
