using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    #region Fields
    public static int startScore = 0;
    public static int startHealth = 100;
    public static int startEnergy = 50;
    public static int maxEnergy = 50;
    
    public static int energy;
    public static int health;
    public static int score;

    public GameObject pulsePrefab;

    private float myTimer = 0;
    #endregion

    #region Functions
    void Awake() {
        ResetStats();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) && energy-10 >= 0 && Time.timeScale != 0) {
            Vector3 tempPos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0f);
            Level.ShowTouchSprite(tempPos);
            Instantiate(pulsePrefab, new Vector3(0, 0, 0), pulsePrefab.transform.localRotation);
            energy -= 10;
        }
        
        myTimer += Time.deltaTime;
        if(myTimer > 2 && energy < maxEnergy) {
            energy++;
            myTimer = 0;
        }

    }

    public void ResetStats() {
        score = startScore;
        health = startHealth;
        energy = startEnergy;
    }
    #endregion
}