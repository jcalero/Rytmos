using UnityEngine;
using System.Collections;

public class PowerupSpawnScript : MonoBehaviour {
	public GameObject[] powerupList;
	
	//Note: as this currently stands, this might be better off being shared by another class
	void Start() {
		SpawnPowerup();		
	}

	public void SpawnPowerup() {
		Vector3 spawnPos = randomPos();
		Instantiate(powerupList[Random.Range(0, powerupList.Length)], spawnPos, Quaternion.identity);
		Level.SetUpParticlesFeedback(4, spawnPos);
	}
	
	private Vector3 randomPos() {
		float x = Random.Range (1, Game.screenRight-1);
		float y = Random.Range (1, Game.screenTop-1);
		float neg = Random.Range (0, 2);
		if(neg==0) x = -x;
		neg = Random.Range (0, 2);
		if(neg==0) y = -y;
		return new Vector3(x,y,0);
	}
	
}
