using UnityEngine;
using System.Collections;
using MPG123Wrapper;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;

public class FileReader : DecoderInterface {
	
	// Instance vars
	private bool reading;
	private AudioClip clip;
	private FileFormat format;
	private string path;
	private float[] data;
	private int readDataPointer;
	private int frequency;
	private float audioLength;
	private float[][] analData;
	private int[] loudTriggers;
	private MP3 mp3Reader;
	
	// Supported Audio Formats.. not all of them are in yet!!
	public enum FileFormat {WAV, OGG, MPEG, RYT, ERROR};
	public enum ReadStatus {SUCCESS,FAIL,UNSUPPORTED_FORMAT};
	
	// Disable empty constructor
	private FileReader() {}
	
	// Constructor
	public FileReader(string path) {
		// Init instance vars
		reading = false;
		clip = null;
		this.path = path;
		if(path.EndsWith(".wav")) this.format = FileFormat.WAV;
		else if(path.EndsWith(".mp3")) this.format = FileFormat.MPEG;
		else if(path.EndsWith(".ogg")) this.format = FileFormat.OGG;
		else if(path.EndsWith(".ryt")) this.format = FileFormat.RYT;
		else this.format = FileFormat.ERROR;
		readDataPointer = 0;
		//if(this.format == AudioFormat.MPEG) mp3Reader = new MP3(this.path);
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
		} else if(this.format == FileFormat.OGG || this.format == FileFormat.ERROR) {
			Debug.Log("This format: " + this.format.ToString() + " is not supported yet");
			this.reading = false;
			return ReadStatus.UNSUPPORTED_FORMAT;	
		}
		
