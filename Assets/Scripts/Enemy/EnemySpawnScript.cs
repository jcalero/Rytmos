using UnityEngine;
using System.Collections;
/// <summary>
/// EnemySpawnScript.cs
/// 
/// Handles the spawning of the enemies.
/// </summary>
public class EnemySpawnScript : MonoBehaviour,PeakListener {

	#region Public Vars
	public UIAtlas enemyAtlas;
	public GameObject[] EnemyPrefabs;       // List of enemy types to spawn. Inspector reference. Location: EnemySpawner
	public float SpawnRate;                 // The time between spawns
	public int[] spawnPositions;
	public static int currentlySelectedEnemy;
	public static int spawnCount;
	public static int spawnerCounter;
	#endregion
	
	#region Private Vars
	private float maxMag;
	
	private int RandomSeed;                 // The enemy type to spawn
	private int[] counters;					// "Pointers" for the triggers of each channel
	private float timer;					// Used to sync framerate with music playback-rate
	private float audioLength;
	private float[] timers;
	private int[] spawnRestrictors;
	private int[] spawnDivisors;
	private float baseSpeed;
	private bool lastTime;
	private float storeTime;
	
	private float timeSinceLastSpawn;
	private readonly float SPAWN_ANIM_TIME = 0.2f;
	
	private bool resetColor;
	private static int rotateDirection;
	private int loudPartCounter;
	private float loudFlag;
	private bool swapColours;
	
	private int spawnerNumber;
	private static EnemySpawnScript instance;
	
	private float spawnBoundsBottom;
	private float spawnBoundsTop;
	private float spawnBoundsLeft;
	private float spawnBoundsRight;
	
	#endregion
	
	#region Functions
	void Awake() {
		instance = this;
	}

	void Start() {
		init ();		
	}
	
	void Update() {
		timer += Time.deltaTime;
		timeSinceLastSpawn += Time.deltaTime;
		
		
		if (timer >= audioLength){
			if (!AudioManager.songLoaded) Application.LoadLevel("LoadScreen");
			else {
				if(spawnCount == Level.EnemiesDespawned) {
					if(!lastTime)
						storeTime = timer;
					lastTime = true;
				} else {
					lastTime = false;
					storeTime = 0;
				}
				if(timer >= audioLength + 7 || (lastTime && storeTime + 1.5f < timer)) 
					Application.LoadLevel("Win");		
			}
			
		}
		
//		if(Player.multiplier < 3) spawnerCounter = 2;
//		else if(Player.multiplier < 6) spawnerCounter = 3;
//		else if(Player.multiplier < 12) spawnerCounter = 3;
//		else if(Player.multiplier < 17) spawnerCounter = 3;
//		else if(Game.GameMode != Game.Mode.Casual) spawnerCounter = 3;
		
		updateSpawnPositions();
	}
	
	/// <summary>
	/// Initialise this instance.
	/// 
	/// Find the camera and assign it the loaded audio clip
	/// ...and then initialize a load of other stuff
	/// </summary>
	void init() {
		
		// "Register" with the PeakTriggerManager to be alerted when peaks arise, loudness changes, etc..
		PeakTriggerManager.addSelfToListenerList(this);
		
		// Initialize other variables
		audioLength = AudioManager.audioLength;
		timer = 0f;
		timers = new float[AudioManager.peaks.Length];
		spawnRestrictors = new int[AudioManager.peaks.Length];
		spawnDivisors = new int[]{1,1,8,2,2,2};
		spawnerNumber = 0;
		Game.SyncMode = true;
		timeSinceLastSpawn = 0.2f;
		//spawnPositions = new int[]{40,90,35,85};
		spawnPositions = new int[] { 0 };
		spawnerCounter = 3;
		currentlySelectedEnemy = Random.Range(0,6);
		spawnCount = 0;
		rotateDirection = 1;
		loudFlag = 0;
		maxMag = new Vector2(Game.screenTop,Game.screenRight).magnitude;
//		if(Game.GameMode == Game.Mode.Casual) baseSpeed = 1.5f;
//		else 
		baseSpeed = 2.5f;
		updateSpawnPositions();
		swapColours = false;
		lastTime = false;
		storeTime = 0;
		
		spawnBoundsRight = Game.screenRight + (0.09f*Game.screenLeft);
		spawnBoundsLeft = -spawnBoundsRight;
		spawnBoundsTop = Game.screenTop + (0.16f*Game.screenBottom);
		spawnBoundsBottom = -spawnBoundsTop;
		
	}
	
