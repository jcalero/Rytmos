using UnityEngine;
using System.Collections;

public class LoadingUI : MonoBehaviour {
	
	public UILabel LoadingLabel;

	// Use this for initialization
	void Start () {

		LoadingLabel.text = "";
		//LoadingLabel.text = "[FDD017]" + "0";
		// Check whether we want to load in a song (Game.Song is set) or use one which has been provided as an asset
		if (Game.Song != null && Game.Song != "") {
			if (Game.Song != AudioManager.getCurrentSong ())
				StartCoroutine( AudioManager.initMusic (Game.Song));
		}
//		} else if (audioSources [0].clip != null) {
//			if (!AudioManager.isSongLoaded ()) {
//				AudioManager.setCam (audioSources [0]);
//				StartCoroutine( AudioManager.initMusic (""));
//			}
//		}	
	}
	
	// Update is called once per frame
	void Update () {
		if(!AudioManager.isSongLoaded()) {
			LoadingLabel.text = "[FDD017]" + AudioManager.loadingProgress;	
		}
		else Application.LoadLevel("Game");
	}
}
