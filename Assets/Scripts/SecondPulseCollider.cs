using UnityEngine;
using System.Collections;

public class SecondPulseCollider : MonoBehaviour {
	
	protected void Start () {
		transform.position = gameObject.transform.parent.transform.position;
		transform.localEulerAngles = new Vector3(0, 0, -90);
		float scale = (1/gameObject.transform.parent.localScale.x);
		transform.localScale = new Vector3(scale, scale, scale);
	}
	
	protected void OnTriggerExit (Collider otherObject) {
		if (otherObject.name == "Pulse(Clone)") {
		   if (otherObject.gameObject.GetComponent<LineRenderer>().material.color == gameObject.transform.parent.GetComponent<EnemyScript>().MainColor) {
				Player.score += 10;
				Player.energy += 5;
				if (Player.energy > 50)
					Player.energy = 50;
				gameObject.transform.parent.GetComponent<EnemyScript>().CreateExplosion();
				StartCoroutine(gameObject.transform.parent.GetComponent<EnemyScript>().DamageEnemy());
			} else         //Makes sure it only emits particles when the object is a pulse, not of the same colour
				 gameObject.transform.parent.GetComponent<ParticleSystem>().Emit(10);	
		}  
	}
	
}
