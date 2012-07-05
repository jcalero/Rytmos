using UnityEngine;
using System.Collections;

public static class AudioManager {
	
	public static FileReader freader;			// File Reader to get audio file
	public static float[][] peaks;				// Holds the triggers for the currently loaded audio file
	private static GameObject cam;
	private static AudioClip clip;
	private static bool songLoaded;
	private static string currentlyLoadedSong;
	public static int frequency;
	public static float audioLength;
	public static int[] loudPartTimeStamps;
		
	public static void initMusic(string pathToMusicFile) {
		
		if(cam!=null) {
			float[] data = new float[cam.audio.clip.samples];
			cam.audio.clip.GetData(data,0);
			peaks = SoundProcessor.getPeaks(new MockDecoder(data));
			songLoaded = true;
			currentlyLoadedSong = "xXBACKgroundMUSICXx";
		} else {
			// Read Audio Data
			freader = new FileReader (pathToMusicFile);
			int success = (int)freader.read ();
			while (freader.isReading())
				yieldRoutine ();
			if(success != (int)FileReader.ReadStatus.SUCCESS)
				return;
				
			clip = freader.getClip();
			frequency = freader.getFrequency();
			audioLength = freader.getAudioLengthInSecs();
			currentlyLoadedSong = pathToMusicFile;
			
			peaks = SoundProcessor.getPeaks(freader);
			loudPartTimeStamps = SoundProcessor.findVolumeLevels(freader);
			
			freader = null;
			songLoaded = true;
		}
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
