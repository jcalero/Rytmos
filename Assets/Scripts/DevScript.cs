using UnityEngine;
using System.Collections;

public class DevScript : MonoBehaviour
{

    #region Fields
    // TODO: Refactor this to something cleaner. Maybe an enum + case switch.
    private bool devMode1 = false;
    private bool devMode3 = false;
    private bool devMode4 = false;
    private float lastSpawnRate;
    private float originalSpawnRate;

    private EnemySpawnScript enemySpawner;
    #endregion

    #region Functions
    void Awake() {
        if (Application.loadedLevelName == "Game") enemySpawner = (EnemySpawnScript)GameObject.Find("EnemySpawner").GetComponent("EnemySpawnScript");
    }

    void Update() {
        if (Game.DevMode && Application.loadedLevelName == "Game") {
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
                Player.energy = 100000;
            } else if (Input.GetKeyDown(KeyCode.Alpha1) && devMode1) {
                Debug.Log("God Mode Disabled");
                devMode1 = false;
                Player.health = Player.startHealth;
                Player.energy = Player.startEnergy;
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
        }
    }

    #endregion
}
