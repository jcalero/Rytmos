using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SpectrumProvider 
{
	/** the decoder to use **/
	private readonly DecoderInterface decoder;	
	
	/** the current sample array **/
	private float[] samples;
	
	/** the look ahead sample array **/
	private float[] nextSamples;
	
	/** temporary samples array **/ 
	private float[] tempSamples; 
	
	/** the current sample, always modulo sample window size **/
	private int currentSample = 0;	
	
	/** the hop size **/
	private readonly int hopSize;
	
	/** the fft **/
    private readonly FFT fft;	
	
	/**
	 * Constructor, sets the {@link Decoder}, the sample window size and the
	 * hop size for the spectra returned. Say the sample window size is 1024
	 * samples. To get an overlapp of 50% you specify a hop size of 512 samples,
	 * for 25% overlap you specify a hopsize of 256 and so on. Hop sizes are of
	 * course not limited to powers of 2. 
	 * 
	 * @param decoder The decoder to get the samples from.
	 * @param sampleWindowSize The sample window size.
	 * @param hopSize The hop size.
	 * @param useHamming Wheter to use hamming smoothing or not.
	 */
	public SpectrumProvider( DecoderInterface decoder, int sampleWindowSize, int hopSize, bool useHamming )
	{
		if( decoder == null )
			throw new ArgumentException( "Decoder must be != null" );
		
		if( sampleWindowSize <= 0 )
			throw new ArgumentException( "Sample window size must be > 0" );
		if( hopSize <= 0 )
			throw new ArgumentException( "Hop size must be > 0" );
		
		if( sampleWindowSize < hopSize )
			throw new ArgumentException( "Hop size must be <= sampleSize" );


        this.decoder = decoder;		
		this.samples = new float[sampleWindowSize];
		this.nextSamples = new float[sampleWindowSize];
		this.tempSamples = new float[sampleWindowSize];
		this.hopSize = hopSize;			
		fft = new FFT( sampleWindowSize, AudioManager.frequency );
		if( useHamming )
			fft.window(FFT.HAMMING);
		
        decoder.readSamples( ref samples );
		decoder.readSamples( ref nextSamples );
	}
	
	public float[] getCurrentSamples() {
		return samples;	
	}
	
	public void getCurrentSamples(ref float[] inArray) {
		inArray = samples;
	}
	
	/**
	 * Returns the next spectrum or null if there's no more data.
	 * @return The next spectrum or null.
	 */
	public float[] nextSpectrum( )
	{		
		if( currentSample >= samples.Length )
		{
			float[] tmp = nextSamples;
			nextSamples = samples;
			samples = tmp;
			if( decoder.readSamples( ref nextSamples ) == 0 )
				return null;
			currentSample -= samples.Length;
		}
		
		Array.Copy( samples, currentSample, tempSamples, 0, samples.Length - currentSample );
		Array.Copy( nextSamples, 0, tempSamples, samples.Length - currentSample, currentSample );					
		fft.forward( tempSamples );		
		currentSample += hopSize;						
		return fft.getSpectrum();
	}
	
	/**
	 * @return the FFT instance used
	 */
	public FFT getFFT( )
	{
		return fft;
	}
}
