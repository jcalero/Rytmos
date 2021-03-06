﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Decoder interface.
/// This interface needs to be extended by any class
/// which provides data to the SoundProcessor.
/// </summary>
public interface DecoderInterface
{	/**
	 * Reads in samples.length samples from the decoder. Returns
	 * the actual number read in. If this number is smaller than
	 * samples.length then the end of stream has been reached.
	 * 
	 * @param samples The number of read samples.
	 */
	int readSamples (ref float[] samples);

	void reset ();
}