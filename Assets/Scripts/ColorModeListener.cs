using UnityEngine;

public class ColorModeListener : MonoBehaviour {
	void OnActivate(bool isChecked) {
		if(isChecked) {
			Debug.Log(">> Mode: SixColors is selected");
			Game.ColorMode = Game.NumOfColors.Six;
		}
		else {
			Debug.Log(">> Mode: FourColors is selected");
			Game.ColorMode = Game.NumOfColors.Four;	
		}
	}
}
