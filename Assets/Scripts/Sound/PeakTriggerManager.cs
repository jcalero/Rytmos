using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PeakTriggerManager : MonoBehaviour
{
	
	#region vars
	private static List<PeakListener> listeners; // List of object which are to be alerted when an event occurs
	private int loudPartCounter;											// Iterator for the loud parts
	private float timer;													// Keep track of time
	private int[] peakCounters;												// Iterator for the array of peaks
	private int loudFlag;													// Flag to check how loud the song currently is
	public static float[] timeThreshs;
	public static float[] peakReductionFactors;
	#endregion
	
	void Awake() {
		listeners = new List<PeakListener> ();	
	}
	
	// Init
	void Start ()
	{
		peakCounters = new int[AudioManager.peaks.Length];
//		if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) {
//			timer = -0.15f;		
//		} else timer = -0.25f;
		timer = 0f;
		loudPartCounter = 0;
		loudFlag = -1;
		timeThreshs = new float[AudioManager.peaks.Length];
		peakReductionFactors = new float[AudioManager.peaks.Length];
		if(Game.GameMode == Game.Mode.Casual) peakReductionFactors[0] = 2f;
		else peakReductionFactors[0] = 1f;
		peakReductionFactors[1] = 0;
		peakReductionFactors[2] = 0;
		peakReductionFactors[3] = 1f;
	}
	
	/// <summary>
	/// Iterate through all the peaks and trigger them in sync with the time
	/// </summary>
	void Update ()
	{
		if(AudioPlayer.isPlaying) {
			timer += Time.deltaTime;
			
			/* Update the flags for loud parts of a song */
			if (loudPartCounter < AudioManager.loudPartTimeStamps.Length-1 && AudioManager.loudPartTimeStamps [loudPartCounter] / (float)AudioManager.frequency < timer) {
				loudFlag = AudioManager.loudPartTimeStamps[loudPartCounter+1];
				foreach (PeakListener l in listeners) l.setLoudFlag (loudFlag);
				loudPartCounter+=2;			
			}
	
			// Iterate over every channel
			for (int t = 0; t < AudioManager.peaks.Length; t++) {
				
				// Sync peaks (can call them triggers) to the music
				while (peakCounters[t] < AudioManager.peaks[t].Length-1 && AudioManager.peaks[t][peakCounters[t]] * (1024f/(float)AudioManager.frequency) < timer) {
					#region TRIGGER PEAKS
					// Call the trigger methods in the classes which have "registered" with peakTriggerManager
					foreach (PeakListener l in listeners) l.onPeakTrigger (t,AudioManager.peaks[t][peakCounters[t]+1]);
					#endregion
					
					#region TIME THRESHOLDING
					// Recalculate the time thresholds
					int start = peakCounters[t] - 10;
					if(start < 0) start = 0;
					
					int end = peakCounters[t] + 10;
					if(end > AudioManager.peaks[t].Length) end = AudioManager.peaks[t].Length;
					
					// Find average time between peaks using the sourrounding 10 peaks
					float timeAverage = 0f;
					for(int i = start+2; i < end; i+=2) {
						timeAverage += AudioManager.peaks[t][i] - AudioManager.peaks[t][i-2];
					}
					timeAverage /= ((end-start)/2)-1;
					timeAverage *= 1024f/(float)AudioManager.frequency;
									
					// Update the time thresholds according to the reduction factors
					timeThreshs[t] = Mathf.Floor(peakReductionFactors[t]/timeAverage)*0.95f*timeAverage;
					if(timeThreshs[t] < 0.1) timeThreshs[t] = 0.1f;
					#endregion
								
					// Update the "pointer"
					peakCounters [t]+=2;
				}
			}
		}
	}
	
	/// <summary>
	/// Use this method to add a "this"-reference to the PeakTriggerManager, so that it can alert "this" when a peak occurs
	/// </summary>
	/// <param name='self'>
	/// The "this"-reference
	/// </param>
	public static void addSelfToListenerList (PeakListener self)
	{
		listeners.Add (self);
	}
	
	/// <summary>
	/// Removes the "this"-reference from the trigger list.
	/// </summary>
	/// <param name='self'>
	/// The "this"-reference
	/// </param>
	public static void removeSelfFromListenerList (PeakListener self)
	{
		listeners.Remove (self);
	}
	
	public static IEnumerator yieldRoutine ()
	{
		yield return 0;
	}
}
