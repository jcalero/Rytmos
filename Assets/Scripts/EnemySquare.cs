using UnityEngine;
/// <summary>
/// EnemySquare.cs
/// 
/// Class for enemy type: Square. Inherits from EnemyScript.
/// </summary>
public class EnemySquare : EnemyScript {
    #region Fields
    private LinkedSpriteManager spriteManager;
    private Sprite enemySquare;
    #endregion

    #region Functions
    protected override void Awake() {
        minSpeed = 0.5f;                                // Sets minimum speed
        maxSpeed = 2.5f;                                // Sets maximum speed
        health = 1;                                     // Sets health
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
        enemySquare = spriteManager.AddSprite(gameObject, 1f, 1f, left, bottom, width, height, false);

        if (spriteName == "default")
            enemySquare.SetColor(MainColor);

        base.Start();                                   // Initialises the enemy by calling the Start() of EnemyScript
    }

    void OnDestroy() {
        if (enemySquare != null)
            spriteManager.RemoveSprite(enemySquare);
    }
    #endregion
}