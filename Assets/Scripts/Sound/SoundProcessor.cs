using UnityEngine;
using System;
using System.Collections.Generic;

public class SoundProcessor
{
	
//	/// <summary>
//	/// All the data needed for processing and storing results
//	/// </summary>
//	private float[] audioData;
//	private int fftWindow;
//	private float progress;
//	private float[] vars;
//	private float[] means;
//	private float[] stDevs;
//	private int[][] triggers;
//	private int frequencyRange;
//	private int hzPerBin;
//	private float[] onlineMeans;
//	private float[] onlineM2;
//	private float[] onlineStDevs;
//	private float[] onlineLowMidHighMeans;
//	private float[] onlineLowMidHighStDevs;
//	private float[] onlineLowMidHighM2;
//	private int onlineCounter;
	
	// Disable the empty constructor
	private SoundProcessor ()
	{
	}
	
	public static readonly int HOP_SIZE = 1024;
	public static readonly int HISTORY_SIZE = 50;
	public static readonly float[] multipliers = { 2f, 2f, 2f, 2f, 2f, 2f };
	public static readonly float[] bands = { 0, 1000, 1000, 4000, 4000, 8000, 8000, 22000};
	// { 0, 500, 500, 2000, 2000, 4000, 4000, 8000, 8000, 16000, 16000, 22000 };
	
	public static float[][] getPeaks(DecoderInterface decoder)
	{	
		
		// Get spectral flux
		SpectrumProvider spectrumProvider = new SpectrumProvider( decoder, 1024, HOP_SIZE, true );			
		float[] spectrum = spectrumProvider.nextSpectrum();
		float[] lastSpectrum = new float[spectrum.Length];
		List<List<float>> spectralFlux = new List<List<float>>();
		for( int i = 0; i < bands.Length / 2; i++ )
			spectralFlux.Add( new List<float>() );
				
		do
		{						
			for( int i = 0; i < bands.Length; i+=2 )
			{				
				int startFreq = spectrumProvider.getFFT().freqToIndex( bands[i] );
				int endFreq = spectrumProvider.getFFT().freqToIndex( bands[i+1] );
				float flux = 0;
				for( int j = startFreq; j <= endFreq; j++ )
				{
					float value = (spectrum[j] - lastSpectrum[j]);
					value = (value + Mathf.Abs(value))/2;
					flux += value;
				}
				(spectralFlux[i/2]).Add( flux );
			}
					
			System.Array.Copy( spectrum, 0, lastSpectrum, 0, spectrum.Length );
		}
		while( (spectrum = spectrumProvider.nextSpectrum() ) != null );
		
		// Convert spectral flux arraylist to array
		float[][] spectralFluxArray = new float[spectralFlux.Count][];
		for(int i = 0; i < spectralFluxArray.Length; i++) {
			spectralFluxArray[i] = spectralFlux[i].ToArray();
		}
		spectralFlux.Clear();
		spectralFlux = null;
		
		// Get thresholds
		float[][] thresholds = new float[bands.Length/2][];
		float[][] prunnedSpectralFlux = new float[bands.Length/2][];
		for( int i = 0; i < bands.Length / 2; i++ )
		{
			float[] threshold = new ThresholdFunction( HISTORY_SIZE, multipliers[i] ).calculate( spectralFluxArray[i] );
			thresholds[i] = threshold;
			
			float[] tempPSF = new float[spectralFluxArray[i].Length];
			for(int j = 0; j < spectralFluxArray[i].Length; j++) {
				if (threshold[j] <= spectralFluxArray[i][j] )
		      		tempPSF[j] = spectralFluxArray[i][j] - threshold[j];
		  		else
		      		tempPSF[j] = 0;	
			}
			prunnedSpectralFlux[i] = tempPSF;
		}
		
		// Get Peaks
		List<float[]> peaks = new List<float[]>();
		float alpha = 2f/21f;
		for(int i = 0; i < prunnedSpectralFlux.Length; i++) {
			
			List<float> tempPeaks = new List<float>();
			float movingMean = 0;
			
			for(int j = 0; j < prunnedSpectralFlux[i].Length -1; j++){
				if(prunnedSpectralFlux[i][j] > prunnedSpectralFlux[i][j+1] ) {
					if(prunnedSpectralFlux[i][j] >= movingMean) tempPeaks.Add(j);

					movingMean = (alpha * prunnedSpectralFlux[i][j]) + ((1-alpha) * movingMean);
				}
			}
			peaks.Add(tempPeaks.ToArray());
		}
		
//		float songBPM = 0;
//		int closestChannel = 0;
//		float closestMean = float.MaxValue;
//		
//		// Get the song bpm from the onset analysis over the entire spectrum
//		for(int i = 0; i < peaks[peaks.Count-1].Length-1; i++) {
//				float diff = peaks[peaks.Count-1][i+1] - peaks[peaks.Count-1][i];
//				songBPM += diff;
//		}
//		songBPM /= peaks[peaks.Count-1].Length-1;
//		
//		// Check which channel fits most
//		for(int j = 0; j < peaks.Count-1; j++) {
//			float mean = 0;
//			for(int i = 0; i < peaks[j].Length-1; i++) {
//				float diff = peaks[j][i+1] - peaks[j][i];
//				mean += diff;
//			}
//			mean /= peaks[j].Length-1;
//			
//			if(Math.Abs(songBPM - mean) < closestMean) {
//				closestMean = Math.Abs(songBPM - mean);
//				closestChannel = j;
//			}
//		}
//		
//		Debug.Log("closest Channel: " + closestChannel);
//		peaks.RemoveAt(peaks.Count-1);
//		float[] closetChan = peaks[closestChannel];
//		peaks.RemoveAt(closestChannel);
//		
//		List<float[]> newPeaks = new List<float[]>();
//		newPeaks.Add(closetChan);
//		for(int i = 0; i < peaks.Count; i++) {
//			newPeaks.Add(peaks[i]);
//		}
//		peaks = newPeaks;
		
		return peaks.ToArray();
	}
	
