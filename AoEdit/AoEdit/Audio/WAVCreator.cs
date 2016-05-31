using System;
using System.Collections.Generic;

namespace AoEdit.Audio
{
    class WAVCreator
    {
        private enum FormWAV
        {
            Null = -1,
            Sin,
            Square,
            Triangle,
            Sawtooth,
            WhiteNoise
        }

        public DataHeader Header { get; set; }
        public byte[] Buffer { get; set; }
        private FormWAV Form { get; set; }

        public WAVCreator(short formatAudio, short channels, int frequency, short bitspersample, uint time, int formWAV, string path)
        {
            Header = CreateHeader(formatAudio, channels, frequency, bitspersample);
            Form = (FormWAV)formWAV;
            CreateSamples(time);
            WAVFile.WriteFile(path, Header, Buffer);
        }

        private void CreateSamples(uint time)
        {
            uint numSamples = (uint)(Header.Frequency * Header.Channels) * time + (uint)Header.Channels;
            short[] buffer = new short[numSamples];
            short amplitude = short.MaxValue;
            double freq = 440.0f;

            double t = (Math.PI * 2 * freq) / Header.Frequency;//(Math.PI * 2 * freq) / (Header.Frequency);
            double samplesPerWaveLenght = Header.Frequency / (freq / Header.Channels);

            switch (Form)
            {
                case FormWAV.Sin:
                    Buffer = ShortToByte(Sin(numSamples, Header.Channels, amplitude, t));
                    break;
                case FormWAV.Square:
                    Buffer = ShortToByte(Square(numSamples, Header.Channels, amplitude, t));
                    break;
                case FormWAV.Sawtooth:
                    Buffer = ShortToByte(Sawtooth(numSamples, Header.Channels, samplesPerWaveLenght, amplitude));
                    break;
                case FormWAV.Triangle:
                    Buffer = ShortToByte(Triangle(numSamples, Header.Channels, samplesPerWaveLenght, amplitude));
                    break;
                case FormWAV.WhiteNoise:
                    Buffer = ShortToByte(WhiteNoise(numSamples, Header.Channels, amplitude));
                    break;

            }

            Header.ChunkSizeData = buffer.Length * Header.BlockAlign;
            Header.FileLenght = Header.SizeOfBytes[2] + (Header.SizeOfBytes[3] + Header.SizeOfBytes[4] + Header.ChunkSizeFormat) + (Header.SizeOfBytes[11] + Header.SizeOfBytes[12] + Header.ChunkSizeData);
        }

        private byte[] ShortToByte(short[] data)
        {
            byte[] tmp = new byte[data.Length * Header.Channels];

            for (int i = 0, j = 0; i < data.Length; i++, j += Header.Channels)
            {
                var a = BitConverter.GetBytes(data[i]);

                for(int k = 0; k < Header.Channels; k++)
                {
                    tmp[j + k] = a[k];
                }
            }

            return tmp;
        }

        private DataHeader CreateHeader(short formatAudio, short channels, int frequency, short bitspersample)
        {
            DataHeader tmp = new DataHeader();
            tmp.FileTypeID = "RIFF".ToCharArray();
            tmp.MediaTypedID ="WAVE".ToCharArray();
            tmp.ChunkIDFormat = "fmt ".ToCharArray();
            tmp.ChunkIDData = "data".ToCharArray();
            tmp.ChunkSizeFormat = 16;
            tmp.FormatTag = formatAudio;
            tmp.Channels = channels;
            tmp.Frequency = frequency;
            tmp.BitsPerSample = bitspersample;
            tmp.BlockAlign = (short) (tmp.Channels * (tmp.BitsPerSample / 8));
            tmp.AverageBytesPerSec = tmp.Frequency * tmp.BlockAlign;
            return tmp;
        }

        private short[] Sin(uint numSamples, short channels, int amplitude, double t)
        {
            short[] tmp = new short[numSamples];
            short tempSample = 0;

            for (int i = 0; i < numSamples; i += channels)
            {
                tempSample = Convert.ToInt16(amplitude * Math.Sin(t * i));
                for (int channel = 0; channel < channels; channel++)
                {
                    tmp[i + channel] = tempSample;
                }
            }
            return tmp;
        }

        private short[] Square(uint numSamples, short channels, int amplitude, double t)
        {
            short[] tmp = new short[numSamples];
            for (uint i = 0; i < numSamples - 1; i++)
            {
                for (int channel = 0; channel < channels; channel++)
                {
                    tmp[i] = Convert.ToInt16(amplitude * Math.Sign(Math.Sin(t * i)));
                }
            }
            return tmp;
        }

        private short[] Sawtooth(uint numSamples, int channels, double samplesPerWaveLenght, int amplitude)
        {
            short[] tmp = new short[numSamples];
            short ampStep = Convert.ToInt16((amplitude * 2) / samplesPerWaveLenght);
            short tempSample;
            int totalSamplesWritten = 0;

            while (totalSamplesWritten < numSamples)
            {
                tempSample = (short)-amplitude;

                for (uint i = 0; i < samplesPerWaveLenght && totalSamplesWritten < numSamples; i++)
                {
                    tempSample += ampStep;
                    for (int channel = 0; channel < channels; channel++)
                    {
                        tmp[totalSamplesWritten] = tempSample;

                        totalSamplesWritten++;
                    }
                }
            }

            return tmp;
        }

        private short[] Triangle(uint numSamples, int channels, double samplesPerWaveLenght, short amplitude)
        {
            short[] tmp = new short[numSamples];
            short ampStep = Convert.ToInt16((amplitude * 2) / samplesPerWaveLenght);
            short tempSample = (short)-amplitude;

            for (uint i = 0; i < numSamples - 1; i++)
            {
                if (Math.Abs(tempSample) > amplitude)
                {
                    ampStep = (short)-ampStep;
                }

                if (i % 200 == 0)
                    Console.WriteLine("");

                if (tempSample + ampStep > -amplitude && tempSample + ampStep < amplitude)
                    tempSample += ampStep;
                else
                    tempSample = (short)(tempSample > 0 ? (amplitude + 1) : (-amplitude - 1));

                for (int channel = 0; channel < channels; channel++)
                {
                    tmp[i + channel] = tempSample;
                }
            }

            return tmp;
        }

        private short[] WhiteNoise(uint numSamples, int channels, int amplitude)
        {
            short[] tmp = new short[numSamples];
            Random rnd = new Random();
            for (uint i = 0; i < numSamples - 1; i++)
            {
                for (int channel = 0; channel < channels; channel++)
                {
                    tmp[i + channel] = Convert.ToInt16(rnd.Next(-amplitude, amplitude));
                }
            }
            return tmp;
        }
    }
}
