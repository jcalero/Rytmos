using UnityEngine;
using System.Collections;

public class SuperCenterPW : CenterPowerupDisplay {
	// Use this for initialization
	void Start () {
		CalculateSprite(SpriteAtlas, "Atom");
		centerDisplay = spriteManager.AddSprite(gameObject, UVWidth, UVHeight, left, bottom, width, height, false);
	}
	
	// Update is called once per frame
	void Update () {
		transform.localEulerAngles = new Vector3(0,0,transform.localEulerAngles.z+(Time.deltaTime*50f));
	}
}
