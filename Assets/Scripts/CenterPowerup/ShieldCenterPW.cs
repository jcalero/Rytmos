using UnityEngine;
using System.Collections;

public class ShieldCenterPW : CenterPowerupDisplay {
	void Start () {		
		CalculateSprite(SpriteAtlas, "shield");
		centerDisplay = spriteManager.AddSprite(gameObject, UVWidth, UVHeight, left, bottom, width, height, false);
	}
}
