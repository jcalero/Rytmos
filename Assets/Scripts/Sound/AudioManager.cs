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
			freader = new FileReader (pathToMusicFile);
			int success = (int)freader.read ();
			while (freader.isReading())
				yieldRoutine ();
			if(success == (int)FileReader.ReadStatus.SUCCESS)
				peaks = SoundProcessor.getPeaks(freader);
		}
	}
	
	
	
	public static void setCam(GameObject newCam) {
		cam = newCam;
	}
	
	public static IEnumerator yieldRoutine () {
		yield return 0;
	}
	
}
