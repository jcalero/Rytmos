using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    #region Fields
    public static int startScore = 0;
    public static int startHealth = 100;
    public static int score;
    public static int startEnergy = 50;
	public static int maxEnergy = 50;
    
    public static int energy;
    public static int health;

    public GameObject pulsePrefab;
	public GameObject colorFeedback;

    public float screenLeft;
    public float screenBottom;
	private float screenTop;
	private float screenRight;
	private float screenMiddleX;
	
	private float myTimer = 0;

    #endregion

    #region Functions

    void Awake()
    {
        screenLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10)).x;
        screenBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10)).y;
		screenRight = Camera.main.ViewportToWorldPoint(new Vector3(1,0,10)).x;
		screenTop = Camera.main.ViewportToWorldPoint(new Vector3(0,1,10)).y;
		screenMiddleX = Camera.main.ViewportToWorldPoint(new Vector3(.5f, 0, 10)).x;
        ResetStats();
    }
	
	void Start() {
		colorFeedback.GetComponent<ParticleSystem>().startColor = Color.green;
		Instantiate(colorFeedback, new Vector3(screenLeft, screenBottom, 0), colorFeedback.transform.localRotation);
		colorFeedback.GetComponent<ParticleSystem>().startColor = Color.cyan;
		Instantiate(colorFeedback, new Vector3(screenMiddleX, screenBottom, 0), colorFeedback.transform.localRotation);
		colorFeedback.GetComponent<ParticleSystem>().startColor = Color.blue;
		Instantiate(colorFeedback, new Vector3(screenRight, screenBottom, 0), colorFeedback.transform.localRotation);
		colorFeedback.GetComponent<ParticleSystem>().startColor = Color.yellow;
		Instantiate(colorFeedback, new Vector3(screenLeft, screenTop, 0), colorFeedback.transform.localRotation);
		colorFeedback.GetComponent<ParticleSystem>().startColor = Color.red;
		Instantiate(colorFeedback, new Vector3(screenMiddleX, screenTop, 0), colorFeedback.transform.localRotation);
		colorFeedback.GetComponent<ParticleSystem>().startColor = new Color(.5f, 0, .5f, 1);
		Instantiate(colorFeedback, new Vector3(screenRight, screenTop, 0), colorFeedback.transform.localRotation);
	}

    void Update()
    {	
		//gameObject.renderer.material.color = singleColourSelect(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && energy-10 >= 0 && Time.timeScale != 0)
        {
            Instantiate(pulsePrefab, new Vector3(0, 0, 0), pulsePrefab.transform.localRotation);
            energy -= 10;
        }
		
		myTimer += Time.deltaTime;
		if(myTimer > 2 && energy < maxEnergy) {
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
	
	Color singleColourSelect(Vector2 xy) 
	{
		float normalizedX = xy.x - (Screen.width/2);
		float normalizedY = xy.y - (Screen.height/2);
		float angle = (Mathf.Rad2Deg * Mathf.Atan2(normalizedY, normalizedX));
		
		if(angle > 0) {
			if(angle < 60) {
				//Purple - top right.
				return new Color(.5f, 0f, .5f, 1f);	
			}
			else {
				if(angle > 120) {
					//Yellow - Top left
					return Color.yellow;
				}
				else {
					//Red - Top Middle
					return Color.red;
				}
			}
		}
		else {
			if(angle > -60) {
				//Blue - Bottom right
				return Color.blue;
			}
			else {
				if(angle < -120) {
					//Green - bottom left
					return Color.green;
				}
				else {
					//cyan - bottom middle
					return Color.cyan;
				}
			}
		}
	}

    #endregion
}
