using UnityEngine;
using System.Collections;

public class EnemySpawnScript : MonoBehaviour
{

    #region Fields

    public GameObject EnemyPrefab;
    public float FirstSpawn;
    public float SpawnRate;    

    #endregion

    #region Properties

    #endregion

    #region Functions

    // Use this for initialization
    void Start()
    {
        InvokeRepeating("SpawnEnemy", FirstSpawn, SpawnRate);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnEnemy()
    {
        Vector3 position = new Vector3(EnemyPrefab.transform.position.x,
                                       EnemyPrefab.transform.position.y,
                                       EnemyPrefab.transform.position.z);
        Instantiate(EnemyPrefab, position, EnemyPrefab.transform.localRotation);
    }

    #endregion
}