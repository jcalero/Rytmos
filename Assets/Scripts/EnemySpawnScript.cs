using UnityEngine;
using System.Collections;

public class EnemySpawnScript : MonoBehaviour
{

    #region Fields

    public GameObject[] EnemyPrefabs;
    public float FirstSpawn;
    public float SpawnRate;
    public int randomSeed;

    #endregion

    #region Functions

    // Use this for initialization
    void Start()
    {
        InvokeRepeating("SpawnEnemy", FirstSpawn, SpawnRate);
    }

    void SpawnEnemy()
    {
        randomSeed = Random.Range(0, EnemyPrefabs.Length);
        Vector3 position = new Vector3(EnemyPrefabs[randomSeed].transform.position.x,
                                       EnemyPrefabs[randomSeed].transform.position.y,
                                       EnemyPrefabs[randomSeed].transform.position.z);
        Instantiate(EnemyPrefabs[randomSeed], position, EnemyPrefabs[randomSeed].transform.localRotation);
    }

    #endregion
}