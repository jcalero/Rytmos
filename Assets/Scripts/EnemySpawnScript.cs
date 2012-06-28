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
	private FileReader freader;				// File Reader to get audio file
	private SoundProcessor sProc;			// Sound Processor used to get triggers
	private int[][] triggers;				/* 2D array to hold the triggers: triggers[] is array of size 4 for low,lowMid,highMid,High frequency-bins, SORTED BY NUMBER OF TRIGGERS FROM LOW TO HIGH */
	private int[] counters;					// "Pointers" for the triggers of each channel
	private int pastSample;					// Used to sync framerate with music playback-rate
    #endregion

    #region Functions

    void Start() {
        InvokeRepeating("SpawnEnemy", FirstSpawn, SpawnRate); // Start repeatedly spawning enemies
		//init (@"C:/Users/Samuel/Desktop/test2.wav");
		//playMusic();
    }
	
	void Update() {
		if(triggers != null) triggerEnemiesOnMusic();	
	}
	
	void init(string pathToMusicFile) {
		
		// Read Audio Data
		freader = new FileReader (pathToMusicFile, FileReader.AudioFormat.WAV);
		freader.read ();
		while (freader.isReading())
			yieldRoutine ();
		
		// Set the camera's audio clip to the read data	
		cam = GameObject.Find ("Main Camera");
		cam.audio.clip = freader.getClip ();
		
		// Calculate the triggers for the music
		sProc = new SoundProcessor (freader.getData (), 128);
		triggers = sProc.calculateAllTheTriggers ();
		counters = new int[4];
		pastSample = 0;
//		Debug.Log(triggers[0].Length);
//		Debug.Log(triggers[1].Length);
//		Debug.Log(triggers[2].Length);
//		Debug.Log(triggers[3].Length);
//		Debug.Break();
		
	}
	
	void playMusic() {
		if(!cam.audio.isPlaying) cam.audio.Play ();	
	}
	
	void triggerEnemiesOnMusic() {
		
		// Check where in the sound file we are at, and whether this has updated yet (framerate vs. music playback issue)
		int sample = cam.audio.timeSamples;
		if (sample > pastSample) {
			pastSample = sample;
			for (int t = 0; t < 3; t++) {
				while (counters[t] < triggers[t].Length && triggers[t][counters[t]] < sample) {
					counters [t]++;
					SpawnEnemy(t);
				}
			}
		}	
	}

    /// <summary>
    /// Spawns an enemy of random type.
    /// </summary>
	public void SpawnEnemy ()
	{	
		SpawnEnemy (Random.Range (0, EnemyPrefabs.Length));	
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
	
	public IEnumerator yieldRoutine () {
		yield return 0;
	}

    #endregion
}