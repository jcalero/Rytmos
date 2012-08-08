using UnityEngine;
using System;
using System.Collections.Generic;

public class SoundProcessor
{
	
	// Disable the empty constructor
	private SoundProcessor ()
	{
	}
	
	public static readonly int HOP_SIZE = 1024;
	public static readonly int BUFFER_SIZE = 1024;
	public static readonly int HISTORY_SIZE = 50;
	public static readonly float[] multipliers = { 2f, 2f, 2f, 2f, 2f, 2f };
	public static readonly float[] bands = { 0, 1000, 1000, 4000, 4000, 8000, 8000, 22000};
	private static int[] volumeLevels;
	private static int[][] peaks;
	private static float variationFactor;
	private static bool abortAnalysis;
	public static bool isAnalyzing = true;
	public static float loadingProgress;
	// { 0, 500, 500, 2000, 2000, 4000, 4000, 8000, 8000, 16000, 16000, 22000 };
	
	public static void analyse (DecoderInterface decoder)
	{			
		loadingProgress = 0;
		abortAnalysis = false;
		// For finding the volume levels
		List<int> volumeLevelList = new List<int> ();
		float rollingAverage = 0.01f;
		float alpha = 0.15f;		
		int activePart = -1;
		int sampleCounter = 0;
		float totalMax = -1;
		
		// Get spectral flux
		SpectrumProvider spectrumProvider = new SpectrumProvider (decoder, BUFFER_SIZE, HOP_SIZE, true);			
		float[] spectrum = spectrumProvider.nextSpectrum ();
		float[] lastSpectrum = new float[spectrum.Length];
		List<List<float>> spectralFlux = new List<List<float>> ();
		for (int i = 0; i < bands.Length / 2; i++)
			spectralFlux.Add (new List<float> ());
		
		int bufferCounter = 0;
			
		do {
			
			if(abortAnalysis) return;
			
			#region SPECTRAL ANALYSIS
			for (int i = 0; i < bands.Length; i+=2) {				
				int startFreq = spectrumProvider.getFFT ().freqToIndex (bands [i]);
				int endFreq = spectrumProvider.getFFT ().freqToIndex (bands [i + 1]);
				float flux = 0;
				for (int j = startFreq; j <= endFreq; j++) {
					float value = (spectrum [j] - lastSpectrum [j]);
					value = (value + Mathf.Abs (value)) / 2;
					flux += value;
				}
				(spectralFlux [i / 2]).Add (flux);
			}
					
			System.Array.Copy (spectrum, 0, lastSpectrum, 0, spectrum.Length);
			#endregion
			
			#region GET MAX SAMPLE
			foreach (float sample in spectrumProvider.getCurrentSamples()) {
				if (sample > totalMax)
					totalMax = sample;
			}			
			#endregion
			
			bufferCounter++;
			loadingProgress = 0.75f*(((bufferCounter*BUFFER_SIZE)/(float)AudioManager.frequency)/AudioManager.audioLength);
						
		} while( (spectrum = spectrumProvider.nextSpectrum() ) != null );
		
		#region VOLUME CLASSIFICATION		
		DecoderInterface dec = spectrumProvider.getDecoder ();
		dec.reset ();
		
		float [] samples = new float[spectrumProvider.getWinSize ()];
		float factor = 1f / totalMax;
		List<float> decibelLevels = new List<float>();
		float avgDecibels = 0;
		
		float oldProgress = loadingProgress;
		
		while (dec.readSamples(ref samples) !=0) {
			
			if(abortAnalysis) return;
			
			float max = -1;
			
			/* Normalize the volume using the previously calculated factor */ 
			for (int i =0; i < samples.Length; i++) {
				samples [i] = Mathf.Abs (samples[i] * factor);
				if (samples [i] > max)
					max = samples [i];
			}
			
			float db = Mathf.Abs(20*Mathf.Log10(max));
			if(max != 0) decibelLevels.Add(db);
			avgDecibels += db;
			
			rollingAverage = (alpha * max) + ((1 - alpha) * rollingAverage);
			
			// Have we found a part which classifies as extremely loud?
			if (rollingAverage > 0.82f) { //0.8
				
				// Are we already in that part?
				if (activePart != 5) {
					activePart = 5; // Set flag that we are now in that part
					volumeLevelList.Add (sampleCounter * spectrumProvider.getCurrentSamples ().Length);
					volumeLevelList.Add (activePart);
				}
			
				// Have we found a part which classifies as damn loud?
			} else if (rollingAverage > 0.7f) { //0.6
				
				// Are we already in that part?
				if (activePart != 4) {
					activePart = 4; // Set flag that we are now in that part
					volumeLevelList.Add (sampleCounter * spectrumProvider.getCurrentSamples ().Length);
					volumeLevelList.Add (activePart);
				}
			
				// Have we found a part which classifies as pretty loud?
			} else if (rollingAverage > 0.4f) { //0.2
				
				// Are we already in that part?
				if (activePart != 3) {
					activePart = 3; // Set flag that we are now in that part
					volumeLevelList.Add (sampleCounter * spectrumProvider.getCurrentSamples ().Length);
					volumeLevelList.Add (activePart);
				}
			
				// Have we found a part which classifies as pretty normal?
			} else if (rollingAverage > 0.1f) { //0.0716
				
				// Are we already in that part?
				if (activePart != 2) {
					activePart = 2; // Set flag that we are now in that part
					volumeLevelList.Add (sampleCounter * spectrumProvider.getCurrentSamples ().Length);
					volumeLevelList.Add (activePart);					
				}
			
				// Have we found a part which classifies as pretty quiet?
			} else if (rollingAverage > 0.045f) { //0.0016
				
				// Are we already in that part?
				if (activePart != 1) {
					activePart = 1; // Set flag that we are now in that part
					volumeLevelList.Add (sampleCounter * spectrumProvider.getCurrentSamples ().Length);
					volumeLevelList.Add (activePart);
				}
			
				// Have we found a part which classifies as very quiet?
				// Below 40db (== below 0.06f amplitude)
			} else {

				// Are we already in that part?
				if (activePart != 0) {
					activePart = 0; // Set flag that we are now in that part
					volumeLevelList.Add (sampleCounter * spectrumProvider.getCurrentSamples ().Length);
					volumeLevelList.Add (activePart);
				}
			}
			sampleCounter++;
			loadingProgress = oldProgress + 0.25f*(((sampleCounter*BUFFER_SIZE)/(float)AudioManager.frequency)/AudioManager.audioLength);
		}
		#endregion
		
		if(abortAnalysis) return;
		
		// Store volumelevels
		volumeLevels = volumeLevelList.ToArray ();
		volumeLevelList.Clear();
		volumeLevelList = null;
		
		#region SPECTRAL FLUX ANALYSIS		
		// Convert spectral flux arraylist to array
		float[][] spectralFluxArray = new float[spectralFlux.Count][];
		for (int i = 0; i < spectralFluxArray.Length; i++) {
			spectralFluxArray [i] = spectralFlux [i].ToArray ();
		}
		spectralFlux.Clear ();
		spectralFlux = null;
		
		if(abortAnalysis) return;
		
		// Get thresholds
		float[][] thresholds = new float[bands.Length / 2][];
		float[][] prunnedSpectralFlux = new float[bands.Length / 2][];
		for (int i = 0; i < bands.Length / 2; i++) {
			float[] threshold = new ThresholdFunction (HISTORY_SIZE, multipliers [i]).calculate (spectralFluxArray [i]);
			thresholds [i] = threshold;
			
			float[] tempPSF = new float[spectralFluxArray [i].Length];
			for (int j = 0; j < spectralFluxArray[i].Length; j++) {
				if (threshold [j] <= spectralFluxArray [i] [j])
					tempPSF [j] = spectralFluxArray [i] [j] - threshold [j];
				else
					tempPSF [j] = 0;	
			}
			prunnedSpectralFlux [i] = tempPSF;
		}
		thresholds = null;
		
		if(abortAnalysis) return;
		
		// Get Peaks
		List<int[]> peaksList = new List<int[]> ();
		float[] peakAvgs = new float[prunnedSpectralFlux.Length];
		float[] minPeaks = new float[prunnedSpectralFlux.Length];
		for (int i = 0; i < prunnedSpectralFlux.Length; i++) {
			
			List<int> tempPeaks = new List<int> ();
			minPeaks [i] = float.MaxValue;
			
			for (int j = 0; j < prunnedSpectralFlux[i].Length -1; j++) {
				if (prunnedSpectralFlux [i] [j] > prunnedSpectralFlux [i] [j + 1]) {
					tempPeaks.Add (j);
					peakAvgs [i] += prunnedSpectralFlux [i] [j];
					if (prunnedSpectralFlux [i] [j] != 0 && prunnedSpectralFlux [i] [j] < minPeaks [i])
						minPeaks [i] = prunnedSpectralFlux [i] [j];
				}
			}
			peaksList.Add (tempPeaks.ToArray ());
			peakAvgs [i] /= tempPeaks.Count;
		}
		
		if(abortAnalysis) return;
		
		// Save current peaks & reset list
		peaks = peaksList.ToArray ();
		peaksList.Clear ();
		
		// Lowpass filter the peaks
		for (int i = 0; i < peaks.Length; i++) {
			List<int> tempPeaks = new List<int> ();
			for (int j = 0; j < peaks[i].Length; j++) {
				if (prunnedSpectralFlux [i] [peaks [i] [j]] > minPeaks [i] + 0.7f * (peakAvgs [i] - minPeaks [i])) {
					tempPeaks.Add (peaks [i] [j]);
					tempPeaks.Add (Mathf.RoundToInt(prunnedSpectralFlux[i][peaks[i][j]]));
				}
			}
			peaksList.Add (tempPeaks.ToArray ());
		}
		peaks = peaksList.ToArray ();
		#endregion
		
		if(abortAnalysis) return;
		
		#region NORMALIZE PEAK INTENSITIES
		for(int i = 0; i < peaks.Length; i++) {
			float max = -1;
			for(int j = 1;j < peaks[i].Length; j+=2) {
				if(peaks[i][j] > max) max = peaks[i][j];
			}
			for(int j = 1;j < peaks[i].Length; j+=2) {
				peaks[i][j] = Mathf.RoundToInt(100f*(peaks[i][j]/max));
			}
		}
		#endregion
		
		if(abortAnalysis) return;
		
		#region VARIATION FACTOR
		float[] dbLvls = decibelLevels.ToArray();
		decibelLevels.Clear();
		decibelLevels = null;
		
		avgDecibels /= (float)dbLvls.Length;
		
		float stDev = 0;
		for (int i = 0; i < dbLvls.Length; i++) {
			stDev += (dbLvls[i] - avgDecibels) * (dbLvls[i] - avgDecibels);
		}
		
		stDev = Mathf.Sqrt (stDev / (float)dbLvls.Length);
		
		if(stDev > 25f) stDev = 25f;
		else if (stDev < 5f) stDev = 5f;
		
		stDev /= 25f;
		variationFactor = stDev;
		#endregion
		
		isAnalyzing = false;
	}
	
	public static int[][] getPeaks ()
	{
		return peaks;	
	}
	
	public static int[] getVolumeLevels ()
	{
		return volumeLevels;	
	}
	
	public static float getVariationFactor() {
		return variationFactor;	
	}
	
	public static void reset () {
		isAnalyzing = true;
		loadingProgress = 0f;
	}
	
	public static void abort() {
		abortAnalysis = true;	
	}
}
