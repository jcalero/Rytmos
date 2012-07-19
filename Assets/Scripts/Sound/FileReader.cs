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
	private int[][] rytData;
	private int[] loudTriggers;
	private MP3 mp3Reader;
	private int channels;
	private int mp3FrameSize;

	// Supported Audio Formats.. not all of them are in yet!!
	public enum FileFormat { WAV, OGG, MPEG, RYT, ERROR };
	public enum ReadStatus { SUCCESS, FAIL, UNSUPPORTED_FORMAT };

	// Disable empty constructor
	private FileReader() { }

	// Constructor
	public FileReader(string path) {
		// Init instance vars
		reading = false;
		clip = null;
		this.path = path;
		if (path.EndsWith(".wav")) this.format = FileFormat.WAV;
		else if (path.EndsWith(".mp3")) this.format = FileFormat.MPEG;
		else if (path.EndsWith(".ogg")) this.format = FileFormat.OGG;
		else if (path.EndsWith(".ryt")) this.format = FileFormat.RYT;
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
		if (this.path == null || this.path == "") {
			Debug.LogError("Path not specified!");
			this.reading = false;
			return ReadStatus.FAIL;
		} else if (this.format == FileFormat.OGG || this.format == FileFormat.ERROR) {
			Debug.Log("This format: " + this.format.ToString() + " is not supported yet");
			this.reading = false;
			return ReadStatus.UNSUPPORTED_FORMAT;
		}

		// Get audioclip according to file format
		switch (this.format) {
			case FileFormat.WAV:
				readWAV();
				break;
			case FileFormat.MPEG:
				//readMp3();
				mp3Reader = new MP3(this.path);
				this.frequency = mp3Reader.getFrequency();
				this.channels = mp3Reader.getChannels();
				this.mp3FrameSize = mp3Reader.getFrameSize();
				break;
			case FileFormat.RYT:
				readRytData();
				break;
		}
		this.reading = false;
		return ReadStatus.SUCCESS;

	}

	public int getFrequency() {
		return frequency;
	}

	public int getFrameSize() {
		if (this.format == FileFormat.MPEG) return this.mp3FrameSize;
		else return 0;
	}

	public float getAudioLengthInSecs() {
		if (this.format == FileFormat.WAV) return this.audioLength;
		else if (this.format == FileFormat.MPEG) return mp3Reader.audioLength;
		else return 0;
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

		if (this.clip == null) {
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

		if (data == null || data.Length == 0) {
			Debug.Log("No bytes available. File not read, or reading failed?");
			return null;
		}

		return this.data;
	}

	public int[][] getPeaks() {
		return this.rytData;
	}

	public string getArtist() {
		if (this.format == FileFormat.MPEG) {
			return mp3Reader.getArtist();
		} else return "Unknown";
	}

	public string getTitle() {
		if (this.format == FileFormat.MPEG) {
			return mp3Reader.getTitle();
		} else return "Unknown";
	}

	public int[] getLoudnessData() {
		return this.loudTriggers;
	}

	public int getChannels() {
		return this.channels;
	}

	private void readRytData() {
		StreamReader reader = new StreamReader(this.path);
		List<int[]> channelData = new List<int[]>();
		while (!reader.EndOfStream) {

			// Read line, check if channel data or loudness data
			String input = reader.ReadLine();
			bool channel = false;
			if (input.StartsWith("c")) channel = true;
			input = input.Substring(input.IndexOf(':') + 1); // Remove the channel/loudness data "flag" at the beginning

			// Read line into floats for channel data
			if (channel) {
				List<int> tempList = new List<int>();
				foreach (string s in input.Split(';')) {
					int result = -1;
					int.TryParse(s, out result);
					if (result > -1)
						tempList.Add(result);
				}
				if (tempList.Count > 0 && tempList[tempList.Count - 1] == 0)
					tempList.RemoveAt(tempList.Count - 1);
				channelData.Add(tempList.ToArray());
			}
				// Read line into ints for loudness data
			else {
				List<int> tempList = new List<int>();
				foreach (string s in input.Split(';')) {
					int result = -1;
					int.TryParse(s, out result);
					if (result > -1)
						tempList.Add(result);
				}
				if (tempList.Count > 0 && tempList[tempList.Count - 1] == 0)
					tempList.RemoveAt(tempList.Count - 1);
				this.loudTriggers = tempList.ToArray();
			}
		}
		this.rytData = channelData.ToArray();
	}

	private void readWAV() {

		// Read WAV
		WWW fileLoader = new WWW("file://" + this.path);
		yieldRoutine(fileLoader);
		if (fileLoader.bytes == null || fileLoader.bytes.Length == 0) {
			Debug.LogError("File could not be read correctly");
			return;
		}

		// "else"..
		// Get rid of WAV header
		byte[] noHeader = new byte[fileLoader.bytes.Length - 44];
		System.Array.Copy(fileLoader.bytes, 44, noHeader, 0, noHeader.Length);

		// Convert byte data to float data
		this.data = byteArrayToFloatArray(noHeader);
		this.clip = fileLoader.GetAudioClip(true, false, AudioType.WAV);
		this.frequency = 44100;
		this.audioLength = this.data.Length / (float)this.frequency;

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
		for (int i = 0; i < inputData.Length; i += 4) {
			if (i + 4 > inputData.Length) {
				Debug.Log(i);
				Debug.Log(inputData.Length);
				break;
			}
			float left = System.BitConverter.ToInt16(inputData, i);
			float right = System.BitConverter.ToInt16(inputData, i + 2);
			outData[k] = (left + right) / 2f;
			k++;
		}

		return outData;
	}

	public int readSamples(ref float[] samples) {
		return readSamples(ref samples, true);
	}

	public int readSamples(ref float[] samples, bool forAnalysis) {
		if (this.format == FileFormat.WAV) {
			int readLength = samples.Length;
			if (readDataPointer + samples.Length >= data.Length) readLength = samples.Length - ((readDataPointer + samples.Length) - data.Length);

			System.Array.Copy(data, readDataPointer, samples, 0, readLength);
			readDataPointer += readLength;

			return readLength;
		} else if (this.format == FileFormat.MPEG) {
			int re = 0;
			if (forAnalysis) re = mp3Reader.readMp3ForAnalysis(ref samples);
			else re = mp3Reader.readMp3ForPlayback(ref samples);
			//if(re < samples.Length) mp3Reader.close();
			return re;

		} else {
			Debug.Log("UNSUPPORTED AUDIO FORMAT");
			return 0;
		}
	}

	public void reset() {
		readDataPointer = 0;
		mp3Reader.close();
		mp3Reader = new MP3(this.path);
	}

	public void close() {
		if (mp3Reader != null) mp3Reader.close();
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
		IntPtr errPtr = new IntPtr();
		IntPtr frequency = new IntPtr();
		IntPtr channels = new IntPtr();
		IntPtr encoding = new IntPtr();
		IntPtr id3v1 = new IntPtr();
		IntPtr id3v2 = new IntPtr();
		IntPtr done = new IntPtr();

		String txtRate;
		String txtChannels;
		String txtEnc;
		String txtArtist = "Unknown";
		String txtTitle = "Unknown";
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
		public float audioLength;

		private MP3() { }
		public MP3(string path) {
			this.path = path;
			init();
		}

		protected void init() {

			MPGImport.mpg123_init();

			handle_mpg = MPGImport.mpg123_new(null, errPtr);

			errorCheck = MPGImport.mpg123_open(handle_mpg, this.path);

			MPGImport.mpg123_getformat(handle_mpg, out frequency, out channels, out encoding);

			_frequency = frequency.ToInt32();
			_channels = channels.ToInt32();
			_encoding = encoding.ToInt32();

			txtRate = _frequency.ToString();
			txtChannels = _channels.ToString();
			txtEnc = _encoding.ToString();
			Debug.Log("This gets rid of warnings: " + txtRate + " " + txtChannels + " " + txtEnc + " " + errorCheck + " " + id3v2 + " " + len + " " + done + " " + txtArtist + " " + txtTitle);

			MPGImport.mpg123_id3(handle_mpg, out id3v1, out id3v2);
			MPGImport.mpg123_id3v1 MP3Data = new MPGImport.mpg123_id3v1();
			MPGImport.mpg123_id3v2 MP3v2Data = new MPGImport.mpg123_id3v2();

			// Try getting id3v2 data
			try {
				MP3v2Data = (MPGImport.mpg123_id3v2)Marshal.PtrToStructure(id3v2, (Type)typeof(MPGImport.mpg123_id3v2));
			} catch (ArgumentNullException e) {
				Debug.Log("No ID3v2 Data found: " + e.Message);
			}
			// ID3v2: Artist
			try {
				MPGImport.mpg123_string ptrArtist = (MPGImport.mpg123_string)Marshal.PtrToStructure(MP3v2Data.artist, (Type)typeof(MPGImport.mpg123_string));
				txtArtist = ptrArtist.p;
			} catch (ArgumentNullException e) {
				Debug.Log("No ID3v2 artist found: " + e.ToString());
			}
			// ID3v2: Title
			try {
				MPGImport.mpg123_string ptrTitle = (MPGImport.mpg123_string)Marshal.PtrToStructure(MP3v2Data.title, (Type)typeof(MPGImport.mpg123_string));
				txtTitle = ptrTitle.p;
			} catch (ArgumentNullException e) {
				Debug.Log("No ID3v2 title found: " + e.ToString());
			}

			// Try getting id3v1 data if not all id3v2 data was found
			if (txtArtist == "Unknown" || txtTitle == "Unknown") {
				try {
				} catch (ArgumentNullException e) {
					Debug.Log("No ID3v1 Data found: " + e.Message);
					MP3Data = (MPGImport.mpg123_id3v1)Marshal.PtrToStructure(id3v1, (Type)typeof(MPGImport.mpg123_id3v1));
				}
			}
			// ID3v1: Artist
			if (txtArtist == "Unknown") {
				try {
					txtArtist = new string(MP3Data.artist);
					if (txtArtist.Trim().Length < 1)
						txtArtist = "Unknown";
					Debug.Log("ID3v1 Artist Found: " + txtArtist);
				} catch (ArgumentNullException e) {
					Debug.Log("No ID3v1 artist: " + e.ToString());
				}
			}
			// ID3v1: Title
			if (txtTitle == "Unknown") {
				try {
					txtTitle = new string(MP3Data.title);
					if (txtTitle.Trim().Length < 1)
						txtTitle = "Unknown";
					Debug.Log("ID3v1 Title Found: " + txtTitle);
				} catch (ArgumentNullException e) {
					Debug.Log("No ID3v1 title: " + e.ToString());
				}
			}
			
			Debug.Log("Final id3 data: " + txtArtist + " - " + txtTitle);
			
			MPGImport.mpg123_format_none(handle_mpg);
			MPGImport.mpg123_format(handle_mpg, _frequency, _channels, _encoding);

			FrameSize = MPGImport.mpg123_outblock(handle_mpg);

			len = MPGImport.mpg123_length(handle_mpg);
			//float[] clipData = new float[len*_channels];
			//			dataCounter = 0;
			//			int clipDataCounter = 0;
			maxVal = (float)short.MaxValue;
			fChan = (float)_channels;

			Buffer = new byte[FrameSize];
			overFlow = new byte[0];

			audioLength = len / (float)_frequency;
		}

		public int getFrequency() {
			return _frequency;
		}

		public string getTitle() {
			return txtTitle;
		}

		public string getArtist() {
			return txtArtist;
		}

		public int getChannels() {
			return _channels;
		}

		public int getFrameSize() {
			return FrameSize;
		}

		public int readMp3ForPlayback(ref float[] outBuff) {

			// Init Counters for the elements in a buffer
			int outBuffFillCounter = 0;

			while (outBuffFillCounter < outBuff.Length && 0 == MPGImport.mpg123_read(handle_mpg, Buffer, FrameSize, out done)) {
				for (int i = 0; i < Buffer.Length; i += _channels * 2) {

					for (int j = 0; j < _channels * 2; j += 2) {

						float tempVal = System.BitConverter.ToInt16(Buffer, i + j);
						outBuff[outBuffFillCounter] = tempVal / maxVal;
						outBuffFillCounter++;
					}
				}
			}
			return outBuffFillCounter;
		}

		public int readMp3ForAnalysis(ref float[] outBuff) {

			// Init Counters for the elements in a buffer
			int outBuffFillCounter = 0;
			int overFlowCounter = 0;

			// Check if we have any overflowing bytes from the previous buffer
			if (overFlow.Length > 0) {

				for (int i = 0; i < overFlow.Length; i += _channels * 2) {

					for (int j = 0; j < _channels * 2; j += 2) {

						float tempVal = System.BitConverter.ToInt16(overFlow, i + j);
						outBuff[outBuffFillCounter] += tempVal;
						//clipData[clipDataCounter] = tempVal/maxVal;
						//clipDataCounter++;
					}
					outBuff[outBuffFillCounter] /= fChan;
					outBuff[outBuffFillCounter] /= maxVal;
					outBuffFillCounter++;

					// Have we filled the entire output buffer?
					if (outBuffFillCounter == outBuff.Length) {

						if (i + (_channels * 2) < overFlow.Length) {
							overFlowCounter = i + (_channels * 2);
							break;
						} else {
							overFlow = new byte[0];
							return outBuffFillCounter;
						}
					}
				}

				// If the overflow array holds more samples than we want, we will have to save the overflow again!!
				if (overFlowCounter > 0) {
					byte[] tempOverFlow = new byte[overFlow.Length - overFlowCounter];
					System.Array.Copy(overFlow, overFlowCounter, tempOverFlow, 0, tempOverFlow.Length);
					overFlow = tempOverFlow;
					return outBuffFillCounter;
				}
			}

			// All overflow should have been passed to outBuff at this point, so we reset the array
			overFlow = new byte[0];

			// After adding the overflow form last time, get the next buffer
			while (outBuffFillCounter < outBuff.Length && 0 == MPGImport.mpg123_read(handle_mpg, Buffer, FrameSize, out done)) {
				for (int i = 0; i < Buffer.Length; i += _channels * 2) {

					for (int j = 0; j < _channels * 2; j += 2) {

						float tempVal = System.BitConverter.ToInt16(Buffer, i + j);
						outBuff[outBuffFillCounter] += tempVal;

					}

					outBuff[outBuffFillCounter] /= fChan;
					outBuff[outBuffFillCounter] /= maxVal;
					outBuffFillCounter++;

					if (outBuffFillCounter == outBuff.Length) {
						if (i + (_channels * 2) < Buffer.Length) overFlowCounter = i + (_channels * 2);

						break;
					}
				}
			}

			// Fill overflow if there is anything to fill
			if (overFlowCounter > 0) {
				overFlow = new byte[Buffer.Length - overFlowCounter];
				System.Array.Copy(Buffer, overFlowCounter, overFlow, 0, overFlow.Length);
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