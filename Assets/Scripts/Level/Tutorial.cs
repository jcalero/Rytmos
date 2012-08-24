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
	public Camera cameraSize;

	
	private static DeathMatch Instance;                                  // The Instance of this class for self reference

	public static float audioTimer;
	private float waitTimer;
	private float activatedTime;
	
	public static bool firstEnemyMessage;
	public static bool secondEnemyMessage;
	public static bool thirdEnemyMessage;
	public static bool shieldPowerupMessage;
	public static bool comboMessage;
	public static bool slideMessage;
	public static bool superPulseMessage;
	public static bool chainPulseMessage;
	public static bool hasBeenHitMessage;
	public static bool energyWarningMessage;
	
	public static bool firstSpawn;
	public static bool secondSpawn;
	public static bool thirdSpawn;
	public static bool done;
	public static bool spawnPowerupsNormal;
	public static bool secondChain;
	public static bool showingMessage;
	public static bool hasBeenHit;
	public static bool sentTutorialPulse;	
	public static int sceneNumber;
	#endregion

	#region Functions
	protected override void Awake() {
		base.Awake();
	}

	protected override void Start() {
		cameraSize.orthographicSize = Game.GetCameraScaleFactor();
		firstEnemyMessage = false;
		secondEnemyMessage = false;
		thirdEnemyMessage = false;
		comboMessage = false;
		shieldPowerupMessage = false;
		slideMessage = false;
		superPulseMessage = false;
		chainPulseMessage = false;
		hasBeenHitMessage = false;
		energyWarningMessage = false;
		
		firstSpawn = false;
		secondSpawn = false;
		thirdSpawn = false;
		done = false;
		spawnPowerupsNormal = false;
		activatedTime = 1000000;
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
			Game.CommonPauseOperation();
			Game.disablePause = true;
			TutorialMenu.Show();
			
		}
	}
	
	void showMessage(int scene, ref bool message) {
		sceneNumber = scene;
		showingMessage = true;
		message = true;
		Game.CommonPauseOperation();
		Game.disablePause = true;
		TutorialMenu.Show();
	}
	
	void Update() {
		if(AudioPlayer.isPlaying)
			audioTimer += Time.deltaTime;		
		
		//First enemy spawn message
		if(firstSpawn && Level.EnemiesDespawned < 3 && !firstEnemyMessage && !showingMessage) 
			showMessage(1, .75f, ref firstEnemyMessage);
		
		//Second enemy spawn message
		else if(secondSpawn && Level.EnemiesDespawned < 6 && !secondEnemyMessage && !showingMessage) 
			showMessage (2, 1f, ref secondEnemyMessage);
		
		//Third enemy spawn message
		else if(thirdSpawn && Level.EnemiesDespawned < 9 && !thirdEnemyMessage && !showingMessage) 
			showMessage (3, .75f, ref thirdEnemyMessage);
		
		//PLayer multiplier has increased
		if(Player.multiplier >= 7 && !comboMessage && !showingMessage) 
			showMessage (4, .2f, ref comboMessage);
		
		//Advanced play message
		if(audioTimer > 32.8f && !slideMessage && !showingMessage) 
			showMessage (5, ref slideMessage);
		
		
		//Spawn powerup
		if(audioTimer > 78f && !shieldPowerupMessage && !showingMessage) {
			activatedTime = audioTimer;
			powerupScript.GetComponent<PowerupScript>().spawnPowerupOnScreen(1, new Vector3(3,3,0));
			showMessage (6, ref shieldPowerupMessage);
			
		}
		
		//Spawn a superPulse, with the intention to help the player through the difficult part
		if(!superPulseMessage && audioTimer >=activatedTime + 9f && !showingMessage) {
			powerupScript.GetComponent<PowerupScript>().spawnPowerupOnScreen(0, new Vector3(-4,3,0));
			showMessage(7, ref superPulseMessage);
		}
		
		//Spawn a chainPulse, to help the player through a difficult part
		if(!chainPulseMessage && audioTimer >=138 && !showingMessage) {
			powerupScript.GetComponent<PowerupScript>().spawnPowerupOnScreen(2, new Vector3(2.4f,1f,0));
			showMessage(8, ref chainPulseMessage);
		}
		
		//Energy Level Popup
		if(Player.Energy < 0.25f*Player.maxEnergy && !energyWarningMessage && !showingMessage) {
			showMessage(10, ref energyWarningMessage);
		}
		
		//Player has been hit popup
		if(hasBeenHit && !hasBeenHitMessage) {
			showMessage(9, ref hasBeenHitMessage);	
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
	#endregion

}
