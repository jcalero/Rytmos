using UnityEngine;
using System.Collections;
/// <summary>
/// EnemyScript.cs
/// 
/// Main class for any enemy instance. Enemy types inherit from this class.
/// </summary>
public class EnemyScript : MonoBehaviour,PeakListener {

	#region Fields
	//public GameObject ExplosionPrefab;  // Inspector reference. Location: Enemy[Type]Prefab.
	public GameObject CollisionParticles;
	public GameObject ExplosionParticles;
	public ParticleSystem TrailParticles;
	public GameObject PulsePrefab;		// Inspector reference. Location: Enemy[Type]Prefab.
	public GameObject SecondPulseColl;		// Inspector reference. Location: Enemy[Type]Prefab.
	public LinkedSpriteManager spriteManager;

	// Protected values with access from its descendants
	protected UIAtlas SpriteAtlas;
	//protected LinkedSpriteManager spriteManager;
	protected Sprite enemyCircle;
	protected float minSpeed;           // The minimum random speed of the enemy
	protected float maxSpeed;           // The maximum random speed of the enemy
	protected int health;               // The health (nr of hits) of the enemy
	protected Color[] colors = new Color[] { Color.red, Color.green, Color.cyan, Color.blue, Color.yellow, Level.purple };
	protected int colorIndex;         // The index in the colors list, defines what color the enemy will be.
	//protected Color[] colors;

	// Sprite parameters for Atlas-lookup
	protected string spriteName = "Yellow";
	protected int left;
	protected int bottom;
	protected int width;
	protected int height;
	protected float UVHeight = 1f;
	protected float UVWidth = 1f;

	protected float loudFlag;

	private bool givenScore;			// Has the enemy given its score upon death?
	private bool givenDespawn;			// Make the enemy only increment the despawnCount once 
	private bool spawnInvincible;		//Make the enemy spawn invincible, so that it lasts for a bit
	private Color mainColor;            // The color of the enemy
	protected float currentSpeed = 10;  // The speed of the enemy
	private float x, y, z;              // Position coordinates of the enemy
	private int fixPos;                 // Random value for moving the enemy off the screen
	private float baseSpeed;			// Base speed on the enemy, varies depending on the game mode.

	public static int energyReturn = 2;			// The amount of energy to return to the player when an enemy dies.

	#endregion

	#region Functions

	protected virtual void Awake() {
		SpriteAtlas = EnemySpawnScript.EnemyAtlas;
		
		loudFlag = 0;
		spawnInvincible = true;
		givenDespawn = false;
		givenScore = false;
		spriteManager = GameObject.Find("GameAtlas").GetComponent<LinkedSpriteManager>();
		SetPositionAndSpeed();
		if (Level.fourColors) colors = new Color[] { Color.red, Color.cyan, Color.blue, Color.yellow };
		if(Game.GameMode == Game.Mode.Casual) baseSpeed = 1.5f;
		else baseSpeed = 2.5f;

	}

	protected virtual void Start() {
		// Start moving towards the player
		PeakTriggerManager.addSelfToListenerList(this);
		SetColor();
		iTween.MoveTo(gameObject, iTween.Hash("position", Vector3.zero,
											  "speed", currentSpeed,
											  "easetype", "linear"));	
	}
	
	protected virtual void Update() {
		if(spawnInvincible) 
			spawnInvincible = false;		
	}

	public void ChangeColor(Color c) {
		enemyCircle.SetColor(c);
	}

	public Color GetColor() {
		return enemyCircle.color;
	}
	
	private float calcSpeed() {

		// Loudflag between 0 and 5, median is 2.5
		// variationFactor between 0.2 & 1
		float speed = baseSpeed + (loudFlag - 2.5f)*AudioManager.variationFactor;
		if(speed < 1) speed = 1f;
		else if(speed > 4.5 && Game.GameMode != Game.Mode.Casual) speed = 4.5f;
		else if(speed > 3.5 && Game.GameMode == Game.Mode.Casual) speed = 3.5f;
	
		return speed;
		
	}
	
	public void onPeakTrigger(int channel,int intensity) {}
	
	public void setLoudFlag(int flag) {
		loudFlag = flag;
		currentSpeed = calcSpeed();
		iTween.MoveTo(gameObject, iTween.Hash("position", Vector3.zero,
								  "speed", currentSpeed,
								  "easetype", "linear"));
	}

	// Triggered when the enemy collides with something
	void OnTriggerEnter(Collider otherObject) {
		// If the enemy collides with the player, reduce health of player, destroy the enemy.
		if (otherObject.tag == "Player") {
			if (Game.PowerupActive != Game.Powerups.Invincible) {
				//				Player.health -= 10 * health;       // Reduces the player health by 10 * the remaining enemy health
				Player.ReduceMultiplier();
				//Debug.Log(Level.cameraShakeTimer);
				//if (Level.cameraShakeTimer == 0) {
				Camera.mainCamera.animation.Play("CameraShake");
				//    StartCoroutine(Level.InitiateCameraShakeTimer());
				//}

			} else {
				Player.shieldFlash = true;
			}
			if(!givenDespawn) {
				givenDespawn = true;
				Player.playGetHitSound();
				StartCoroutine(DamageEnemy(true));
			}
		}
		// If the enemy collides with a pulse of the right color, reduce enemy health, increase score
		if ((otherObject.name == "Pulse(Clone)" || otherObject.name == "SuperPulse(Clone)" )&& !spawnInvincible) {
			if (otherObject.gameObject.GetComponent<PulseSender>().CurrentColor == MainColor ||
				otherObject.gameObject.GetComponent<PulseSender>().SecondaryColor == MainColor ||
				otherObject.gameObject.GetComponent<PulseSender>().CurrentColor == Color.white) {
				
				if(!givenDespawn) {
					givenDespawn = true;
					StartCoroutine(DamageEnemy(false));
				}

				if (Game.PowerupActive == Game.Powerups.ChainReaction) {
					GameObject pulse = (GameObject)Instantiate(PulsePrefab, gameObject.transform.position, Quaternion.identity);
					pulse.GetComponent<PulseSender>().SetFinalColor(mainColor);
				}

			} else {
				//CollisionParticles.GetComponent<ParticleSystem>().Emit(10);
			}
		}
	}

