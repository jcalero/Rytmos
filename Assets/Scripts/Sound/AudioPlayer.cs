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
	#endregion
	
	void Awake ()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		if(Game.GameMode != Game.Mode.Tutorial) {
			androidPlayer = new AndroidJavaObject("android.media.MediaPlayer",new object[]{});
			androidPlayer.Call("setDataSource",new object[]{Game.Song});
			androidPlayer.Call("prepare",new object[]{});
		}
		else {
			audioSource = gameObject.GetComponentInChildren<AudioSource> (); // get references to audiosources
		}
#elif UNITY_WEBPLAYER
		AudioSource[] audioSources = gameObject.GetComponentsInChildren<AudioSource> (); // get references to audiosources
		if(Game.Song == "Jazz-Fog") audioSource = audioSources[1];
		else if(Game.Song == "KnoxCanyon") audioSource = audioSources[2];
		else if(Game.Song == "LG-F1") audioSource = audioSources[3];
		else if(Game.Song == "YouGotToChange") audioSource = audioSources[4];
		else audioSource = audioSources[0];
		
		AudioManager.audioLength = audioSource.clip.length;
		AudioManager.frequency = audioSource.clip.frequency;
#else
		if(Game.GameMode != Game.Mode.Tutorial) {
			audioSource = gameObject.GetComponentInChildren<AudioSource> (); // get references to audiosources
			audioSource.clip = AudioManager.getAudioClip ();
		} else {
			AudioSource[] audioSources = gameObject.GetComponentsInChildren<AudioSource> (); // get references to audiosources
			audioSource = audioSources[0];
			AudioManager.audioLength = audioSource.clip.length;
			AudioManager.frequency = audioSource.clip.frequency;
		}
#endif
		isPlaying = false;
		timer = 0;
	}
	
	// Start playing automatically, may want to get rid of it doing that..
	void Start ()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		if(Game.GameMode != Game.Mode.Tutorial) {
			androidPlayer.Call ("setVolume", new object[]{Game.MusicVolume,Game.MusicVolume});
			androidPlayer.Call ("start", new object[]{});
		}
		else {
			audioSource.volume = Game.MusicVolume;
			audioSource.Play ();
		}
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
		if(!isPlaying && Game.GameMode != Game.Mode.Tutorial && androidPlayer.Call<int>("getCurrentPosition") > 171)
			isPlaying = true;
		else if(!isPlaying && Game.GameMode == Game.Mode.Tutorial && audioSource.timeSamples > 0)
			isPlaying = true;
#else
		if(!isPlaying && audioSource.timeSamples > 0)
			isPlaying = true;
#endif		
		timer += Time.deltaTime;
		if (!Game.Paused) {
			if (timer >= AudioManager.audioLength) {
#if UNITY_ANDROID && !UNITY_EDITOR
		if(Game.GameMode != Game.Mode.Tutorial) {		
			androidPlayer.Call ("stop", new object[]{});
			Debug.Log("timer made audio stop");
		}
		else audioSource.Stop ();
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
		if(Game.GameMode != Game.Mode.Tutorial)
			androidPlayer.Call ("pause", new object[]{});
		else if (audioSource != null && audioSource.isPlaying)
			audioSource.Pause ();
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
		if(Game.GameMode != Game.Mode.Tutorial)
			androidPlayer.Call ("start", new object[]{});
		else if (audioSource != null && !audioSource .isPlaying)
			audioSource .Play ();
#else
			if (audioSource != null && !audioSource .isPlaying)
						audioSource .Play ();
#endif
	}
	
	void OnDisable ()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		if(Game.GameMode != Game.Mode.Tutorial) {
			androidPlayer.Call ("stop", new object[]{});
			androidPlayer.Call ("reset", new object[]{});
			androidPlayer.Call ("release", new object[]{});
			androidPlayer.Dispose();
			androidPlayer = null;
		}
#endif
	}
	
	void OnApplicationQuit() {
		if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor || Game.GameMode == Game.Mode.Tutorial) {
			AudioManager.clear ();
		}
	}
}
