using UnityEngine;
/// <summary>
/// EnemyCircle.cs
/// 
/// Class for enemy type: Circle. Inherits from EnemyScript.
/// </summary>
public class EnemyCircle : EnemyScript {
	#region Fields
	#endregion

	#region Functions
	protected override void Awake() {
		minSpeed = 0.4f;                                // Sets minimum speed
		maxSpeed = 0.8f;                                // Sets maximum speed
		health = 2;                                     // Sets health
		base.Awake();
	}

	/// <summary>
	/// Overriding Start() to set unique values for this enemy type
	/// </summary>
	protected override void Start() {
		spriteManager = GameObject.Find("EnemySpawner").GetComponent<LinkedSpriteManager>();

		// Choose what sprite to show
		if (MainColor == Color.green) {
			spriteName = "8-green";
		} else if (MainColor == Color.red) {
			spriteName = "12-red";
		} else if (MainColor == Level.purple) {
			spriteName = "4-purple";
		} else if (MainColor == Color.cyan) {
			spriteName = "1-cyan";
		} else if (MainColor == Color.blue) {
			spriteName = "7-blue";
		} else if (MainColor == Color.yellow) {
			spriteName = "5-yellow";
		}

		// Checks that the sprite name exists in the atlas, if not falls back to default sprite
		if (SpriteAtlas.GetSprite(spriteName) == null) {
			Debug.LogWarning("Sprite " + "\"" + spriteName + "\" " + "not found in atlas " + "\"" + SpriteAtlas + "\"" + ". Using default sprite, \"circle\".");
			spriteName = "circle";
		}
		// Calculate sprite atlas coordinates
		base.CalculateSprite(SpriteAtlas, spriteName);
		// Add sprite to game object
		enemyCircle = spriteManager.AddSprite(gameObject, UVWidth, UVHeight, left, bottom, width, height, false);

		if (spriteName == "default")
			enemyCircle.SetColor(MainColor);

		base.Start();                                   // Initialises the enemy by calling the Start() of EnemyScript
	}

	void OnDestroy() {
		//if (enemyCircle != null)
		//    spriteManager.RemoveSprite(enemyCircle);
	}
	#endregion
}