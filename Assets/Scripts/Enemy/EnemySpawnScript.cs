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
	public int[] spawnPositions;
	private int currentlySelectedEnemy;
    
    private int RandomSeed;                 // The enemy type to spawn
    private GameObject cam;					// Camera gameobject to play audio
    private int[] counters;					// "Pointers" for the triggers of each channel
	private float timer;					// Used to sync framerate with music playback-rate
	private float audioLength;
	private float[] timers;
	private readonly float timeThresh = 0.25f;
	private int[] spawnRestrictors;
	private int[] spawnDivisors;

	private int rotateDirection;
	private int loudPartCounter;
	private bool loudFlag;
    #endregion

    #region Functions

    void Start() {
        //InvokeRepeating("SpawnEnemy", FirstSpawn, SpawnRate); // Start repeatedly spawning enemies		
        init ();
        playMusic();
    }
    
    void Update() {
		if (timer >= audioLength || (!Game.Paused && !cam.audio.isPlaying && timer > 0)){
			cam.audio.Stop();
			Application.LoadLevel("Win");			
		}
    	else if(AudioManager.peaks != null && cam != null && cam.audio.isPlaying) {
			timer += Time.deltaTime;
			triggerEnemiesOnMusic(timer);
		} //else timer += Time.deltaTime;
    }
    
	/// <summary>
	/// Initialise this instance.
	/// 
	/// Find the camera and assign it the loaded audio clip
	/// ...and then initialize a load of other stuff
	/// </summary>
    void init() {
		cam = GameObject.Find ("Main Camera");
		if(Game.Song != null && Game.Song != "") {
			if(Game.Song != AudioManager.getCurrentSong()) AudioManager.initMusic(Game.Song);
		} else if (cam.audio.clip != null) {
			if(!AudioManager.isSongLoaded()) {
				AudioManager.setCam(this.cam);
				AudioManager.initMusic("");
			}
		}
		while(!AudioManager.isSongLoaded()) yieldRoutine();
		cam.audio.loop = false;
		audioLength = AudioManager.audioLength;
		counters = new int[AudioManager.peaks.Length];
		timers = new float[AudioManager.peaks.Length];
		spawnRestrictors = new int[AudioManager.peaks.Length];
		spawnDivisors = new int[]{2,2,8,2,2,2};
		timer = 0f;
		spawnPositions = new int[]{0};
		currentlySelectedEnemy = 0;
		rotateDirection = 1;
		Level.SetUpParticlesFeedback(spawnPositions.Length, currentlySelectedEnemy);		
		loudPartCounter = 0;
		loudFlag = false;
    }
    
    void playMusic() {
        cam.audio.clip = AudioManager.getAudioClip();	// Set the camera's audio clip to the read data	
        if(!cam.audio.isPlaying) cam.audio.Play ();
    }
	
	void triggerEnemiesOnMusic(float timer) {
		
		/* Music related logic: */
		if(loudPartCounter < AudioManager.loudPartTimeStamps.Length && AudioManager.loudPartTimeStamps[loudPartCounter]/(float)AudioManager.frequency < timer+0.5f) {
			loudFlag = !loudFlag;
			loudPartCounter++;			
		}
		
		// Iterate over every channel
		for(int t = 0; t < AudioManager.peaks.Length; t++) {
			
			// Sync peaks (can call them triggers) to the music
			while(counters[t] < AudioManager.peaks[t].Length && AudioManager.peaks[t][counters[t]] * (1024f/(float)AudioManager.frequency) < timer) {
				counters[t]++;
				
				// Are the peaks withing range? Do we have a peak?
				//if(	counters[t] < AudioManager.peaks[t].Length && AudioManager.peaks[t][counters[t]] > 0) {
					
					// Filter out too frequent peaks
					if(timer - timers[t] > timeThresh) {
						
						// Filter out every 2nd, or 3rd, or what ever specified trigger
						if(spawnRestrictors[t] == 0) {
							
							/* Spawning/Gameplay related logic */
							switch (t) {
							case 0:
								// This is the bass frequency, used for spawning enemies (for now at least)
								Level.SetUpParticlesFeedback(spawnPositions.Length, currentlySelectedEnemy);
								//Find magnitude of the furthest away
								float maxMag = 0;
								if(Game.SyncMode) {
									foreach(int spawnPosition in spawnPositions) {
										float currMaxMag = findSpawnPositionVector(spawnPosition).magnitude;
										if(currMaxMag > maxMag) {
											maxMag = currMaxMag;	
										}
									}
								}

								foreach(int spawnPosition in spawnPositions) {
									Vector3 spawnDist = findSpawnPositionVector(spawnPosition);
									float speed = 3f;
									if(Game.SyncMode) {
										speed *= (spawnDist.magnitude)/maxMag;
									}
									SpawnEnemy (currentlySelectedEnemy, speed, spawnDist);
									SpawnEnemy(currentlySelectedEnemy,loudFlag? 3 : 1,spawnPosition);
								}
								break;
							case 1:
								// These are more medium ranged frequencies, used to change the spawn position (for now at least)
								for (int i = 0; i < spawnPositions.Length; i++) {
									incrementSpawnPosition(ref spawnPositions[i],10,rotateDirection);
								}								
								break;
							case 2:
								// These are even more medium ranged frequencies, used to change the direction (for now, again :P )
								if(rotateDirection == 1) rotateDirection-=2;
								else rotateDirection+=2;
								break;
							case 3:
								// Some higher frequencies to change the currently spawned enemy
								Level.SetUpParticlesFeedback(spawnPositions.Length, currentlySelectedEnemy);
								changeEnemy(ref currentlySelectedEnemy);
								break;
							case 4:
								break;
							case 5:
								break;
							default:
								
								break;								
							}
							
							// Update the time of last spawn
							timers[t] = timer;
						}
						// Update the spawning restrictors
						spawnRestrictors[t] = (spawnRestrictors[t]+1)%spawnDivisors[t];
					}
				}
			//}

		}		
	}
	
	private static void changeEnemy(ref int currentEnemy) {
		int rnd = Random.Range(0,101);
		if(Level.fourColors) {
			if(rnd < 25) currentEnemy = 0;
			else if(rnd < 50) currentEnemy = 1;
			else if(rnd < 75) currentEnemy = 2;
			else if(rnd < 100) currentEnemy = 3;
		} else {
			if(rnd < 30) currentEnemy = 0;
			else if(rnd < 55) currentEnemy = 1;
			else if(rnd < 75) currentEnemy = 2;
			else if(rnd < 85) currentEnemy = 3;
			else if(rnd < 95) currentEnemy = 4;
			else if(rnd < 101) currentEnemy = 5;
		}
		
	}
	
	private static void incrementSpawnPosition(ref int currentPos, int increment, int direction) {
		currentPos += increment*direction;
		if(currentPos > 100) currentPos -= 100;
		else if (currentPos < 0) currentPos += 100;
		//return currentPos;		
	}
		
	public static IEnumerator yieldRoutine () {
		yield return 0;
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
	
	public void SpawnEnemy (int prefab, float speed, int percentage) {
		Vector3 temp = findSpawnPositionVector(percentage);
		SpawnEnemy(prefab, speed, temp.x, temp.y);
	}
		
	public void SpawnEnemy (int prefab, float speed, Vector3 dist) {
		SpawnEnemy (prefab, speed, dist.x, dist.y);
	}
	
	public Vector3 findSpawnPositionVector(int percentage) {
		float xpos;
		float ypos;
		if(percentage <= 25) {
			xpos = Game.screenRight;
			ypos = (Game.screenTop*2*percentage/25) + Game.screenBottom;
		} else if (percentage <= 50) {
			xpos = Game.screenRight - (Game.screenRight*2*(percentage-25)/25);
			ypos = Game.screenTop;
		} else if (percentage <= 75) {
			xpos = Game.screenLeft;
			ypos = Game.screenTop - (Game.screenTop*2*(percentage-50)/25);
		} else if (percentage <= 100) {
			xpos = (Game.screenRight*2*(percentage-75)/25) + Game.screenLeft;
			ypos = Game.screenBottom;
		} else {
			return findSpawnPositionVector (percentage-100);
		}
		return new Vector3(xpos, ypos, 0);	
	}	

    /// <summary>
    /// Restarts the Invoke method to allow new spawn rates to be initialised
    /// </summary>
    public void RestartSpawner() {
        StopSpawner();
        //Start();
    }

    /// <summary>
    /// Stops the spawner/Invoke method.
    /// </summary>
    public void StopSpawner() {
        //CancelInvoke("SpawnEnemy");
    }
    #endregion
}