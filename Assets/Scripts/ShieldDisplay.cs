using UnityEngine;
using System.Collections;

public class ShieldDisplay : MonoBehaviour {
	public UIAtlas SpriteAtlas;
	public GameObject smManager;
	public float intensity;
	
	private LinkedSpriteManager sm;
	private Sprite shield;
	private string spriteName = "default";
    private int left;
    private int bottom;
    private int width;
    private int height;
    private float UVHeight = 1f;
    private float UVWidth = 1f;
	private bool first;
	
	// Use this for initialization
	void Start () {
		first = true;
		sm = smManager.GetComponent<LinkedSpriteManager>();
		updateSprite("ForceField");
		gameObject.transform.localScale = new Vector3(2.3f,2.3f,1);
	}
	
	// Update is called once per frame
	void Update () {
		if(Game.PowerupActive == Game.Powerups.Invincible) {
			if(first) {
				intensity = .3f;
				first = false;
			}
			if(Player.shieldFlash) {
				if(gameObject.GetComponent<Animation>().isPlaying) 
					gameObject.GetComponent<Animation>().Stop();
				gameObject.GetComponent<Animation>().Play("ShieldFlash");
				Player.shieldFlash = false;			
			} 
			
		} else {
			intensity = 0f;
			first = true;
		}
		
		shield.SetColor (new Color(shield.color.r, shield.color.g, shield.color.b, intensity));
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
	
	public void updateSprite(string name) {
		spriteName = name;
		
		 if (SpriteAtlas.GetSprite(spriteName) == null) {
            Debug.LogWarning("Sprite " + "\"" + spriteName + "\" " + "not found in atlas " + "\"" + SpriteAtlas + "\"" + ". Using default sprite, \"circle\".");	
        }
        // Calculate sprite atlas coordinates
        CalculateSprite(SpriteAtlas, spriteName);
        // Add sprite to game object
        shield = sm.AddSprite(gameObject, UVWidth, UVHeight, left, bottom, width, height, false);
	}
}
