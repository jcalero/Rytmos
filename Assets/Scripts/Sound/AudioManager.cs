using UnityEngine;
using System.Collections;

public static class AudioManager {
	
	public static FileReader freader;			// File Reader to get audio file
	public static int[][] triggers;				// Holds the triggers for the currently loaded audio file

		
	public static void initMusic(string pathToMusicFile) {
		
		// Read Audio Data
		freader = new FileReader (pathToMusicFile, FileReader.AudioFormat.WAV);
		freader.read ();
		while (freader.isReading())
			yieldRoutine ();
		
		// Calculate the triggers for the music
		SoundProcessor sProc = new SoundProcessor (freader.getData (), 128);
		triggers = sProc.calculateAllTheTriggers ();
//		Debug.Log(triggers[0].Length);
//		Debug.Log(triggers[1].Length);
//		Debug.Log(triggers[2].Length);
//		Debug.Log(triggers[3].Length);
//		Debug.Break();
		
	}
	
	private static IEnumerator yieldRoutine () {
		yield return 0;
	}
	
}
