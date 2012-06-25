using UnityEngine;
using System.Collections;

public class EnemySpawnScript : MonoBehaviour
{

    #region Fields

    public GameObject[] EnemyPrefabs;
    public float FirstSpawn;
    public float SpawnRate;
    public int RandomSeed;

    #endregion

    #region Functions

    // Use this for initialization
    void Start() {
        InvokeRepeating("SpawnEnemy", FirstSpawn, SpawnRate);
    }

    public void SpawnEnemy() {
        RandomSeed = Random.Range(0, EnemyPrefabs.Length);
        Vector3 position = EnemyPrefabs[RandomSeed].transform.position;
        Instantiate(EnemyPrefabs[RandomSeed], position, EnemyPrefabs[RandomSeed].transform.localRotation);
    }

    public void RestartSpawner() {
        StopSpawner();
        Start();
    }

    public void StopSpawner() {
        CancelInvoke("SpawnEnemy");
    }

    #endregion
}