		// Get audioclip according to file format
		switch(this.format) {
		case FileFormat.WAV:
			readWAV();
			break;
		case FileFormat.MPEG:
			readMp3();
			//mp3Reader = new MP3(this.path);
			//this.frequency = mp3Reader.getFrequency();
			break;
		case FileFormat.RYT:
			readAnalData();
			break;
		}
		this.reading = false;
		return ReadStatus.SUCCESS;
		
	}
	
	public int getFrequency() {
		return frequency;	
	}
	
	public float getAudioLengthInSecs() {
		return this.audioLength;
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
	
	public float[][] getPeaks() {
		return this.analData;	
	}
	
	public int[] getLoudnessData() {
		return this.loudTriggers;	
	}
	
	private void readAnalData() {
		StreamReader reader = new StreamReader(this.path);
		List<float[]> channelData = new List<float[]>();
		while(!reader.EndOfStream) {
			
			// Read line, check if channel data or loudness data
			String input = reader.ReadLine();
			bool channel = false;
			if(input.StartsWith("c")) channel = true;
			input = input.Substring(input.IndexOf(':')+1); // Remove the channel/loudness data "flag" at the beginning
			
			// Read line into floats for channel data
			if(channel) {
				List<float> tempList = new List<float>();
				foreach(string s in input.Split(';')) {
					float result = -1;
					float.TryParse(s,out result);
					if(result > -1)
						tempList.Add(result);
				}
				if(tempList.Count > 0 && tempList[tempList.Count-1] == 0)
					tempList.RemoveAt(tempList.Count-1);
				channelData.Add(tempList.ToArray());
			}
			// Read line into ints for loudness data
			else {
				List<int> tempList = new List<int>();
				foreach(string s in input.Split(';')) {
					int result = -1;
					int.TryParse(s,out result);
					if(result > -1)
						tempList.Add(result);
				}
				if(tempList.Count > 0 && tempList[tempList.Count-1] == 0)
					tempList.RemoveAt(tempList.Count-1);
				this.loudTriggers = tempList.ToArray();
			}	
		}
		this.analData = channelData.ToArray();
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
		this.frequency = 44100;
		this.audioLength = this.data.Length/(float)this.frequency;
		
	}
	
//	private void readMP3() {
//		
//		// Init
//		Decoder dec = new Decoder();
//		Header header;
//		int frequency;
//		int channels;
//		
//		// Open File Streams
//		System.IO.FileStream file = new System.IO.FileStream(this.path,System.IO.FileMode.Open);
//		Bitstream bS = new Bitstream(file);
//		
//		// Read mp3 header and extract freq/chan info
//		header = bS.readFrame();
//		frequency = header.frequency();
//		channels = (header.mode() == Header.SINGLE_CHANNEL) ? 1 : 2;
//		
//		// Start reading rest of the mp3
//		short[] bufferData;
//		SampleBuffer buffer;
//		float maxVal = (float)short.MaxValue;
//		ArrayList monoData = new ArrayList();
//		ArrayList channelData = new ArrayList();
//		
//		do {
//			// Get header, check if any more data available
//			header = bS.readFrame();
//            if (header == null)
//                break;
//			
//			// Decode frame
//			buffer = (SampleBuffer)dec.decodeFrame(header,bS);
//			bS.closeFrame();
//			
//			// Read PCM data
//			bufferData = buffer.getBuffer();
//			for(int i = 0; i < bufferData.Length; i+=2) {
//				channelData.Add(bufferData[i]/maxVal);
//				channelData.Add(bufferData[i+1]/maxVal);
//				monoData.Add(((bufferData[i] + bufferData[i+1])/2f)/maxVal);
//			}
//		} while(bufferData.Length > 0);
//		
//		// Convert arraylists to useable data
//		this.data = (float[])monoData.ToArray(typeof(float));
//		this.clip = AudioClip.Create("gameAudio",channelData.Count,channels,frequency,true,false);
//		this.clip.SetData((float[])channelData.ToArray(typeof(float)),0);
//		
//		monoData.Clear();
//		monoData = null;
//		channelData.Clear();
//		channelData = null;
//		
//		// Close streams
//		bS.close();
//		file.Close();
//	}
	
	private void readMp3() {
		
		IntPtr handle_mpg = new IntPtr();
		IntPtr errPtr = new IntPtr ();
		IntPtr frequency = new IntPtr();
		IntPtr channels = new IntPtr();
		IntPtr encoding = new IntPtr();
		IntPtr id3v1 = new IntPtr();
		IntPtr id3v2 = new IntPtr();
		IntPtr done = new IntPtr();
	
		String txtRate;	
		String txtChannels;
		String txtEnc;
		String txtArtist;
		String txtTitle;
		
		
		MPGImport.mpg123_init ();
		
		handle_mpg = MPGImport.mpg123_new (null, errPtr);
		
		int errorCheck = MPGImport.mpg123_open (handle_mpg, this.path);
		
		MPGImport.mpg123_getformat (handle_mpg, out frequency, out channels, out encoding);
		
		int _frequency = frequency.ToInt32 ();
		int _channels = channels.ToInt32();
		int _encoding = encoding.ToInt32 ();
		
		this.frequency = _frequency;
		
		txtRate = _frequency.ToString ();
		txtChannels = _channels.ToString ();
		txtEnc = _encoding.ToString ();
		
		MPGImport.mpg123_id3 (handle_mpg, out id3v1, out id3v2);
		MPGImport.mpg123_id3v1 MP3Data = new MPGImport.mpg123_id3v1 ();
		
		try {
			MP3Data = (MPGImport.mpg123_id3v1)Marshal.PtrToStructure (id3v1, (Type)typeof(MPGImport.mpg123_id3v1));
			
			txtArtist = new string (MP3Data.artist);
			txtTitle = new string (MP3Data.title);
			
		} catch (ArgumentNullException e) {
			String fuckoffwarnings = e.ToString ();
			Debug.Log ("No ID3v1 data");
			txtArtist = "N/A";
			txtTitle = "N/A";
		}
		
		MPGImport.mpg123_format_none (handle_mpg);
		MPGImport.mpg123_format (handle_mpg, _frequency, _channels, _encoding);
		
		int FrameSize = MPGImport.mpg123_outblock(handle_mpg);
		
		int len = MPGImport.mpg123_length(handle_mpg);
		this.data = new float[len];
		float[] clipData = new float[len*_channels];
		
		Debug.Log("len*_channels: " + (len*_channels));
		Debug.Log("len: " + len);
		
		int dataCounter = 0;
		int clipDataCounter = 0;
		float maxVal = (float)short.MaxValue;
		float fChan = (float)_channels;
		
		byte[] Buffer = new byte[FrameSize];
        while (0 == MPGImport.mpg123_read(handle_mpg, Buffer, FrameSize, out done))
        {
			for(int i = 0; i < Buffer.Length; i+=_channels*2) {
				
				for (int j = 0; j < _channels*2; j+=2) {
					float tempVal = System.BitConverter.ToInt16(Buffer,i+j);
					if(dataCounter < this.data.Length)
						this.data[dataCounter] += tempVal;
					if(clipDataCounter < clipData.Length)
						clipData[clipDataCounter] = tempVal/maxVal;
					clipDataCounter++;
				}
				if(dataCounter < this.data.Length) {
					this.data[dataCounter] /= fChan;
					this.data[dataCounter] /= maxVal;
				}
				dataCounter++;	
			}			
        }
		
		Debug.Log("data.length: " + data.Length);
		Debug.Log("dataCounter: " + dataCounter);
		Debug.Log("clipDataCounter: "+ clipDataCounter);
		Debug.Log("channels: " + _channels);
		
		float[] tempData = new float[(dataCounter < this.data.Length)? dataCounter : this.data.Length];
		System.Array.Copy(this.data,tempData,tempData.Length);
		this.data = tempData;
		
		tempData = new float[(clipDataCounter < clipData.Length)? clipDataCounter : clipData.Length];
		System.Array.Copy(clipData,tempData,tempData.Length);
		clipData = tempData;
		
		tempData = null;		
		
		this.clip = AudioClip.Create("gameAudio",clipData.Length/_channels,_channels,_frequency,true,false);
		this.clip.SetData(clipData,0);
		
		clipData = null;
		
		this.audioLength = (clipDataCounter/(float)_channels)/(float)_frequency;
		
		MPGImport.mpg123_close(handle_mpg);
		MPGImport.mpg123_delete(handle_mpg);
		MPGImport.mpg123_exit();
		
		
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
//		if(this.format == FileFormat.WAV) {
			int readLength = samples.Length;
			if(readDataPointer + samples.Length >= data.Length) readLength = samples.Length - ((readDataPointer + samples.Length) - data.Length);
			
			System.Array.Copy(data,readDataPointer,samples,0,readLength);
			readDataPointer += readLength;
			
			return readLength;
//		} else if(this.format == FileFormat.MPEG) {
//			int re = mp3Reader.readMp3(ref samples);
//			if(re == 0) mp3Reader.close();
//			return re;
//			
//		} else {
//			Debug.Log("UNSUPPORTED AUDIO FORMAT");
//			return 0;
//		}
	}
	
	public void reset() {
		readDataPointer = 0;
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
	
	private class MP3 {
		
		IntPtr handle_mpg = new IntPtr();
		IntPtr errPtr = new IntPtr ();
		IntPtr frequency = new IntPtr();
		IntPtr channels = new IntPtr();
		IntPtr encoding = new IntPtr();
		IntPtr id3v1 = new IntPtr();
		IntPtr id3v2 = new IntPtr();
		IntPtr done = new IntPtr();
	
		String txtRate;	
		String txtChannels;
		String txtEnc;
		String txtArtist;
		String txtTitle;
		String path;
		
		int errorCheck;
		int _frequency;
		int _channels;
		int _encoding;
		int FrameSize;
		int len;
		float maxVal;
		float fChan;
		
		byte[] Buffer;
		byte[] overFlow;
		
		
		private MP3(){}
		public MP3(string path) {
			this.path = path;
			init();
		}
		
		protected void init() {
			
			MPGImport.mpg123_init ();
			
			handle_mpg = MPGImport.mpg123_new (null, errPtr);
			
			errorCheck = MPGImport.mpg123_open (handle_mpg, this.path);
			
			MPGImport.mpg123_getformat (handle_mpg, out frequency, out channels, out encoding);
			
			_frequency = frequency.ToInt32 ();
			_channels = channels.ToInt32();
			_encoding = encoding.ToInt32 ();
			
			txtRate = _frequency.ToString ();
			txtChannels = _channels.ToString ();
			txtEnc = _encoding.ToString ();
			
			MPGImport.mpg123_id3 (handle_mpg, out id3v1, out id3v2);
			MPGImport.mpg123_id3v1 MP3Data = new MPGImport.mpg123_id3v1 ();
			
			try {
				MP3Data = (MPGImport.mpg123_id3v1)Marshal.PtrToStructure (id3v1, (Type)typeof(MPGImport.mpg123_id3v1));
				
				txtArtist = new string (MP3Data.artist);
				txtTitle = new string (MP3Data.title);
				
			} catch (ArgumentNullException e) {
				String fuckoffwarnings = e.ToString ();
				Debug.Log ("No ID3v1 data");
				txtArtist = "N/A";
				txtTitle = "N/A";
			}
			
			MPGImport.mpg123_format_none (handle_mpg);
			MPGImport.mpg123_format (handle_mpg, _frequency, _channels, _encoding);
			
			FrameSize = MPGImport.mpg123_outblock(handle_mpg);
		
			len = MPGImport.mpg123_length(handle_mpg);
			//float[] clipData = new float[len*_channels];
//			dataCounter = 0;
//			int clipDataCounter = 0;
			maxVal = (float)short.MaxValue;
			fChan = (float)_channels;
			
			Buffer = new byte[FrameSize];
			overFlow = new byte[0];
			
		}
		
		public int getFrequency() {
			return _frequency;	
		}
		
		public int readMp3(ref float[] outBuff) {			
			
			// Init Counters for the elements in a buffer
			int outBuffFillCounter = 0;
			int overFlowCounter = 0;
			
			// Check if we have any overflowing bytes from the previous buffer
			if(overFlow.Length > 0) {
				for(int i = 0; i < overFlow.Length; i+=_channels*2) {
					for (int j = 0; j < _channels*2; j+=2) {
						float tempVal = System.BitConverter.ToInt16(overFlow,i+j);
						outBuff[outBuffFillCounter] += tempVal;
						//clipData[clipDataCounter] = tempVal/maxVal;
						//clipDataCounter++;
					}
					outBuff[outBuffFillCounter] /= fChan;
					outBuff[outBuffFillCounter] /= maxVal;
					outBuffFillCounter++;
					
					// Have we filled the entire output buffer?
					if(outBuffFillCounter == outBuff.Length) {
						if (i+(_channels*2) < overFlow.Length) overFlowCounter = i+(_channels*2);
						else return outBuffFillCounter;
						break;
					}
				}
				
				// If the overflow array holds more samples than we want, we will have to save the overflow again!!
				if(overFlowCounter > 0) {
					byte[] tempOverFlow = new byte[overFlow.Length - overFlowCounter];
					System.Array.Copy(overFlow,overFlowCounter,tempOverFlow,0,tempOverFlow.Length);
					overFlow = tempOverFlow;
					return outBuffFillCounter;
				}
			}
			
			// After adding the overflow form last time, get the next buffer
			bool whileFlag = true;
	        while (0 == MPGImport.mpg123_read(handle_mpg, Buffer, FrameSize, out done) && outBuffFillCounter < outBuff.Length && whileFlag)
	        {
				for(int i = 0; i < Buffer.Length; i+=_channels*2) {
					
					for (int j = 0; j < _channels*2; j+=2) {
						float tempVal = System.BitConverter.ToInt16(Buffer,i+j);
						outBuff[outBuffFillCounter] += tempVal;
					}
					outBuff[outBuffFillCounter] /= fChan;
					outBuff[outBuffFillCounter] /= maxVal;
					outBuffFillCounter++;
					
					if(outBuffFillCounter == outBuff.Length) {
						if (i+(_channels*2) < Buffer.Length) overFlowCounter = i+(_channels*2);
						whileFlag = false;
						break;
					}
				}
	        }
			
			// Fill overflow if there is anything to fill
			if(overFlowCounter > 0) {
					overFlow = new byte[Buffer.Length - overFlowCounter];
					System.Array.Copy(Buffer,overFlowCounter,overFlow,0,overFlow.Length);	
			}
			return outBuffFillCounter;
		}
		
		public void close() {
			MPGImport.mpg123_close(handle_mpg);
			MPGImport.mpg123_delete(handle_mpg);
			MPGImport.mpg123_exit();
		}
		
		
	}
	
	
}