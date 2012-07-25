using UnityEngine;
using System.Collections;

public class CenterPowerupDisplay : MonoBehaviour, PeakListener {
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
	
	void Start() {
		PeakTriggerManager.addSelfToListenerList(this);
	}
	
	void Update() {
		if(spriteName == "Atom" || spriteName == "Circles") {
			transform.localEulerAngles = new Vector3(0,0,transform.localEulerAngles.z+.125f);
		}
	}
	
	public void onPeakTrigger(int channel,int intensity) {}
	
	public void setLoudFlag(int flag) {
		if(spriteName == "Atom" || spriteName == "Circles") {
			Debug.Log ("Yes");
			transform.localEulerAngles = new Vector3(0,0,transform.localEulerAngles.z+(float)(flag/5)*90);
		}
		if(spriteName == "shield") {
			iTween.ScaleTo(gameObject, new Vector3(flag/3, flag/3, 1), .5f);
		}
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
		
		 if (SpriteAtlas.GetSprite(spriteName) == null) {
            Debug.LogWarning("Sprite " + "\"" + spriteName + "\" " + "not found in atlas " + "\"" + SpriteAtlas + "\"" + ". Using default sprite, \"circle\".");	
        }
        // Calculate sprite atlas coordinates
        CalculateSprite(SpriteAtlas, spriteName);
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
        else if (widthHeightRatio < 1)
            UVWidth = 1f * widthHeightRatio;        // It's a "tall" sprite
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
