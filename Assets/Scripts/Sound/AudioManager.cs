using UnityEngine;
using System.Collections;

public static class AudioManager {
	
	public static FileReader freader;			// File Reader to get audio file
	public static int[][] triggers;				// Holds the triggers for the currently loaded audio file
	private static GameObject cam;

		
	public static void initMusic(string pathToMusicFile) {
		
		SoundProcessor sProc;
		if(cam!=null) {
			float[] data = new float[cam.audio.clip.samples];
			cam.audio.clip.GetData(data,0);
			sProc = new SoundProcessor(data,128);
		} else {
			// Read Audio Data
			freader = new FileReader (pathToMusicFile, FileReader.AudioFormat.WAV);
			freader.read ();
			while (freader.isReading())
				yieldRoutine ();
			sProc = new SoundProcessor (freader.getData (), 128);
		}
		triggers = sProc.calculateAllTheTriggers (44100,1000,4);
		
		// Calculate the triggers for the music
		//SoundProcessor sProc = new SoundProcessor (freader.getData (), 128);
		//triggers = sProc.calculateAllTheTriggers ();
		Debug.Log(triggers[0].Length);
		Debug.Log(triggers[1].Length);
		Debug.Log(triggers[2].Length);
		Debug.Log(triggers[3].Length);
//		Debug.Break();
		
	}
	
	
	
	public static void setCam(GameObject newCam) {
		cam = newCam;
	}
	
	public static IEnumerator yieldRoutine () {
		yield return 0;
	}
	
}
