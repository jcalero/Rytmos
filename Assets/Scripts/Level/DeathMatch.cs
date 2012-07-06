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
    private float increaseInterval = 20f;
    private float increaseRate = 0.1f;
    private float minRate = 0.2f;

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
        Game.GameMode = Game.Mode.DeathMatch;
        Game.GameState = Game.State.Playing;
        base.Start();
        SurviveLabel.text = "[FFAA22]Survive!";
        Player.maxEnergy = 100;
        Player.ResetStats();
        EnemyScript.energyReturn = 5;
    }

    void Update() {
        // Shows the "Survive!" label for 5 seconds
        StartCoroutine(DelayLabel());

        // Increases the spawnrate the more time has passed
        if (timer > increaseInterval) {
            if (enemySpawner.SpawnRate > minRate + increaseRate)
                enemySpawner.SpawnRate -= increaseRate;
            else
                enemySpawner.SpawnRate = minRate;
            enemySpawner.RestartSpawner();
            timer = 0;
        } else {
            timer += Time.deltaTime;
        }
		
		//If you invincible, display the words (TODO: Temporary fix, make more visual)
		if(Game.PowerupActive == Game.Powerups.Invincible) {
			invincibility.enabled = true;
		} else {
			invincibility.enabled = false;
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
