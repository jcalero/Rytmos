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
	public static int[][] peaks;				// Holds the triggers for the currently loaded audio file
	public static int[] loudPartTimeStamps;		// Holds the triggers for loud/quiet parts of the audio file
	public static float variationFactor;
	public static int frequency;				// Sampling Frequency of the music file, needed for time syncing
	public static int channels;					// Number of channels in the music file
	public static float audioLength;			// Total length in seconds (float) of the music file
	public static int audioBufferSize;			// How many samples we want to store in each buffer
	public static string artist = "Unknown";	// id3 data
	public static string title = "Unknown";		// id3 data
	public static float loadingProgress;		// Shows how much of the sound has been processed. Float between 0 and 1.
	public static volatile bool isWritingCacheFile;	// Used so that we don't abort while having an open filehandle.
	public static bool songLoaded = false;		// Flag if the song has finished loading
	public static bool tagDataSet = false;		// Flag to announce that we have read the id3 data
	#endregion
	
	#region private vars
	/*Variables for internal logic*/
	private static AudioClip audioClip;			// AudioClip reference for the buffered music file, part 1
	private static string currentlyLoadedSong;	// Path to the currently loaded song (can be used to check if a new song is loaded)
	private static System.Threading.Thread soundProcessingThread; // The thread which is created to do he analysis
	private static bool abortSoundProcessing;	// Flag to cancel analysis
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
	public static IEnumerator initMusic (string pathToMusicFile)
	{
		// Set flags
		songLoaded = false;
		tagDataSet = false;
		abortSoundProcessing = false;
		isWritingCacheFile = false;
		loadingProgress = 0;
		
#if UNITY_WEBPLAYER
		// For the WebPlayer: Just get everything from WebPlayerRytData.cs
		peaks = WebPlayerRytData.getPeaks(Game.Song);
		loudPartTimeStamps = WebPlayerRytData.getLoudFlags(Game.Song);
		variationFactor = WebPlayerRytData.getVariationFactor(Game.Song);
		songLoaded = true;
		currentlyLoadedSong = "xXBACKgroundMUSICXx";
		yield break;
#else	
		
		// For Tutorial: Just get everything from TutorialRytData.cs
		if(Game.GameMode == Game.Mode.Tutorial) {
			peaks = TutorialRytData.getPeaks();
			loudPartTimeStamps = TutorialRytData.getLoudFlags();
			variationFactor = TutorialRytData.getVariationFactor();
			songLoaded = true;
			currentlyLoadedSong = "xXBACKgroundMUSICXx";
			tagDataSet = true;
			yield break;
		}
		
		// Initialize file handle
		float start = Time.realtimeSinceStartup;
		if(freader!=null) {
			freader.close();
			freader = null;
		}
		freader = new FileReader (pathToMusicFile);
		FileReader.ReadStatus success = freader.read (); // Doesn't really "read" (unless it's a WAV file)
		while (freader.isReading()) {
			yield return null;
		}
		
		// Succeeded reading? (Which means it found the file when we're just streaming)
		if (success != FileReader.ReadStatus.SUCCESS)
		{
			yield break;
		}
		
		// Set useful information, like AudioClip,length,etc..	
		frequency = freader.getFrequency ();
		channels = freader.getChannels ();
		audioLength = freader.getAudioLengthInSecs ();
		currentlyLoadedSong = pathToMusicFile;
		artist = freader.getArtist();
		title = freader.getTitle();
				
		tagDataSet = true;
		
		start = Time.realtimeSinceStartup;
		
		// Check if we have a cache file of the analyzed data for the current song
		string cacheFile = FileWriter.convertToCacheFileName (pathToMusicFile);
		System.IO.FileInfo cachedRytData = new System.IO.FileInfo (cacheFile);
		
		if (cachedRytData.Exists) {
			// We have a cache file, so we just read the peaks etc from there.
			FileReader rytFile = new FileReader (cacheFile);
			success = rytFile.read ();
			while (rytFile.isReading()) {
				yield return 0;
			}				
			if (success != FileReader.ReadStatus.SUCCESS)
			{
				yield break;
			}
			
			peaks = rytFile.getPeaks ();
			loudPartTimeStamps = rytFile.getLoudnessData ();
			variationFactor = rytFile.getVariationFactor();
			rytFile.close ();
			rytFile = null;
			
		} else {
			// We have no cache file, so do the actual analysis!
			soundProcessingThread = new System.Threading.Thread(() => SoundProcessor.analyse(freader));
			soundProcessingThread.Start();
			
			while(SoundProcessor.isAnalyzing) {
				loadingProgress = SoundProcessor.loadingProgress;
				if(abortSoundProcessing) {
					// ABORT: Cancel processing thread, release file handle & collect all dat garbage!
					SoundProcessor.abort();
					soundProcessingThread.Join();
					soundProcessingThread.Abort();
					soundProcessingThread = null;
					SoundProcessor.reset();
					freader.close();
					freader = null;
					System.GC.Collect();
					yield break;
				}
				yield return null;
			}
			
			isWritingCacheFile = true;
			SoundProcessor.reset();
			soundProcessingThread.Join();
			soundProcessingThread.Abort();
			soundProcessingThread = null;
			
			peaks = SoundProcessor.getPeaks ();
			loudPartTimeStamps = SoundProcessor.getVolumeLevels ();
			variationFactor = SoundProcessor.getVariationFactor();
			FileWriter.writeAnalysisData (pathToMusicFile, peaks, loudPartTimeStamps, variationFactor);
			isWritingCacheFile = false;
		}
		
		if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
			// Now that we have analyzed the song, we need to reset & initialize everything for playback
			freader.reset ();
			
			audioBufferSize = (int)(channels * Mathf.Ceil (audioLength) * frequency);
			audioClip = AudioClip.Create ("main_music1", audioBufferSize, channels, frequency, false, false);
			
			// Fill audio buffer with the first few samples
			initAudioBuffer ();
		}
		closeMusicStream();
		Debug.Log ("Song loaded in: " + (Time.realtimeSinceStartup - start) + " seconds");
		songLoaded = true;
		
		// Done a lot of work there, better clean up after ourselves!
		System.GC.Collect();
#endif
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
	}
	
	/// <summary>
	/// Closes the music stream.
	/// </summary>
	private static void closeMusicStream ()
	{
		freader.close ();
		freader = null;
	}
	
	public static void abort() {
		abortSoundProcessing = true;	
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
