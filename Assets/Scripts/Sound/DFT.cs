using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    public class DFT : FourierTransform
    {

        public DFT(int timeSize, float sampleRate) : base(timeSize, sampleRate)
        {
            if (timeSize % 2 != 0)
                throw new ArgumentException("DFT: timesize must be even.");
            buildTrigTables();
        }

        protected override void allocateArrays()
        {
            spectrum = new float[timeSize / 2 + 1];
            real = new float[timeSize / 2 + 1];
            imag = new float[timeSize / 2 + 1];
        }

        /**
         * Not currently implemented.
         */
        public override void scaleBand(int i, float s)
        {
        }

        /**
         * Not currently implemented.
         */
        public override void setBand(int i, float a)
        {
        }

        public override void forward(float[] samples)
        {
            if (samples.Length != timeSize)
            {
                throw new ArgumentException("DFT.forward: The length of the passed sample buffer must be equal to DFT.timeSize().");
            }
            doWindow(samples);
            int N = samples.Length;
            for (int f = 0; f <= N / 2; f++)
            {
                real[f] = 0.0f;
                imag[f] = 0.0f;
                for (int t = 0; t < N; t++)
                {
                    real[f] += samples[t] * cos(t * f);
                    imag[f] += samples[t] * -sin(t * f);
                }
            }
            fillSpectrum();
        }

        public override void inverse(float[] buffer)
        {
            int N = buffer.Length;
            real[0] /= N;
            imag[0] = -imag[0] / (N / 2);
            real[N / 2] /= N;
            imag[N / 2] = -imag[0] / (N / 2);
            for (int i = 0; i < N / 2; i++)
            {
                real[i] /= (N / 2);
                imag[i] = -imag[i] / (N / 2);
            }
            for (int t = 0; t < N; t++)
            {
                buffer[t] = 0.0f;
                for (int f = 0; f < N / 2; f++)
                {
                    buffer[t] += real[f] * cos(t * f) + imag[f] * sin(t * f);
                }
            }
        }

        // lookup table data and functions

        private float[] sinlookup;
        private float[] coslookup;

        private void buildTrigTables()
        {
            int N = spectrum.Length * timeSize;
            sinlookup = new float[N];
            coslookup = new float[N];
            for (int i = 0; i < N; i++)
            {
                sinlookup[i] = (float)Math.Sin(i * TWO_PI / timeSize);
                coslookup[i] = (float)Math.Cos(i * TWO_PI / timeSize);
            }
        }

        private float sin(int i)
        {
            return sinlookup[i];
        }

        private float cos(int i)
        {
            return coslookup[i];
        }
    }
