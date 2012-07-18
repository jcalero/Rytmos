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
	// { 0, 500, 500, 2000, 2000, 4000, 4000, 8000, 8000, 16000, 16000, 22000 };
	
	public static void analyse(DecoderInterface decoder)
	{			
		// For finding the volume levels
		List<int> volumeLevelList = new List<int>();
		float rollingAverage = 0.01f;
		float alpha = 0.15f;		
		int activePart = -1;
		int sampleCounter = 0;
		
		// Get spectral flux
		SpectrumProvider spectrumProvider = new SpectrumProvider( decoder, BUFFER_SIZE, HOP_SIZE, true );			
		float[] spectrum = spectrumProvider.nextSpectrum();
		float[] lastSpectrum = new float[spectrum.Length];
		List<List<float>> spectralFlux = new List<List<float>>();
		for( int i = 0; i < bands.Length / 2; i++ )
			spectralFlux.Add( new List<float>() );
			
		do
		{			
			#region SPECTRAL ANALYSIS
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
			#endregion
			
			#region VOLUME ANALYSIS
			float avg = 0;
			float max = -1;
			foreach(float sample in spectrumProvider.getCurrentSamples()) {
				avg+=Mathf.Abs(sample);
				if(sample > max) max = sample;
			}
			avg /= (float)spectrumProvider.getCurrentSamples().Length;
			
			rollingAverage = (alpha * (0.8f*max + 0.2f*avg)) + ((1-alpha) * rollingAverage);
			
			// Have we found a part which classifies as extremely loud?
			if(rollingAverage > 0.7f) {
				
				// Are we already in that part?
				if(activePart != 5) {
					activePart = 5; // Set flag that we are now in that part
					volumeLevelList.Add(sampleCounter*spectrumProvider.getCurrentSamples().Length);
					volumeLevelList.Add(activePart);
				}
			
			// Have we found a part which classifies as damn loud?
			} else if(rollingAverage > 0.5f) {
				
				// Are we already in that part?
				if(activePart != 4) {
					activePart = 4; // Set flag that we are now in that part
					volumeLevelList.Add(sampleCounter*spectrumProvider.getCurrentSamples().Length);
					volumeLevelList.Add(activePart);
				}
			
			// Have we found a part which classifies as pretty loud?
			} else if (rollingAverage > 0.126f) {
				
				// Are we already in that part?
				if(activePart != 3) {
					activePart = 3; // Set flag that we are now in that part
					volumeLevelList.Add(sampleCounter*spectrumProvider.getCurrentSamples().Length);
					volumeLevelList.Add(activePart);
				}
			
			// Have we found a part which classifies as pretty normal?
			} else if (rollingAverage > 0.0316f) {
				
				// Are we already in that part?
				if(activePart != 2) {
					activePart = 2; // Set flag that we are now in that part
					volumeLevelList.Add(sampleCounter*spectrumProvider.getCurrentSamples().Length);
					volumeLevelList.Add(activePart);
				}
			
			// Have we found a part which classifies as pretty quiet?
			} else if (rollingAverage > 0.0016f) {
				
				// Are we already in that part?
				if(activePart != 1) {
					activePart = 1; // Set flag that we are now in that part
					volumeLevelList.Add(sampleCounter*spectrumProvider.getCurrentSamples().Length);
					volumeLevelList.Add(activePart);
				}
			
			// Have we found a part which classifies as very quiet?
			// Below 40db (== below 0.06f amplitude)
			} else {

				// Are we already in that part?
				if(activePart != 0) {
					Debug.Log(20*Mathf.Log10(rollingAverage));
					activePart = 0; // Set flag that we are now in that part
					volumeLevelList.Add(sampleCounter*spectrumProvider.getCurrentSamples().Length);
					volumeLevelList.Add(activePart);
				}
			}
			sampleCounter++;
			#endregion
			
		}
		while( (spectrum = spectrumProvider.nextSpectrum() ) != null );
		
		// Store volumelevels
		volumeLevels = volumeLevelList.ToArray();
		
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
		List<int[]> peaksList = new List<int[]>();
		alpha = 2f/21f;
		for(int i = 0; i < prunnedSpectralFlux.Length; i++) {
			
			List<int> tempPeaks = new List<int>();
			float movingMean = 0;
			float x = 20;
			for(int j = 0; j < prunnedSpectralFlux[i].Length && x<0; j++) {
				movingMean += prunnedSpectralFlux[i][j];
				if(prunnedSpectralFlux[i][j] == 0) x--;
			}
			movingMean /= 20f;
			
			for(int j = 0; j < prunnedSpectralFlux[i].Length -1; j++){
				if(prunnedSpectralFlux[i][j] > prunnedSpectralFlux[i][j+1] ) {
					if(prunnedSpectralFlux[i][j] >= movingMean) tempPeaks.Add(j);

					movingMean = (alpha * prunnedSpectralFlux[i][j]) + ((1-alpha) * movingMean);
				}
			}
			peaksList.Add(tempPeaks.ToArray());
		}
		
		peaks = peaksList.ToArray();
	}
	
	public static int[][] getPeaks() {
		return peaks;	
	}
	
	public static int[] getVolumeLevels() {
		return volumeLevels;	
	}
}
