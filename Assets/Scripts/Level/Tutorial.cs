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
	
	public static bool showFirstMessage;
	public static bool showSecondMessage;
	public static bool showThirdMessage;
	public static bool showFourthMessage;
	public static bool showFifthMessage;
	public static bool showSixthMessage;
	public static bool showSeventhMessage;
	
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
		showFirstMessage = false;
		showSecondMessage = false;
		showThirdMessage = false;
		showFourthMessage = false;
		showFifthMessage = false;
		showSixthMessage = false;
		showSeventhMessage = false;
		
		firstSpawn = false;
		secondSpawn = false;
		thirdSpawn = false;
		done = false;
		selectedPowerup = false;
		spawnPowerupsNormal = false;
		
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
		if(firstSpawn && !showFirstMessage) {
			waitTimer += Time.deltaTime;
			if(waitTimer > .75f) {
				Game.Pause(true);
				showFirstMessage = true;
				waitTimer = 0;
			}
		//Second enemy spawn message
		} else if(secondSpawn && !showSecondMessage) {
			waitTimer += Time.deltaTime;
			if(waitTimer >.85f) {
				Game.Pause (true);
				showSecondMessage = true;
				waitTimer = 0;
			}
		//Third enemy spawn message
		} else if(thirdSpawn && !showThirdMessage) {
			waitTimer += Time.deltaTime;
			if(waitTimer > .75f) {
				Game.Pause (true);
				showThirdMessage = true;
				waitTimer = 0;
			}
		}
		
		//PLayer multiplier has increased
		if(Player.multiplier >= 2 && !showFourthMessage) {
			waitTimer += Time.deltaTime;
			if(waitTimer >.2f) {
				showFourthMessage = true;	
				Game.Pause(true);
				waitTimer = 0;
			}
		}
		
		//Spawn powerup
		if(audioTimer > 44.5f && !showFifthMessage) {
			powerupScript.GetComponent<PowerupScript>().spawnPowerupOnScreen(1, new Vector3(3,3,0));
			Game.Pause (true);
			showFifthMessage = true;
			resendMessageTimer = 0;
			resendMessage = true;
		}
		
		//Spawn powerup reset
		if(resendMessage && !showSixthMessage) {
			resendMessageTimer += Time.deltaTime;
			if(resendMessageTimer > 5) {
				//Revert here
				resendMessageTimer = 0;
				showFifthMessage = false;
			}
		}
		
		//Activate powerup
		if(selectedPowerup && !showSixthMessage) {
			resendMessage = false;
			resendMessageTimer = 0;
			waitTimer += Time.deltaTime;
			if(waitTimer > .2f) {
				showSixthMessage = true;
				Game.Pause (true);
				waitTimer = 0;
				resendMessage = true;
			}
		}
		
		//Activate powerup reset
		if(resendMessage && !showSeventhMessage) {
			resendMessageTimer += Time.deltaTime;
			if(resendMessageTimer > 5) {
				//Revert here
				resendMessageTimer = 0;
				showSixthMessage = false;
			}
		}
		
		//Shield powerup demo
		if(activatedPowerup && !showSeventhMessage) {
			waitTimer+= Time.deltaTime;
			resendMessage = false;
			resendMessageTimer = 0;
			if(waitTimer > .2f) {
				Game.Pause(true);
				showSeventhMessage = true;
				waitTimer = 0;
				spawnPowerupsNormal = true;
			}
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
