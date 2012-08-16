using UnityEngine;
using System.Collections;

public class SpawnerDisplayManager : MonoBehaviour {
	
	public GameObject[] spawnerRefs;
	public GameObject EnemySpawner;
	public GameObject gameAtlas;
	public GameObject player;
	public UIAtlas uiatlas;
	
	private EnemySpawnScript ess;
	private Player p;
	private int activeSpawner;
	private int oldSpawner;
	private GameObject[][] allSpawners;
	private int[] oldPositions;
	
	private LinkedSpriteManager sm;
	private int left = 0;
	private int bottom = 0;
	private int width = 0;
	private int height = 0;
	private float UVHeight = 1f;
	private float UVWidth = 1f;
	
	private Vector3 originalSize;
	
	void Awake() {
		// Create pool of spawner instances
		allSpawners = new GameObject[spawnerRefs.Length][];
		sm = gameAtlas.GetComponent<LinkedSpriteManager>();
		for(int i = 0; i < allSpawners.Length; i++) {
			allSpawners[i] = new GameObject[3];
			for(int j = 0; j < allSpawners[i].Length; j++) {
				allSpawners[i][j] = (GameObject)Instantiate(spawnerRefs[i]);
				CalculateSprite(uiatlas, spawnerRefs[i].name+"1");
				sm.AddSprite(allSpawners[i][j], UVWidth, UVHeight, left, bottom, width, height, false);
				allSpawners[i][j].transform.localPosition = new Vector3(100f,100f,0f);
			}
		}
		originalSize = allSpawners[0][0].transform.localScale;
		p = player.GetComponentInChildren<Player>();
	}
	
	// Use this for initialization
	void Start () {
		ess = EnemySpawner.GetComponentInChildren<EnemySpawnScript>();
		oldPositions = new int[EnemySpawnScript.spawnerCounter];
		activeSpawner = -1;
		oldSpawner = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(EnemySpawnScript.currentlySelectedEnemy != activeSpawner) {
			if(activeSpawner != -1) oldSpawner = activeSpawner;
			activeSpawner = EnemySpawnScript.currentlySelectedEnemy;
			swapSpawner();
		}
		
		updateSpawnerPositionAndSize();
		
//		foreach(GameObject spawner in allSpawners[activeSpawner]) {
//			spawner.transform.localScale = new Vector3(1.05f*Background.PlayerSizeFactor, 
//				1.05f*Background.PlayerSizeFactor, 
//				spawner.transform.localScale.z);
//		}
	}
	
	private void swapSpawner() {
		
		if(oldPositions.Length != ess.spawnPositions.Length) oldPositions = new int[ess.spawnPositions.Length];
		for (int i = 0; i < allSpawners[activeSpawner].Length; i++) {
			allSpawners[oldSpawner][i].transform.localPosition = new Vector3(100f,100f,0f);
			allSpawners[activeSpawner][i].transform.localPosition = ess.findSpawnPositionVector(ess.spawnPositions[i]);
			oldPositions[i] = ess.spawnPositions[i];
		}
	}
	
	private void updateSpawnerPositionAndSize() {		
		for (int i = 0; i < allSpawners[activeSpawner].Length; i++) {
			if(ess.spawnPositions[i] != oldPositions[i]) {
				allSpawners[activeSpawner][i].transform.localPosition = ess.findSpawnPositionVector(ess.spawnPositions[i]);
				oldPositions[i] = ess.spawnPositions[i];
			}
			allSpawners[activeSpawner][i].transform.localScale = new Vector3(originalSize.x*p.scaleFactor, originalSize.y*p.scaleFactor, originalSize.z);
		}
	}
	
	private void CalculateSprite(UIAtlas atlas, string name) {
		UIAtlas.Sprite sprite = atlas.GetSprite(name);
		if (sprite == null) {
			Debug.LogError("No sprite with that name: " + name);
			return;
		}
		left = (int)sprite.inner.xMin;
		bottom = (int)sprite.inner.yMax;
		width = (int)sprite.inner.width;
		height = (int)sprite.inner.height;
		
		UVHeight = 1f;
		UVWidth = 1f;

		float widthHeightRatio = sprite.inner.width / sprite.inner.height;
		if (widthHeightRatio > 1)
			UVHeight = 1f / widthHeightRatio;       // It's a "wide" sprite
		else if (widthHeightRatio < 1)
			UVWidth = 1f * widthHeightRatio;        // It's a "tall" sprite
	}
	
	private string getNameFromPrefabNumber(int prefab) {
		switch(prefab) {
			case 0: return "BlueSpawner1";
			case 1: return "CyanSpawner1";
			case 2: return "RedSpawner1";
			case 3: return "YellowSpawner1";
			case 4: return "GreenSpawner1";
			case 5: return "PurpleSpawner1";
			default: return "";			
		}
	}
}
