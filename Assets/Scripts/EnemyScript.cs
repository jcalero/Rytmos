using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour
{

    #region Fields
    public float MinSpeed;
    public float MaxSpeed;
    public GameObject ExplosionPrefab;


    //public int colorIndex;

    private Color mainColor;
    private float currentSpeed;
    private float x, y, z;
    private float fixX, fixY;
    private GameObject player;

    // Screen edges
    private float left;
    private float right;
    private float top;
    private float bottom;

    #endregion

    #region Functions

    protected virtual void Awake()
    {
        left = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10)).x;
        right = -left;
        bottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10)).y;
        top = -bottom;
        player = GameObject.Find("Player");
    }

    // Use this for initialization
    protected virtual void Start()
    {
        SetPositionAndSpeed();
        SetColor();
        iTween.MoveTo(gameObject, iTween.Hash("position", player.transform.position ,"speed", currentSpeed, "easetype", "linear", "looktarget", player.transform.position));
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
		
		//Makes sure it only emits particles when the object is a pulse, not of the same colour
		if(otherObject.name == "Pulse(Clone)" &&
            otherObject.gameObject.GetComponent<LineRenderer>().material.color != gameObject.renderer.material.color) {
				gameObject.GetComponent<ParticleSystem>().Emit(10);
		}
		
    }

    protected void SetPositionAndSpeed() {
        currentSpeed = Random.Range(MinSpeed, MaxSpeed);

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

    protected Color MainColor
    {
        set { mainColor = value; }
        get { return mainColor; }
    }

    void SetColor()
    {
        gameObject.renderer.material.color = MainColor;
        gameObject.GetComponent<ParticleSystem>().startColor = MainColor;
    }

    #endregion
}
