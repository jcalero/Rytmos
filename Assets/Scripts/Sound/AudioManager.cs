using UnityEngine;
using System.Collections;

public static class AudioManager {
	
	public static FileReader freader;			// File Reader to get audio file
	public static ArrayList peaks;				// Holds the triggers for the currently loaded audio file
	private static GameObject cam;

		
	public static void initMusic(string pathToMusicFile) {
		
		if(cam!=null) {
			float[] data = new float[cam.audio.clip.samples];
			cam.audio.clip.GetData(data,0);
			peaks = SoundProcessor.getPeaks(new MockDecoder(data));
		} else {
			// Read Audio Data
			freader = new FileReader (pathToMusicFile, FileReader.AudioFormat.WAV);
			freader.read ();
			while (freader.isReading())
				yieldRoutine ();
			peaks = SoundProcessor.getPeaks(new MockDecoder(freader.getData()));
		}
		
		Debug.Log("how many bands: " + peaks.Count);
		for(int i = 0; i < peaks.Count; i++) {
			
			int count = 0;
			Debug.Log("how many entries in peaks " + i + ": " + ((ArrayList)peaks[i]).Count);
			for(int k = 0; k < ((ArrayList)peaks[i]).Count; k++) {
				if((float)((ArrayList)peaks[i])[k] > 0) count++;
			}
			Debug.Log("number of peaks in " + i + ": " + count);
		}
		Debug.Log("#####################################");
	}
	
	
	
	public static void setCam(GameObject newCam) {
		cam = newCam;
	}
	
	public static IEnumerator yieldRoutine () {
		yield return 0;
	}
	
}
