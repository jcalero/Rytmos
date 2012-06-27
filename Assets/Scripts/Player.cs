using UnityEngine;
using System.Collections;
/// <summary>
/// Player.cs
/// 
/// Player manager. Handles player values and control.
/// </summary>
public class Player : MonoBehaviour
{
    #region Fields
    public static int startScore = 0;           // Start score
    public static int startHealth = 100;        // Starting health of the player
    public static int maxHealth = 100;          // Maximum health of the player
    public static int startEnergy = 50;         // Starting energy of the player
    public static int maxEnergy = 50;           // Maximum energy of the player
    
    public static int energy;                   // Current energy of the player
    public static int health;                   // Current health of the player
    public static int score;                    // Current score of the player

    public GameObject pulsePrefab;              // The pulse. Inspector reference. Location: Player

    private float myTimer = 0;                  // Timer for energy regeneration
    private int pulseCost = 10;                 // Cost of a pulse
    private float energyRegenRate = 0.5f;       // The rate at which the energy regenerates. Lower is faster.
    #endregion

    #region Functions
    void Awake() {
        // Resets player stats at the start of a level
        ResetStats();
    }

    void Update() {
        if (!Game.Paused) {
            // If the player clicks, and has enough energy, sends out a pulse
            if (Input.GetMouseButtonDown(0) && energy - pulseCost >= 0) {
                // Calculates screen location based on mouse position
                Vector3 tempPos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0f);
                // Show the touch sprite at the mouse location.
                Level.ShowTouchSprite(tempPos);
                // Create a pulse
                Instantiate(pulsePrefab, new Vector3(0, 0, 0), pulsePrefab.transform.localRotation);
                // Reduce the player energy
                energy -= pulseCost;
            }

            // Regenerate energy. 1 energy every 2 seconds.
            myTimer += Time.deltaTime;
            if (myTimer > energyRegenRate && energy < maxEnergy) {
                energy++;
                myTimer = 0;
            }
        }
    }

    /// <summary>
    /// Reset the player stats to default values
    /// </summary>
    public void ResetStats() {
        score = startScore;
        health = startHealth;
        energy = startEnergy;
    }
    #endregion
}