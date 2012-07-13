using UnityEngine;
using System.Collections;

public class PowerupScript : MonoBehaviour {
    public UIAtlas SpriteAtlas;

	
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
		spriteManager = GameObject.Find("PowerupManager").GetComponent<LinkedSpriteManager>();
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
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Player.health < 40) respawnTime = 10f;
		else respawnTime = 20f;
		
		
		if(Player.takenPowerup == true) {
			Debug.Log("Taken");
			//Makes sure that Player.hasPowerup is only set to be true during the main scene
			moveSprite(new Vector3(20,20, 0));
			powerUpTimer = 0;
			totalTimer = respawnTime;
			spawned = false;
			Player.takenPowerup = false;
		}
		
		if(powerUpTimer > totalTimer) {
			if(spawned) {
				moveSprite(new Vector3(20,20, 0));
				spawned = false;
			}
			else {
				Vector3 spawnPos = randomPos();
				moveSprite(spawnPos);
				spawned = true;
				totalTimer = screenTime;
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
	
	private Vector3 randomPos() {
		float x = Random.Range (1, Game.screenRight-1);
		float y = Random.Range (1, Game.screenTop-1);
		float neg = Random.Range (0, 2);
		if(neg==0) x = -x;
		neg = Random.Range (0, 2);
		if(neg==0) y = -y;
		return new Vector3(x,y,0);
	}
    
}
