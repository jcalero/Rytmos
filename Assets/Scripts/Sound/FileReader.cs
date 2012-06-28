using UnityEngine;
using System.Collections;

public class FileReader {
	
	// Instance vars
	private bool reading;
	private AudioClip clip;
	private byte[] rawBytes;
	private AudioFormat format;
	private string path;
	
	// Supported Audio Formats.. not all of them are in yet!!
	public enum AudioFormat {WAV, OGG, MPEG};
	
	// Disable empty constructor
	private FileReader() {}
	
	// Constructor
	public FileReader(string path, AudioFormat format) {
		// Init instance vars
		reading = false;
		clip = null;
		rawBytes = null;
		this.format = format;
		this.path = path;
	}
	
	/// <summary>
	/// Read the file specified for this instance of FileReader
	/// </summary>
	public void read() {
		this.reading = true;
		
		// Check input
		if(this.path == null || this.path == "") {
			Debug.LogError("Path not specified!");
			this.reading = false;
			return;			
		} else if(this.format != AudioFormat.WAV) {
			Debug.Log("This format: " + this.format.ToString() + " is not supported yet");
			this.reading = false;
			return;	
		}
		
		// Read File
		WWW fileLoader = new WWW("file://" + path);
		yieldRoutine(fileLoader);
		this.reading = false;
		if(fileLoader.bytes == null || fileLoader.bytes.Length == 0) {			
			Debug.LogError("File could not be read correctly");
			return;
		} else this.rawBytes = fileLoader.bytes;
		
		// Get audioclip according to file format
		switch((int)this.format) {
		case 0:
			this.clip = fileLoader.GetAudioClip(true,false,AudioType.WAV);
			break;
		}
		
	}
	
	/// <summary>
	/// Checks if this instance is currently reading the file
	/// </summary>
	/// <returns>
	/// bool isReading
	/// </returns>
	public bool isReading() {
		return this.reading;
	}
	
	/// <summary>
	/// Gets the AudioClip.
	/// </summary>
	/// <returns>
	/// AudioClip clip
	/// </returns>
	public AudioClip getClip() {
	
		if(this.clip == null) {
			Debug.LogError("No bytes available. File not read, or reading failed?");
			return null;
		}
		
		return this.clip;
	}
	
	/// <summary>
	/// Gets the audio data as a float array. Every float is one sample in mono.
	/// </summary>
	/// <returns>
	/// float[] audioData
	/// </returns>
	public float[] getData() {
		
		if(rawBytes == null || rawBytes.Length == 0) {
			Debug.Log("No bytes available. File not read, or reading failed?");
			return null;
		}
		
		switch((int)this.format) {
		case 0:
			return getWAVData();
		default:
			Debug.Log("Unsupported audio format!");
			return null;			
		}

	}
	
	// IMPORTANT: This function has to be improved to take the header into account
	/// <summary>
	/// Gets the audio data for the WAV file format.
	/// </summary>
	/// <returns>
	/// float[] wavData
	/// </returns>
	private float[] getWAVData() {
				
		// Get rid of WAV header
		byte[] noHeader = new byte[this.rawBytes.Length - 44];
		System.Array.Copy(this.rawBytes,44,noHeader,0,noHeader.Length);
		
		// Convert byte data to float data
		return byteArrayToFloatArray(noHeader);
	}
	
	
	/// <summary>
	/// Bytes the array to float array.
	/// (Rudimentary way to convert the data. Only works for 16bit stereo wavs)
	/// </summary>
	/// <returns>
	/// The array to floatarize.
	/// </returns>
	/// <param name='inputData'>
	/// Input data.
	/// </param>
	private float[] byteArrayToFloatArray(byte[] inputData) {
		
		float[] outData = new float[inputData.Length / 4];
		
		int k = 0;
		for(int i = 0; i < inputData.Length; i+=4) {
			if(i+4 > inputData.Length) {
				Debug.Log(i);
				Debug.Log(inputData.Length);
				break;
			}
			float left = System.BitConverter.ToInt16(inputData,i);
			float right = right = System.BitConverter.ToInt16(inputData,i+2);
			outData[k] = left + right;
			k++;
		}
		
		return outData;
	}
	
	/// <summary>
	/// Routine for yielding.
	/// </summary>
	/// <returns>
	/// The WWW object to yield over.
	/// </returns>
	/// <param name='obj'>
	/// The WWW object used to yield.
	/// </param>
	public static IEnumerator yieldRoutine(WWW obj) {
		yield return obj;
	}
	
	
}