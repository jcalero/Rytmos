using UnityEngine;
using System.Collections;

/// <summary>
/// Peak listener Interface.
/// This interface is required to be extended by classes
/// which want to be alerted when a peak is triggered.
/// </summary>
public interface PeakListener
{
	
	void onPeakTrigger (int channel);

	void setLoudFlag (int flag);
}
