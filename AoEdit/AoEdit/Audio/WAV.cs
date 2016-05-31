using AoEdit.Audio;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AoEdit
{

    class WAV
    {
        public DataHeader Header;
        public Stream StreamWAV { get; set; }
        public string Name { get; set; }
        public byte[] Buffer { get; set; }
        public float[] Samples { get; set; }

        private int sizeHeader = 44;
        public int SizeHeader
        {
            get { return sizeHeader; }
        }

        public string Log { get; private set; }

        public bool Passed { get; set; }

        public WAVPlay Player { get; set; }

        public WAV(string name, Stream wavFile)
        { 
            //Constructeur
            Name = name;
            StreamWAV = wavFile;
            Header = new DataHeader();
            Log = ReadBuffer();
            if (Passed)
            {
                Samples = new float[Buffer.Length];
                for (int i = 0; i < Buffer.Length - 1; i++)
                {
                    Samples[i] = BitConverter.ToInt16(Buffer, i);
                }
            }

            Player = new WAVPlay(name);
        }

        public string ReadBuffer()
        {
            using (BinaryReader reader = new BinaryReader(StreamWAV))
            {
                Header.FileTypeID = reader.ReadChars(4);
                if (!Header.FileTypeID.SequenceEqual("RIFF"))
                {
                    Passed = false;
                    return "Format non conforme du fichier";
                }

                Header.FileLenght = reader.ReadInt32();

                Header.MediaTypedID = reader.ReadChars(8);
                if (!Header.MediaTypedID.SequenceEqual("WAVEfmt "))
                {
                    Passed = false;
                    return "Format non conforme du fichier";
                }

                Header.ChunkSizeFormat = reader.ReadInt32();
                Header.FormatTag = Math.Abs(reader.ReadInt16());
                Header.Channels = reader.ReadInt16();
                Header.Frequency = reader.ReadInt32();
                Header.AverageBytesPerSec = reader.ReadInt32();
                Header.BlockAlign = reader.ReadInt16();
                Header.BitsPerSample = reader.ReadInt16();

                if (Header.ChunkSizeFormat > 16)
                {
                    int ExtraSize = reader.ReadInt16();
                    var ExtraData = reader.ReadBytes(ExtraSize);
                }

                Header.ChunkIDData = reader.ReadChars(4);
                if (!Header.ChunkIDData.SequenceEqual("data"))
                {
                    Passed = false;
                    return "Format non conforme du fichier";
                }

                Header.ChunkSizeData = reader.ReadInt32();

                Buffer = reader.ReadBytes(Header.ChunkSizeData);

                reader.Close();

                Passed = true;
                StreamWAV.Close();
                return "Analyse du fichier réussi";
            }
        }
    }
}