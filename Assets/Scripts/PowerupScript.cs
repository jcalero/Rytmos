using UnityEngine;
using System.Collections;

public class PowerupScript : MonoBehaviour {
	public UIAtlas SpriteAtlas;
	public GameObject powerupManager;
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
	private float respawnTime = 20f;
	private bool spawned;
	// Use this for initialization
	void Awake () {
		pw = Game.Powerups.None;
		spriteManager = powerupManager.GetComponent<LinkedSpriteManager>();
		spriteName = "Powerup";
		spawned = true;
		// Checks that the sprite name exists in the atlas, if not falls back to default sprite
		if (SpriteAtlas.GetSprite(spriteName) == null) {
			Debug.LogWarning("Sprite " + "\"" + spriteName + "\" " + "not found in atlas " + "\"" + SpriteAtlas + "\"" + ". Using default sprite, \"circle\".");
			spriteName = "circle";		
		}
		// Calculate sprite atlas coordinates
		CalculateSprite(SpriteAtlas, spriteName);
		// Add sprite to game object
		powerup = spriteManager.AddSprite(gameObject, UVWidth, UVHeight, left, bottom, width, height, false);
		
		//Move object 
		Vector3 spawnPos = randomPos();
		Level.SetUpParticlesFeedback(4, spawnPos);
		gameObject.transform.localPosition = spawnPos;
		
		// Set audio volume
		gameObject.audio.volume = Game.EffectsVolume;
		
	}
	
	// Update is called once per frame
	void Update () {
		// DEPRECATED: Sets the respawn time to be relative to the player health
		respawnTime = 20f;
		
		//If the player has selected the powerup, move it from visibility, and restart the timer
		if(Player.takenPowerup == true) {
			moveSprite(new Vector3(20,20, 0));
			powerUpTimer = 0;
			totalTimer = respawnTime;
			spawned = false;
			Player.takenPowerup = false;
		}
		
		//Check the timer		
		if(powerUpTimer > totalTimer) {
			//If the powerup is spawned (on the screen), remove it
			if(spawned) {
				moveSprite(new Vector3(20,20, 0));
				spawned = false;
			}
			
			//If the powerup is not on the screen, move it to a random position and set the timer to be the screenTime
			else {
				Vector3 spawnPos = randomPos();
				//Here, we assign the powerup - this will be moved below once we have graphics
				int choice = Random.Range(0,3);
				moveSprite(spawnPos,setPowerup(choice));
				spawned = true;
				totalTimer = screenTime;
				gameObject.audio.Play();
			}
			powerUpTimer = 0;
		}
		
		powerUpTimer += Time.deltaTime;
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
	
	void OnDestroy() {
		if (powerup != null)
			spriteManager.RemoveSprite(powerup);
	}
	
	private void moveSprite(Vector3 movePos) {
		gameObject.transform.localPosition = movePos;
		Level.SetUpParticlesFeedback(4, movePos);
	}
	
	private void moveSprite(Vector3 movePos, Color c) {
		gameObject.transform.localPosition = movePos;
		Level.SetUpParticlesFeedback(4, movePos, c);
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
				c = Color.white;
				pw = Game.Powerups.None;
				break;
		}
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
	
}
