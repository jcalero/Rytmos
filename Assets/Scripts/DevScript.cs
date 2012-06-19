using UnityEngine;
using System.Collections;

public class DevScript : MonoBehaviour
{

    #region Fields
    private bool devMode = false;
    private bool devMode1 = false;
    private bool devMode3 = false;
    private float oldSpawnRate;

    private EnemySpawnScript enemySpawner;
    #endregion

    #region Functions

    void Awake()
    {
        enemySpawner = (EnemySpawnScript)GameObject.Find("EnemySpawner").GetComponent("EnemySpawnScript");
    }

    void Update()
    {
        // God Mode
        if (Input.GetKeyDown(KeyCode.Alpha1) && DevMode && !devMode1)
        {
            Debug.Log("God Mode Enabled");
            devMode1 = true;
            Player.health = 100000;
            Player.energy = 100000;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && DevMode && devMode1)
        {
            Debug.Log("God Mode Disabled");
            devMode1 = false;
            Player.health = Player.startHealth;
            Player.energy = Player.startEnergy;
        }

        // Spawn Enemy
        if (Input.GetKeyDown(KeyCode.Alpha2) && DevMode)
        {
            Debug.Log("Enemy spawned");
            enemySpawner.SpawnEnemy();
        }

        // Mega Swarm
        if (Input.GetKeyDown(KeyCode.Alpha3) && DevMode && !devMode3)
        {

            oldSpawnRate = enemySpawner.SpawnRate;
            enemySpawner.SpawnRate = 0.2f;
            enemySpawner.RestartSpawner();
            devMode3 = true;
            Debug.Log("Massive Swarm Enabled");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && DevMode && devMode3)
        {
            enemySpawner.SpawnRate = oldSpawnRate;
            enemySpawner.RestartSpawner();
            devMode3 = false;
            Debug.Log("Massive Swarm Disabled");
        }
    }

    public bool DevMode
    {
        get { return devMode; }
        set { devMode = value; }
    }

    #endregion
}