	void updateSpawnPositions() {
		// We have as many spawners as we want -> Do nothing
		if(spawnPositions.Length == spawnerCounter) return;
		
		/* Change he number of spawners:
		 * 1. Divide 100 into (spawnerCounter) parts.
		 * 2. Take the first spawner in the list as an offset.
		 * 3. Calculate position of remaining (spawnerCounter)-1 spawners 
		 */
		
		int spacing = 100/spawnerCounter;
		int firstPosition = spawnPositions[0];
		spawnPositions = new int[spawnerCounter];
		spawnPositions[0] = firstPosition;
		for(int i = 1; i < spawnPositions.Length; i++) {
			spawnPositions[i] = firstPosition + (i*spacing);
		}
	}
	
	public void setLoudFlag(int flag) {
		loudFlag = 0.5f*loudFlag + 0.5f*flag;
	}
	
	/// <summary>
	/// This function is called by PeakTriggerManager, every time a peak is detected at the current song time.
	/// </summary>
	/// <param name='channel'>
	/// Channel.
	/// </param>
	public void onPeakTrigger(int channel, int intensity) {
		
		// Filter out spawns which are too close together
		if(timer - timers[channel] > PeakTriggerManager.timeThreshs[channel]) {			
			
			// Filter out every 2nd, or 3rd, or what ever specified trigger		
			if(spawnRestrictors[channel] == 0) {
								
				/* Spawning/Gameplay related logic */
				switch (channel) {
				case 0:
					// This is the bass frequency, used for spawning enemies (for now at least)
					
					
					for (int i = 0; i < spawnPositions.Length; i++) {
						incrementSpawnPosition(ref spawnPositions[i], 1, rotateDirection);
					}
					
					if(swapColours) {
						changeEnemy();
						swapColours = false;
					}
					//Find magnitude of the furthest away
					if(Game.SyncMode) {
						foreach (int spawnPosition in spawnPositions) {
							float currMaxMag = findSpawnPositionVector(spawnPosition).magnitude;
							if (currMaxMag > maxMag) {
								maxMag = currMaxMag;
							}
						}
//						float currMaxMag = findSpawnPositionVector(spawnPositions[spawnerNumber]-5).magnitude;
//						if(currMaxMag > maxMag) maxMag = currMaxMag;
//						currMaxMag = findSpawnPositionVector(spawnPositions[spawnerNumber]+5).magnitude;
//						if(currMaxMag > maxMag) maxMag = currMaxMag;
					}
					if(Game.GameMode == Game.Mode.Tutorial && !Tutorial.done) {
						if(!Tutorial.firstSpawn) Tutorial.firstSpawn = true;
						else if(Tutorial.firstSpawn && !Tutorial.secondSpawn) Tutorial.secondSpawn = true;
						else if(Tutorial.firstSpawn && Tutorial.secondSpawn && !Tutorial.thirdSpawn) {
							Tutorial.thirdSpawn = true;
							Tutorial.done = true;
						}
					}
					foreach(int spawnPosition in spawnPositions) {
						Vector3 spawnDist = findSpawnPositionVector(spawnPosition);
						float speed = calcSpeed();
						if(Game.SyncMode) speed *= (spawnDist.magnitude)/maxMag;
						SpawnEnemy(currentlySelectedEnemy,speed,spawnDist);
						spawnCount++;
//						spawnDist = findSpawnPositionVector(spawnPositions[spawnerNumber]+5);
//						if(Game.SyncMode) speed *= (spawnDist.magnitude)/maxMag;									
//						SpawnEnemy(currentlySelectedEnemy,speed,spawnDist);
//						spawnCount++;
					}
//						moveSpawnersMirrored(1,rotateDirection);
					break;
				case 1:
					// These are more medium ranged frequencies, used to change the spawn position (for now at least)
					if(timeSinceLastSpawn >= SPAWN_ANIM_TIME) {
						for (int i = 0; i < spawnPositions.Length; i++) {
							incrementSpawnPosition(ref spawnPositions[i], 3, rotateDirection);
						}
					}
//					moveSpawnersMirrored(3,rotateDirection);
					if(Game.GameMode == Game.Mode.Tutorial) setupTutorialSpawns(spawnCount, false);
					break;
				case 2:
					// These are even more medium ranged frequencies, used to change the direction (for now, again :P )
					if(rotateDirection == 1) rotateDirection=-1;
					else rotateDirection=1;
					int rand = Random.Range (0,spawnPositions.Length);
					if(rand == spawnerNumber) rand = (rand +1)%spawnPositions.Length;
					spawnerNumber = rand;
					break;
				case 3:
					// Some higher frequencies to change the currently spawned enemy
					if(timeSinceLastSpawn >= SPAWN_ANIM_TIME) {
						changeEnemy();
						if(swapColours) swapColours = false;
					} else swapColours = true;
					if(Game.GameMode == Game.Mode.Tutorial) {
						if(swapColours) {
							changeEnemy();
							swapColours = false;
						}
						setupTutorialSpawns(spawnCount, true);
					}
					break;
				case 4:
					break;
				case 5:
					break;
				default:
					
					break;								
				}
				
				// Update the time of last spawn
				timers[channel] = timer;
			}
			// Update the spawning restrictors
			spawnRestrictors[channel] = (spawnRestrictors[channel]+1)%spawnDivisors[channel];
		}
	}
	
