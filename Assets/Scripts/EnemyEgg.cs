using UnityEngine;
/// <summary>
/// EnemyEgg.cs
/// 
/// Class for enemy type: Egg. Inherits from EnemyScript.
/// </summary>
public class EnemyEgg : EnemyScript {
    #region Fields
    private int colorIndex;         // The index in the colors list, defines what color the enemy will be.

    private LinkedSpriteManager spriteManager;
    private Sprite enemyEgg;
    #endregion

    #region Functions
    /// <summary>
    /// Overriding Start() to set unique values for this enemy type
    /// </summary>
    protected override void Start() {
        colorIndex = Random.Range(0, colors.Length);    // Defines the colour
        MainColor = colors[colorIndex];                 // Sets the colour
        minSpeed = 3f;                                  // Sets minimum speed
        maxSpeed = 3.5f;                                // Sets maximum speed
        health = 1;                                     // Sets health
        spriteManager = GameObject.Find("EnemySpawner").GetComponent<LinkedSpriteManager>();

        int left = 0;
        int bottom = 0;
        int width = 100;
        int height = 100;

        if (MainColor == Color.green) {
            left   = 267;
            bottom = 756;
            width  = 253;
            height = 245;
        }

        else if (MainColor == Color.red) {
            left = 521;
            bottom = 757;
            width = 263;
            height = 180;
        }

        else if (MainColor == Level.purple) {
            left = 532;
            bottom = 1023;
            width = 264;
            height = 264;
        }

        else if (MainColor == Color.cyan) {
            left = 0;
            bottom = 495;
            width = 263;
            height = 265;
        }

        else if (MainColor == Color.yellow || MainColor == Color.blue) {
            left = 261;
            bottom = 503;
            width = 110;
            height = 100;
        }

        enemyEgg = spriteManager.AddSprite(gameObject, 1f, 1f, left, bottom, width, height, false);

        if (MainColor == Color.yellow || MainColor == Color.blue)
            enemyEgg.SetColor(MainColor);

        base.Start();                                   // Initialises the enemy by calling the Start() of EnemyScript
    }

    void OnDestroy() {
        if (enemyEgg != null)
            spriteManager.RemoveSprite(enemyEgg);
    }
    #endregion
}