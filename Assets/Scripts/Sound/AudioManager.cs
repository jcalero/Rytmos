using UnityEngine;
using System.Collections;

/// <summary>
/// Audio manager. Static class which can be accessed from anywhere to read information about the loaded music file.
/// </summary>
public static class AudioManager {
	
	public static FileReader freader;			// File Reader to get audio file
	public static float[][] peaks;				// Holds the triggers for the currently loaded audio file
	public static int[] loudPartTimeStamps;		// Holds the triggers for loud/quiet parts of the audio file
	private static GameObject cam;				// Main Camera reference. If set, the ingame music is used instead of anything loaded at runtime!
	private static AudioClip clip;				// AudioClip reference for the loaded music file
	private static bool songLoaded;				// Flag if the song has finished loading
	private static string currentlyLoadedSong;	// Path to the currently loaded song (can be used to check if a new song is loaded)
	public static int frequency;				// Sampling Frequency of the music file, needed for time syncing
	public static float audioLength;			// Total length in seconds (float) of the music file
	
	/// <summary>
	/// Initializes the static class.
	/// Loads the music, analyzes the music, stores useful information about it.
	/// THIS FUNCTION WILL REANALYZE MUSIC! LONG RUNTIME!
	/// </summary>
	/// <param name='pathToMusicFile'>
	/// Path to music file.
	/// </param>
	public static void initMusic(string pathToMusicFile) {
		
		// If the cam object has been set, analyze the music file which is loaded as the background music in unity
		// May be useful for the story mode
		if(cam!=null) {
			float[] data = new float[cam.audio.clip.samples];
			cam.audio.clip.GetData(data,0);
			peaks = SoundProcessor.getPeaks(new MockDecoder(data));
			songLoaded = true;
			currentlyLoadedSong = "xXBACKgroundMUSICXx";
		// If the cam object is not set, analyze the music file which has been passed
		} else {
			// Read Audio Data
			float start = Time.realtimeSinceStartup;
			freader = new FileReader (pathToMusicFile);
			FileReader.ReadStatus success = freader.read ();
			while (freader.isReading())
				yieldRoutine ();
			
			Debug.Log("Time to read: " + (Time.realtimeSinceStartup-start));
			
			// Succeeded reading?
			if(success != FileReader.ReadStatus.SUCCESS)
				return;
			
			// Set useful information, like AudioClip,length,etc..
			clip = freader.getClip();
			frequency = freader.getFrequency();
			audioLength = freader.getAudioLengthInSecs();
			currentlyLoadedSong = pathToMusicFile;
			
			start = Time.realtimeSinceStartup;
			
			// Check if this file already exists!
			string cacheFile = FileWriter.convertToCacheFileName(pathToMusicFile);
			System.IO.FileInfo fInf = new System.IO.FileInfo(cacheFile);
			if(fInf.Exists) {
				freader = new FileReader(cacheFile);
				success = freader.read();
				while(freader.isReading())
					yieldRoutine();
				if(success != FileReader.ReadStatus.SUCCESS) return;
				//else..
				peaks = freader.getPeaks();
				loudPartTimeStamps = freader.getLoudnessData();
				foreach(int i in loudPartTimeStamps) Debug.Log("bbb" + i);
				
			} else {
				// Do the actual analysis!
				peaks = SoundProcessor.getPeaks(freader);
				loudPartTimeStamps = SoundProcessor.findVolumeLevels(freader);
				Debug.Log("Time to analyze: " + (Time.realtimeSinceStartup-start));			
				//Application.persistentDataPath;
				
				FileWriter.writeAnalysisData(pathToMusicFile,peaks,loudPartTimeStamps);
			}
			// Cleanup
			freader = null;
			songLoaded = true;
		}
	}
	
	public static int getCurrenAmplitude(int sample) {
		if(clip == null) return 0;
		sample = sample - sample/2;
		if(sample < 0) sample = 0;
		float[] data = new float[frequency/50];
		if(sample + data.Length > clip.samples) sample = clip.samples - data.Length;
		clip.GetData(data,sample);
		
		float mean = 0;
		for(int i = 0; i < data.Length; i++) {
				mean += data[i]*100.0f;
		}
		mean /= data.Length;
		return (int)mean;
	}
	
	public static string getCurrentSong() {
		return currentlyLoadedSong;	
	}
	
	public static bool isSongLoaded () {
		return songLoaded;
	}
	
	public static AudioClip getAudioClip() {
		return clip;
	}
	
	public static void setCam(GameObject newCam) {
		cam = newCam;
	}
	
	public static IEnumerator yieldRoutine () {
		yield return 0;
	}
	
}
