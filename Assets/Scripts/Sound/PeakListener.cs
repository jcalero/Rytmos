using UnityEngine;
using System.Collections;

public interface PeakListener {
	
	void onPeakTrigger(int channel);
	void setLoudFlag(bool flag);
}
