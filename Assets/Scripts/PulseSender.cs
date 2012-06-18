using UnityEngine;
using System.Collections;

public class PulseSender : MonoBehaviour {
	public int segments = 50;
	public float radius;
	LineRenderer line;
	SphereCollider sphereColl;
	bool held;
	Color lastColor;
	public float amountToHit;
	private float pulseHealth = 3;
    //Material pulseMat;
	
	
	// Use this for initialization
	void Start () 
	{
		held = true;
		lastColor = Color.clear;
		line = gameObject.GetComponent<LineRenderer>();
		line.SetVertexCount (segments+1);
		line.useWorldSpace = false;
		amountToHit = pulseHealth;
		line.SetWidth(amountToHit/10, amountToHit/10); 
		
		sphereColl = gameObject.GetComponent<SphereCollider>();
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonUp(0) && held) {
			held = false;
			lastColor = singleColourSelect(Input.mousePosition);
		}
		Color chosen;
		if(held) {
			chosen = singleColourSelect(Input.mousePosition);
		} else {
			chosen = lastColor;	
		}
		CreatePoints(chosen);
        line.material.color = chosen;
		radius = radius + 3 * Time.deltaTime;
		sphereColl.radius = radius + 0.1f;
		if(radius > 10.2) {
			Destroy(gameObject);
		}
	}
	
	void OnTriggerEnter (Collider otherObject) 
	{
		//if (otherObject.name == "EnemyPrefab(Clone)" &&
         //   otherObject.gameObject.GetComponent<MeshRenderer>().material.color == line.material.color)
		//{
			amountToHit--;
			line.SetWidth(amountToHit/10, amountToHit/10);
			if(amountToHit==0) {
				Destroy(gameObject);
			}
		//}
		
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
			line.SetColors(c*(amountToHit/pulseHealth),c*(amountToHit/pulseHealth));
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
