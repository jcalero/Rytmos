using UnityEngine;
using System.Collections;
/// <summary>
/// EnemySpawnScript.cs
/// 
/// Handles the spawning of the enemies.
/// </summary>
public class EnemySpawnScript : MonoBehaviour {

    #region Fields
    public GameObject[] EnemyPrefabs;       // List of enemy types to spawn. Inspector reference. Location: EnemySpawner
    public float FirstSpawn;                // The delay the spawner will initialise itself with (time for first spawn)
    public float SpawnRate;                 // The time between spawns
    
    private int RandomSeed;                 // The enemy type to spawn
    #endregion

    #region Functions

    void Start() {
        InvokeRepeating("SpawnEnemy", FirstSpawn, SpawnRate); // Start repeatedly spawning enemies
    }

    /// <summary>
    /// Spawns an enemy of random type.
    /// </summary>
    public void SpawnEnemy() {
        RandomSeed = Random.Range(0, EnemyPrefabs.Length);
        Vector3 position = EnemyPrefabs[RandomSeed].transform.position; // Initial position. Doesn't matter as position is set in the enemy script
        Instantiate(EnemyPrefabs[RandomSeed], position, EnemyPrefabs[RandomSeed].transform.localRotation);
    }
    /// <summary>
    /// Restarts the Invoke method to allow new spawn rates to be initialised
    /// </summary>
    public void RestartSpawner() {
        StopSpawner();
        Start();
    }

    /// <summary>
    /// Stops the spawner/Invoke method.
    /// </summary>
    public void StopSpawner() {
        CancelInvoke("SpawnEnemy");
    }

    #endregion
}