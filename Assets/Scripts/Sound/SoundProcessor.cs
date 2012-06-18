using UnityEngine;
using System.Collections;
using Exocortex.DSP;

public class SoundProcessor {
	
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
	
	// Disable the empty constructor
	private SoundProcessor() {}
	
	/// <summary>
	/// Initializes a new instance of the "SoundProcessor" class.
	/// </summary>
	/// <param name='audioData'>
	/// The ENTIRE Audio data used for processing
	/// </param>
	/// <param name='fftWindow'>
	/// FFT window-size: Corresponds to the number of channels that are computed
	/// </param>
	public SoundProcessor(float[] audioData, int fftWindow) {
		if(audioData.Length < fftWindow) {
			Debug.LogError("Audio data smaller than FFT window!");
			Debug.Break();
		}
		this.audioData = audioData;
		this.fftWindow = fftWindow;
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
	public float[] getFFTAtSample(int sample) {
		
		// Init vars
		float[] outputData = new float[fftWindow];
		int fftWindowHalf = fftWindow/2;
		int min = 0; // Holds the "pointer" to the start of the window for the FFT, given the current sound sample
		
		// Set the "pointer"
		if(sample - (fftWindowHalf -1) < 0) {
			min = 0;
		}
		else if(sample + fftWindowHalf >= audioData.Length) {
			min = sample - (fftWindowHalf) - Mathf.Abs((sample + fftWindowHalf) - audioData.Length);
		}
		else {
			min = sample - (fftWindowHalf -1);
		}
		
		// Get a specific subset of the entire audio array for the FFT
		System.Array.Copy(audioData,min,outputData,0,fftWindow);
		
		// Using a HANN WINDOW
		for(int k = 0; k < outputData.Length; k++) {
    		float multiplier = 0.5f * (1 - Mathf.Cos(2*Mathf.PI*k/(fftWindow-1)));
    		outputData[k] = multiplier * outputData[k];
		}
		
		// Get FFT
		Fourier.RFFT(outputData,FourierDirection.Forward);
		return outputData;
		
	}
	
	/// <summary>
	/// Analyzes the song:
	/// Calculates FOR EACH CHANNEL of the FFT: Mean, Variance, Standard Deviation
	/// Runs throug the entire song twice -> VERY EXPENSIVE & TIME CONSUMING
	/// </summary>
	public void analyzeSong() {
		
		// Init Vars
		this.progress = 0f;
		// TODO: use a divisor to the sample rate which is coprime to it. This should guarantee a good spread of the samples.
		int sampleRate = 44100/300;
		float[] temp = new float[this.fftWindow];
		float[] means = new float[this.fftWindow];
		float[] vars = new float[this.fftWindow];
		float[] stDevs = new float[this.fftWindow];
		
		// First Pass: Calculate means for each channel
		for(int i = 0; i < this.audioData.Length; i+=sampleRate) {
			
			//Get FFT
			temp = getFFTAtSample(i);
			//Calc means
			for(int k = 0; k < temp.Length; k++) {
				means[k] += temp[k];
			}
			
			// This is 50% of the work done
			this.progress = (i / (float)this.audioData.Length) * 50f;
		}
		
		//Finish calculating means
		for(int i = 0; i < means.Length; i++) {
			means[i] /= (float)(this.audioData.Length / sampleRate);
		}
		
		
		// Second Pass: calculate variances
		for(int i = 0; i < this.audioData.Length; i+=sampleRate) {
			// Get FFT again
			temp = getFFTAtSample(i);
			// Calc vars
			for(int k = 0; k < temp.Length; k++) {
				vars[k] += Mathf.Pow(means[k] - temp[k],2);
			}
			//The other 50% work
			this.progress = 50f + (i / (float)this.audioData.Length) * 50f;
		}
		
		// Finish calculating variances and calculate standard deviations as well
		for(int i = 0; i < vars.Length; i++) {
			vars[i] /= (float)(this.audioData.Length / sampleRate);
			stDevs[i] = Mathf.Sqrt(vars[i]);
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
	public int[][] calculateTriggers(int samplesPerSecond, int triggersPerSecond, float threshold) {
		
		int sampleRate = samplesPerSecond / triggersPerSecond;
		int samples = this.audioData.Length / sampleRate;
		float[] temp;
		this.triggers = new int[samples][];
		
		// Calculate Triggers
		// Third Pass: calculate triggers
		for(int i = 0; i < samples; i++) {
			// Get FFT again
			temp = getFFTAtSample(i*sampleRate);
			this.triggers[i] = new int[this.fftWindow];
			// Calc vars
			for(int k = 0; k < temp.Length; k++) {
				if(Mathf.Abs(Mathf.Abs(temp[k]) - Mathf.Abs(this.means[k])) > threshold * this.stDevs[k]) {
					this.triggers[i][k] = 1;	
				} else this.triggers[i][k] = 0;
			}
		}
		
		return this.triggers;	
	}
	
	//GETTERS ####################################
	public float[] getVars() {
		return this.vars;	
	}
	
	public float[] getMeans() {
		return this.means;	
	}
	
	public float[] getStDevs() {
		return this.stDevs;	
	}
	
	public float getProgress() {
		if(this.progress.Equals(null)) return 0;
		else return this.progress;
	}
	// ############################################
	
}
