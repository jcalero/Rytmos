using UnityEngine;
using System.Collections;

public class LoadingUI : MonoBehaviour
{

	public UILabel SongLabel;
	public UISlider ProgressBar;
	public UILabel LoadingTextLabel;
	public UISprite WaveForm1;
	public UISprite WaveForm2;
	public UIPanel LoadingPanel;
	public UIPanel ReadyToPlayPanel;
	private bool SongTitleSet;
	private string[,] bullshitText;
	private float stepTimer;
	private bool songIsReady;
	private float timeCounter;
	private int offSet;
	private const float TIME_OFFSET = 0.5f;
	private bool reset = false;
	private float current_step = 0f;
	private float new_step =0f;


	// Use this for initialization
	void Start ()
	{
		timeCounter = 0f;
		offSet = 0;
		
//		bullshitText = new string[5];
//		bullshitText[0] = "Copying Audiosurfs idea...";
//		bullshitText[1] = "Hand drawing audio form data...";
//		bullshitText[2] = "Assigning notes to color spectrum...";
//		bullshitText[3] = "Mishmashing peaks with other stuff...";
//		bullshitText[4] = "Humming to the tune...";
		
		bullshitText = new string[5,4];
		
		bullshitText [0,0] = "Splitting Channels";
		bullshitText [0,1] = "Splitting Channels.";
		bullshitText [0,2] = "Splitting Channels..";
		bullshitText [0,3] = "Splitting Channels...";
		
		bullshitText [1,0] = "Loading Spectral Flux Capacitors";
		bullshitText [1,1] = "Loading Spectral Flux Capacitors.";
		bullshitText [1,2] = "Loading Spectral Flux Capacitors..";
		bullshitText [1,3] = "Loading Spectral Flux Capacitors...";
		
		bullshitText [2,0] = "Inspecting Waveform Data";
		bullshitText [2,1] = "Inspecting Waveform Data.";
		bullshitText [2,2] = "Inspecting Waveform Data..";
		bullshitText [2,3] = "Inspecting Waveform Data...";
		
		bullshitText [3,0] = "Disregarding Waveform Data";
		bullshitText [3,1] = "Disregarding Waveform Data.";
		bullshitText [3,2] = "Disregarding Waveform Data..";
		bullshitText [3,3] = "Disregarding Waveform Data...";
		
		bullshitText [4,0] = "Taping Channels Back Together";
		bullshitText [4,1] = "Taping Channels Back Together.";
		bullshitText [4,2] = "Taping Channels Back Together..";
		bullshitText [4,3] = "Taping Channels Back Together...";
		

		//LoadingLabel.text = "[FDD017]" + "0";
		// Check whether we want to load in a song (Game.Song is set) or use one which has been provided as an asset
		if (Game.Song != null && Game.Song != "") {
			if (Game.Song != AudioManager.getCurrentSong ())
				StartCoroutine (AudioManager.initMusic (Game.Song));
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
	void Update ()
	{
		if (songIsReady)
			return;
		
		
		if (!SongTitleSet && AudioManager.tagDataSet) {
			SongLabel.text = AudioManager.artist + " - " + AudioManager.title;
			CalculateSongLabelSize ();
			SongTitleSet = true;
		}

		if (!AudioManager.isSongLoaded ()) 
		{
			
			
			
			ProgressBar.sliderValue = AudioManager.loadingProgress;
			
			timeCounter += Time.deltaTime;
			
			
			if (AudioManager.loadingProgress > 0.8) 
			{
				new_step = 0.8f;
			} 
			else if (AudioManager.loadingProgress > 0.6) 
			{
				LoadingTextLabel.text = bullshitText [3,offSet];
				new_step = 0.6f;
			} 
			else if (AudioManager.loadingProgress > 0.4) 
			{
				LoadingTextLabel.text = bullshitText [2,offSet];
				new_step = 0.4f;
			} 
			else if (AudioManager.loadingProgress > 0.2) 
			{
				LoadingTextLabel.text = bullshitText [1,offSet];
				new_step = 0.2f;
			} 
			else 
			{
				LoadingTextLabel.text = bullshitText [0,offSet];
				new_step = 0.0f;
			}
			
			if(new_step != current_step)
			{
				offSet = 0;
				timeCounter = 0;
			}
			
			
			if(timeCounter >= TIME_OFFSET * 4 && reset)
			{
				offSet = 0;
				timeCounter = 0;
				reset = false;
			}
			
			if(timeCounter >= TIME_OFFSET * 3)
			{
				offSet = 3;
				reset = true;
			}
			else if(timeCounter >= TIME_OFFSET * 2)
			{
				offSet = 2;
			}
			else if(timeCounter >= TIME_OFFSET)
			{
				offSet = 1;
				
			}
			else
			{
			 	offSet = 0;
			}
			
			if (AudioManager.loadingProgress > 0.8) 
			{
				LoadingTextLabel.text = bullshitText [4,offSet];
				current_step = 0.8f;
			} 
			else if (AudioManager.loadingProgress > 0.6) 
			{
				LoadingTextLabel.text = bullshitText [3,offSet];
				current_step = 0.6f;
			} 
			else if (AudioManager.loadingProgress > 0.4) 
			{
				LoadingTextLabel.text = bullshitText [2,offSet];
				current_step = 0.4f;
			} 
			else if (AudioManager.loadingProgress > 0.2) 
			{
				LoadingTextLabel.text = bullshitText [1,offSet];
				current_step = 0.2f;
			} 
			else 
			{
				LoadingTextLabel.text = bullshitText [0,offSet];
				current_step = 0.0f;
			}
	
			
			

			if (WaveForm1.transform.localPosition.x < -661)
				WaveForm1.transform.localPosition = new Vector2 (670, WaveForm1.transform.localPosition.y);
			if (WaveForm2.transform.localPosition.x < -661)
				WaveForm2.transform.localPosition = new Vector2 (670, WaveForm2.transform.localPosition.y);

			stepTimer += (15 * Time.deltaTime);

			if (stepTimer > 1) {
				WaveForm2.transform.localPosition = new Vector2 (WaveForm2.transform.localPosition.x - 1,
																WaveForm2.transform.localPosition.y);
				WaveForm1.transform.localPosition = new Vector2 (WaveForm1.transform.localPosition.x - 1,
																WaveForm1.transform.localPosition.y);
				stepTimer = 0;
			}
		} else {
			songIsReady = true;
			UITools.SetActiveState (LoadingPanel, false);
			UITools.SetActiveState (ReadyToPlayPanel, true);
		}
	}

	void OnPlayClicked ()
	{
		if(Game.GameMode != Game.Mode.Tutorial) Application.LoadLevel ("Game");
		else Application.LoadLevel("Tutorial");
	}

	void CalculateSongLabelSize ()
	{
		if (SongLabel.text.Length > 38) {
			SongLabel.transform.localScale = new Vector2 (38, 38);
		} else if (SongLabel.text.Length > 28) {
			SongLabel.transform.localScale = new Vector2 (50, 50);
		}
	}
}
