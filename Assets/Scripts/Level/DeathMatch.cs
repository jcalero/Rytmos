using UnityEngine;
using System.Collections;
/// <summary>
/// Level.cs
/// 
/// Level manager. Handles all the level feedback. Main class for potentially different level types that
/// inherit from this class.
/// </summary>
public class DeathMatch : Level {

	#region Fields
	private static DeathMatch Instance;                                  // The Instance of this class for self reference

	private float timer;

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
		Game.GameMode = Game.Mode.Arcade;
		Game.GameState = Game.State.Playing;
		base.Start();
		//SurviveLabel.text = "[FFAA22]Survive!";
		Player.maxEnergy = 100;
		Player.ResetStats();
		EnemyScript.energyReturn = 5;
		//StartCoroutine(DelayLabel());
	}

	void Update() {
		// Shows the "Survive!" label for 5 seconds
		
		//If you invincible, display the words (TODO: Temporary fix, make more visual)
		//if(Game.PowerupActive == Game.Powerups.Invincible) {
		//    invincibility.enabled = true;
		//} else {
		//    invincibility.enabled = false;
		//}
		
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
