using UnityEngine;

public class ColorModeListener : MonoBehaviour {
	
	void Awake() {
		Game.SyncMode = true;
		Game.ColorMode = Game.NumOfColors.Six;
	}

	void OnActivate(bool isChecked) {
		//Check for four/six colors
		if(isChecked && gameObject.name == "ColorListener") {
			Debug.Log(">> Mode: SixColors is selected");
			Game.ColorMode = Game.NumOfColors.Six;
		} else if(!isChecked && gameObject.name == "ColorListener") {
			Debug.Log(">> Mode: FourColors is selected");
			Game.ColorMode = Game.NumOfColors.Four;	
		} 
		
		//Check for syncronization
		else if(gameObject.name == "SyncListener") {
			Game.SyncMode = isChecked;
		}
		 
	}
}
