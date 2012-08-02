using UnityEngine;
using System.Collections;
/// <summary>
/// DevScript.cs
/// 
/// Handles the dev/debug/cheat modes.
/// </summary>
public class DevScript : MonoBehaviour {
    #region Fields
	public GameObject powerup;
	public static bool devModeAccess = false;
	
    // TODO: Refactor this to something cleaner. Maybe an enum + case switch.
    private bool devMode1 = false;          // Flag: God mode
	private bool devMode2 = false;			// Flag: Powerups
    private bool devMode3 = false;          // Flag: Massive Spawn
    private bool devMode4 = false;          // Flag: No spawn
    private float lastSpawnRate;            // Last spawn rate before it was last modified
    private float originalSpawnRate;        // Original (default) spawn rate before any modifications

    private EnemySpawnScript enemySpawner;
    private bool wasDevMode;
	private bool spawned;
    #endregion

    #region Functions
    void Awake() {
        if (Game.GameState.Equals(Game.State.Playing)) {
			enemySpawner = (EnemySpawnScript)GameObject.Find("EnemySpawner").GetComponent("EnemySpawnScript");
		}
    }

    void Update() {
        if (Game.DevMode && Game.GameState == Game.State.Playing) {
            wasDevMode = true;
            // Instantiate the enemySpawner script if not already instantiated (e.g. the level
            // this script was awaken in was a menu where there is no enemy spawner)
            if (enemySpawner == null) {
                enemySpawner = (EnemySpawnScript)GameObject.Find("EnemySpawner").GetComponent("EnemySpawnScript");
                originalSpawnRate = enemySpawner.SpawnRate;
            }

            // God Mode
            if (Input.GetKeyDown(KeyCode.Alpha1) && !devMode1) {
                Debug.Log("God Mode Enabled");
                devMode1 = true;
                Player.energy = 100000;
                Player.maxEnergy = 100000;
            } else if (Input.GetKeyDown(KeyCode.Alpha1) && devMode1) {
                Debug.Log("God Mode Disabled");
                devMode1 = false;
                Player.energy = Player.startEnergy;
                Player.maxEnergy = Player.startEnergy;
            }

            // Spawn Enemy
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                Debug.Log("Enemy spawned");
                enemySpawner.SpawnEnemy();
            }
			
			if (Input.GetKeyDown(KeyCode.P) && !devMode2) {
				Debug.Log ("Powerup Mode Enabled");
				devModeAccess = true;
				devMode2 = true;
			} else if(Input.GetKeyDown(KeyCode.P) && devMode2) {
				Debug.Log ("Powerup Mode Disabled");
				devModeAccess = false;
				devMode2 = false;
				Game.PowerupActive = Game.Powerups.None;
			}
			
			if(Input.GetKeyDown (KeyCode.A) && devMode2) {
				if(Game.PowerupActive != Game.Powerups.ChainReaction) {
					Game.PowerupActive = Game.Powerups.ChainReaction;
					Player.hasPowerup = true;
				}
				else {
					Game.PowerupActive = Game.Powerups.None;
					Player.hasPowerup = false;
				}
				Debug.Log ("Chain reaction powerup active");
			} 
			
			if(Input.GetKeyDown(KeyCode.S) && devMode2) {
				if(Game.PowerupActive != Game.Powerups.Invincible) {
					Game.PowerupActive = Game.Powerups.Invincible;
					Player.hasPowerup = true;
				}
				else  {
					Game.PowerupActive = Game.Powerups.None;
					Player.hasPowerup = false;
				}
				Debug.Log ("Invincible powerup active");
			}
			
			if(Input.GetKeyDown(KeyCode.D) && devMode2) {
				if(Game.sendSuper != true)  {
					Game.sendSuper = true;
				}
				else {
					Game.sendSuper = false;
				}
				Debug.Log ("Massive pulse powerup");
			}

            // Mega Swarm
            if (Input.GetKeyDown(KeyCode.Alpha3) && !devMode3) {
                lastSpawnRate = enemySpawner.SpawnRate;
                enemySpawner.SpawnRate = 0.2f;
                enemySpawner.RestartSpawner();
                devMode3 = true;
                Debug.Log("Massive Swarm Enabled");
            } else if (Input.GetKeyDown(KeyCode.Alpha3) && devMode3) {
                enemySpawner.SpawnRate = originalSpawnRate;
                enemySpawner.RestartSpawner();
                devMode3 = false;
                Debug.Log("Massive Swarm Disabled");
            }

            // Stop Spawn
            if (Input.GetKeyDown(KeyCode.Alpha4) && !devMode4) {
                lastSpawnRate = enemySpawner.SpawnRate;
                enemySpawner.StopSpawner();
                devMode4 = true;
                Debug.Log("Auto-Spawn Disabled");
            } else if (Input.GetKeyDown(KeyCode.Alpha4) && devMode4) {
                enemySpawner.SpawnRate = lastSpawnRate;
                enemySpawner.RestartSpawner();
                devMode4 = false;
                Debug.Log("Auto-Spawn Re-enabled with spawnrate: " + enemySpawner.SpawnRate);
            }
        } else if (wasDevMode && Game.GameState == Game.State.Playing) {
            DisableAll();
            wasDevMode = false;
        }
    }

    void DisableAll() {
        enemySpawner.SpawnRate = originalSpawnRate;
        enemySpawner.RestartSpawner();
        Player.energy = Player.startEnergy;
        Player.maxEnergy = Player.startEnergy;
        devMode1 = false;
        devMode3 = false;
        devMode4 = false;
    }

    #endregion
}
