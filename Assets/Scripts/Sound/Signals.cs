using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Signals
    {
        public static float mean(float[] signal)
        {
            float mean = 0;
            for (int i = 0; i < signal.Length; i++)
                mean += signal[i];
            mean /= signal.Length;
            return mean;
        }

        public static float energy(float[] signal)
        {
            float totalEnergy = 0;
            for (int i = 0; i < signal.Length; i++)
                totalEnergy += (signal[i] * signal[i]);
            return totalEnergy;
        }

        public static float power(float[] signal)
        {
            return energy(signal) / signal.Length;
        }

        public static float norm(float[] signal)
        {
            return (float)Math.Sqrt(energy(signal));
        }

        public static float minimum(float[] signal)
        {
            float min = float.PositiveInfinity;
            for (int i = 0; i < signal.Length; i++)
                min = Math.Min(min, signal[i]);
            return min;
        }

        public static float maximum(float[] signal)
        {
            float max = float.NegativeInfinity;
            for (int i = 0; i < signal.Length; i++)
                max = Math.Max(max, signal[i]);
            return max;
        }

        public static void scale(float[] signal, float scale)
        {
            for (int i = 0; i < signal.Length; i++)
                signal[i] *= scale;
        }
    }
