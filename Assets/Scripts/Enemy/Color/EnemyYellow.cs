using UnityEngine;
/// <summary>
/// EnemyYellow.cs
/// 
/// Class for enemy type: Yellow Coloured. Inherits from EnemyScript.
/// </summary>
public class EnemyYellow : EnemyScript {
    #region Fields
    private LinkedSpriteManager spriteManager;
    private Sprite enemyCircle;
    #endregion

    #region Functions
    protected override void Awake() {
        minSpeed = 0.4f;                                // Sets minimum speed
        maxSpeed = 0.8f;                                // Sets maximum speed
        health = 1;                                     // Sets health
        base.Awake();
    }

    /// <summary>
    /// Overriding Start() to set unique values for this enemy type
    /// </summary>
    protected override void Start() {
        spriteManager = GameObject.Find("EnemySpawner").GetComponent<LinkedSpriteManager>();
        MainColor = Color.yellow;
        spriteName = "5-yellow";

        // Checks that the sprite name exists in the atlas, if not falls back to default sprite
        if (SpriteAtlas.GetSprite(spriteName) == null) {
            Debug.LogWarning("Sprite " + "\"" + spriteName + "\" " + "not found in atlas " + "\"" + SpriteAtlas + "\"" + ". Using default sprite, \"circle\".");
            spriteName = "circle";
			
        }
        // Calculate sprite atlas coordinates
        base.CalculateSprite(SpriteAtlas, spriteName);
        // Add sprite to game object
        enemyCircle = spriteManager.AddSprite(gameObject, UVWidth, UVHeight, left, bottom, width, height, false);

        if (spriteName == "default" || spriteName == "circle")
            enemyCircle.SetColor(MainColor);

        base.Start();                                   // Initialises the enemy by calling the Start() of EnemyScript
    }

    void OnDestroy() {
        if (enemyCircle != null)
            spriteManager.RemoveSprite(enemyCircle);
    }
    #endregion
}