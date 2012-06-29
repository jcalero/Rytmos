using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThresholdFunction {
   
   private readonly int historySize;
   private readonly float multiplier;
   
   	public ThresholdFunction( int historySize, float multiplier ) {
		this.historySize = historySize;
		this.multiplier = multiplier;
	}
   
   	public ArrayList calculate( ArrayList spectralFlux )
	{
		ArrayList thresholds = new ArrayList( spectralFlux.Count );
		
		for( int i = 0; i < spectralFlux.Count; i++ )
		{
			float sum = 0;
			int start = Mathf.Max( 0, i - historySize / 2);
			int end = Mathf.Min( spectralFlux.Count-1, i + historySize / 2 );
			for( int j = start; j <= end; j++ )
				sum += (float)spectralFlux[j];
			sum /= (end-start);
			sum *= multiplier;
			thresholds.Add( sum );
		}
		
		return thresholds;
	}
   
}