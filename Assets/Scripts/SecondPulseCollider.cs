using UnityEngine;
using System.Collections;

public class SecondPulseCollider : MonoBehaviour {
    
    protected void Start () {
        transform.position = gameObject.transform.parent.transform.position;
    }
    
    protected void OnTriggerExit (Collider otherObject) {
        if (otherObject.name == "Pulse(Clone)") {
            if(otherObject.gameObject.GetComponent<LineRenderer>().material.color == gameObject.transform.parent.renderer.material.color) {
                Player.score += 10;
                Player.energy += 5;
                if (Player.energy > 50)
                    Player.energy = 50;
                gameObject.transform.parent.GetComponent<EnemyScript>().CreateExplosion();
                gameObject.transform.parent.GetComponent<EnemyScript>().DamageEnemy();
            } else         //Makes sure it only emits particles when the object is a pulse, not of the same colour
                 gameObject.transform.parent.GetComponent<ParticleSystem>().Emit(10);	
        }  
    }
    
}
