using UnityEngine;
using System.Collections;
/// <summary>
/// PulseSender.cs
/// 
/// Handles the pulse animation, color and events.
/// </summary>
public class PulseSender : MonoBehaviour {
	public float Radius;                            // The radius of he pulse
	public float CurrentHealth;                     // Current health of the pulse
	public Color CurrentColor;						// Currently selected color (discrete values)
	public Color SecondaryColor;					// Used for the boundaries between discrete color selection to determine a hit
	public float MaxHealth = 6;                     // Max health of the pulse
	Camera SuperPulseCamera;

	private int segments = 80;                      // The nr of segments the pulse has. Fewer means less "smooth".
	private LineRenderer line;                      // The line renderer that creates the pulse
	private SphereCollider sphereColl;              // The collider attatched to the pulse
	public bool held;                              // Flag for whether the player is keeping his pulse "active"
	private Color finalColor = Color.clear;         // The final color of the pulse once the player releases his finger
	private float pulseMax;                         // Maximum range of the pulse
	float flashTimer = 1f;
	float flashTimerMax = 1f;
	bool isFlashingScreen;

	void Awake() {
		SuperPulseCamera = GameObject.Find("3DCamera").camera;
	}

	void Start() {
		held = true;
		Radius = .4f;
		line = gameObject.GetComponent<LineRenderer>();
		line.SetVertexCount(segments + 1);
		line.useWorldSpace = false;
		line.material.color = finalColor;
		if(Game.GameMode == Game.Mode.Casual)
			CurrentHealth = EnemySpawnScript.spawnerCounter;
		else
			CurrentHealth = EnemySpawnScript.spawnerCounter+1;
		CurrentColor = Color.clear;
		float lineWidth = CurrentHealth / 10;
		line.SetWidth(lineWidth, lineWidth);

		sphereColl = gameObject.GetComponent<SphereCollider>();

		//Find distance for the maximum radius
		pulseMax = new Vector2(Game.screenLeft, Game.screenTop).magnitude;
		gameObject.audio.volume = Game.EffectsVolume;

	}

	void Update() {
		if(gameObject.transform.position!= new Vector3(0,0,0)) ChainPulse ();
		else MainPulse ();
	}
	
	void ChainPulse() {
		Radius += 3 * Time.deltaTime;
		RedrawPoints(finalColor);
		line.material.color = finalColor;
		sphereColl.radius = Radius + 0.1f;

		//If too big, destroy itself
		if (Radius > 2f || CurrentHealth == 0)
			Destroy(gameObject);
	}
	
	void MainPulse() {
		Radius = Radius + 3 * Time.deltaTime;
		//If you have released the button, and the pulse is the current one, set it to be not held and set the Colour
		if (Input.GetMouseButtonUp(0) && held) {
			held = false;
			#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
			finalColor = Level.chunkyColorSelect(Input.GetTouch(0).position);
			CurrentColor = Level.singleColourSelect(Input.GetTouch(0).position);
			#else
			finalColor = Level.chunkyColorSelect(Input.mousePosition);
			CurrentColor = Level.singleColourSelect(Input.mousePosition);
			#endif
			if(Game.PowerupActive==Game.Powerups.MassivePulse) {
				finalColor = Color.white;
				CurrentColor = Color.white;
			}
		}

		//What the colour should be - this is where the transition has to take place. 
		Color chosen;
		if (held) {
			#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
			if(Game.PowerupActive==Game.Powerups.MassivePulse) {
				chosen = Color.white;
				CurrentColor = Color.white;
			} else {
				chosen = Level.chunkyColorSelect(Input.GetTouch(0).position);
				CurrentColor = Level.singleColourSelect(Input.GetTouch(0).position);
			}
			#else
			chosen = Level.chunkyColorSelect(Input.mousePosition);
			CurrentColor = Level.singleColourSelect(Input.mousePosition);
			if(Game.PowerupActive==Game.Powerups.MassivePulse) {
				chosen = Color.white;
				CurrentColor = Color.white;
			}
			#endif
			
		} else 
			chosen = finalColor;
		
		//Super pulse effect
		if (Game.PowerupActive == Game.Powerups.MassivePulse && !isFlashingScreen) StartCoroutine(FlashScreen());
		
		//Select the secondary color
		#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
		if(held) SecondaryColor = Level.secondaryColourSelect(Input.GetTouch(0).position);
		#else
		if(held) SecondaryColor = Level.secondaryColourSelect(Input.mousePosition);
		#endif
		
		//Create the circle, and set the line material
		RedrawPoints(chosen);
		line.material.color = chosen;
		sphereColl.radius = Radius + 0.1f;

		//If too big, destroy itself
		if (Radius > pulseMax || (Radius < .3f) || CurrentHealth == 0) {
			SuperPulseCamera.backgroundColor = Color.black;
			Destroy(gameObject);
		}
	}

