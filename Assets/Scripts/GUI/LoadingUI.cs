using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadingUI : MonoBehaviour {

	public UILabel SongLabel;
	public UISlider ProgressBar;
	public UISlicedSprite ProgressBarBackground;
	public UILabel LoadingTextLabel;
	public UILabel ConfirmTextLabel;
	public UILabel AbortedTextLabel;
	public UISprite WaveForm1;
	public UISprite WaveForm2;
	public UISlicedSprite WaveFormBorder;
	public UISlicedSprite WaveFormMidLine;
	public GameObject WaveFormLeftNrs;
	public GameObject WaveFormRightNrs;
	public UIPanel WaveFormPanel;
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
	private float waveFormSwitchPoint = -661;
	private float waveFormResetPoint = 670;

	// Use this for initialization
	void Start() {

		float aspect = Camera.mainCamera.aspect;
		Debug.Log(aspect);

		// Set up the loading bar and waveform for different aspect ratios
		// Aspect ratio: 1.333..
		if (aspect == 640.0f / 480.0f) {
			Debug.Log("Setting up UI for aspect ratio: " + aspect);
			ProgressBarBackground.transform.localScale = new Vector2(600, ProgressBarBackground.transform.localScale.y);
			ProgressBar.transform.localPosition = new Vector2(-300, ProgressBar.transform.localPosition.y);
			WaveFormBorder.transform.localScale = new Vector2(520, WaveFormBorder.transform.localScale.y);
			WaveFormMidLine.transform.localScale = new Vector2(509, WaveFormMidLine.transform.localScale.y);
			WaveFormLeftNrs.transform.localPosition = new Vector2(70, 0);
			WaveFormRightNrs.transform.localPosition = new Vector2(-70, 0);
			WaveFormPanel.clipRange = new Vector4(WaveFormPanel.clipRange.x, WaveFormPanel.clipRange.y, 511, WaveFormPanel.clipRange.w);
			SongLabel.lineWidth = 600;
		}
			// Aspect ratio: 1.5
		else if (aspect == (480.0f / 320.0f)) {
			Debug.Log("Setting up UI for aspect ratio: " + aspect);
			ProgressBarBackground.transform.localScale = new Vector2(650, ProgressBarBackground.transform.localScale.y);
			ProgressBar.transform.localPosition = new Vector2(-325, ProgressBar.transform.localPosition.y);
			WaveFormBorder.transform.localScale = new Vector2(650, WaveFormBorder.transform.localScale.y);
			WaveFormMidLine.transform.localScale = new Vector2(636, WaveFormMidLine.transform.localScale.y);
			WaveFormLeftNrs.transform.localPosition = new Vector2(15, 0);
			WaveFormRightNrs.transform.localPosition = new Vector2(-15, 0);
			WaveFormPanel.clipRange = new Vector4(WaveFormPanel.clipRange.x, WaveFormPanel.clipRange.y, 638, WaveFormPanel.clipRange.w);
		}
			// Aspect ratio: 1.6
		else if (aspect == (1280.0f / 800.0f)) {
			Debug.Log("Setting up UI for aspect ratio: " + aspect);
			ProgressBarBackground.transform.localScale = new Vector2(650, ProgressBarBackground.transform.localScale.y);
			ProgressBar.transform.localPosition = new Vector2(-325, ProgressBar.transform.localPosition.y);
			WaveFormBorder.transform.localScale = new Vector2(650, WaveFormBorder.transform.localScale.y);
			WaveFormMidLine.transform.localScale = new Vector2(636, WaveFormMidLine.transform.localScale.y);
			WaveFormLeftNrs.transform.localPosition = new Vector2(15, 0);
			WaveFormRightNrs.transform.localPosition = new Vector2(-15, 0);
			WaveFormPanel.clipRange = new Vector4(WaveFormPanel.clipRange.x, WaveFormPanel.clipRange.y, 638, WaveFormPanel.clipRange.w);
		}
			// Aspect ratio: 1.666.. (Default android)
		else if (aspect == (800.0f / 480.0f)) {
			Debug.Log("Setting up UI for aspect ratio: " + aspect);
			ProgressBarBackground.transform.localScale = new Vector2(750, ProgressBarBackground.transform.localScale.y);
			ProgressBar.transform.localPosition = new Vector2(-375, ProgressBar.transform.localPosition.y);
		}
			// Aspect ratio: 1.7066..
		else if (aspect == (1024.0f / 600.0f)) {
			// Using the same as 1.666.. (Default)
		}
			// Aspect ratio: 1.779166..
		else if (aspect == (854.0f / 480.0f)) {
			// Using the same as 1.666.. (Default)
		}
			// Aspect ratio: 1.8
		else if (aspect == (432.0f / 240.0f)) {
			// Using the same as 1.666.. (Default)
		}
		ProgressBar.fullSize = new Vector2(ProgressBarBackground.transform.localScale.x - 4, ProgressBarBackground.transform.localScale.y - 2);
		ProgressBar.foreground.transform.localScale = ProgressBar.fullSize;

		//GameObject cam = GameObject.Find("Camera");
		//cam.GetComponentInChildren<Camera>().orthographicSize = Game.cameraScaleFactor;	

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

				if (threeDotsTimer < DOTSPEED) { } else if (threeDotsTimer < DOTSPEED * 2f) baseString += ".";
				else if (threeDotsTimer < DOTSPEED * 3f) baseString += "..";
				else if (threeDotsTimer < DOTSPEED * 4f) baseString += "...";
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



			if (WaveForm1.transform.localPosition.x < waveFormSwitchPoint)
				WaveForm1.transform.localPosition = new Vector2(waveFormResetPoint, WaveForm1.transform.localPosition.y);
			if (WaveForm2.transform.localPosition.x < waveFormSwitchPoint)
				WaveForm2.transform.localPosition = new Vector2(waveFormResetPoint, WaveForm2.transform.localPosition.y);

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
		int textLengthLong = 38;
		int textLengthShort = 28;
		int longSize = 38;
		int shortSize = 50;
		if (Camera.mainCamera.aspect < 1.5) {
			textLengthLong = 20;
			textLengthShort = 10;
			longSize = 30;
			shortSize = 45;
		}
		if (SongLabel.text.Length > textLengthLong) {
			SongLabel.transform.localScale = new Vector2(longSize, longSize);
		} else if (SongLabel.text.Length > textLengthShort) {
			SongLabel.transform.localScale = new Vector2(shortSize, shortSize);
		}
	}
}
