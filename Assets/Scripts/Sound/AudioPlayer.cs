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
	private static float timer;					// Timer which is used to check if the song has finished playing!
	private static AndroidJavaObject androidPlayer;
	private static IntPtr playerPointer;
	#endregion
	
	void Awake ()
	{
		Debug.Log("awake");
		audioSources = gameObject.GetComponentsInChildren<AudioSource> (); // get references to audiosources
		
		// Check whether we want to load in a song (Game.Song is set) or use one which has been provided as an asset
		if (Game.Song != null && Game.Song != "") {
			if (Game.Song != AudioManager.getCurrentSong ())
				AudioManager.initMusic (Game.Song);
		} else if (audioSources [0].clip != null) {
			if (!AudioManager.isSongLoaded ()) {
				AudioManager.setCam (audioSources [0]);
				AudioManager.initMusic ("");
			}
		}
		while (!AudioManager.isSongLoaded()) {
		} // idlewait

		if(Application.platform == RuntimePlatform.Android) {
			androidPlayer = new AndroidJavaObject("android.media.MediaPlayer",new object[]{});
			playerPointer = androidPlayer.GetRawObject();
			androidPlayer.Call("setDataSource",new object[]{Game.Song});
			androidPlayer.Call("prepare",new object[]{});
		}else if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
			audioSources [0].clip = AudioManager.getAudioClip ();
		}
		
	}
	
	// Start playing automatically, may want to get rid of it doing that..
	void Start ()
	{
		if(Application.platform == RuntimePlatform.Android) {
			androidPlayer.Call ("start", new object[]{});
		} else if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
			audioSources [0].Play ();
		}
	}
	
	/// <summary>
	/// In here we keep track of the buffers and update the accordingly
	/// </summary>
	void Update ()
	{
		if (!Game.Paused) {
			if (timer >= AudioManager.audioLength) {
				if(Application.platform == RuntimePlatform.Android) {
					androidPlayer.Call ("stop", new object[]{});
				} else if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
					audioSources [0].Stop ();
				}
			} else timer += Time.deltaTime;
		}
	}
	
	/// <summary>
	/// Play the music. NOT TESTED YET!
	/// </summary>
	public static void play ()
	{
		if(Application.platform == RuntimePlatform.Android) { 
			androidPlayer.Call ("stop", new object[]{});
			androidPlayer.Call ("start", new object[]{});
		} else if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
			if (audioSources [0] != null) {
				audioSources [0].Stop ();
				audioSources [0].Play ();
			}
		}		
	}
	
	/// <summary>
	/// Pause playback.
	/// </summary>
	public static void pause ()
	{
		if(Application.platform == RuntimePlatform.Android) {
			androidPlayer.Call ("pause", new object[]{});
		} else if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
			if (audioSources [0] != null && audioSources [0].isPlaying)
						audioSources [0].Pause ();
		}
	}
	
	/// <summary>
	/// Resume playback.
	/// </summary>
	public static void resume ()
	{
		if(Application.platform == RuntimePlatform.Android) {
			androidPlayer.Call ("start", new object[]{});
		} else if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
			if (audioSources [0] != null && !audioSources [0].isPlaying)
						audioSources [0].Play ();
		}
	}
	
	void OnDisable ()
	{
		Debug.Log("ondisable");
		if(Application.platform == RuntimePlatform.Android) {
			androidPlayer.Call ("stop", new object[]{});
			androidPlayer.Call ("reset", new object[]{});
			androidPlayer.Call ("release", new object[]{});
			androidPlayer.Dispose();
			playerPointer = IntPtr.Zero;
			androidPlayer = null;
		}
	}
	
	void OnApplicationQuit() {
		if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
			AudioManager.clear ();
		}
	}
	
	/* DEPRECATED
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
		
	} */
	
		/* DEPRECATED
	/// <summary>
	/// Reset so that we can start playing from the beginning.
	/// </summary>
	private static void reset ()
	{
if(Application.platform == RuntimePlatform.Android) {
			androidPlayer.Call ("stop", new object[]{});
else if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor
) {
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

	}*/
}
