using UnityEngine;
using System.Collections;
using System.Threading;
using System;

/// <summary>
/// Audio player.
/// Class which plays the background music.
/// Methods are static to allow access from anywhere.
/// </summary>
public class TutorialAudioPlayer : MonoBehaviour
{
	
	#region vars
	private static AudioSource audioSource;	// The two audiosources attached to the gameObject this class is attached to
	private static float timer;					// Timer which is used to check if the song has finished playing!
	public static bool isPlaying;
	#endregion
	
	void Awake ()
	{
		audioSource = gameObject.GetComponentInChildren<AudioSource> (); // get references to audiosources
		isPlaying = false;
		timer = 0;
	}
	
	// Start playing automatically, may want to get rid of it doing that..
	void Start ()
	{
			audioSource.volume = Game.MusicVolume;
			audioSource.Play ();
	}
	
	/// <summary>
	/// In here we keep track of the buffers and update the accordingly
	/// </summary>
	void Update ()
	{
		if(!isPlaying && audioSource.timeSamples > 0)
			isPlaying = true;
		timer += Time.deltaTime;
		if (!Game.Paused) {
			if (timer >= AudioManager.audioLength) {
				audioSource.Stop ();
			}
		}
	}
	
	/// <summary>
	/// Play the music. NOT TESTED YET!
	/// </summary>
	public static void play ()
	{		
		if (audioSource != null) {
			audioSource.Stop ();
			audioSource.Play ();
		}
	}
	
	/// <summary>
	/// Pause playback.
	/// </summary>
	public static void pause ()
	{
		if (audioSource != null && audioSource.isPlaying)
					audioSource.Pause ();
	}
	
	/// <summary>
	/// Resume playback.
	/// </summary>
	public static void resume ()
	{
		if (audioSource != null && !audioSource .isPlaying)
					audioSource .Play ();
	}
	
	void OnDisable ()
	{
	}
	
	void OnApplicationQuit() {
			AudioManager.clear ();
	}
}
