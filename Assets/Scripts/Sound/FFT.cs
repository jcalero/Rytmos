﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/**
 * FFT stands for Fast Fourier Transform. It is an efficient way to calculate the Complex 
 * Discrete Fourier Transform. There is not much to say about this class other than the fact 
 * that when you want to analyze the spectrum of an audio buffer you will almost always use 
 * this class. One restriction of this class is that the audio buffers you want to analyze 
 * must have a length that is a power of two. If you try to construct an FFT with a 
 * <code>timeSize</code> that is not a power of two, an IllegalArgumentException will be 
 * thrown.
 * 
 * @see FourierTransform
 * @see <a href="http://www.dspguide.com/ch12.htm">The Fast Fourier Transform</a>
 * 
 * @author Damien Di Fede
 * 
 */
public class FFT : FourierTransform
{
  /**
   * Constructs an FFT that will accept sample buffers that are
   * <code>timeSize</code> long and have been recorded with a sample rate of
   * <code>sampleRate</code>. <code>timeSize</code> <em>must</em> be a
   * power of two. This will throw an exception if it is not.
   * 
   * @param timeSize
   *          the length of the sample buffers you will be analyzing
   * @param sampleRate
   *          the sample rate of the audio you will be analyzing
   */
  public FFT(int timeSize, float sampleRate) : base(timeSize, sampleRate)
  {
   
    if ((timeSize & (timeSize - 1)) != 0)
      throw new ArgumentException(
          "FFT: timeSize must be a power of two.");
    buildReverseTable();
    buildTrigTables();
  }

  protected override void allocateArrays()
  {
    spectrum = new float[timeSize / 2 + 1];
    real = new float[timeSize];
    imag = new float[timeSize];
  }

  public override void scaleBand(int i, float s)
  {
    if (s < 0)
    {
      throw new ArgumentException("Can't scale a frequency band by a negative value.");
    }
    if (spectrum[i] != 0)
    {
      real[i] /= spectrum[i];
      imag[i] /= spectrum[i];
      spectrum[i] *= s;
      real[i] *= spectrum[i];
      imag[i] *= spectrum[i];
    }
    if (i != 0 && i != timeSize / 2)
    {
      real[timeSize - i] = real[i];
      imag[timeSize - i] = -imag[i];
    }
  }

  public override void setBand(int i, float a)
  {
    if (a < 0)
    {
      throw new ArgumentException("Can't set a frequency band to a negative value.");
    }
    if (real[i] == 0 && imag[i] == 0)
    {
      real[i] = a;
      spectrum[i] = a;
    }
    else
    {
      real[i] /= spectrum[i];
      imag[i] /= spectrum[i];
      spectrum[i] = a;
      real[i] *= spectrum[i];
      imag[i] *= spectrum[i];
    }
    if (i != 0 && i != timeSize / 2)
    {
      real[timeSize - i] = real[i];
      imag[timeSize - i] = -imag[i];
    }
  }

  // performs an in-place fft on the data in the real and imag arrays
  // bit reversing is not necessary as the data will already be bit reversed
  private void fft()
  {
    for (int halfSize = 1; halfSize < real.Length; halfSize *= 2)
    {
      // float k = -(float)Math.PI/halfSize;
      // phase shift step
      // float phaseShiftStepR = (float)Math.cos(k);
      // float phaseShiftStepI = (float)Math.sin(k);
      // using lookup table
      float phaseShiftStepR = cos(halfSize);
      float phaseShiftStepI = sin(halfSize);
      // current phase shift
      float currentPhaseShiftR = 1.0f;
      float currentPhaseShiftI = 0.0f;
      for (int fftStep = 0; fftStep < halfSize; fftStep++)
      {
        for (int i = fftStep; i < real.Length; i += 2 * halfSize)
        {
          int off = i + halfSize;
          float tr = (currentPhaseShiftR * real[off]) - (currentPhaseShiftI * imag[off]);
          float ti = (currentPhaseShiftR * imag[off]) + (currentPhaseShiftI * real[off]);
          real[off] = real[i] - tr;
          imag[off] = imag[i] - ti;
          real[i] += tr;
          imag[i] += ti;
        }
        float tmpR = currentPhaseShiftR;
        currentPhaseShiftR = (tmpR * phaseShiftStepR) - (currentPhaseShiftI * phaseShiftStepI);
        currentPhaseShiftI = (tmpR * phaseShiftStepI) + (currentPhaseShiftI * phaseShiftStepR);
      }
    }
  }

  public override void forward(float[] buffer)
  {
    if (buffer.Length != timeSize)
    {
    	throw new ArgumentException("FFT.forward: The length of the passed sample buffer must be equal to timeSize().");
    }
    doWindow(buffer);
    // copy samples to real/imag in bit-reversed order
    bitReverseSamples(buffer);
    // perform the fft
    fft();
    // fill the spectrum buffer with amplitudes
    fillSpectrum();
  }

  /**
   * Performs a forward transform on the passed buffers.
   * 
   * @param buffReal the real part of the time domain signal to transform
   * @param buffImag the imaginary part of the time domain signal to transform
   */
  public void forward(float[] buffReal, float[] buffImag)
  {
    if (buffReal.Length != timeSize || buffImag.Length != timeSize)
    {
    	throw new ArgumentException("FFT.forward: The length of the passed buffers must be equal to timeSize().");
    }
    setComplex(buffReal, buffImag);
    bitReverseComplex();
    fft();
    fillSpectrum();
  }

  public override void inverse(float[] buffer)
  {
    if (buffer.Length > real.Length)
    {
    	throw new ArgumentException("FFT.inverse: the passed array's length must equal FFT.timeSize().");
    }
    // conjugate
    for (int i = 0; i < timeSize; i++)
    {
      imag[i] *= -1;
    }
    bitReverseComplex();
    fft();
    // copy the result in real into buffer, scaling as we do
    for (int i = 0; i < buffer.Length; i++)
    {
      buffer[i] = real[i] / real.Length;
    }
  }

  private int[] reverse;

  private void buildReverseTable()
  {
    int N = timeSize;
    reverse = new int[N];

    // set up the bit reversing table
    reverse[0] = 0;
    for (int limit = 1, bit = N / 2; limit < N; limit <<= 1, bit >>= 1)
      for (int i = 0; i < limit; i++)
        reverse[i + limit] = reverse[i] + bit;
  }

  // copies the values in the samples array into the real array
  // in bit reversed order. the imag array is filled with zeros.
  private void bitReverseSamples(float[] samples)
  {
    for (int i = 0; i < samples.Length; i++)
    {
      real[i] = samples[reverse[i]];
      imag[i] = 0.0f;
    }
  }

  // bit reverse real[] and imag[]
  private void bitReverseComplex()
  {
    float[] revReal = new float[real.Length];
    float[] revImag = new float[imag.Length];
    for (int i = 0; i < real.Length; i++)
    {
      revReal[i] = real[reverse[i]];
      revImag[i] = imag[reverse[i]];
    }
    real = revReal;
    imag = revImag;
  }

  // lookup tables

  private float[] sinlookup;
  private float[] coslookup;

  private float sin(int i)
  {
    return sinlookup[i];
  }

  private float cos(int i)
  {
    return coslookup[i];
  }

  private void buildTrigTables()
  {
    int N = timeSize;
    sinlookup = new float[N];
    coslookup = new float[N];
    for (int i = 0; i < N; i++)
    {
      sinlookup[i] = (float) Math.Sin(-(float) Math.PI / i);
      coslookup[i] = (float) Math.Cos(-(float) Math.PI / i);
    }
  }
}