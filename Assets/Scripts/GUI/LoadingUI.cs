using UnityEngine;
using System.Collections;

public class LoadingUI : MonoBehaviour {
	
	public UILabel SongLabel;
	public UISlider ProgressBar;
	public UILabel LoadingTextLabel;

	private bool SongTitleSet;
	private string[] bullshitText;

	// Use this for initialization
	void Start () {

		bullshitText = new string[5];
		bullshitText[0] = "Copying Audiosurfs idea...";
		bullshitText[1] = "Hand drawing audio form data...";
		bullshitText[2] = "Assigning notes to color spectrum...";
		bullshitText[3] = "Mishmashing peaks with other stuff...";
		bullshitText[4] = "Humming to the tune...";

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

	//void IEnumerate() 
	
	// Update is called once per frame
	void Update () {

		if (!SongTitleSet && AudioManager.tagDataSet) {
			SongLabel.text = AudioManager.artist + " - " + AudioManager.title;
			//CalculateSongLabelSize();
			SongTitleSet = true;
		}

		if(!AudioManager.isSongLoaded()) {
			ProgressBar.sliderValue = AudioManager.loadingProgress;
			if (AudioManager.loadingProgress > 0.8) {
				LoadingTextLabel.text = bullshitText[4];
			} else if (AudioManager.loadingProgress > 0.6) {
				LoadingTextLabel.text = bullshitText[3];
			} else if (AudioManager.loadingProgress > 0.4) {
				LoadingTextLabel.text = bullshitText[2];
			} else if (AudioManager.loadingProgress > 0.2) {
				LoadingTextLabel.text = bullshitText[1];
			} else {
				LoadingTextLabel.text = bullshitText[0];
			}
		}
		else Application.LoadLevel("Game");
	}
	//void CalculateSongLabelSize() {
	//    if (SongLabel.text.Length > 
	//}
}
