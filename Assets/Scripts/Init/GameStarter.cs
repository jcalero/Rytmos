using UnityEngine;
using System.Collections;

public class GameStarter : MonoBehaviour {

    void Awake() {
        Application.LoadLevel("MainMenu");
		if(!PlayerPrefs.HasKey("FirstPlay")) PlayerPrefs.SetInt("FirstPlay", 1);
		if(!PlayerPrefs.HasKey ("FirstNote")) PlayerPrefs.SetInt ("FirstNote", 1);
		
    }
}
