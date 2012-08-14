using UnityEngine;
using System.Collections;

public class SpawnerDisplayManager : MonoBehaviour {
	
	public GameObject[] spawnerRefs;
	public GameObject EnemySpawner;
	
	private int activeSpawner;
	private GameObject[] activeSpawners;
	private EnemySpawnScript ess;
	private int[] oldPositions;
	// Use this for initialization
	void Start () {
		ess = EnemySpawner.GetComponentInChildren<EnemySpawnScript>();
		activeSpawners = new GameObject[EnemySpawnScript.spawnerCounter];
		oldPositions = new int[EnemySpawnScript.spawnerCounter];
		activeSpawner = -1;
		updateSpawnerPosition();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(EnemySpawnScript.currentlySelectedEnemy != activeSpawner) {
			activeSpawner = EnemySpawnScript.currentlySelectedEnemy;
			swapSpawner();
		}
		
		updateSpawnerPosition();
	}
	
	private void swapSpawner() {
		
		if(EnemySpawnScript.spawnerCounter != activeSpawners.Length) {
			foreach(GameObject spawner in activeSpawners) Destroy(spawner);
			activeSpawners = new GameObject[EnemySpawnScript.spawnerCounter];
		}
		
		for (int i = 0; i < activeSpawners.Length; i++) {
			if(activeSpawners[i] != null) Destroy(activeSpawners[i]);
			activeSpawners[i] = (GameObject)Instantiate(spawnerRefs[activeSpawner]);
		}
	}
	
	private void updateSpawnerPosition() {
		if(oldPositions.Length != EnemySpawnScript.spawnerCounter) oldPositions = new int[EnemySpawnScript.spawnerCounter];
		
		for (int i = 0; i < activeSpawners.Length; i++) {
			if(ess.spawnPositions[i] != oldPositions[i]) {
				activeSpawners[i].transform.localPosition = ess.findSpawnPositionVector(ess.spawnPositions[i]);
				oldPositions[i] = ess.spawnPositions[i];
			}
		}
	}
}