	/// <summary>
	/// Sets the initial position and speed of the enemy. Clamps the position to a location outside the screen.
	/// </summary>
	protected void SetPositionAndSpeed() {
		// Sets the speed of the enemy between minSpeed and max
		//currentSpeed = Random.Range(minSpeed, maxSpeed);

		// Sets an initial position of the enemies to somewhere inside the game area
		x = Random.Range(Game.screenLeft, Game.screenRight);
		y = Random.Range(Game.screenTop, Game.screenBottom);
		z = transform.localRotation.z;      // z is just the original z coordinate, probably 0.

		fixPos = Random.Range(1, -1);       // Sets fixPos to either 1 or 0 randomly

		// If fixPos is 0 then move the enemy outside the screen along the x axis
		// If fixPos is 1 move it along the y axis.
		// This "clamps" the location of the enemy to somewhere just outside the screen
		// but randomly between the four sides.
		if (fixPos == 0)
			x = Mathf.Sign(x) * Game.screenRight;
		if (fixPos == 1)
			y = Mathf.Sign(y) * Game.screenTop;

		// Position the enemy on it's final position.
		transform.position = new Vector3(x, y, z);
		Rotation();

	}

	public void SetPositionAndSpeed(float speed, float xpos, float ypos) {
		currentSpeed = speed;
		x = xpos;
		y = ypos;
		z = transform.localRotation.z;
		transform.position = new Vector3(x, y, z);
		Rotation();
	}

	private void Rotation() {
		float angle = Mathf.Atan2(gameObject.transform.position.y, gameObject.transform.position.x);
		gameObject.GetComponentInChildren<Transform>().localEulerAngles = new Vector3(0f, 0f, Mathf.Rad2Deg * angle + 90);
	}

	/// <summary>
	/// Setter and Getter for the main colour of the enemy
	/// </summary>
	/// <value>The main colour of the enemy</value>
	public Color MainColor {
		set { mainColor = value; }
		get { return mainColor; }
	}

	/// <summary>
	/// Sets the color of the material of the enemy to MainColor.
	/// </summary>
	protected void SetColor() {
		CollisionParticles.GetComponent<ParticleSystem>().startColor = MainColor;
	}

	/// <summary>
	/// Calculates the corresponding sprite on an atlas given the sprite name.
	/// </summary>
	/// <param name="atlas">The atlas the sprite belongs to</param>
	/// <param name="name">The name of the sprite</param>
	protected void CalculateSprite(UIAtlas atlas, string name) {
		UIAtlas.Sprite sprite = atlas.GetSprite(name);
		if (sprite == null) {
			Debug.LogError("No sprite with that name: " + name);
			return;
		}
		left = (int)sprite.inner.xMin;
		bottom = (int)sprite.inner.yMax;
		width = (int)sprite.inner.width;
		height = (int)sprite.inner.height;

		float widthHeightRatio = sprite.inner.width / sprite.inner.height;
		if (widthHeightRatio > 1)
			UVHeight = 1f / widthHeightRatio;       // It's a "wide" sprite
		else if (widthHeightRatio < 1)
			UVWidth = 1f * widthHeightRatio;        // It's a "tall" sprite
	}

	/// <summary>
	/// Reduces the health of the enemy, destroys it if low on health and gives energy to the player
	/// </summary>
	public IEnumerator DamageEnemy(bool isPlayer) {
		health--;
		if (health < 1) {
			Level.EnemiesDespawned++;
			PeakTriggerManager.removeSelfFromListenerList(this);
			if (Game.PowerupActive != Game.Powerups.MassivePulse) {
				Player.energy += energyReturn;            // Return a bit of energy when the enemy is killed
				if (Player.energy > Player.maxEnergy)     // Make sure energy is never more than maxEnergy
					Player.energy = Player.maxEnergy;
			}
			if (!givenScore && !isPlayer) {
				//Player.score += 10 * Player.multiplier;
				Vector2 floatScorePos = Camera.mainCamera.WorldToViewportPoint(transform.position);
				UILabel floatLabel = FloatingScorePool.Spawn(HUD.Camera.ViewportToWorldPoint(floatScorePos));
				floatLabel.text = "[4499DD]+" + Player.IncrementScore();
				floatLabel.animation.Play();
				Player.KillStreakCounter++;
				Player.TotalKills++;

				//FloatingScorePool.FloatScoreLabels[0].transform.position = HUD.Camera.ViewportToWorldPoint(floatScorePos);
				//Player.IncrementScore();
				givenScore = true;
			}
			CreateExplosion();

			iTween.Stop(gameObject);
			collider.enabled = false;
			SecondPulseColl.collider.enabled = false;
			TrailParticles.Stop();
			spriteManager.HideSprite(enemyCircle);
			yield return new WaitForSeconds(1f);
			Destroy(gameObject);
		}
	}

	public void CreateExplosion() {
		if(gameObject.audio != null) {
			gameObject.audio.volume = Game.EffectsVolume;
			gameObject.audio.Play();
		}
		ExplosionParticles.GetComponent<ParticleSystem>().Emit(70);
	}

	#endregion
}
