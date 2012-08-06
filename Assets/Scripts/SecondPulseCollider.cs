using UnityEngine;
using System.Collections;

public class SecondPulseCollider : MonoBehaviour {
	public GameObject CollisionParticles;

	protected void Start() {
		transform.position = gameObject.transform.parent.transform.position;
		transform.localEulerAngles = new Vector3(0, 0, -90);
		float scale = (1 / gameObject.transform.parent.localScale.x);
		transform.localScale = new Vector3(scale, scale, scale);
	}

	protected void OnTriggerExit(Collider otherObject) {
		if (otherObject.name == "Pulse(Clone)") {
			Color c = gameObject.transform.parent.GetComponent<EnemyScript>().MainColor;
			if (otherObject.gameObject.GetComponent<PulseSender>().CurrentColor == c ||
				otherObject.gameObject.GetComponent<PulseSender>().SecondaryColor == c) {
				Player.IncrementScore();
				Player.Energy += 5;
				if (Player.Energy > 50)
					Player.Energy = 50;
				gameObject.transform.parent.GetComponent<EnemyScript>().CreateExplosion();
				StartCoroutine(gameObject.transform.parent.GetComponent<EnemyScript>().DamageEnemy(false));
			} else         //Makes sure it only emits particles when the object is a pulse, not of the same colour
				CollisionParticles.GetComponent<ParticleSystem>().Emit(10);
		}
	}

}
