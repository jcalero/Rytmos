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
    
    private GameObject cam;					// Camera gameobject to play audio
    private int[] counters;					// "Pointers" for the triggers of each channel
    private int pastSample;					// Used to sync framerate with music playback-rate

    #endregion

    #region Functions

    void Start() {
        InvokeRepeating("SpawnEnemy", FirstSpawn, SpawnRate); // Start repeatedly spawning enemies
        //init ();
        //playMusic();
    }
    
    void Update() {
		//       if(AudioManager.triggers != null && cam != null && cam.audio.isPlaying) triggerEnemiesOnMusic();
    }
    
    void init() {
        counters = new int[4];
        pastSample = 0;	
    }
    
    void playMusic() {
        cam = GameObject.Find ("Main Camera");
        cam.audio.clip = AudioManager.freader.getClip ();	// Set the camera's audio clip to the read data	
        if(!cam.audio.isPlaying) cam.audio.Play ();	
    }
    
    void triggerEnemiesOnMusic() {
        
        // Check where in the sound file we are at, and whether this has updated yet (framerate vs. music playback issue)
        int sample = cam.audio.timeSamples;
        if (sample > pastSample) {
            pastSample = sample;
            for (int t = 0; t < 3; t++) {
               // while (counters[t] < AudioManager.triggers[t].Length && AudioManager.triggers[t][counters[t]] < sample) {
                    counters [t]++;
                    SpawnEnemy(t);
               // }
            }
        }	
    }

    /// <summary>
    /// Spawns an enemy of random type.
    /// </summary>
    public void SpawnEnemy ()
    {	if(Level.fourColors) SpawnEnemy (Random.Range (0, 3));	
		else SpawnEnemy (Random.Range (0, EnemyPrefabs.Length));	
    }
    
    /// <summary>
    /// Spawns an enemy of a specified type
    /// </summary>
    /// <param name='prefab'>
    /// int index of the enemy prefab
    /// </param>
    public void SpawnEnemy (int prefab)
    {
        Vector3 position = EnemyPrefabs [prefab].transform.position;
        Instantiate (EnemyPrefabs [prefab], position, EnemyPrefabs [prefab].transform.localRotation);
    }
	
	public void SpawnEnemy (int prefab, float speed, float xpos, float ypos) {
		Vector3 position = EnemyPrefabs [prefab].transform.position;
        GameObject enemy = (GameObject) Instantiate (EnemyPrefabs [prefab], position, EnemyPrefabs [prefab].transform.localRotation);
		enemy.GetComponent<EnemyScript>().SetPositionAndSpeed(speed, xpos, ypos);
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