using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PeakTriggerManager : MonoBehaviour
{
	
	#region vars
	private static List<PeakListener> listeners = new List<PeakListener> (); // List of object which are to be alerted when an event occurs
	private int loudPartCounter;											// Iterator for the loud parts
	private float timer;													// Keep track of time
	private int[] peakCounters;												// Iterator for the array of peaks
	private int loudFlag;													// Flag to check how loud the song currently is
	#endregion
	
	// Init
	void Start ()
	{
		peakCounters = new int[AudioManager.peaks.Length];
		timer = 0f;		
		loudPartCounter = 0;
		loudFlag = -1;
	}
	
	/// <summary>
	/// Iterate through all the peaks and trigger them in sync with the time
	/// </summary>
	void Update ()
	{
		
		timer += Time.deltaTime;
		
		/* Update the flags for loud parts of a song */
		if (loudPartCounter < AudioManager.loudPartTimeStamps.Length && AudioManager.loudPartTimeStamps [loudPartCounter] / (float)AudioManager.frequency < timer + 0.5f) {
			loudFlag = AudioManager.loudPartTimeStamps[loudPartCounter+1];
			foreach (PeakListener l in listeners)
				l.setLoudFlag (loudFlag);
			loudPartCounter+=2;			
		}
		
		// Iterate over every channel
		for (int t = 0; t < AudioManager.peaks.Length; t++) {
			
			// Sync peaks (can call them triggers) to the music
			while (peakCounters[t] < AudioManager.peaks[t].Length && AudioManager.peaks[t][peakCounters[t]] * (1024f/(float)AudioManager.frequency) < timer) {
				foreach (PeakListener l in listeners)
					l.onPeakTrigger (t);
				peakCounters [t]++;
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
