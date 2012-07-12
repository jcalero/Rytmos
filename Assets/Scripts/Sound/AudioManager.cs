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
	public static bool lastSamples;				// Used to notify AudioPlayer that we have reached the end of the song
	#endregion
	
	#region private vars
	/*Variables for internal logic*/
	private static AudioSource ingameMusic;		// Reference which needs to be set if we want to use music which we prvovide ingame!
	private static AudioClip buffer1Clip;		// AudioClip reference for the buffered music file, part 1
	private static AudioClip buffer2Clip;		// AudioClip reference for the buffered music file, part 2
	private static bool songLoaded = false;		// Flag if the song has finished loading
	private static string currentlyLoadedSong;	// Path to the currently loaded song (can be used to check if a new song is loaded)
	
	/*Flags used to keep track of the two audiobuffers*/
	private static bool buffer1Played;
	private static bool buffer2Played;
	private static int currentBuffer;
	private static float[] updateBuffer;
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
			
			Debug.Log ("Time to read: " + (Time.realtimeSinceStartup - start));
			
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
			
			// Now that we have analyzed the song, we need to reset & initialize everything for playback
			freader.reset ();
			audioBufferSize = freader.getFrameSize()*10;
			if(audioBufferSize%freader.getFrameSize() != 0) {
				Debug.Log("This should not happen!!");
				return;
			}
			buffer1Played = true;
			buffer2Played = true;
			lastSamples = false;
			currentBuffer = 1;
			
			buffer1Clip = AudioClip.Create ("main_music1", audioBufferSize / channels, channels, frequency, false, false);
			buffer2Clip = AudioClip.Create ("main_music2", audioBufferSize / channels, channels, frequency, false, false);
			
			// Fill audio buffer with the first few samples
			initBuffers ();
			songLoaded = true;
		}
	}
	
	/// <summary>
	/// Initializes the buffers for audio playback.
	/// </summary>
	private static void initBuffers ()
	{
		buffer1Played = true;
		buffer2Played = true;
		lastSamples = false;
		currentBuffer = 1;
		
		float[] buffer = new float[audioBufferSize];
		
		freader.readSamples (ref buffer, false);
		
		buffer1Clip.SetData (buffer, 0);
	}
	
	/// <summary>
	/// Updates the music buffers: If we are playing the second buffer, update the first and vice versa.
	/// </summary>
	/// <returns>
	/// Nothing, really. Just the yield stuff for coroutine funcionality
	/// </returns>
	public static IEnumerator updateMusic ()
	{		
			if (buffer1Clip == null || buffer2Clip == null || lastSamples) {
			} else {
				updateBuffer = new float[audioBufferSize];
				
				if (currentBuffer == 2 && buffer1Played) {
					// Swap out bottom half
					if (freader.readSamples (ref updateBuffer, false) < updateBuffer.Length)
						lastSamples = true;
									
					buffer1Clip.SetData (updateBuffer, 0);
								
					buffer1Played = false;
					buffer2Played = true;
					currentBuffer = 1;
				
				} else if (currentBuffer == 1 && buffer2Played) {
					// Swap out top half
					if (freader.readSamples (ref updateBuffer, false) < updateBuffer.Length)
						lastSamples = true;
					
					buffer2Clip.SetData (updateBuffer, 0);
					
					buffer2Played = false;
					buffer1Played = true;
					currentBuffer = 2;
				}
			}
		yield return null;
	}
	
	/// <summary>
	/// Closes the music stream.
	/// </summary>
	public static void closeMusicStream ()
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
		if (currentBuffer == 1)
			return buffer1Clip;
		return buffer2Clip;
	}
	
	public static AudioClip getAudioClip (int clip)
	{
		if (clip == 1)
			return buffer1Clip;
		else if (clip == 2)
			return buffer2Clip;
		else
			return null;
	}
	
	public static void setCam (AudioSource reference)
	{
		ingameMusic = reference;
	}
	
	public static IEnumerator yieldRoutine ()
	{
		yield return 0;
	}
	
	public static void reset ()
	{
		freader.reset ();
		initBuffers ();
	}
	
}
