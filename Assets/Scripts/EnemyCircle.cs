using UnityEngine;
/// <summary>
/// EnemyCircle.cs
/// 
/// Class for enemy type: Circle. Inherits from EnemyScript.
/// </summary>
public class EnemyCircle : EnemyScript {
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
        minSpeed = 0.4f;                                // Sets minimum speed
        maxSpeed = 0.8f;                                // Sets maximum speed
        health = 2;                                     // Sets health
        base.Start();                                   // Initialises the enemy by calling the Start() of EnemyScript
    }
    #endregion
}