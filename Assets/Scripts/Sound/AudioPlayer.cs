using UnityEngine;
using System.Collections;

/// <summary>
/// Audio player.
/// Class which plays the background music.
/// Methods are static to allow access from anywhere.
/// </summary>
public class AudioPlayer : MonoBehaviour
{
	
	#region vars
	private static AudioSource[] audioSources;	// The two audiosources attached to the gameObject this class is attached to
	private static int currentSource;			// The audiosource that is currenlty playing
	private static bool bufferSource1;			// Flag to decide if a buffer needs to be updated - buffer 1
	private static bool bufferSource2;			// Flag to decide if a buffer needs to be updated - buffer 2 
	private static float timer;					// Timer which is used to check if the song has finished playing!
	private static int pauseSample;				// Holds the point at which sample we have currently paused. Needed in order to play the buffer after the currently playing one!
	private static bool pauseFlag;				// Self explanatory
	private static bool playFlag;				// Self explanatory
	#endregion
	
	void Awake ()
	{
		
		audioSources = gameObject.GetComponentsInChildren<AudioSource> (); // get references to audiosources
		
		// Check whether we want to load in a song (Game.Song is set) or use one which has been provided as an asset
		if (Game.Song != null && Game.Song != "") {
			if (Game.Song != AudioManager.getCurrentSong ())
				AudioManager.initMusic (Game.Song);
			else if (AudioManager.isSongLoaded ())
				AudioManager.reset ();
		} else if (audioSources [0].clip != null) {
			if (!AudioManager.isSongLoaded ()) {
				AudioManager.setCam (audioSources [0]);
				AudioManager.initMusic ("");
			}
		}
		while (!AudioManager.isSongLoaded()) {
		} // idlewait
		
		// Initialization
		currentSource = 0;
		audioSources [0].clip = AudioManager.getAudioClip (1);
		audioSources [1].clip = AudioManager.getAudioClip (2);
		
		audioSources [0].velocityUpdateMode = AudioVelocityUpdateMode.Fixed;
		audioSources [1].velocityUpdateMode = AudioVelocityUpdateMode.Fixed;

		bufferSource1 = false;
		bufferSource2 = true;
		
		timer = 0f;
		pauseSample = 0;
		pauseFlag = false;
		playFlag = false;
	}
	
	// Start playing automatically, may want to get rid of it doing that..
	void Start ()
	{
		playFlag = true;
		audioSources [currentSource].Play ();
	}
	
	/// <summary>
	/// In here we keep track of the buffers and update the accordingly
	/// </summary>
	void Update ()
	{
		if (playFlag && !Game.Paused) {
			
			if (AudioManager.lastSamples) {
				return;
			}
			
			if (timer >= AudioManager.audioLength) {
				audioSources [currentSource].Stop ();
			} else if (audioSources [0].clip != null && audioSources [1].clip != null && gameObject != null) {
				
				if (audioSources [0].timeSamples > 0 && audioSources [0].isPlaying && bufferSource2 && !audioSources [1].isPlaying) {
					StartCoroutine (AudioManager.updateMusic ());
					audioSources [1].Play ((ulong)(audioSources [0].clip.samples - audioSources [0].timeSamples));
					bufferSource2 = false;
					bufferSource1 = true;
					currentSource = 0;
				} else if (audioSources [1].timeSamples > 0 && audioSources [1].isPlaying && bufferSource1 && !audioSources [0].isPlaying) {
					StartCoroutine (AudioManager.updateMusic ());
					audioSources [0].Play ((ulong)(audioSources [1].clip.samples - audioSources [1].timeSamples));
					bufferSource1 = false;
					bufferSource2 = true;
					currentSource = 1;
				}
				
				timer += Time.deltaTime;
			}
		}
	}
	
	/// <summary>
	/// Play the music. NOT TESTED YET!
	/// </summary>
	public static void play ()
	{
		if (audioSources [0] != null)
			audioSources [0].Stop ();
		if (audioSources [1] != null)
			audioSources [1].Stop ();
		reset ();
		playFlag = true;
		audioSources [currentSource].Play ();
	}
	
	/// <summary>
	/// Pause playback.
	/// </summary>
	public static void pause ()
	{
		if (audioSources [0] != null && audioSources [1] != null) {
			pauseFlag = true;
			if (audioSources [currentSource].isPlaying) {
				if (audioSources [currentSource].isPlaying)
					audioSources [currentSource].Pause ();
				if (audioSources [currentSource == 1 ? 0 : 1].isPlaying)
					audioSources [currentSource == 1 ? 0 : 1].Stop ();
				
				pauseSample = audioSources [currentSource].timeSamples;
			}
		}
	}
	
	/// <summary>
	/// Resume playback.
	/// </summary>
	public static void resume ()
	{
		if (audioSources [0] != null && audioSources [1] != null) {
			if (pauseFlag) {
				if (!audioSources [currentSource].isPlaying)
					audioSources [currentSource].Play ();
				if (!audioSources [currentSource == 1 ? 0 : 1].isPlaying)
					audioSources [currentSource == 1 ? 0 : 1].Play ((ulong)(audioSources [currentSource].clip.samples - pauseSample));
				pauseSample = 0;
				pauseFlag = false;
			}
		}
	}
	
	/// <summary>
	/// Reset so that we can start playing from the beginning.
	/// </summary>
	private static void reset ()
	{
		AudioManager.reset ();

		currentSource = 0;
		audioSources [0].clip = AudioManager.getAudioClip (1);
		audioSources [1].clip = AudioManager.getAudioClip (2);
		
		audioSources [0].velocityUpdateMode = AudioVelocityUpdateMode.Fixed;
		audioSources [1].velocityUpdateMode = AudioVelocityUpdateMode.Fixed;

		bufferSource1 = false;
		bufferSource2 = true;
		
		timer = 0f;
		pauseSample = 0;
		pauseFlag = false;
		playFlag = false;
	}	
}
