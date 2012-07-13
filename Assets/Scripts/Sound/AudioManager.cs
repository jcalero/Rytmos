using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

/// <summary>
/// Audio manager. Static class to provide information about the loaded music file.
/// </summary>
public static class AudioManager
{
	#region public vars
	/*Variables provided for other classes*/
	public static FileReader freader;			// File Reader reference which does all the I/O
	public static float[][] peaks;				// Holds the triggers for the currently loaded audio file
	public static int[] loudPartTimeStamps;		// Holds the triggers for loud/quiet parts of the audio file
	public static int frequency;				// Sampling Frequency of the music file, needed for time syncing
	public static int channels;					// Number of channels in the music file
	public static float audioLength;			// Total length in seconds (float) of the music file
	public static int audioBufferSize;			// How many samples we want to store in each buffer
	#endregion
	
	#region private vars
	/*Variables for internal logic*/
	private static AudioSource ingameMusic;		// Reference which needs to be set if we want to use music which we prvovide ingame!
	private static AudioClip audioClip;			// AudioClip reference for the buffered music file, part 1
	private static bool songLoaded = false;		// Flag if the song has finished loading
	private static string currentlyLoadedSong;	// Path to the currently loaded song (can be used to check if a new song is loaded)
	#endregion
	
	/// <summary>
	/// Initializes the static class.
	/// Loads the music, analyzes the music, stores useful information about it.
	/// THIS FUNCTION WILL ANALYZE MUSIC UNLESS IT FINDS A CACHE FILE!
	/// (analysis takes ages!)
	/// </summary>
	/// <param name='pathToMusicFile'>
	/// Path to music file.
	/// </param>
	public static void initMusic (string pathToMusicFile)
	{
		
		// If the cam object has been set, analyze the music file which is loaded as the background music in unity
		// May be useful for the story mode
		if (ingameMusic != null) {
			float[] data = new float[ingameMusic.audio.clip.samples];
			ingameMusic.audio.clip.GetData (data, 0);
			peaks = SoundProcessor.getPeaks (new MockDecoder (data));
			songLoaded = true;
			currentlyLoadedSong = "xXBACKgroundMUSICXx";
			// If the AudioSource object is not set, analyze the music file which has been passed
		} else {			
			// Read Audio Data/Initialize everything to read on the fly
			float start = Time.realtimeSinceStartup;
			freader = new FileReader (pathToMusicFile);
			FileReader.ReadStatus success = freader.read ();
			while (freader.isReading()) {
			}
			
			// Succeeded reading? (Which means it found the file when we're just streaming)
			if (success != FileReader.ReadStatus.SUCCESS)
				return;
			
			// Set useful information, like AudioClip,length,etc..	
			frequency = freader.getFrequency ();
			channels = freader.getChannels ();
			audioLength = freader.getAudioLengthInSecs ();
			currentlyLoadedSong = pathToMusicFile;
			
			start = Time.realtimeSinceStartup;
			
			// Check if we have a cache file for the current song
			string cacheFile = FileWriter.convertToCacheFileName (pathToMusicFile);
			System.IO.FileInfo fInf = new System.IO.FileInfo (cacheFile);
			
			if (fInf.Exists) {
				// We have a cache file, so we just read the peaks etc from there.
				FileReader rytFile = new FileReader (cacheFile);
				success = rytFile.read ();
				while (rytFile.isReading()) {
				}				
				if (success != FileReader.ReadStatus.SUCCESS)
					return;
				
				peaks = rytFile.getPeaks ();
				loudPartTimeStamps = rytFile.getLoudnessData ();
				rytFile.close ();
				rytFile = null;
				
			} else {
				// We have no cache file, so do the actual analysis!
				peaks = SoundProcessor.getPeaks (freader);
				loudPartTimeStamps = SoundProcessor.findVolumeLevels (freader);
				Debug.Log ("Time to analyze: " + (Time.realtimeSinceStartup - start));			
				//Application.persistentDataPath;
				FileWriter.writeAnalysisData (pathToMusicFile, peaks, loudPartTimeStamps);
			}
			
			if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
				Debug.Log("here");
				// Now that we have analyzed the song, we need to reset & initialize everything for playback
				freader.reset ();
				
				audioBufferSize = (int)(channels * Mathf.Ceil (audioLength) * frequency);
				
				Debug.Log (audioLength);
				Debug.Log (frequency);
				Debug.Log (channels);
				Debug.Log (freader.getFrameSize());
				Debug.Log (audioBufferSize);
				
				audioClip = AudioClip.Create ("main_music1", audioBufferSize, channels, frequency, false, false);
				
				// Fill audio buffer with the first few samples
				initAudioBuffer ();
			}
			closeMusicStream();
			Debug.Log ("Time to read: " + (Time.realtimeSinceStartup - start));
			songLoaded = true;
		}
	}
	
	/// <summary>
	/// Initializes the buffers for audio playback.
	/// </summary>
	private static void initAudioBuffer ()
	{		
		float[] buffer = new float[audioBufferSize];
		freader.readSamples (ref buffer, false);
		audioClip.SetData (buffer, 0);
		buffer = null;
		// Want to actually clear the buffer array! Call garbage collector
		System.GC.Collect();
	}
	
	/// <summary>
	/// Closes the music stream.
	/// </summary>
	private static void closeMusicStream ()
	{
		freader.close ();
		freader = null;
	}
	
	/// <summary>
	/// Gets the current amplitude. NOT IMPLEMENTED
	/// </summary>
	/// <returns>
	/// The current amplitude.
	/// </returns>
	/// <param name='sample'>
	/// Sample.
	/// </param>
	public static int getCurrentAmplitude (int sample)
	{
//		if(buffer1Clip == null) return 0;
//		sample = sample - sample/2;
//		if(sample < 0) sample = 0;
//		float[] data = new float[frequency/50];
//		if(sample + data.Length > buffer1Clip.samples) sample = buffer1Clip.samples - data.Length;
//		buffer1Clip.GetData(data,sample);
//		
//		float mean = 0;
//		for(int i = 0; i < data.Length; i++) {
//				mean += data[i]*100.0f;
//		}
//		mean /= data.Length;
//		return (int)mean;
		return 0;
	}
	
	/// <summary>
	/// Gets the string (path) of the song which is currently loaded by AudioManager
	/// </summary>
	/// <returns>
	/// The current song.
	/// </returns>
	public static string getCurrentSong ()
	{
		return currentlyLoadedSong;	
	}
	
	public static bool isSongLoaded ()
	{
		return songLoaded;
	}
	
	public static AudioClip getAudioClip ()
	{
		return audioClip;
	}
	
	public static void setCam (AudioSource reference)
	{
		ingameMusic = reference;
	}
	
	public static IEnumerator yieldRoutine ()
	{
		yield return 0;
	}
	
	public static void clear() {
		peaks = null;
		loudPartTimeStamps = null;
		audioClip = null;
	}
}
