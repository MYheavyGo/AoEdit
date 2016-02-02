using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoEdit
{
    struct wavfile
    {
        public char[] id;
        public int totalLenght;
        public char[] wavefmt;
        public int format;
        public short pcm;
        public short channels;
        public int frequency;
        public int bytesPerSecond;
        public short bytesByCapture;
        public short bitsPerSample;
        public char[] data;
        public int bytesInData;
    }

    class WAV
    {
        public wavfile Header;

        public Stream StreamWAV { get; set; }

        public string Name { get; set; }

        public byte[] Buffer { get; set; }
        public float[] output { get; set; }

        private int sizeHeader = 44;
        public int SizeHeader
        {
            get { return sizeHeader; }
        }

        public string Log { get; private set; }

        public bool Passed { get; set; }

        public WAV()
        {
        }

        public WAV(string name, Stream wavFile)
        {
            Name = name;
            StreamWAV = wavFile;
            Header = new wavfile();
            Log = ReadBuffer();
        }

        public string ReadBuffer()
        {
            using (BinaryReader reader = new BinaryReader(StreamWAV, Encoding.UTF8))
            {
                Header.id = reader.ReadChars(4);
                if (!Header.id.SequenceEqual("RIFF"))
                {
                    Passed = false;
                    return "Format non conforme du fichier";
                }

                Header.totalLenght = reader.ReadInt32();

                Header.wavefmt = reader.ReadChars(8);
                if (!Header.wavefmt.SequenceEqual("WAVEfmt "))
                {
                    Passed = false;
                    return "Format non conforme du fichier";
                }

                Header.format = reader.ReadInt32();
                Header.pcm = Math.Abs(reader.ReadInt16());
                Header.channels = reader.ReadInt16();
                Header.frequency = reader.ReadInt32();
                Header.bytesPerSecond = reader.ReadInt32();
                Header.bytesByCapture = reader.ReadInt16();
                Header.bitsPerSample = reader.ReadInt16();

                if (Header.format > 16)
                {
                    int ExtraSize = reader.ReadInt16();
                    var ExtraData = reader.ReadBytes(ExtraSize);
                }

                Header.data = reader.ReadChars(4);
                if (!Header.data.SequenceEqual("data"))
                {
                    Passed = false;
                    return "Format non conforme du fichier";
                }

                Header.bytesInData = reader.ReadInt32();

                Buffer = reader.ReadBytes(Header.bytesInData);

                reader.Close();

                Passed = true;
                return "Analyse du fichier réussi";
            }
        }
    }
}