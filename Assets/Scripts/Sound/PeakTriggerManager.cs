using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PeakTriggerManager : MonoBehaviour {
	
	private static List<PeakListener> listeners = new List<PeakListener>();
	private int loudPartCounter;
	private float timer;
	private int[] counters;
	private bool loudFlag;	
	
	// Use this for initialization
	void Start () {
		counters = new int[AudioManager.peaks.Length];
		timer = 0f;		
		loudPartCounter = 0;
		loudFlag = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		timer += Time.deltaTime;
		
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
	
	void OnDestroy() {
		AudioManager.closeMusicStream();	
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
