using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadingUI : MonoBehaviour {

	public UILabel SongLabel;
	public UISlider ProgressBar;
	public UILabel LoadingTextLabel;
	public UILabel ConfirmTextLabel;
	public UILabel AbortedTextLabel;
	public UISprite WaveForm1;
	public UISprite WaveForm2;
	public UIPanel LoadingPanel;
	public UIPanel ReadyToPlayPanel;
	public UIPanel ConfirmPanel;
	public UIPanel AbortingPanel;
	private bool SongTitleSet;
	private string[] BSText;
	private float threeDotsTimer;
	private readonly float DOTSPEED = 0.5f;
	private float stepTimer;
	private bool songIsReady;
	private int TextIterator = 0;
	private int currentText = 0;
	private List<int> usedStrings;
	private bool isConfirming;
	private GameObject cam;

	// Use this for initialization
	void Start() {
		
		cam = GameObject.Find("Camera");
		cam.GetComponentInChildren<Camera>().orthographicSize = Game.cameraScaleFactor;	
		
		BSText = new string[] {
			"Splitting Channels",
			"Charging Spectral Flux Capacitors",
			"Inspecting Waveform Data",
			"Disregarding Waveform Data",
			"Taping Channels Together",
			"Taking A Break",
			"Imagining Lyrics",
			"Making Silence Louder",
			"Turning Volume Up To 11",
			"Adding More Cowbell",
			"Surfing Waveform",
			"Composing Noise",
			"Tripping Over Amplitudes",
			"Deafening Test Subjects",
			"Warming Up Touchscreen",
			"Motivating Red Enemies",
			"Motivating Blue Enemies",
			"Motivating Green Enemies",
			"Motivating Yellow Enemies",
			"Motivating Cyan Enemies",
			"Motivating Purple Enemies",
			"Adding Random Circles",
			"Applying Autotune",
			"Shifting Pitch",
			"Correcting Pitch",
			"Injecting Neon Lights"
		};

		ProgressBar.sliderValue = 0f;
		threeDotsTimer = 0f;

		currentText = Random.Range(0, BSText.Length);
		usedStrings = new List<int>();

		// Check whether we want to load in a song (Game.Song is set) or use one which has been provided as an asset
		if (Game.Song != null && Game.Song != "") {
			StartCoroutine(AudioManager.initMusic(Game.Song));
		}
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape) && !AudioManager.isWritingCacheFile) {
			if (!isConfirming) {
				if (songIsReady) {
					UITools.SetActiveState(ReadyToPlayPanel, false);
					ConfirmTextLabel.text = "Back to Main Menu?";
				} else {
					UITools.SetActiveState(LoadingPanel, false);
					ConfirmTextLabel.text = "Abort song analysis?";
				}
				UITools.SetActiveState(ConfirmPanel, true);
				isConfirming = true;
			} else {
				UITools.SetActiveState(ConfirmPanel, false);
				if (songIsReady)
					UITools.SetActiveState(ReadyToPlayPanel, true);
				else
					UITools.SetActiveState(LoadingPanel, true);
				isConfirming = false;
			}
		}

		if (songIsReady)
			return;


		if (!SongTitleSet && AudioManager.tagDataSet) {
			SongLabel.text = AudioManager.artist + " - " + AudioManager.title;
			CalculateSongLabelSize();
			SongTitleSet = true;
		}
		
		threeDotsTimer += Time.deltaTime;

		if (!AudioManager.isSongLoaded()) {

			ProgressBar.sliderValue = AudioManager.loadingProgress;

			if (AudioManager.loadingProgress >= TextIterator / 5f && AudioManager.loadingProgress < (TextIterator + 1) / 5f) {

				string baseString = BSText[currentText];
				
				if (threeDotsTimer < DOTSPEED) {}
				else if (threeDotsTimer < DOTSPEED*2f) baseString += ".";
				else if (threeDotsTimer < DOTSPEED*3f) baseString += "..";
				else if (threeDotsTimer < DOTSPEED*4f) baseString += "...";
				else threeDotsTimer = 0f;

				LoadingTextLabel.text = baseString;

			} else {
				TextIterator++;
				usedStrings.Add(currentText);
				currentText = Random.Range(0, BSText.Length);
				threeDotsTimer = 0f;
				while (usedStrings.Contains(currentText))
					currentText = Random.Range(0, BSText.Length);
			}



			if (WaveForm1.transform.localPosition.x < -661)
				WaveForm1.transform.localPosition = new Vector2(670, WaveForm1.transform.localPosition.y);
			if (WaveForm2.transform.localPosition.x < -661)
				WaveForm2.transform.localPosition = new Vector2(670, WaveForm2.transform.localPosition.y);

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
			if (!isConfirming) {
				UITools.SetActiveState(LoadingPanel, false);
				UITools.SetActiveState(ReadyToPlayPanel, true);
			}
		}
	}

	void OnYesClicked() {
		if (songIsReady)
			AbortedTextLabel.text = "Loading Main Menu...";
		else
			AbortedTextLabel.text = "Aborting analysis...";
		UITools.SetActiveState(ConfirmPanel, false);
		UITools.SetActiveState(AbortingPanel, true);
		AudioManager.abort();
		System.GC.Collect();
		Application.LoadLevel("MainMenu");
	}

	void OnNoClicked() {
		isConfirming = false;
		UITools.SetActiveState(ConfirmPanel, false);
		if (songIsReady)
			UITools.SetActiveState(ReadyToPlayPanel, true);
		else
			UITools.SetActiveState(LoadingPanel, true);
	}

	void OnPlayClicked() {
		if (Game.GameMode != Game.Mode.Tutorial) Application.LoadLevel("Game");
		else Application.LoadLevel("Tutorial");
	}

	void CalculateSongLabelSize() {
		if (SongLabel.text.Length > 38) {
			SongLabel.transform.localScale = new Vector2(38, 38);
		} else if (SongLabel.text.Length > 28) {
			SongLabel.transform.localScale = new Vector2(50, 50);
		}
	}
}
