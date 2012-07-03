using UnityEngine;
using System.Collections;
//using Mp3Sharp;
using NLayer.Decoder;

public class FileReader : DecoderInterface {
	
	// Instance vars
	private bool reading;
	private AudioClip clip;
	private AudioFormat format;
	private string path;
	private float[] data;
	private int readDataPointer;
	
	// Supported Audio Formats.. not all of them are in yet!!
	public enum AudioFormat {WAV, OGG, MPEG, ERROR};
	public enum ReadStatus {SUCCESS,FAIL,UNSUPPORTED_FORMAT};
	
	// Disable empty constructor
	private FileReader() {}
	
	// Constructor
	public FileReader(string path) {
		// Init instance vars
		reading = false;
		clip = null;
		this.path = path;
		if(path.EndsWith(".wav")) this.format = AudioFormat.WAV;
		else if(path.EndsWith(".mp3")) this.format = AudioFormat.MPEG;
		else if(path.EndsWith(".ogg")) this.format = AudioFormat.OGG;
		else this.format = AudioFormat.ERROR;
		readDataPointer = 0;
	}
	
	/// <summary>
	/// Read the file specified for this instance of FileReader
	/// </summary>
	public ReadStatus read() {
		this.reading = true;
		
		// Check input
		if(this.path == null || this.path == "") {
			Debug.LogError("Path not specified!");
			this.reading = false;
			return ReadStatus.FAIL;			
		} else if(this.format == AudioFormat.OGG || this.format == AudioFormat.ERROR) {
			Debug.Log("This format: " + this.format.ToString() + " is not supported yet");
			this.reading = false;
			return ReadStatus.UNSUPPORTED_FORMAT;	
		}
		
		// Get audioclip according to file format
		switch((int)this.format) {
		case 0:
			readWAV();
			break;
		case 2:
			readMP3();
			break;
		}
		this.reading = false;
		return ReadStatus.SUCCESS;
		
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
		
		if(data == null || data.Length == 0) {
			Debug.Log("No bytes available. File not read, or reading failed?");
			return null;
		}
		
		return this.data;
	}
	
	private void readWAV() {
		
		// Read WAV
		WWW fileLoader = new WWW("file://" + this.path);
		yieldRoutine(fileLoader);
		if(fileLoader.bytes == null || fileLoader.bytes.Length == 0) {			
			Debug.LogError("File could not be read correctly");
			return;
		}
		
		// "else"..
		// Get rid of WAV header
		byte[] noHeader = new byte[fileLoader.bytes.Length - 44];
		System.Array.Copy(fileLoader.bytes,44,noHeader,0,noHeader.Length);
		
		// Convert byte data to float data
		this.data = byteArrayToFloatArray(noHeader);
		this.clip = fileLoader.GetAudioClip(true,false,AudioType.WAV);
		
	}
	
	private void readMP3() {
		
		// Init
		Decoder dec = new Decoder();
		Header header;
		int frequency;
		int channels;
		
		// Open File Streams
		System.IO.FileStream file = new System.IO.FileStream(this.path,System.IO.FileMode.Open);
		Bitstream bS = new Bitstream(file);
		
		// Read mp3 header and extract freq/chan info
		header = bS.readFrame();
		frequency = header.frequency();
		channels = (header.mode() == Header.SINGLE_CHANNEL) ? 1 : 2;
		
		// Start reading rest of the mp3
		short[] bufferData;
		SampleBuffer buffer;
		float maxVal = (float)short.MaxValue;
		ArrayList monoData = new ArrayList();
		ArrayList channelData = new ArrayList();
		
		do {
			// Get header, check if any more data available
			header = bS.readFrame();
            if (header == null)
                break;
			
			// Decode frame
			buffer = (SampleBuffer)dec.decodeFrame(header,bS);
			bS.closeFrame();
			
			// Read PCM data
			bufferData = buffer.getBuffer();
			for(int i = 0; i < bufferData.Length; i+=2) {
				channelData.Add(bufferData[i]/maxVal);
				channelData.Add(bufferData[i+1]/maxVal);
				monoData.Add(((bufferData[i] + bufferData[i+1])/2f)/maxVal);
			}
		} while(bufferData.Length > 0);
		
		// Convert arraylists to useable data
		this.data = (float[])monoData.ToArray(typeof(float));
		this.clip = AudioClip.Create("gameAudio",this.data.Length,channels,frequency,true,false);
		this.clip.SetData((float[])channelData.ToArray(typeof(float)),0);
		
		// Close streams
		bS.close();
		file.Close();
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
			float right = System.BitConverter.ToInt16(inputData,i+2);
			outData[k] = (left + right)/2f;
			k++;
		}
		
		return outData;
	}
	
	public int readSamples(ref float[] samples) {
		
		int readLength = samples.Length;
		if(readDataPointer + samples.Length >= data.Length) readLength = samples.Length - ((readDataPointer + samples.Length) - data.Length);
		
		System.Array.Copy(data,readDataPointer,samples,0,readLength);
		readDataPointer += readLength;
		
		return readLength;
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
	private static IEnumerator yieldRoutine(WWW obj) {
		yield return obj;
	}
	
	
}