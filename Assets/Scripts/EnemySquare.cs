using UnityEngine;
/// <summary>
/// EnemySquare.cs
/// 
/// Class for enemy type: Square. Inherits from EnemyScript.
/// </summary>
public class EnemySquare : EnemyScript {
    #region Fields
    private int colorIndex;         // The index in the colors list, defines what color the enemy will be.
    #endregion

    #region Functions
    /// <summary>
    /// Overriding Start() to set unique values for this enemy type
    /// </summary>
    protected override void Start() {
        colorIndex = Random.Range(0, colors.Length);    // Defines the colour
        MainColor = colors[colorIndex];                 // Sets the colour
        minSpeed = 0.5f;                                // Sets minimum speed
        maxSpeed = 2.5f;                                // Sets maximum speed
        health = 1;                                     // Sets health
        base.Start();                                   // Initialises the enemy by calling the Start() of EnemyScript
    }
    #endregion
}