using UnityEngine;
using System.Collections;

/// <summary>
/// Mock decoder.
/// This class is used when we already somehow have all the music data
/// and just need the interface so that we can do the analysis on it
/// </summary>
public class MockDecoder : DecoderInterface
{
	
	private float[] data;
	private int pointer;
	
	private MockDecoder ()
	{
	}
	
	public MockDecoder (float[] data)
	{
		this.data = data;
		pointer = 0;
		
	}
	
	public int readSamples (ref float[] samples)
	{
		
		int readLength = samples.Length;
		if (pointer + samples.Length >= data.Length)
			readLength = samples.Length - ((pointer + samples.Length) - data.Length);
		
		System.Array.Copy (data, pointer, samples, 0, readLength);
		pointer += readLength;
		
		return readLength;
	}
	
	public void reset ()
	{
		pointer = 0;	
	}
}
