using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    #region Fields
    public static int startScore = 0;
    public static int startHealth = 100;
    public static int score;
    public static int health;
    public static int startEnergy = 50;
    public static int energy;

    public GameObject pulsePrefab;

    public float screenLeft;
    public float screenBottom;
	
	private float myTimer = 0;

    #endregion

    #region Properties

    #endregion

    #region Functions

    void Awake()
    {
        screenLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10)).x;
        screenBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10)).y;
        ResetStats();
    }

    void Update()
    {		
        if (Input.GetMouseButtonDown(0) && energy-10 >= 0 && Time.timeScale != 0)
        {
            Instantiate(pulsePrefab, new Vector3(0, 0, 0), pulsePrefab.transform.localRotation);
            energy -= 10;
        }
		
		myTimer += Time.deltaTime;
		if(myTimer > 2) {
			energy++;
			myTimer = 0;
		}
		
		
		
    }

    public void ResetStats()
    {
        score = startScore;
        health = startHealth;
        energy = startEnergy;
    }

    #endregion
}
