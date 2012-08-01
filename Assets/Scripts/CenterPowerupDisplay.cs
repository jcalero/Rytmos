using UnityEngine;
using System.Collections;

public class CenterPowerupDisplay : MonoBehaviour {
	public GameObject smObject;
	public UIAtlas SpriteAtlas;
	
	private LinkedSpriteManager spriteManager;
    private Sprite powerupDisplay;
	private string spriteName = "default";
    private int left;
    private int bottom;
    private int width;
    private int height;
    private float UVHeight = 1f;
    private float UVWidth = 1f;
	
	// Use this for initialization
	void Awake () {
		spriteManager = smObject.GetComponent<LinkedSpriteManager>();
	}
	
	void Update() {
		if(spriteName == "Atom" || spriteName == "Circles") 
			transform.localEulerAngles = new Vector3(0,0,transform.localEulerAngles.z+(Time.deltaTime*50f));
	}
	
	public void hideSprite() {
		if (powerupDisplay != null)
            spriteManager.RemoveSprite(powerupDisplay);
	}
	
	public void changeSprite(Game.Powerups pw) {
		if(pw == Game.Powerups.MassivePulse) {
			updateSprite ("Atom");
		} else if (pw == Game.Powerups.ChainReaction) {
			updateSprite ("Circles");
		} else if (pw == Game.Powerups.Invincible) {
			updateSprite ("shield");
		}		
	}
	
	public void updateSprite(string name) {
		spriteName = name;
		
		 if (SpriteAtlas.GetSprite(spriteName) == null) 
            Debug.LogWarning("Sprite " + "\"" + spriteName + "\" " + "not found in atlas " + "\"" + SpriteAtlas + "\"" + ". Using default sprite, \"circle\".");	
     
        // Calculate sprite atlas coordinates
        CalculateSprite(SpriteAtlas, spriteName);
		transform.localEulerAngles = new Vector3(0,0,0);
		transform.localScale = new Vector3(1,1,1);
		
        // Add sprite to game object
        powerupDisplay = spriteManager.AddSprite(gameObject, UVWidth, UVHeight, left, bottom, width, height, false);
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
			UVHeight = 1;
		if (widthHeightRatio < 1)
            UVWidth = 1f * widthHeightRatio;        // It's a "tall" sprite
		else 
			UVWidth = 1;
    }
	
	void OnDestroy() {
		try {
       	 	if (powerupDisplay != null)
            	spriteManager.RemoveSprite(powerupDisplay);
		} catch(System.Exception) {
			//Sprite likes to throw errors here, but this seems to fix it. 
		}
	}
}
