using UnityEngine;
using System.Collections;

public class LoadingUI : MonoBehaviour {

	public UILabel SongLabel;
	public UISlider ProgressBar;
	public UILabel LoadingTextLabel;
	public UISprite WaveForm1;
	public UISprite WaveForm2;
	public UIPanel LoadingPanel;
	public UIPanel ReadyToPlayPanel;

	private bool SongTitleSet;
	private string[] bullshitText;
	private float stepTimer;
	private bool songIsReady;

	// Use this for initialization
	void Start() {

		bullshitText = new string[5];
		bullshitText[0] = "Copying Audiosurfs idea...";
		bullshitText[1] = "Hand drawing audio form data...";
		bullshitText[2] = "Assigning notes to color spectrum...";
		bullshitText[3] = "Mishmashing peaks with other stuff...";
		bullshitText[4] = "Humming to the tune...";

		//LoadingLabel.text = "[FDD017]" + "0";
		// Check whether we want to load in a song (Game.Song is set) or use one which has been provided as an asset
		if (Game.Song != null && Game.Song != "") {
			if (Game.Song != AudioManager.getCurrentSong())
				StartCoroutine(AudioManager.initMusic(Game.Song));
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
	void Update() {
		if (songIsReady) return;

		if (!SongTitleSet && AudioManager.tagDataSet) {
			SongLabel.text = AudioManager.artist + " - " + AudioManager.title;
			CalculateSongLabelSize();
			SongTitleSet = true;
		}

		if (!AudioManager.isSongLoaded()) {
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

			if (WaveForm1.transform.localPosition.x < -661) WaveForm1.transform.localPosition = new Vector2(670, WaveForm1.transform.localPosition.y);
			if (WaveForm2.transform.localPosition.x < -661) WaveForm2.transform.localPosition = new Vector2(670, WaveForm2.transform.localPosition.y);

			stepTimer += (15 * Time.deltaTime);

			if (stepTimer > 1) {
				WaveForm2.transform.localPosition = new Vector2(WaveForm2.transform.localPosition.x - 1,
																WaveForm2.transform.localPosition.y);
				WaveForm1.transform.localPosition = new Vector2(WaveForm1.transform.localPosition.x - 1,
																WaveForm1.transform.localPosition.y);
				stepTimer = 0;
			}
		} else {
			songIsReady = true;
			UITools.SetActiveState(LoadingPanel, false);
			UITools.SetActiveState(ReadyToPlayPanel, true);
		}
	}

	void OnPlayClicked() {
		Application.LoadLevel("Game");
	}

	void CalculateSongLabelSize() {
		if (SongLabel.text.Length > 38) {
			SongLabel.transform.localScale = new Vector2(38, 38);
		} else if (SongLabel.text.Length > 28) {
			SongLabel.transform.localScale = new Vector2(50, 50);
		}
	}
}
