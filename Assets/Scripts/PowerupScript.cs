using UnityEngine;
using System.Collections;

public class PowerupScript : MonoBehaviour {
	public UIAtlas SpriteAtlas;
	public GameObject powerupManager;
	public GameObject[] powerups;
	
	private Game.Powerups pw;
	private string spriteName = "default";
	private int left;
	private int bottom;
	private int width;
	private int height;
	private float UVHeight = 1f;
	private float UVWidth = 1f;
	
	private LinkedSpriteManager spriteManager;
	private Sprite powerup;
	private float powerUpTimer;
	private float totalTimer;
	private readonly float screenTime = 5f;
	private float respawnTime = 15f;
	private bool spawned;
	
	private static bool canTriggerSpawn;
	private static bool canSpawn;
	private static Vector3 spawnPosition;
	private ParticleSystem ps;

	// Use this for initialization
	void Awake () {
		pw = Game.Powerups.None;
		spriteManager = powerupManager.GetComponent<LinkedSpriteManager>();
		ps = gameObject.GetComponent<ParticleSystem>();
		spriteName = "Powerup";
		spawned = true;
		canTriggerSpawn = false;
		canSpawn = false;
		if(Game.GameMode == Game.Mode.Tutorial)
			totalTimer = respawnTime/2f;
		else totalTimer = respawnTime;
		// Checks that the sprite name exists in the atlas, if not falls back to default sprite
		if (SpriteAtlas.GetSprite(spriteName) == null) {
			Debug.LogWarning("Sprite " + "\"" + spriteName + "\" " + "not found in atlas " + "\"" + SpriteAtlas + "\"" + ". Using default sprite, \"circle\".");
			spriteName = "circle";		
		}
		// Calculate sprite atlas coordinates and add sprite for 
		CalculateSprite(SpriteAtlas, "");
		powerup = spriteManager.AddSprite(gameObject, UVWidth, UVHeight, left, bottom, width, height, false);
		
		
		//Initialize the powerup-sprite to be off the screen
		moveSprite(new Vector3(20,20,0));
		// Set audio volume
		gameObject.audio.volume = Game.EffectsVolume;
	}
	
	// Update is called once per frame
	void Update () {
		ps.Emit(2);
		//If the player has selected the powerup, move it from visibility, and restart the timer
		if(Player.takenPowerup == true) {
			moveSprite(new Vector3(20,20, 0));
			powerUpTimer = 0;
			totalTimer = respawnTime;
			spawned = false;
			Player.takenPowerup = false;
		}
		
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
					moveSprite(new Vector3(20,20, 0));
					spawned = false;
				} else {
					canTriggerSpawn = true;
				}
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
		else if (widthHeightRatio < 1)
			UVWidth = 1f * widthHeightRatio;        // It's a "tall" sprite
	}
	
	private void moveSprite(Vector3 movePos) {
		gameObject.transform.localPosition = movePos;
	}

	private void moveSprite(Vector3 movePos, Color c) {
		gameObject.transform.localPosition = movePos;
	}

	private void spawnPowerupOnScreen() {
		Vector3 spawnPos = randomPos();
		int choice = Random.Range(0,3);
		moveSprite(spawnPos,setPowerup(choice));
		spawned = true;
		totalTimer = screenTime;
		gameObject.audio.Play();	
	}
	
	private void spawnPowerupOnScreen(Vector3 spawnPos) {
		int choice = Random.Range(0,3);
		setPowerup(choice);
		moveSprite(spawnPos);
		spawned = true;
		totalTimer = screenTime;
		gameObject.audio.Play();	
	}
	
	public void spawnPowerupOnScreen(int choice) {
		Vector3 spawnPos = randomPos();
		if(choice > 2 || choice < 0) 
			choice = 0;
		moveSprite(spawnPos,setPowerup(choice));
		spawned = true;
		totalTimer = screenTime;
		gameObject.audio.Play();		
	}
	
	public void spawnPowerupOnScreen(int choice, Vector3 spawnPos) {
		if(choice > 2 || choice < 0) 
			choice = 0;
		moveSprite(spawnPos,setPowerup(choice));
		spawned = true;
		totalTimer = screenTime;
		gameObject.audio.Play();		
	}
	

	
	public bool spawnSprite() {
		int choice = Random.Range(0,3);
		return spawnSprite(choice);
	}
	
	public bool spawnSprite(int i) {
		Vector3 movePos = randomPos();
		moveSprite(movePos, setPowerup(i));
		spawned = true;
		totalTimer = screenTime;
		return spawned;
	}
	
	public bool removeSprite() {
		moveSprite(new Vector3(20,20, 0));
		spawned = false;
		return spawned;
	}
	
	private Color setPowerup(int choice) {
		Color c;
		switch(choice) {
			case 0:
				pw = Game.Powerups.MassivePulse;
				c = Color.white;
				break;
			case 1:
				pw = Game.Powerups.Invincible;
				c = Color.magenta;
				break;
			case 2:
				pw = Game.Powerups.ChainReaction;
				c = Color.grey;
				break;
			default:
				c = Color.black;
				pw = Game.Powerups.None;
				break;
		}
		ps.startColor = c;
		return c;
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
