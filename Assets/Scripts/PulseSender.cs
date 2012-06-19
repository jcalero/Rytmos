using UnityEngine;
using System.Collections;

public class PulseSender : MonoBehaviour {
	public int segments = 50;
	public float radius;
	LineRenderer line;
	SphereCollider sphereColl;
	bool held;
	bool transition;
	Color finalColor;
	public float amountToHit;
	private float pulseHealth = 3;
	private Vector2 lastPos;
	
    //Material pulseMat;
	
	
	// Use this for initialization
	void Start () 
	{
		lastPos = Vector2.zero;
		held = true;
		transition = false;
		finalColor = Color.clear;
		line = gameObject.GetComponent<LineRenderer>();
		line.SetVertexCount (segments+1);
		line.useWorldSpace = false;
		line.material.color = finalColor;
		amountToHit = pulseHealth;
		float lineWidth = amountToHit/10;
		line.SetWidth(lineWidth, lineWidth); 
		
		sphereColl = gameObject.GetComponent<SphereCollider>();
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Check if you are transitioning between colours
		if(!(singleColourSelect(lastPos).Equals(singleColourSelect(Input.mousePosition)))) {
			transition = true;	
		}
		
		//If you have released the button, and the pulse is the current one, set it to be not held and set the Colour
		if(Input.GetMouseButtonUp(0) && held) {
			held = false;
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
		radius = radius + 3 * Time.deltaTime;
		sphereColl.radius = radius + 0.1f;
		
		//If too big, destroy itself
		if(radius > 10.2) {
			Destroy(gameObject);
		}
		
		//Debug statement
		if(transition) {
			//print("Transition to : " + chosen);
		}
		
		//Ready the values for the next update - needs boolean check for if currently transitioning
		lastPos = Input.mousePosition;
		transition = false;
	}
	
	void OnTriggerEnter (Collider otherObject) 
	{
		amountToHit--;
		float lineWidth = amountToHit/10;
		if(lineWidth < .2f) {
			lineWidth += .05f;	
		}
		line.SetWidth(lineWidth, lineWidth);
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
