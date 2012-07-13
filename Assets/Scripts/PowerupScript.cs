using UnityEngine;
using System.Collections;

public class PowerupScript : MonoBehaviour {
    public UIAtlas SpriteAtlas;

	
	protected string spriteName = "default";
    protected int left;
    protected int bottom;
    protected int width;
    protected int height;
    protected float UVHeight = 1f;
    protected float UVWidth = 1f;
	
	private LinkedSpriteManager spriteManager;
    private Sprite powerup;
	// Use this for initialization
	void Awake () {
		spriteManager = GameObject.Find("PowerupManager").GetComponent<LinkedSpriteManager>();
        spriteName = "Powerup";

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
}
