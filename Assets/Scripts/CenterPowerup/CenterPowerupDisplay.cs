using UnityEngine;
using System.Collections;

public class CenterPowerupDisplay : MonoBehaviour {
	public GameObject smObject;
	public UIAtlas SpriteAtlas;
	public GameObject supercp;
	public GameObject chaincp;
	public GameObject shieldcp;
	
	protected LinkedSpriteManager spriteManager;
	protected Sprite centerDisplay;    
	protected string spriteName = "default";
    protected int left;
    protected int bottom;
    protected int width;
    protected int height;
    protected float UVHeight = 1f;
    protected float UVWidth = 1f;
	
	void Awake() {
		spriteManager = smObject.GetComponent<LinkedSpriteManager>();
		hideSprite();
	}
	
	public void changeSprite(GameObject self) {
		self.transform.localPosition = new Vector3(0,0,0);
	}
	
	public void hideSprite() {
		supercp.transform.localPosition = new Vector3(20,20,0);	
		shieldcp.transform.localPosition = new Vector3(20,20,0);
		chaincp.transform.localPosition = new Vector3(20,20,0);
	}
	
	public void changeSprite(Game.Powerups pw) {
		hideSprite();
		if(pw == Game.Powerups.MassivePulse) 
			changeSprite (supercp);
		 else if (pw == Game.Powerups.ChainReaction) 
			changeSprite(chaincp);
		 else if (pw == Game.Powerups.Invincible) 
			changeSprite (shieldcp);
		
	}
	
	protected void CalculateSprite(UIAtlas atlas, string name) {
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
}
