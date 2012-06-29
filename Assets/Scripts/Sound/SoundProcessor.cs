using UnityEngine;
using System.Collections;
using Exocortex.DSP;

public class SoundProcessor
{
	
	/// <summary>
	/// All the data needed for processing and storing results
	/// </summary>
	private float[] audioData;
	private int fftWindow;
	private float progress;
	private float[] vars;
	private float[] means;
	private float[] stDevs;
	private int[][] triggers;
	private int frequencyRange;
	private int hzPerBin;
	private float[] onlineMeans;
	private float[] onlineM2;
	private float[] onlineStDevs;
	private float[] onlineLowMidHighMeans;
	private float[] onlineLowMidHighStDevs;
	private float[] onlineLowMidHighM2;
	private int onlineCounter;
	
	// Disable the empty constructor
	private SoundProcessor ()
	{
	}
	
	/// <summary>
	/// Initializes a new instance of the "SoundProcessor" class.
	/// </summary>
	/// <param name='audioData'>
	/// The ENTIRE Audio data used for processing
	/// </param>
	/// <param name='fftWindow'>
	/// FFT window-size: Corresponds to the number of channels that are computed
	/// </param>
	public SoundProcessor (float[] audioData, int fftWindow)
	{
		if (audioData.Length < fftWindow) {
			Debug.LogError ("Audio data smaller than FFT window!");
			Debug.Break ();
		}
		this.audioData = audioData;
		this.fftWindow = fftWindow;
		this.onlineMeans = new float[fftWindow];
		this.onlineM2 = new float[fftWindow];
		this.onlineStDevs = new float[fftWindow];
		this.onlineCounter = 0;
		this.frequencyRange = 44100 / 2;
		this.hzPerBin = frequencyRange / fftWindow;
		this.onlineLowMidHighMeans = new float[4];
		this.onlineLowMidHighStDevs = new float[4];
		this.onlineLowMidHighM2 = new float[4];
		
	
	}
	
	/// <summary>
	/// Gets the FFT of the specified sample.
	/// The FFT-window spans from: sample-(fftWindow/2-1) to: sample+(fftWindow/2).
	/// (Unless this is not possible: at the start and end of the data array).
	/// </summary>
	/// <returns>
	/// The FFT for the current sample.
	/// </returns>
	/// <param name='sample'>
	/// The INDEX(!) of the current sound sample in the array.
	/// </param>
	public float[] getFFTAtSample (int sample)
	{
		
		// Init vars
		float[] outputData = new float[fftWindow];
		int fftWindowHalf = fftWindow / 2;
		int min = 0; // Holds the "pointer" to the start of the window for the FFT, given the current sound sample
		
		// Set the "pointer"
		if (sample - (fftWindowHalf - 1) < 0) {
			min = 0;
		} else if (sample + fftWindowHalf >= audioData.Length) {
			min = sample - (fftWindowHalf) - Mathf.Abs ((sample + fftWindowHalf) - audioData.Length);
		} else {
			min = sample - (fftWindowHalf - 1);
		}
		
		// Get a specific subset of the entire audio array for the FFT
		System.Array.Copy (audioData, min, outputData, 0, fftWindow);
		
		// Using a HANN WINDOW
		for (int k = 0; k < outputData.Length; k++) {
			float multiplier = 0.5f * (1 - Mathf.Cos (2 * Mathf.PI * k / (fftWindow - 1)));
			outputData [k] = multiplier * outputData [k];
		}
		
		// Get FFT
		Fourier.RFFT (outputData, FourierDirection.Forward);
		return outputData;
		
	}
	
	/// <summary>
	/// Analyzes the song:
	/// Calculates FOR EACH CHANNEL of the FFT: Mean, Variance, Standard Deviation
	/// Runs throug the entire song twice -> VERY EXPENSIVE & TIME CONSUMING
	/// </summary>
	public void analyzeSong ()
	{
		
		// Init Vars
		this.progress = 0f;
		// TODO: use a divisor to the sample rate which is coprime to it. This should guarantee a good spread of the samples.
		int sampleRate = 44100 / 300;
		float[] temp = new float[this.fftWindow];
		float[] means = new float[this.fftWindow];
		float[] vars = new float[this.fftWindow];
		float[] stDevs = new float[this.fftWindow];
		
		// First Pass: Calculate means for each channel
		for (int i = 0; i < this.audioData.Length; i+=sampleRate) {
			
			//Get FFT
			temp = getFFTAtSample (i);
			//Calc means
			for (int k = 0; k < temp.Length; k++) {
				means [k] += temp [k];
			}
			
			// This is 50% of the work done
			this.progress = (i / (float)this.audioData.Length) * 50f;
		}
		
		//Finish calculating means
		for (int i = 0; i < means.Length; i++) {
			means [i] /= (float)(this.audioData.Length / sampleRate);
		}
		
		
		// Second Pass: calculate variances
		for (int i = 0; i < this.audioData.Length; i+=sampleRate) {
			// Get FFT again
			temp = getFFTAtSample (i);
			// Calc vars
			for (int k = 0; k < temp.Length; k++) {
				vars [k] += Mathf.Pow (means [k] - temp [k], 2);
			}
			//The other 50% work
			this.progress = 50f + (i / (float)this.audioData.Length) * 50f;
		}
		
		// Finish calculating variances and calculate standard deviations as well
		for (int i = 0; i < vars.Length; i++) {
			vars [i] /= (float)(this.audioData.Length / sampleRate);
			stDevs [i] = Mathf.Sqrt (vars [i]);
		}
		
		// Set instance vriables
		this.vars = vars;
		this.means = means;
		this.stDevs = stDevs;
		
		this.progress = 100f;
		
	}
	
