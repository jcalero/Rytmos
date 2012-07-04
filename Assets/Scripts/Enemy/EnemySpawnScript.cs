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
	private float timer;					// Used to sync framerate with music playback-rate
	private float audioLength;

    #endregion

    #region Functions

    void Start() {
        //InvokeRepeating("SpawnEnemy", FirstSpawn, SpawnRate); // Start repeatedly spawning enemies
		Debug.Log("start called");
        init ();
        playMusic();
        //InvokeRepeating("SpawnEnemy", FirstSpawn, SpawnRate); // Start repeatedly spawning enemies
		//SpawnEnemy(0, 5f, 335);
        //InvokeRepeating("SpawnEnemy", FirstSpawn, SpawnRate); // Start repeatedly spawning enemies
        //init ();
        //playMusic();
    }
    
    void Update() {
		if (timer >= audioLength){
			cam.audio.Stop();
			Application.LoadLevel("Win");			
		}
    	else if(AudioManager.peaks != null && cam != null && cam.audio.isPlaying) {
			timer += Time.deltaTime;
			triggerEnemiesOnMusic(timer);
		}
    }
    
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
		audioLength = cam.audio.clip.frequency;
		Debug.Log("audio frequency from unity: " + audioLength);
		Debug.Log("Audio frequency from mpg: " + AudioManager.frequency);
		counters = new int[AudioManager.peaks.Length];
		timer = 0f;
    }
    
    void playMusic() {
        cam.audio.clip = AudioManager.getAudioClip();	// Set the camera's audio clip to the read data	
        if(!cam.audio.isPlaying) cam.audio.Play ();
		Debug.Log("length of song in seconds :" + cam.audio.time);
    }
	
	void triggerEnemiesOnMusic(float timer) {
		for(int t = 0; t < AudioManager.peaks.Length; t++) {
			while(counters[t] * (1024f/cam.audio.clip.frequency) < timer) {
				counters[t]++;
				if(	AudioManager.peaks[t][counters[t]] > 0) SpawnEnemy(t,3f,(int)((t/(float)AudioManager.peaks.Length)*100));
			}

		}		
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
			SpawnEnemy(prefab, speed, percentage-100);
			return;
		}
		SpawnEnemy(prefab, speed, xpos, ypos);
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