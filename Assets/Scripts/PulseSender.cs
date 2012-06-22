using UnityEngine;
using System.Collections;

public class PulseSender : MonoBehaviour {
	public int segments = 50;
	public float radius;
	LineRenderer line;
	SphereCollider sphereColl;
	public bool held;
	public bool secondFinger;
	Color finalColor;
	public float amountToHit;
	private float pulseHealth = 3;
	float pulseMax;
    float timer = 0;
	
	// Use this for initialization
	void Start () 
	{
		held = true;
		secondFinger = false;
		finalColor = Color.clear;
		line = gameObject.GetComponent<LineRenderer>();
		line.SetVertexCount (segments+1);
		line.useWorldSpace = false;
		line.material.color = finalColor;
		amountToHit = pulseHealth;
		float lineWidth = amountToHit/10;
		line.SetWidth(lineWidth, lineWidth); 
		
		sphereColl = gameObject.GetComponent<SphereCollider>();
		
		
		//Find distance for the radius
		pulseMax = new Vector2(Player.screenLeft, Player.screenTop).magnitude;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		if(Input.GetMouseButtonDown(1) && held) {
			secondFinger = true;	
		}
		if(Input.GetMouseButtonUp(1) && held) {
			secondFinger = false;	
		}
		//If you have released the button, and the pulse is the current one, set it to be not held and set the Colour
		if(Input.GetMouseButtonUp(0) && held) {
			held = false;
			secondFinger = false;	
			finalColor = singleColourSelect(Input.mousePosition);
		}
		
		//What the colour should be - this is where the transition has to take place. 
		Color chosen;
		if(held) {
			chosen = singleColourSelect(Input.mousePosition);
		} else {
			chosen = finalColor;	
		}
		
		//Create the circle, and set the line material
		CreatePoints(chosen);
        line.material.color = chosen;
		
		//Increase both the radius of the pulse and the sphere collider. 
		if(!secondFinger) {
			radius = radius + 3 * Time.deltaTime;
			sphereColl.radius = radius + 0.1f;
		} else {
            timer += Time.deltaTime;
            if (timer > 0.2f)
            {
                if (Player.energy > 1)
                    Player.energy -= 2;
                else
                    secondFinger = false;
                timer = 0;
            }
			radius = radius - 3 * Time.deltaTime;
			sphereColl.radius = radius + 0.1f;
		}
		
		//If too big, destroy itself
		if(radius > pulseMax || (radius < .1 && secondFinger) || amountToHit == 0) {
			Destroy(gameObject);
		}
		
	}
	
	void OnTriggerEnter (Collider otherObject) 
	{
		amountToHit--;
		if(amountToHit==0) {
			Destroy(gameObject);
		}
		
	}
	
	void CreatePoints (Color c) 
	{
		float x;
		float y;
		float z = 0f;
		float angle = 0f;
		
		for(int i=0; i<(segments+1); i++) {
			x = Mathf.Sin (Mathf.Deg2Rad * angle);
			y = Mathf.Cos (Mathf.Deg2Rad * angle);			
			line.SetPosition (i, new Vector3(x,y,z) * radius);
			line.SetColors(new Color(c.r, c.g, c.b, ((amountToHit/pulseHealth)*.5f)+.5f)   ,new Color(c.r, c.g, c.b, ((amountToHit/pulseHealth)*.5f)+.5f));
			line.material.SetColor("_Emission", new Color(c.r, c.g, c.b, c.a/3));
			float lineWidth = amountToHit/10;
			if(lineWidth < .2f) {
			lineWidth += .05f;	
			}
			line.SetWidth(lineWidth, lineWidth);
			angle += (360f / segments);
		}
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
	
}
