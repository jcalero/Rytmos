using UnityEngine;
using System.Collections;

public class Test
{
	public static readonly string FILE = "C:/Users/Samuel/Desktop/trudat.wav";
	public static readonly int HOP_SIZE = 512;
	public static readonly int HISTORY_SIZE = 50;
	public static readonly float[] multipliers = { 2f, 2f, 2f };
	public static readonly float[] bands = { 80, 4000, 4000, 10000, 10000, 16000 };
	
	public static void test()
	{		
		MP3Decoder decoder = new MP3Decoder( FILE );
		SpectrumProvider spectrumProvider = new SpectrumProvider( decoder, 1024, HOP_SIZE, true );			
		float[] spectrum = spectrumProvider.nextSpectrum();
		float[] lastSpectrum = new float[spectrum.Length];
		ArrayList spectralFlux = new ArrayList();
		for( int i = 0; i < bands.Length / 2; i++ )
			spectralFlux.Add( new ArrayList( ) );
				
		do
		{						
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
				((ArrayList)spectralFlux[i/2]).Add( flux );
			}
					
			System.Array.Copy( spectrum, 0, lastSpectrum, 0, spectrum.Length );
		}
		while( (spectrum = spectrumProvider.nextSpectrum() ) != null );				
		
		ArrayList thresholds = new ArrayList( );
		ArrayList prunnedSpectralFlux = new ArrayList(spectralFlux.Count);
		for( int i = 0; i < bands.Length / 2; i++ )
		{
			ArrayList threshold = new ThresholdFunction( HISTORY_SIZE, multipliers[i] ).calculate( (ArrayList) spectralFlux[i] );
			thresholds.Add( threshold );
			
			ArrayList tempPSF = new ArrayList(((ArrayList)spectralFlux[i]).Count);
			for(int j = 0; j < ((ArrayList)spectralFlux[i]).Count; j++) {
				if ((float)threshold[j] <= (float)((ArrayList)spectralFlux[i])[j] )
		      		tempPSF.Add( (float)((ArrayList)spectralFlux[i])[j] - (float)threshold[j]  );
		  		else
		      		tempPSF.Add( 0f );	
			}
			prunnedSpectralFlux.Add(tempPSF);
		}
	}
}

