using UnityEngine;
using System.Collections;
/// <summary>
/// EnemyScript.cs
/// 
/// Main class for any enemy instance. Enemy types inherit from this class.
/// </summary>
public class EnemyScript : MonoBehaviour {

    #region Fields
    public GameObject ExplosionPrefab;  // Inspector reference. Location: Enemy[Type]Prefab.
    public UIAtlas SpriteAtlas;         // Inspector reference. Location: Enemy[Type]Prefab.
	public GameObject PulsePrefab;		// Inspector reference. Location: Enemy[Type]Prefab.

    // Protected values with access from its descendants
    protected float minSpeed;           // The minimum random speed of the enemy
    protected float maxSpeed;           // The maximum random speed of the enemy
    protected int health;               // The health (nr of hits) of the enemy
    protected Color[] colors = new Color[] { Color.red, Color.green, Color.cyan, Color.blue, Color.yellow, Level.purple };
    protected int colorIndex;         // The index in the colors list, defines what color the enemy will be.
    //protected Color[] colors;

    // Sprite parameters for Atlas-lookup
    protected string spriteName = "default";
    protected int left;
    protected int bottom;
    protected int width;
    protected int height;
    protected float UVHeight = 1f;
    protected float UVWidth = 1f;

    private Color mainColor;            // The color of the enemy
    protected float currentSpeed = 10;         // The speed of the enemy
    private float x, y, z;              // Position coordinates of the enemy
    private int fixPos;                 // Random value for moving the enemy off the screen

    public static int energyReturn = 2;			// The amount of energy to return to the player when an enemy dies.
	#endregion

    #region Functions
	
    protected virtual void Awake() {
		SetPositionAndSpeed();
        if (Level.fourColors) colors = new Color[] { Color.red, Color.cyan, Color.blue, Color.yellow };
   
    }

    protected virtual void Start() {
        // Start moving towards the player
		SetColor();
        iTween.MoveTo(gameObject, iTween.Hash("position", Vector3.zero,
                                              "speed", currentSpeed,
                                              "easetype", "linear"));
    }

    // Triggered when the enemy collides with something
    void OnTriggerEnter(Collider otherObject) {
        // If the enemy collides with the player, reduce health of player, destroy the enemy.
        if (otherObject.tag == "Player") {
			if(Game.PowerupActive != Game.Powerups.Invincible) 
            	Player.health -= 10 * health;       // Reduces the player health by 10 * the remaining enemy health
            CreateExplosion();
            Destroy(gameObject);
        }
        // If the enemy collides with a pulse of the right color, reduce enemy health, increase score
        if (otherObject.name == "Pulse(Clone)") {
            if (otherObject.gameObject.GetComponent<LineRenderer>().material.color == MainColor || Game.PowerupActive == Game.Powerups.MassivePulse) {
				if(Game.PowerupActive != Game.Powerups.MassivePulse)
                	Player.score += 10;
                CreateExplosion();
                DamageEnemy();
				
				if(Game.PowerupActive == Game.Powerups.ChainReaction) {
	                GameObject pulse = (GameObject) Instantiate(PulsePrefab, gameObject.transform.position, Quaternion.identity);
					pulse.GetComponent<PulseSender>().SetFinalColor(mainColor);
				}

				
            } else {
                gameObject.GetComponent<ParticleSystem>().Emit(10);
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
		transform.position = new Vector3(x,y,z);
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
        gameObject.GetComponent<ParticleSystem>().startColor = MainColor;
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
    public void DamageEnemy() {
        health--;
        if (health < 1) {
			if(Game.PowerupActive != Game.Powerups.MassivePulse) {
	            Player.energy += energyReturn;            // Return a bit of energy when the enemy is killed
	            if (Player.energy > Player.maxEnergy)     // Make sure energy is never more than maxEnergy
	                Player.energy = Player.maxEnergy;
			}
            Destroy(gameObject);
        }
    }

    public void CreateExplosion() {
        Instantiate(ExplosionPrefab, gameObject.transform.position, gameObject.transform.rotation);
    }

    #endregion
}
