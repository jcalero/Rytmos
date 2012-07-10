using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PeakTriggerManager : MonoBehaviour {
	
	private static List<PeakListener> listeners = new List<PeakListener>();
	private int loudPartCounter;
	private float timer;
	private int[] counters;
	private GameObject cam;
	private bool loudFlag;
	
	void Awake() {
		
		
		cam = GameObject.Find ("Main Camera");
		if(Game.Song != null && Game.Song != "") {
			if(Game.Song != AudioManager.getCurrentSong()) AudioManager.initMusic(Game.Song);
		} else if (cam.audio.clip != null) {
			if(!AudioManager.isSongLoaded()) {
				AudioManager.setCam(this.cam);
				AudioManager.initMusic("");
			}
		}
		while(!AudioManager.isSongLoaded()) yieldRoutine();
		
		cam.audio.clip = AudioManager.getAudioClip();
		cam.audio.loop = false;
		counters = new int[AudioManager.peaks.Length];
		timer = 0f;		
		loudPartCounter = 0;
		loudFlag = false;
		cam.audio.Play();
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
		if (timer >= AudioManager.audioLength || (!Game.Paused && !cam.audio.isPlaying && timer > 0)){
			cam.audio.Stop();		
		} else if(AudioManager.peaks != null && cam != null && cam.audio.isPlaying) {
			timer += Time.deltaTime;
		}
		
		/* Update the flags for loud parts of a song */
		if(loudPartCounter < AudioManager.loudPartTimeStamps.Length && AudioManager.loudPartTimeStamps[loudPartCounter]/(float)AudioManager.frequency < timer+0.5f) {
			loudFlag = !loudFlag;
			foreach(PeakListener l in listeners) l.setLoudFlag(loudFlag);
			loudPartCounter++;			
		}
		
		// Iterate over every channel
		for(int t = 0; t < AudioManager.peaks.Length; t++) {
			
			// Sync peaks (can call them triggers) to the music
			while(counters[t] < AudioManager.peaks[t].Length && AudioManager.peaks[t][counters[t]] * (1024f/(float)AudioManager.frequency) < timer) {
				foreach(PeakListener l in listeners) l.onPeakTrigger(t);
				counters[t]++;
			}
		}
				
	}
	
	public static void addSelfToListenerList(PeakListener self) {
		listeners.Add(self);
	}
	
	public static void removeSelfFromListenerList(PeakListener self) {
		listeners.Remove(self);
	}
	
			
	public static IEnumerator yieldRoutine () {
		yield return 0;
	}
}
