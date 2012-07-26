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
	private static AudioSource audioSource;	// The two audiosources attached to the gameObject this class is attached to
	private static float timer;					// Timer which is used to check if the song has finished playing!
#if UNITY_ANDROID && !UNITY_EDITOR
	private static AndroidJavaObject androidPlayer;
#endif
	public static bool isPlaying;
	private int test;
	#endregion
	
	void Awake ()
	{
		audioSource = gameObject.GetComponentInChildren<AudioSource> (); // get references to audiosources

#if UNITY_ANDROID && !UNITY_EDITOR
			androidPlayer = new AndroidJavaObject("android.media.MediaPlayer",new object[]{});
			androidPlayer.Call("setDataSource",new object[]{Game.Song});
			androidPlayer.Call("prepare",new object[]{});
#else
			audioSource.clip = AudioManager.getAudioClip ();
#endif
		isPlaying = false;
		timer = 0;
	}
	
	// Start playing automatically, may want to get rid of it doing that..
	void Start ()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
			androidPlayer.Call ("setVolume", new object[]{Game.MusicVolume,Game.MusicVolume});
			androidPlayer.Call ("start", new object[]{});
#else
			audioSource.volume = Game.MusicVolume;
			audioSource.Play ();
#endif
	}
	
	/// <summary>
	/// In here we keep track of the buffers and update the accordingly
	/// </summary>
	void Update ()
	{
		
#if UNITY_ANDROID && !UNITY_EDITOR
		if(!isPlaying && androidPlayer.Call<int>("getCurrentPosition") > 171)
			isPlaying = true;
#else
			if(!isPlaying && audioSource.timeSamples > 0)
			isPlaying = true;
#endif		
		timer += Time.deltaTime;
		if (!Game.Paused) {
			if (timer >= AudioManager.audioLength) {
#if UNITY_ANDROID && !UNITY_EDITOR
					androidPlayer.Call ("stop", new object[]{});
					Debug.Log("timer made audio stop");
#else
					audioSource.Stop ();
#endif
			}
		}
	}
	
	/// <summary>
	/// Play the music. NOT TESTED YET!
	/// </summary>
	public static void play ()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
			androidPlayer.Call ("stop", new object[]{});
			androidPlayer.Call ("start", new object[]{});
#else			
			if (audioSource != null) {
				audioSource.Stop ();
				audioSource.Play ();
			}
#endif
	}
	
	/// <summary>
	/// Pause playback.
	/// </summary>
	public static void pause ()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
			androidPlayer.Call ("pause", new object[]{});
#else
			if (audioSource != null && audioSource.isPlaying)
						audioSource.Pause ();
#endif
	}
	
	/// <summary>
	/// Resume playback.
	/// </summary>
	public static void resume ()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
			androidPlayer.Call ("start", new object[]{});
#else
			if (audioSource != null && !audioSource .isPlaying)
						audioSource .Play ();
#endif
	}
	
	void OnDisable ()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
			androidPlayer.Call ("stop", new object[]{});
			androidPlayer.Call ("reset", new object[]{});
			androidPlayer.Call ("release", new object[]{});
			androidPlayer.Dispose();
			androidPlayer = null;
#endif
	}
	
	void OnApplicationQuit() {
		if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
			AudioManager.clear ();
		}
	}
}