	private float calcSpeed() {

		// Loudflag between 0 and 5, median is 2.5
		// variationFactor between 0.2 & 1
		float speed = baseSpeed + (loudFlag - 2.5f)*AudioManager.variationFactor;
		if(speed < 1) speed = 1f;
		else if(speed > 4.5 && Game.GameMode != Game.Mode.Casual) speed = 4.5f;
		else if(speed > 3.5 && Game.GameMode == Game.Mode.Casual) speed = 4f;
	
		return speed;
		
	}
	
	private void setupTutorialSpawns(int spawn, bool change) {
		switch(spawnCount/3) {
			case 0:
				currentlySelectedEnemy = 1;
				break;
			case 1:
				currentlySelectedEnemy = 3;
				break;
			case 2:
				currentlySelectedEnemy = 4;
				break;
			case 3:
				currentlySelectedEnemy = 2;
				break;
			case 4:
				currentlySelectedEnemy = 5;
				break;
			case 5:
				currentlySelectedEnemy = 0;
				break;
			case 6:
				break;
			default:
				if(change)
					changeEnemy ();
				break;
			}
	}
	
	
	private static void changeEnemy() {
		int rnd = Random.Range(0,101);
		//Check if you are only using four colors, 
		if(Level.fourColors) {
			if(rnd < 25) currentlySelectedEnemy = 0;
			else if(rnd < 50) currentlySelectedEnemy = 1;
			else if(rnd < 75) currentlySelectedEnemy = 2;
			else if(rnd < 100) currentlySelectedEnemy = 3;				
		} else {
			//You are in six color mode, and check if the colorpowerup is not active
//			if(rnd < 30) currentlySelectedEnemy = 0;
//			else if(rnd < 55) currentlySelectedEnemy = 1;
//			else if(rnd < 75) currentlySelectedEnemy = 2;
//			else if(rnd < 85) currentlySelectedEnemy = 3;
//			else if(rnd < 95) currentlySelectedEnemy = 4;
//			else if(rnd < 101) currentlySelectedEnemy = 5;
			if(rnd < 17) currentlySelectedEnemy = 0;
			else if(rnd < 34) currentlySelectedEnemy = 1;
			else if(rnd < 51) currentlySelectedEnemy = 2;
			else if(rnd < 68) currentlySelectedEnemy = 3;
			else if(rnd < 85) currentlySelectedEnemy = 4;
			else if(rnd < 101) currentlySelectedEnemy = 5;
		}
	}
	
	private void moveSpawnersMirrored(int increment,int rotateDirection) {
		spawnPositions[0] += increment*rotateDirection;
		correctSpawnPosition(0);
		
		spawnPositions[1] = spawnPositions[0] + 50;
		correctSpawnPosition(1);
		
		spawnPositions[2] -= increment*rotateDirection;
		correctSpawnPosition(2);
		
		spawnPositions[3] = spawnPositions[2] + 50;
		correctSpawnPosition(3);
	}
	
	private void correctSpawnPosition(int position) {
			
		if(spawnPositions[position] > 100) spawnPositions[position] -= 100;
		else if(spawnPositions[position] < 0) spawnPositions[position] += 100;
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
		timeSinceLastSpawn = 0f;
		Vector3 position = EnemyPrefabs [prefab].transform.position;
		Instantiate (EnemyPrefabs [prefab], position, EnemyPrefabs [prefab].transform.localRotation);
	}
	
	public void SpawnEnemy (int prefab, float speed, float xpos, float ypos) {
		timeSinceLastSpawn = 0f;
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
			xpos = spawnBoundsRight;
			ypos = (spawnBoundsTop*2*percentage/25) + spawnBoundsBottom;
		} else if (percentage <= 50) {
			xpos = spawnBoundsRight - (spawnBoundsRight*2*(percentage-25)/25);
			ypos = spawnBoundsTop;
		} else if (percentage <= 75) {
			xpos = spawnBoundsLeft;
			ypos = spawnBoundsTop - (spawnBoundsTop*2*(percentage-50)/25);
		} else if (percentage <= 100) {
			xpos = (spawnBoundsRight*2*(percentage-75)/25) + spawnBoundsLeft;
			ypos = spawnBoundsBottom;
		} else {
			return findSpawnPositionVector (percentage-100);
		}
		return new Vector3(xpos, ypos, 0);	
	}	
	
	void OnDisable () {
		PeakTriggerManager.removeSelfFromListenerList(this);	
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

	public static UIAtlas EnemyAtlas {
		get {
			return instance.enemyAtlas;
		}
	}
	#endregion
}