	public static int[] findVolumeLevels(DecoderInterface decoder) {
		
		decoder.reset();
		
		List<int> volumeLevels = new List<int>();
		float[] samples = new float[AudioManager.frequency/2];
		float max = -1;
		float min = 2; // amplitudes are between 0 and 1
		
		while(decoder.readSamples(ref samples) > 0) {
			foreach(float sample in samples) {
				if(Mathf.Abs(sample) > max) max = sample;
				if(Mathf.Abs(sample) < min) min = sample;
			}
		}
		
		decoder.reset();
		
		float rollingAverage = min + 0.5f*(max-min);
		float alpha = 0.5f;		
		bool alreadyInMax = false;
		int sampleCounter = 0;
		while(decoder.readSamples(ref samples) > 0) {
			float avg = 0;
			foreach(float sample in samples) {
				avg+=Mathf.Abs(sample);
			}
			avg /= (float)samples.Length;
			
			rollingAverage = (alpha * avg) + ((1-alpha) * rollingAverage);
			
			// Have we found a part which classifies as "loud"?
			if(rollingAverage > min + 0.1f*(max - min)) {
				
				// Are we already in a loud part?
				if(!alreadyInMax) {
					volumeLevels.Add(sampleCounter*samples.Length); // Add timestamp of hitting fast part
					alreadyInMax = !alreadyInMax; // Set flag that we are now in a loud part
				}
				
			} else if(alreadyInMax) {
				// We now left the loud part, add ending timestamp
				volumeLevels.Add(sampleCounter*samples.Length);
				alreadyInMax = !alreadyInMax; // Set flag
			}
			
			sampleCounter++;
			
		}
		return volumeLevels.ToArray();
		
		
	}
	
	
	
