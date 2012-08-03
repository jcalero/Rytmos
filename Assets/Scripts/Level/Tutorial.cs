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
	private static DeathMatch Instance;                                  // The Instance of this class for self reference

	private float audioTimer;
	private float waitTimer;
	public static bool showFirstMessage;
	public static bool showSecondMessage;
	public static bool showThirdMessage;
	

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
		EnemySpawnScript.firstSpawn = false;
		EnemySpawnScript.secondSpawn = false;
		EnemySpawnScript.thirdSpawn = false;
//		Game.GameMode = Game.Mode.Arcade;
		Game.GameState = Game.State.Playing;
		base.Start();
		//SurviveLabel.text = "[FFAA22]Survive!";
		Player.maxEnergy = 100;
		Player.ResetStats();
		EnemyScript.energyReturn = 5;
		//StartCoroutine(DelayLabel());
		Debug.Log("GameMode: " + Game.GameMode);
		audioTimer = 0;
		
	}

	void Update() {
		if(AudioPlayer.isPlaying)
			audioTimer += Time.deltaTime;
		
		if(EnemySpawnScript.firstSpawn && !showFirstMessage) {
			waitTimer += Time.deltaTime;
			if(waitTimer > .75f) {
				Game.Pause(true);
				showFirstMessage = true;
				waitTimer = 0;
			}
		} else if(EnemySpawnScript.secondSpawn && !showSecondMessage) {
			waitTimer += Time.deltaTime;
			if(waitTimer >.55f) {
				Game.Pause (true);
				showSecondMessage = true;
				waitTimer = 0;
			}
		} else if(EnemySpawnScript.thirdSpawn && !showThirdMessage) {
			waitTimer += Time.deltaTime;
			if(waitTimer > .55f) {
				Game.Pause (true);
				showThirdMessage = true;
				waitTimer = 0;
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
