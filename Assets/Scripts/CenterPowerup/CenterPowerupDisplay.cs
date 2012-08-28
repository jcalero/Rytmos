using UnityEngine;
using System.Collections;

public class CenterPowerupDisplay : MonoBehaviour {
	public GameObject smObject;
	public UIAtlas SpriteAtlas;
	public GameObject supercp;
	public GameObject chaincp;
	public GameObject shieldcp;
	
	private LinkedSpriteManager spriteManager;
    private int left;
    private int bottom;
    private int width;
    private int height;
    private float UVHeight = 1f;
    private float UVWidth = 1f;
	
	void Awake() {
		spriteManager = smObject.GetComponent<LinkedSpriteManager>();
		hideSprite();
		CalculateSprite(SpriteAtlas, "CirclesCenter");
		spriteManager.AddSprite(chaincp, UVWidth, UVHeight, left, bottom, width, height, false);
		CalculateSprite(SpriteAtlas, "AtomCenter");
		spriteManager.AddSprite(supercp, UVWidth, UVHeight, left, bottom, width, height, false);
		CalculateSprite(SpriteAtlas, "ShieldCenter");
		spriteManager.AddSprite(shieldcp, UVWidth, UVHeight, left, bottom, width, height, false);
	}
	
	void Update() {
		supercp.transform.localEulerAngles = new Vector3(0,0,supercp.transform.localEulerAngles.z+(Time.deltaTime*50f));
		chaincp.transform.localEulerAngles = new Vector3(0,0,chaincp.transform.localEulerAngles.z+(Time.deltaTime*50f));
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
