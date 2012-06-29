using UnityEngine;
using System.Collections;

public class MP3Decoder : DecoderInterface
{
	private FileReader reader;
	private float[] data;
	private int pointer;
	public MP3Decoder(string path) {
		reader = new FileReader(path,FileReader.AudioFormat.WAV);
		reader.read();
		while(reader.isReading()) yieldRoutine();
		data = reader.getData();
	}
	
	public int readSamples(ref float[] samples) {
		
		int readLength = samples.Length;
		if(pointer + samples.Length >= data.Length) readLength = samples.Length - ((pointer + samples.Length) - data.Length);
		
		System.Array.Copy(data,pointer,samples,0,readLength);
		pointer += readLength;
		
		return readLength;
	}
	
	private IEnumerator yieldRoutine () {
		yield return 0;
	}
}