	/// <summary>
	/// Calculates the Triggers. Runs through the entire song -> FAIRLY EXPENSIVE!
	/// </summary>
	/// <returns>
	/// An int-array with the triggers.
	/// The int array is of size audioData.Length / (samplesPerSecond / triggersPerSecond).
	/// This discretizes the audioData-samples into steps of size triggersPerSecond.
	/// Each entry in the array is another array of size fftWindow.
	/// Each entry in these arrays is either 0 or 1: 1 if the threshold has been surpassed at the current timestep, 0 otherwise.
	/// </returns>
	/// <param name='samplesPerSecond'>
	/// Samples per second. Usually given by "myObject.audio.clip.frequency"
	/// </param>
	/// <param name='triggersPerSecond'>
	/// Triggers per second: Specified by the programmer.
	/// </param>
	/// <param name='threshold'>
	/// Threshold: This float is multiplied with the standard deviation.
	/// threshold = 2, will result in a trigger being saved for a channel if sample-mean > 2*stDev for that channel.
	/// </param>
	public int[][] calculateAllTriggers (int samplesPerSecond, int triggersPerSecond, float threshold)
	{
		
		int sampleRate = samplesPerSecond / triggersPerSecond;
		int samples = this.audioData.Length / sampleRate;
		float[] temp;
		this.triggers = new int[samples][];
		
		// Calculate Triggers
		// Third Pass: calculate triggers
		for (int i = 0; i < samples; i++) {
			// Get FFT again
			temp = getFFTAtSample (i * sampleRate);
			this.triggers [i] = new int[this.fftWindow];
			// Calc vars
			for (int k = 0; k < temp.Length; k++) {
				if (Mathf.Abs (Mathf.Abs (temp [k]) - Mathf.Abs (this.means [k])) > threshold * this.stDevs [k]) {
					this.triggers [i] [k] = 1;	
				} else
					this.triggers [i] [k] = 0;
			}
		}
		
		return this.triggers;	
	}
	
	public int[] calculateTriggersAtSample (int sample)
	{
		
		float[] sampleFFT = getFFTAtSample (sample);
		//foreach(float f in sampleFFT) f *= f;
		
		int[] triggers = new int[sampleFFT.Length + 1];
		int triggerCount = 0;
		
		for (int i = 0; i < sampleFFT.Length; i++) {
			if (Mathf.Abs (sampleFFT [i]) > (Mathf.Abs (means [i]) + (2 * stDevs [i]))) {
				triggers [i] = 1;
				triggerCount++;
			}
		}
		triggers [triggers.Length - 1] = triggerCount;
		
		return triggers;
	}
	
	/// <summary>
	/// Calculates the online triggers.
	/// First int[]: 4 bins with triggers for low, midLow, midHigh, high frequency bins
	/// Second int[]: (number of enemies) bins with triggers for each enemy
	/// </summary>
	/// <returns>
	/// The online triggers.
	/// </returns>
	/// <param name='sample'>
	/// Sample.
	/// </param>
	public int[][] calculateOnlineTriggers (int sample)
	{
		
		onlineCounter++; /*Increment the counter for how many samples the online method has seen*/
		float[] fft = getFFTAtSample (sample); /*Get the FFT*/
		
		
		/* Calculate the means/variances for the four main bins: low, midLow, midHigh, High */
		float[] lowMidHighBins = new float[4];
		int[] lowMidHighTriggers = new int[4];
		
		// Combine the fft bins into ones according to the "natural" frequency division
		for (int i = 0; i < fft.Length; i++) {
			int freq = i * hzPerBin;
			if (freq < 600)
				lowMidHighBins [0] += fft [i];
			else if (freq < 2100)
				lowMidHighBins [1] += fft [i];
			else if (freq < 8200)
				lowMidHighBins [2] += fft [i];
			else
				lowMidHighBins [3] += fft [i];
		}
		
		// Calculate the online mean and variance and triggers
		float delta;
		for (int i = 0; i < onlineLowMidHighMeans.Length; i++) {
			
			// Calculate triggers based on data seen so far
			if (lowMidHighBins [i] > (onlineLowMidHighMeans [i] + onlineLowMidHighStDevs [i]))
				lowMidHighTriggers [i] = 1;
			else
				lowMidHighTriggers [i] = 0;			
			
			// Update the values for next time			
			delta = lowMidHighBins [i] - onlineLowMidHighMeans [i];
			onlineLowMidHighMeans [i] += delta / onlineCounter;
			onlineLowMidHighM2 [i] += delta * (lowMidHighBins [i] - onlineLowMidHighMeans [i]);
			onlineLowMidHighStDevs [i] = Mathf.Sqrt (onlineLowMidHighM2 [i] / onlineCounter);
		}
		
		// Calculate the online mean and variance and triggers... for all enemies?
		for (int i = 0; i < fft.Length; i++) {
			delta = fft [i] - onlineMeans [i];
			onlineMeans [i] += delta / onlineCounter;
			onlineM2 [i] += delta * (fft [i] - onlineMeans [i]);
			onlineStDevs [i] = onlineM2 [i] / onlineCounter;
		}
		
		
		int[][] triggers = new int[2][];
		triggers [0] = lowMidHighTriggers;
		triggers [1] = new int[fftWindow];
		return triggers;
		
//		
//		def online_variance(data):
//    n = 0
//    mean = 0
//    M2 = 0
// 
//    for x in data:
//        n = n + 1
//        delta = x - mean
//        mean = mean + delta/n
//        M2 = M2 + delta*(x - mean)
// 
//    variance_n = M2/n
//    variance = M2/(n - 1)
//    return (variance, variance_n)
	}
	
