using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

/// <summary>
/// Audio manager. Static class which can be accessed from anywhere to read information about the loaded music file.
/// </summary>
public static class AudioManager {
	
	public static FileReader freader;			// File Reader to get audio file
	public static float[][] peaks;				// Holds the triggers for the currently loaded audio file
	public static int[] loudPartTimeStamps;		// Holds the triggers for loud/quiet parts of the audio file
	private static GameObject cam;				// Main Camera reference. If set, the ingame music is used instead of anything loaded at runtime!
	private static AudioClip buffer1Clip;		// AudioClip reference for the loaded music file
	private static AudioClip buffer2Clip;		// AudioClip reference for the loaded music file
	private static bool songLoaded;				// Flag if the song has finished loading
	private static string currentlyLoadedSong;	// Path to the currently loaded song (can be used to check if a new song is loaded)
	public static int frequency;				// Sampling Frequency of the music file, needed for time syncing
	public static float audioLength;			// Total length in seconds (float) of the music file
	public static int audioBufferSize;
	private static bool buffer1Played;
	private static bool buffer2Played;
	public static bool lastSamples;
	private static int currentBuffer;
//	private static Object locker;
	private static Thread readThread;
	private static bool runThread;
	private static bool updateOnce;
	
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
			//clip = freader.getClip();			
			frequency = freader.getFrequency();
			audioLength = freader.getAudioLengthInSecs();
			currentlyLoadedSong = pathToMusicFile;
			
			start = Time.realtimeSinceStartup;
			
			// Check if this file already exists!
			string cacheFile = FileWriter.convertToCacheFileName(pathToMusicFile);
			System.IO.FileInfo fInf = new System.IO.FileInfo(cacheFile);
			if(fInf.Exists) {
				FileReader rytFile = new FileReader(cacheFile);
				success = rytFile.read();
				while(rytFile.isReading())
					yieldRoutine();
				if(success != FileReader.ReadStatus.SUCCESS) return;
				//else..
				peaks = rytFile.getPeaks();
				loudPartTimeStamps = rytFile.getLoudnessData();
				rytFile.close();
				rytFile = null;
				
			} else {
				// Do the actual analysis!
				peaks = SoundProcessor.getPeaks(freader);
				loudPartTimeStamps = SoundProcessor.findVolumeLevels(freader);
				Debug.Log("Time to analyze: " + (Time.realtimeSinceStartup-start));			
				//Application.persistentDataPath;
				FileWriter.writeAnalysisData(pathToMusicFile,peaks,loudPartTimeStamps);
			}
			
			// Initialize our audio buffer	
			freader.reset();
			audioBufferSize = frequency*freader.getChannels();
			Debug.Log(audioBufferSize);
			buffer1Played = true;
			buffer2Played = true;
			lastSamples = false;
			currentBuffer = 1;
			
			buffer1Clip = AudioClip.Create("main_music1",audioBufferSize/freader.getChannels(),freader.getChannels(),frequency,true,false);
			buffer2Clip = AudioClip.Create("main_music2",audioBufferSize/freader.getChannels(),freader.getChannels(),frequency,true,false);
			
			// Fill audio buffer with the first few samples
			initBuffers();
			songLoaded = true;
		}
	}
	
	public static int getCurrentAmplitude(int sample) {
		if(buffer1Clip == null) return 0;
		sample = sample - sample/2;
		if(sample < 0) sample = 0;
		float[] data = new float[frequency/50];
		if(sample + data.Length > buffer1Clip.samples) sample = buffer1Clip.samples - data.Length;
		buffer1Clip.GetData(data,sample);
		
		float mean = 0;
		for(int i = 0; i < data.Length; i++) {
				mean += data[i]*100.0f;
		}
		mean /= data.Length;
		return (int)mean;
	}
	
	private static void initBuffers() {
		float[] buffer = new float[audioBufferSize];
		Debug.Log("buffer: "+ buffer.Length);
		
		freader.readSamples(ref buffer,false);
		
		buffer1Clip.SetData(buffer,0);
	}

	public static IEnumerator updateMusic() {
		if(buffer1Clip == null || buffer2Clip == null || lastSamples) yield return 0;
	
		float[] buffer = new float[audioBufferSize];
		
		if(currentBuffer == 2 && buffer1Played) {
			// Swap out bottom half
			Debug.Log("updating bottom half");
			if(freader.readSamples(ref buffer,false) < buffer.Length) lastSamples = true;
			
			buffer1Clip.SetData(buffer,0);
			
			buffer1Played = false;
			buffer2Played = true;
			currentBuffer = 1;
			
		} else if(currentBuffer == 1 && buffer2Played) {
			// Swap out top half
			//System.Array.Copy(buffer,0,audioBuffer,buffer.Length,buffer.Length);
			Debug.Log("updating top half");
			if(freader.readSamples(ref buffer,false) < buffer.Length) lastSamples = true;

			buffer2Clip.SetData(buffer,0);

			buffer2Played = false;
			buffer1Played = true;
			currentBuffer = 2;
		}
		yield return 0;
	}
	
	public static void closeMusicStream() {
		freader.close();
		freader = null;
	}
	
	public static string getCurrentSong() {
		return currentlyLoadedSong;	
	}
	
	public static bool isSongLoaded () {
		return songLoaded;
	}
	
	public static AudioClip getAudioClip() {
		if(currentBuffer == 1) return buffer1Clip;
		return buffer2Clip;
	}
	
	public static AudioClip getAudioClip(int clip) {
		if(clip == 1) return buffer1Clip;
		else if(clip == 2) return buffer2Clip;
		else return null;
	}
	
	public static void setCam(GameObject newCam) {
		cam = newCam;
	}
	
	public static IEnumerator yieldRoutine () {
		yield return 0;
	}
	
}
