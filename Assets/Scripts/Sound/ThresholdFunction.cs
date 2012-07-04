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
   
	public float[] calculate(float[] spectralFlux) {
			
		float[] thresholds = new float[ spectralFlux.Length ];
		
		for( int i = 0; i < spectralFlux.Length; i++ )
		{
			float sum = 0;
			int start = Mathf.Max( 0, i - historySize / 2);
			int end = Mathf.Min( spectralFlux.Length-1, i + historySize / 2 );
			for( int j = start; j <= end; j++ )
				sum += spectralFlux[j];
			sum /= (end-start);
			sum *= multiplier;
			thresholds[i] = sum ;
		}
		
		return thresholds;
		
	}
   
}