	IEnumerator FlashScreen() {
		isFlashingScreen = true;
		if (flashTimer > 0) {
			//Debug.Log("Increasing camera color");
			if (flashTimer < (flashTimerMax / 2)) {
				SuperPulseCamera.backgroundColor = new Color(SuperPulseCamera.backgroundColor.r - (0.3f * Time.deltaTime),
					SuperPulseCamera.backgroundColor.g - (0.3f * Time.deltaTime),
					SuperPulseCamera.backgroundColor.b - (0.3f * Time.deltaTime));
				flashTimer -= Time.deltaTime;
			} else {
				SuperPulseCamera.backgroundColor = new Color(SuperPulseCamera.backgroundColor.r + (0.3f * Time.deltaTime),
					SuperPulseCamera.backgroundColor.g + (0.3f * Time.deltaTime),
					SuperPulseCamera.backgroundColor.b + (0.3f * Time.deltaTime));
				flashTimer -= Time.deltaTime;
			}
		}
		if (flashTimer <= 0) {
			flashTimer = flashTimerMax;
		}
		yield return 0;
	}

	void OnTriggerExit(Collider otherObject) {
		if (otherObject.GetType() == typeof(BoxCollider) && Game.PowerupActive != Game.Powerups.MassivePulse) {
			if(!(Game.GameMode == Game.Mode.Casual)) CurrentHealth--;
			if (CurrentHealth == 0)
				Destroy(gameObject);
		}
	}

	// Reduce pulse health if it collides with another object
	void OnTriggerEnter(Collider otherObject) {
		if (otherObject.GetType() == typeof(SphereCollider) && CurrentColor != Color.white) {
			if(Game.GameMode != Game.Mode.Casual && Game.GameMode != Game.Mode.Tutorial) CurrentHealth--;
			else if(otherObject.gameObject.GetComponent<EnemyScript>().MainColor == CurrentColor || 
					otherObject.gameObject.GetComponent<EnemyScript>().MainColor == SecondaryColor)
				CurrentHealth--;
			if (CurrentHealth == 0) {
				Destroy(gameObject);
			}
		}
	}
	
	public void SetFinalColor(Color c) {
		finalColor = c;
		CurrentColor = c;
		SecondaryColor = c;
		CurrentHealth = 1;
	}
	
	/// <summary>
	///  Recalculates point positions
	/// </summary>
	/// <param name="c">Color of the points/line</param>
	void RedrawPoints(Color c) {
		float x;
		float y;
		float z = 0f;
		float angle = 0f;

		for (int i = 0; i < (segments + 1); i++) {
			x = Mathf.Sin(Mathf.Deg2Rad * angle);
			y = Mathf.Cos(Mathf.Deg2Rad * angle);
			line.SetPosition(i, new Vector3(x, y, z) * Radius);
			line.SetColors(new Color(c.r, c.g, c.b, ((CurrentHealth / MaxHealth) * .3f) + .3f), new Color(c.r, c.g, c.b, ((CurrentHealth / MaxHealth) * .3f) + .3f));
			line.material.SetColor("_Emission", new Color(c.r, c.g, c.b, c.a / 3));
			float lineWidth = (CurrentHealth / 10) + 0.5f;
			if (lineWidth < .2f)
				lineWidth += .05f;

			line.SetWidth(lineWidth, lineWidth);
			angle += (720f / segments);
		}
	}
}