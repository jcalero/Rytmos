using UnityEngine;
using System.Collections;
using System.Threading;
using System;

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
	private static AndroidJavaObject androidPlayer;
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
		
#if UNITY_ANDROID 
		if(Application.platform == RuntimePlatform.Android) {
			AudioManager.closeMusicStream();
			androidPlayer = new AndroidJavaObject("android.media.MediaPlayer",new object[]{});
			androidPlayer.Call("setDataSource",new object[]{Game.Song});
			androidPlayer.Call("prepare",new object[]{});
			playFlag = false;
#elif UNITY_STANDALONE_WIN
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
		playFlag = true;
#endif
		
	}
	
	// Start playing automatically, may want to get rid of it doing that..
	void Start ()
	{
#if UNITY_ANDROID
		androidPlayer.Call ("start", new object[]{});
#elif UNITY_STANDALONE_WIN
		playFlag = true;
		audioSources [currentSource].Play ();
#endif	
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
					audioSources [1].Play ((ulong)(audioSources [0].clip.samples - audioSources [0].timeSamples));
					StartCoroutine (AudioManager.updateMusic ()); //
					bufferSource2 = false;
					bufferSource1 = true;
					currentSource = 0;
				} else if (audioSources [1].timeSamples > 0 && audioSources [1].isPlaying && bufferSource1 && !audioSources [0].isPlaying) {
					audioSources [0].Play ((ulong)(audioSources [1].clip.samples - audioSources [1].timeSamples));
					StartCoroutine (AudioManager.updateMusic ());
					bufferSource1 = false;
					bufferSource2 = true;
					currentSource = 1;
				}
				
				timer += Time.deltaTime;
			}
		}
	}
	
	public static float Pitch {
		get {
			if (audioSources [currentSource] != null) {
				Debug.Log ("returning: " + audioSources [currentSource].pitch);
				return audioSources [currentSource].pitch;
			} else 
				return 0f;
		}
		set { 
			if (audioSources [currentSource] != null && audioSources [currentSource == 1 ? 0 : 1] != null) {
				Debug.Log ("Setting: " + value);
				audioSources [currentSource].pitch = value;
				audioSources [currentSource == 1 ? 0 : 1].pitch = value;
			}
		}
		
	}
	
	public static void setPitch (float pitch)
	{
		audioSources [currentSource].pitch = pitch;
		audioSources [currentSource == 1 ? 0 : 1].pitch = 1f;
	}
	
	/// <summary>
	/// Play the music. NOT TESTED YET!
	/// </summary>
	public static void play ()
	{
#if UNITY_ANDROID 
			androidPlayer.Call ("start", new object[]{});
#elif UNITY_STANDALONE_WIN
			if (audioSources [0] != null)
				audioSources [0].Stop ();
			if (audioSources [1] != null)
				audioSources [1].Stop ();
			reset ();
			playFlag = true;
			audioSources [currentSource].Play ();
#endif
		
	}
	
	/// <summary>
	/// Pause playback.
	/// </summary>
	public static void pause ()
	{
#if UNITY_ANDROID
			androidPlayer.Call ("pause", new object[]{});
#elif UNITY_STANDALONE_WIN
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
#endif
		
	}
	
	/// <summary>
	/// Resume playback.
	/// </summary>
	public static void resume ()
	{
#if UNITY_ANDROID
			androidPlayer.Call ("start", new object[]{});
#elif UNITY_STANDALONE_WIN
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
#endif		
	}
	
	/// <summary>
	/// Reset so that we can start playing from the beginning.
	/// </summary>
	private static void reset ()
	{
#if UNITY_ANDROID
			androidPlayer.Call ("stop", new object[]{});
#elif UNITY_STANDALONE_WIN
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
#endif
	}
	
	void OnDisable ()
	{
#if UNITY_ANDROID
			androidPlayer.Call ("stop", new object[]{});
#endif
	}
}
