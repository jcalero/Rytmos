using UnityEngine;
using System.Collections;
/// <summary>
/// DevScript.cs
/// 
/// Handles the dev/debug/cheat modes.
/// </summary>
public class DevScript : MonoBehaviour {
    #region Fields

    // TODO: Refactor this to something cleaner. Maybe an enum + case switch.
    private bool devMode1 = false;          // Flag: God mode
    private bool devMode3 = false;          // Flag: Massive Spawn
    private bool devMode4 = false;          // Flag: No spawn
    private float lastSpawnRate;            // Last spawn rate before it was last modified
    private float originalSpawnRate;        // Original (default) spawn rate before any modifications

    private EnemySpawnScript enemySpawner;
    private bool wasDevMode;
    #endregion

    #region Functions
    void Awake() {
        if (Game.GameState.Equals(Game.State.Playing)) enemySpawner = (EnemySpawnScript)GameObject.Find("EnemySpawner").GetComponent("EnemySpawnScript");
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
                Player.health = 100000;
                Player.maxHealth = 100000;
                Player.energy = 100000;
                Player.maxEnergy = 100000;
            } else if (Input.GetKeyDown(KeyCode.Alpha1) && devMode1) {
                Debug.Log("God Mode Disabled");
                devMode1 = false;
                Player.health = Player.startHealth;
                Player.maxHealth = Player.startHealth;
                Player.energy = Player.startEnergy;
                Player.maxEnergy = Player.startEnergy;
            }

            // Spawn Enemy
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                Debug.Log("Enemy spawned");
                enemySpawner.SpawnEnemy();
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
        Player.health = Player.startHealth;
        Player.maxHealth = Player.startHealth;
        Player.energy = Player.startEnergy;
        Player.maxEnergy = Player.startEnergy;
        devMode1 = false;
        devMode3 = false;
        devMode4 = false;
    }

    #endregion
}
