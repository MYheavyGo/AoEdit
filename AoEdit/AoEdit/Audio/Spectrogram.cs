using System;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace AoEdit.Audio
{
    class Spectrogram
    {
        float[] Sound { get; set; }
        Complex[] complexInput { get; set; }

        public Spectrogram(float[] sound)
        {
            Sound = sound;
            complexInput = new Complex[sound.Length];

            for(int i = 0;i < complexInput.Length; i++)
            {
                Complex tmp = new Complex(Sound[i], 0);
                complexInput[i] = tmp;
            }

            Fourier.BluesteinForward(complexInput, FourierOptions.NoScaling);
            //Fourier.Forward(complexInput);

            //Do something

            //Fourier.BluesteinInverse(complexInput, MathNet.Numerics.IntegralTransforms.FourierOptions.Default);

            float[] outSamples = new float[complexInput.Length];
            for(int i = 0;i < outSamples.Length; i++)
            {
                outSamples[i] = (float)complexInput[i].Real;
            }
        
            Console.WriteLine(outSamples[0]);
        }
    }
}