	public int[][] calculateAllTheTriggers (int samplesPerSecond,int triggersPerSecond,int bins)
	{		
		int sampleRate = samplesPerSecond / triggersPerSecond;
		int samples = this.audioData.Length / sampleRate;
		
		float[] lowMidHighBins;
		float[] localMeans = new float[bins];
		float[] localStDevs = new float[bins];
		float[] localM2 = new float[bins];
		int[][] localTriggers = new int[samples][];
		
		float[] fft;
		float delta;
		
		// FIRST PASS : "Binnify", Calculate Mean, Variance, Triggers
		for (int j = 0; j < samples; j++) {
			
			fft = getFFTAtSample (j * sampleRate); 	// Get FFT
			localTriggers [j] = new int[bins];		// Triggerarray
			lowMidHighBins = new float[bins]; 		// Reset values
			
			// Combine Frequencies into 4 bins: Low, lowMid, highMid, High
			for (int i = 0; i < fftWindow; i++) {
				int freq = i * hzPerBin;
				if (freq < 600)
					lowMidHighBins [0] += fft [i];
				else if (freq < 2100)
					lowMidHighBins [1] += fft [i];
				else if (freq < 8200)
					lowMidHighBins [2] += fft [i];
				else
					lowMidHighBins [3] += fft [i];
			}
			
			// Calculate Means, Standard Deviations, Triggers
			for (int i = 0; i < lowMidHighBins.Length; i++) {
				
				// Online Mean & Standard Deviation
				delta = lowMidHighBins [i] - localMeans [i];
				localMeans [i] += (delta / (j+1));
				localM2 [i] += delta * (lowMidHighBins [i] - localMeans [i]);
				localStDevs [i] = Mathf.Sqrt (localM2 [i] / (j+1));
				
				// Does the current sample pass the threshold of (mean + stDev)?
				if (lowMidHighBins [i] > Mathf.Abs(localMeans [i]) + 5*localStDevs[i]) {
					localTriggers [j] [i] = 1;
				}
				else {
					localTriggers [j] [i] = 0;
				}
			}
		}
		
		// SECOND PASS : Parse Triggers
		int[][] timeTriggers = new int[bins][];
		bool[] triggerHistory = new bool[]{false,false,false,false};
		ArrayList[] tempTriggers = new ArrayList[]{new ArrayList(),new ArrayList(),new ArrayList(),new ArrayList()};
		
		for (int i = 0; i < localTriggers.Length; i++) {
			for (int j = 0; j < localTriggers[i].Length; j++) {
				if (localTriggers [i] [j] == 1) {
					if (triggerHistory [j]) {
						localTriggers [i] [j] = 0;
					} else {
						tempTriggers[j].Add(i*sampleRate);
						triggerHistory[j] = true;						
					}
				} else {
					triggerHistory[j] = false;				
				}
			}
		}
		
		System.Array.Sort(tempTriggers, (x, y) => x.Count.CompareTo(y.Count));
		for(int i = 0; i < timeTriggers.Length; i++) {
			timeTriggers[i] = (int[])tempTriggers[i].ToArray(typeof(int));			
		}
		return timeTriggers;
		
	}
	
	//GETTERS ####################################
	public float[] getVars ()
	{
		return this.vars;	
	}
	
	public float[] getMeans ()
	{
		return this.means;	
	}
	
	public float[] getStDevs ()
	{
		return this.stDevs;	
	}
	
	public float getProgress ()
	{
		if (this.progress.Equals (null))
			return 0;
		else
			return this.progress;
	}
	// ############################################
	
}
