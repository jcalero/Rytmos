using UnityEngine;
using System.Collections;

public class SecondPulseCollider : MonoBehaviour {
	
	SphereCollider sphereColl;
	
	// Use this for initialization
	void Start () {
		sphereColl = gameObject.GetComponent<SphereCollider>();
		
	}
	
	// Update is called once per frame
	void Update () {
		sphereColl.radius = gameObject.transform.parent.GetComponent<PulseSender>().radius - 0.1f;
	}
	
	void OnTriggerExit (Collider otherObject) 
	{
		gameObject.transform.parent.GetComponent<PulseSender>().amountToHit--;
	}
}
