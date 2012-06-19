using UnityEngine;
using System.Collections;

public class CornerPulser : MonoBehaviour {
	ParticleSystem ps;
	private float myTimer = 0;
	private bool held = false; 
	
	// Use this for initialization
	void Start () {
		ps = gameObject.GetComponent<ParticleSystem>();	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)) {
			held = true;
		}
		if(Input.GetMouseButtonUp (0)) {
			held = false;	
		}
		if(!held) {
			myTimer += Time.deltaTime;
			if(ps.startColor == Color.red && myTimer >= 0f && myTimer < 1f) {
				ps.Emit(2);
			}
			if(ps.startColor == Color.yellow && myTimer >= 1f && myTimer < 2f) {
				ps.Emit(2);
			}
			if(ps.startColor == Color.green && myTimer >= 2f && myTimer < 3f) {
				ps.Emit(2);
			}
			if(ps.startColor == Color.cyan && myTimer >= 3f && myTimer < 4f) {
				ps.Emit(2);
			}
			if(ps.startColor == Color.blue && myTimer >= 4f && myTimer < 5f) {
				ps.Emit(2);
			}
			if(ps.startColor != Color.red && ps.startColor != Color.yellow && ps.startColor != Color.cyan
				&& ps.startColor != Color.blue && ps.startColor != Color.green && myTimer >= 5f && myTimer < 6f) {
				ps.Emit(2);
			}
		} else {
			ps.Emit (2);	
		}
		
		//All emit
	//	if(myTimer >= 6f && myTimer < 7f) {
	//		ps.Emit (5);
	//	}
		
		if(myTimer > 6f) {
			myTimer = 0;	
		}
	}
}