	private static float calcMean (LinkedList<float> queue) {
	
		float mean = 0;
		if(queue.Count == 0) return mean;
		
		LinkedListNode<float> first = queue.First;
		LinkedListNode<float> next;
		
		mean += first.Value;
		
		while((next = first.Next) != null) {
			mean += next.Value;
		}
		
		mean /= queue.Count;
		
		return mean;
		
	}
	
//	
//	/// <summary>
//	/// Initializes a new instance of the "SoundProcessor" class.
//	/// </summary>
//	/// <param name='audioData'>
//	/// The ENTIRE Audio data used for processing
//	/// </param>
//	/// <param name='fftWindow'>
//	/// FFT window-size: Corresponds to the number of channels that are computed
//	/// </param>
//	public SoundProcessor (float[] audioData, int fftWindow)
//	{
//		if (audioData.Length < fftWindow) {
//			Debug.LogError ("Audio data smaller than FFT window!");
//			Debug.Break ();
//		}
//		this.audioData = audioData;
//		this.fftWindow = fftWindow;
//		this.onlineMeans = new float[fftWindow];
//		this.onlineM2 = new float[fftWindow];
//		this.onlineStDevs = new float[fftWindow];
//		this.onlineCounter = 0;
//		this.frequencyRange = 44100 / 2;
//		this.hzPerBin = frequencyRange / fftWindow;
//		this.onlineLowMidHighMeans = new float[4];
//		this.onlineLowMidHighStDevs = new float[4];
//		this.onlineLowMidHighM2 = new float[4];
//		
//	
//	}
//	
//	/// <summary>
//	/// Gets the FFT of the specified sample.
//	/// The FFT-window spans from: sample-(fftWindow/2-1) to: sample+(fftWindow/2).
//	/// (Unless this is not possible: at the start and end of the data array).
//	/// </summary>
//	/// <returns>
//	/// The FFT for the current sample.
//	/// </returns>
//	/// <param name='sample'>
//	/// The INDEX(!) of the current sound sample in the array.
//	/// </param>
//	public float[] getFFTAtSample (int sample)
//	{
//		
//		// Init vars
//		float[] outputData = new float[fftWindow];
//		int fftWindowHalf = fftWindow / 2;
//		int min = 0; // Holds the "pointer" to the start of the window for the FFT, given the current sound sample
//		
//		// Set the "pointer"
//		if (sample - (fftWindowHalf - 1) < 0) {
//			min = 0;
//		} else if (sample + fftWindowHalf >= audioData.Length) {
//			min = sample - (fftWindowHalf) - Mathf.Abs ((sample + fftWindowHalf) - audioData.Length);
//		} else {
//			min = sample - (fftWindowHalf - 1);
//		}
//		
//		// Get a specific subset of the entire audio array for the FFT
//		System.Array.Copy (audioData, min, outputData, 0, fftWindow);
//		
//		// Using a HANN WINDOW
//		for (int k = 0; k < outputData.Length; k++) {
//			float multiplier = 0.5f * (1 - Mathf.Cos (2 * Mathf.PI * k / (fftWindow - 1)));
//			outputData [k] = multiplier * outputData [k];
//		}
//		
//		// Get FFT
//		Fourier.RFFT (outputData, FourierDirection.Forward);
//		return outputData;
//		
//	}
//	
//	/// <summary>
//	/// Analyzes the song:
//	/// Calculates FOR EACH CHANNEL of the FFT: Mean, Variance, Standard Deviation
//	/// Runs throug the entire song twice -> VERY EXPENSIVE & TIME CONSUMING
//	/// </summary>
//	public void analyzeSong ()
//	{
//		
//		// Init Vars
//		this.progress = 0f;
//		// TODO: use a divisor to the sample rate which is coprime to it. This should guarantee a good spread of the samples.
//		int sampleRate = 44100 / 300;
//		float[] temp = new float[this.fftWindow];
//		float[] means = new float[this.fftWindow];
//		float[] vars = new float[this.fftWindow];
//		float[] stDevs = new float[this.fftWindow];
//		
//		// First Pass: Calculate means for each channel
//		for (int i = 0; i < this.audioData.Length; i+=sampleRate) {
//			
//			//Get FFT
//			temp = getFFTAtSample (i);
//			//Calc means
//			for (int k = 0; k < temp.Length; k++) {
//				means [k] += temp [k];
//			}
//			
//			// This is 50% of the work done
//			this.progress = (i / (float)this.audioData.Length) * 50f;
//		}
//		
//		//Finish calculating means
//		for (int i = 0; i < means.Length; i++) {
//			means [i] /= (float)(this.audioData.Length / sampleRate);
//		}
//		
//		
//		// Second Pass: calculate variances
//		for (int i = 0; i < this.audioData.Length; i+=sampleRate) {
//			// Get FFT again
//			temp = getFFTAtSample (i);
//			// Calc vars
//			for (int k = 0; k < temp.Length; k++) {
//				vars [k] += Mathf.Pow (means [k] - temp [k], 2);
//			}
//			//The other 50% work
//			this.progress = 50f + (i / (float)this.audioData.Length) * 50f;
//		}
//		
//		// Finish calculating variances and calculate standard deviations as well
//		for (int i = 0; i < vars.Length; i++) {
//			vars [i] /= (float)(this.audioData.Length / sampleRate);
//			stDevs [i] = Mathf.Sqrt (vars [i]);
//		}
//		
//		// Set instance vriables
//		this.vars = vars;
//		this.means = means;
//		this.stDevs = stDevs;
//		
//		this.progress = 100f;
//		
//	}
//	
//	/// <summary>
//	/// Calculates the Triggers. Runs through the entire song -> FAIRLY EXPENSIVE!
//	/// </summary>
//	/// <returns>
//	/// An int-array with the triggers.
//	/// The int array is of size audioData.Length / (samplesPerSecond / triggersPerSecond).
//	/// This discretizes the audioData-samples into steps of size triggersPerSecond.
//	/// Each entry in the array is another array of size fftWindow.
//	/// Each entry in these arrays is either 0 or 1: 1 if the threshold has been surpassed at the current timestep, 0 otherwise.
//	/// </returns>
//	/// <param name='samplesPerSecond'>
//	/// Samples per second. Usually given by "myObject.audio.clip.frequency"
//	/// </param>
//	/// <param name='triggersPerSecond'>
//	/// Triggers per second: Specified by the programmer.
//	/// </param>
//	/// <param name='threshold'>
//	/// Threshold: This float is multiplied with the standard deviation.
//	/// threshold = 2, will result in a trigger being saved for a channel if sample-mean > 2*stDev for that channel.
//	/// </param>
//	public int[][] calculateAllTriggers (int samplesPerSecond, int triggersPerSecond, float threshold)
//	{
//		
//		int sampleRate = samplesPerSecond / triggersPerSecond;
//		int samples = this.audioData.Length / sampleRate;
//		float[] temp;
//		this.triggers = new int[samples][];
//		
//		// Calculate Triggers
//		// Third Pass: calculate triggers
//		for (int i = 0; i < samples; i++) {
//			// Get FFT again
//			temp = getFFTAtSample (i * sampleRate);
//			this.triggers [i] = new int[this.fftWindow];
//			// Calc vars
//			for (int k = 0; k < temp.Length; k++) {
//				if (Mathf.Abs (Mathf.Abs (temp [k]) - Mathf.Abs (this.means [k])) > threshold * this.stDevs [k]) {
//					this.triggers [i] [k] = 1;	
//				} else
//					this.triggers [i] [k] = 0;
//			}
//		}
//		
//		return this.triggers;	
//	}
//	
//	public int[] calculateTriggersAtSample (int sample)
//	{
//		
//		float[] sampleFFT = getFFTAtSample (sample);
//		//foreach(float f in sampleFFT) f *= f;
//		
//		int[] triggers = new int[sampleFFT.Length + 1];
//		int triggerCount = 0;
//		
//		for (int i = 0; i < sampleFFT.Length; i++) {
//			if (Mathf.Abs (sampleFFT [i]) > (Mathf.Abs (means [i]) + (2 * stDevs [i]))) {
//				triggers [i] = 1;
//				triggerCount++;
//			}
//		}
//		triggers [triggers.Length - 1] = triggerCount;
//		
//		return triggers;
//	}
//	
//	/// <summary>
//	/// Calculates the online triggers.
//	/// First int[]: 4 bins with triggers for low, midLow, midHigh, high frequency bins
//	/// Second int[]: (number of enemies) bins with triggers for each enemy
//	/// </summary>
//	/// <returns>
//	/// The online triggers.
//	/// </returns>
//	/// <param name='sample'>
//	/// Sample.
//	/// </param>
//	public int[][] calculateOnlineTriggers (int sample)
//	{
//		
//		onlineCounter++; /*Increment the counter for how many samples the online method has seen*/
//		float[] fft = getFFTAtSample (sample); /*Get the FFT*/
//		
//		
//		/* Calculate the means/variances for the four main bins: low, midLow, midHigh, High */
//		float[] lowMidHighBins = new float[4];
//		int[] lowMidHighTriggers = new int[4];
//		
//		// Combine the fft bins into ones according to the "natural" frequency division
//		for (int i = 0; i < fft.Length; i++) {
//			int freq = i * hzPerBin;
//			if (freq < 600)
//				lowMidHighBins [0] += fft [i];
//			else if (freq < 2100)
//				lowMidHighBins [1] += fft [i];
//			else if (freq < 8200)
//				lowMidHighBins [2] += fft [i];
//			else
//				lowMidHighBins [3] += fft [i];
//		}
//		
//		// Calculate the online mean and variance and triggers
//		float delta;
//		for (int i = 0; i < onlineLowMidHighMeans.Length; i++) {
//			
//			// Calculate triggers based on data seen so far
//			if (lowMidHighBins [i] > (onlineLowMidHighMeans [i] + onlineLowMidHighStDevs [i]))
//				lowMidHighTriggers [i] = 1;
//			else
//				lowMidHighTriggers [i] = 0;			
//			
//			// Update the values for next time			
//			delta = lowMidHighBins [i] - onlineLowMidHighMeans [i];
//			onlineLowMidHighMeans [i] += delta / onlineCounter;
//			onlineLowMidHighM2 [i] += delta * (lowMidHighBins [i] - onlineLowMidHighMeans [i]);
//			onlineLowMidHighStDevs [i] = Mathf.Sqrt (onlineLowMidHighM2 [i] / onlineCounter);
//		}
//		
//		// Calculate the online mean and variance and triggers... for all enemies?
//		for (int i = 0; i < fft.Length; i++) {
//			delta = fft [i] - onlineMeans [i];
//			onlineMeans [i] += delta / onlineCounter;
//			onlineM2 [i] += delta * (fft [i] - onlineMeans [i]);
//			onlineStDevs [i] = onlineM2 [i] / onlineCounter;
//		}
//		
//		
//		int[][] triggers = new int[2][];
//		triggers [0] = lowMidHighTriggers;
//		triggers [1] = new int[fftWindow];
//		return triggers;
//		
////		
////		def online_variance(data):
////    n = 0
////    mean = 0
////    M2 = 0
//// 
////    for x in data:
////        n = n + 1
////        delta = x - mean
////        mean = mean + delta/n
////        M2 = M2 + delta*(x - mean)
//// 
////    variance_n = M2/n
////    variance = M2/(n - 1)
////    return (variance, variance_n)
//	}
//	
//	public int[][] calculateAllTheTriggers (int samplesPerSecond,int triggersPerSecond,int bins)
//	{		
//		int sampleRate = samplesPerSecond / triggersPerSecond;
//		int samples = this.audioData.Length / sampleRate;
//		
//		float[] lowMidHighBins;
//		float[] localMeans = new float[bins];
//		float[] localStDevs = new float[bins];
//		float[] localM2 = new float[bins];
//		int[][] localTriggers = new int[samples][];
//		
//		float[] fft;
//		float delta;
//		
//		// FIRST PASS : "Binnify", Calculate Mean, Variance, Triggers
//		for (int j = 0; j < samples; j++) {
//			
//			fft = getFFTAtSample (j * sampleRate); 	// Get FFT
//			localTriggers [j] = new int[bins];		// Triggerarray
//			lowMidHighBins = new float[bins]; 		// Reset values
//			
//			// Combine Frequencies into 4 bins: Low, lowMid, highMid, High
//			for (int i = 0; i < fftWindow; i++) {
//				int freq = i * hzPerBin;
//				if (freq < 600)
//					lowMidHighBins [0] += fft [i];
//				else if (freq < 2100)
//					lowMidHighBins [1] += fft [i];
//				else if (freq < 8200)
//					lowMidHighBins [2] += fft [i];
//				else
//					lowMidHighBins [3] += fft [i];
//			}
//			
//			// Calculate Means, Standard Deviations, Triggers
//			for (int i = 0; i < lowMidHighBins.Length; i++) {
//				
//				// Online Mean & Standard Deviation
//				delta = lowMidHighBins [i] - localMeans [i];
//				localMeans [i] += (delta / (j+1));
//				localM2 [i] += delta * (lowMidHighBins [i] - localMeans [i]);
//				localStDevs [i] = Mathf.Sqrt (localM2 [i] / (j+1));
//				
//				// Does the current sample pass the threshold of (mean + stDev)?
//				if (lowMidHighBins [i] > Mathf.Abs(localMeans [i]) + 5*localStDevs[i]) {
//					localTriggers [j] [i] = 1;
//				}
//				else {
//					localTriggers [j] [i] = 0;
//				}
//			}
//		}
//		
//		// SECOND PASS : Parse Triggers
//		int[][] timeTriggers = new int[bins][];
//		bool[] triggerHistory = new bool[]{false,false,false,false};
//		ArrayList[] tempTriggers = new ArrayList[]{new ArrayList(),new ArrayList(),new ArrayList(),new ArrayList()};
//		
//		for (int i = 0; i < localTriggers.Length; i++) {
//			for (int j = 0; j < localTriggers[i].Length; j++) {
//				if (localTriggers [i] [j] == 1) {
//					if (triggerHistory [j]) {
//						localTriggers [i] [j] = 0;
//					} else {
//						tempTriggers[j].Add(i*sampleRate);
//						triggerHistory[j] = true;						
//					}
//				} else {
//					triggerHistory[j] = false;				
//				}
//			}
//		}
//		
//		System.Array.Sort(tempTriggers, (x, y) => x.Count.CompareTo(y.Count));
//		for(int i = 0; i < timeTriggers.Length; i++) {
//			timeTriggers[i] = (int[])tempTriggers[i].ToArray(typeof(int));			
//		}
//		return timeTriggers;
//		
//	}
//	
//	//GETTERS ####################################
//	public float[] getVars ()
//	{
//		return this.vars;	
//	}
//	
//	public float[] getMeans ()
//	{
//		return this.means;	
//	}
//	
//	public float[] getStDevs ()
//	{
//		return this.stDevs;	
//	}
//	
//	public float getProgress ()
//	{
//		if (this.progress.Equals (null))
//			return 0;
//		else
//			return this.progress;
//	}
//	// ############################################
	
}
