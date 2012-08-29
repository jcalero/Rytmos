using UnityEngine;
using System.Collections;

public class PowerupScript : MonoBehaviour {
	public UIAtlas SpriteAtlas;
	public GameObject powerupManager;
	public GameObject[] powerups;
	
	private Game.Powerups pw;
	private int left;
	private int bottom;
	private int width;
	private int height;
	private float UVHeight = 1f;
	private float UVWidth = 1f;
	
	private LinkedSpriteManager spriteManager;
	private float powerUpTimer;
	private float totalTimer;
	private readonly float screenTime = 5f;
	private float respawnTime = 15f;
	private bool spawned;
	private static int activePW;
	
	private static bool canTriggerSpawn;
	private static bool canSpawn;
	private static Vector3 spawnPosition;
	private Vector3 away = new Vector3(20,20,0);

	// Use this for initialization
	void Awake () {
		activePW = 0;
		pw = Game.Powerups.None;
		spriteManager = powerupManager.GetComponent<LinkedSpriteManager>();
		spawned = true;
		canTriggerSpawn = false;
		canSpawn = false;
		if(Game.GameMode == Game.Mode.Tutorial)
			totalTimer = respawnTime/2f;
		else totalTimer = respawnTime;

		// Calculate sprite atlas coordinates and add sprite for 
		CalculateSprite(SpriteAtlas, "Atom");
		spriteManager.AddSprite(powerups[0], UVWidth, UVHeight, left, bottom, width, height, false);
		CalculateSprite (SpriteAtlas, "Circles");
		spriteManager.AddSprite(powerups[1], UVWidth, UVHeight, left, bottom, width, height, false);
		CalculateSprite (SpriteAtlas, "Shield");
		spriteManager.AddSprite(powerups[2], UVWidth, UVHeight, left, bottom, width, height, false);

		
		
		//Initialize the powerup-sprite to be off the screen
		moveSprite(away, powerups[0]);
		moveSprite(away, powerups[1]);
		moveSprite(away, powerups[2]);
		// Set audio volume
		gameObject.audio.volume = Game.EffectsVolume;
	}
	
	// Update is called once per frame
	void Update () {
		//If the player has selected the powerup, move it from visibility, and restart the timer
		if(Player.takenPowerup == true) {
			moveSprite(away, powerups[activePW]);
			powerUpTimer = 0;
			totalTimer = respawnTime;
			spawned = false;
			Player.takenPowerup = false;
		}
		
		//Spin the atom and the circles about their center
		powerups[0].transform.localEulerAngles = new Vector3(0,0,powerups[0].transform.localEulerAngles.z + (Time.deltaTime*50f)); 
		powerups[1].transform.localEulerAngles = new Vector3(0,0,powerups[1].transform.localEulerAngles.z + (Time.deltaTime*50f)); 
		
		//Debug mode spawn powerups for testing
		if(DevScript.devModeAccess) {
			if(Input.GetKeyDown(KeyCode.Z))
				spawnPowerupOnScreen(0);
			if(Input.GetKeyDown(KeyCode.X))
				spawnPowerupOnScreen (1);
			if(Input.GetKeyDown(KeyCode.C))
				spawnPowerupOnScreen (2);
		}
		//Check the timer		
		if(Game.GameMode != Game.Mode.Tutorial || (Game.Mode.Tutorial == Game.GameMode && Tutorial.spawnPowerupsNormal)) {
			if(powerUpTimer > totalTimer) {
				//If the powerup is spawned (on the screen), remove it
				if(spawned) {
					moveSprite(away, powerups[activePW]);
					spawned = false;
				} else 
					canTriggerSpawn = true;
				
				powerUpTimer = 0;
			}
			else if(canSpawn) {
				spawnPowerupOnScreen(spawnPosition);
				canSpawn = false;
			}
		
			powerUpTimer += Time.deltaTime;
		}
	}
	
	void CalculateSprite(UIAtlas atlas, string name) {
		UIAtlas.Sprite sprite = atlas.GetSprite(name);
		if (sprite == null) {
			Debug.LogError("No sprite with that name: " + name);
			return;
		}
		
		left = (int)sprite.inner.xMin;
		bottom = (int)sprite.inner.yMax;
		width = (int)sprite.inner.width;
		height = (int)sprite.inner.height;

		float widthHeightRatio = sprite.inner.width / sprite.inner.height;
		if (widthHeightRatio > 1)
			UVHeight = 1f / widthHeightRatio;       // It's a "wide" sprite
		else 
			UVHeight = 1f;
		if (widthHeightRatio < 1)
			UVWidth = 1f * widthHeightRatio;        // It's a "tall" sprite
		else 
			UVWidth = 1f;
	}
	

	
	private void moveSprite(Vector3 movePos, GameObject powerup) {
		powerup.transform.localPosition = movePos;
		gameObject.transform.localPosition = movePos;
	}

	private void spawnPowerupOnScreen() {
		Vector3 spawnPos = randomPos();
		int choice = Random.Range(0,3);
		spawnPowerupOnScreen(choice, spawnPos);
	}
	
	private void spawnPowerupOnScreen(Vector3 spawnPos) {
		int choice = Random.Range(0,3);
		spawnPowerupOnScreen(choice, spawnPos);	
	}
	
	public void spawnPowerupOnScreen(int choice) {
		Vector3 spawnPos = randomPos();
		spawnPowerupOnScreen(choice, spawnPos);		
	}
	
	public void spawnPowerupOnScreen(int choice, Vector3 spawnPos) {
		if(choice > 2 || choice < 0) 
			choice = 0;
		moveSprite(spawnPos,setPowerup(choice));
		spawned = true;
		totalTimer = screenTime;
		gameObject.audio.Play();		
	}
	
	private GameObject setPowerup(int choice) {
		switch(choice) {
			case 0:
				pw = Game.Powerups.MassivePulse;
				activePW = 0;
				return powerups[0];
			case 1:
				pw = Game.Powerups.Invincible;
				activePW = 2;
				return powerups[2];
			case 2:
				pw = Game.Powerups.ChainReaction;
				activePW = 1;
				return powerups[1];
			default:
				pw = Game.Powerups.None;
				activePW = 0;
				return powerups[0];
		}
	}
	
	private Vector3 randomPos() {
		float x = Random.Range (1, Game.screenRight-1);
		float y = Random.Range (1, Game.screenTop-1);
		float neg = Random.Range (0, 2);
		if(neg==0) x = -x;
		neg = Random.Range (0, 2);
		if(neg==0) y = -y;
		return new Vector3(x,y,0);
	}
	
	public Game.Powerups Powerup() {
		return pw;
	}
	
	public static void SpawnPowerup(Vector3 position) {
		if(!canTriggerSpawn) return;
		else {
			canSpawn = true;
			canTriggerSpawn = false;
			spawnPosition = position;
		}
	}
	
	public static bool CanSpawn {
		get {return canTriggerSpawn;}
	}
	
}
