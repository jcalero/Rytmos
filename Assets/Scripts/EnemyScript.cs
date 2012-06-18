using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour
{

    #region Fields
    public float MinSpeed;
    public float MaxSpeed;
    public GameObject ExplosionPrefab;
    public Color mainColor;

    public int colorIndex;

    private float CurrentSpeed;
    private float x, y, z;
    private float fixX, fixY;
    private GameObject player;

    private Color[] colors = new Color[]{Color.red, Color.green, Color.cyan, Color.blue, Color.yellow, new Color(.5f, 0f, .5f, 1f)};

    // Screen edges
    private float left;
    private float right;
    private float top;
    private float bottom;

    #endregion

    #region Functions

    void Awake()
    {
        left = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10)).x;
        right = -left;
        bottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10)).y;
        top = -bottom;
		
    }

    // Use this for initialization
	void Start () {
        SetPositionAndSpeed();
        SetColor();
        player = GameObject.Find("Player");
        iTween.MoveTo(gameObject, iTween.Hash("position", player.transform.position ,"speed", CurrentSpeed, "easetype", "linear", "looktarget", player.transform.position));
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    void OnTriggerEnter(Collider otherObject)
    {
        if (otherObject.tag == "Player")
        {
            Player.health -= 10;
            Instantiate(ExplosionPrefab, gameObject.transform.position, gameObject.transform.rotation);
            Destroy(gameObject);

            if (Player.health <= 0) 
                Application.LoadLevel("Lose");
        }

        if (otherObject.name == "Pulse(Clone)" &&
            otherObject.gameObject.GetComponent<LineRenderer>().material.color == gameObject.renderer.material.color)
        {
            Player.score += 10;
			Player.energy += 5;
            Instantiate(ExplosionPrefab, gameObject.transform.position, gameObject.transform.rotation);
            Destroy(gameObject);

            if (Player.score >= 100)
                Application.LoadLevel("Win");
        }
		else 
		{
			gameObject.GetComponent<ParticleSystem>().Emit(10);
		}
		
    }

    void SetPositionAndSpeed() {
        CurrentSpeed = Random.Range(MinSpeed, MaxSpeed);

        x = Random.Range(left, right);
        y = Random.Range(top, bottom);
        z = transform.localRotation.z;

        fixX = Random.Range(1, -1);

        if (fixX <= 0)
            x = Mathf.Sign(x) * right;
        if (fixX > 0)
            y = Mathf.Sign(y) * top;

        transform.position = new Vector3(x, y, z);
        }

    void SetColor()
    {
        colorIndex = Random.Range(0, 5);
        gameObject.renderer.material.color = colors[colorIndex];
		gameObject.GetComponent<ParticleSystem>().startColor = colors[colorIndex];
    }

    #endregion
}
