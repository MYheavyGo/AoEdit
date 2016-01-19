using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoEdit
{
    class WAV
    {
        public WAVRIFF Header { get; set; }

        public WAVfmt Fmt { get; set; }

        public WAVdata Data { get; set; }

        public Stream StreamWAV { get; set; }

        public byte[] Buffer { get; set; }

        private int sizeHeader = 44;
        public int SizeHeader
        {
            get { return sizeHeader; }
        }

        public string Log { get; private set; }

        public WAV()
        {
            /*Header = new WAVRIFF();
            Fmt = new WAVfmt();
            Data = new WAVdata();*/
        }

        public WAV(Stream wavFile)
        {
            StreamWAV = wavFile;
            Header = new WAVRIFF();
            Fmt = new WAVfmt();
            Data = new WAVdata();
            Log = ReadBuffer();
        }

        public string ReadBuffer()
        {

            using (BinaryReader reader = new BinaryReader(StreamWAV))
            {
                foreach (char c in reader.ReadChars(4))
                {
                    Header.ChunkID += c;
                }
                if (Header.ChunkID != "RIFF")
                {
                    return "Format non conforme du fichier";
                }

                Header.ChunkSize = reader.ReadInt32();

                foreach (char c in reader.ReadChars(4))
                {
                    Header.Format += c;
                }
                if (Header.Format != "WAVE")
                {
                    return "Format non conforme du fichier";
                }

                foreach (char c in reader.ReadChars(4))
                {
                    Fmt.Subchunk1ID += c;
                }
                if (Fmt.Subchunk1ID != "fmt ")
                {
                    return "Format non conforme du fichier";
                }

                Fmt.Subchunk1Size = reader.ReadInt32();
                Fmt.AudioFormat = reader.ReadInt16();
                Fmt.NumChannels = reader.ReadInt16();
                Fmt.SampleRate = reader.ReadInt32();
                Fmt.ByteRate = reader.ReadInt32();
                Fmt.BlockAlign = reader.ReadInt16();
                Fmt.BitsPerSample = reader.ReadInt16();

                if (Fmt.Subchunk1Size == 18)
                {
                    int ExtraSize = reader.ReadInt16();
                    reader.ReadBytes(ExtraSize);
                }

                foreach (char c in reader.ReadChars(4))
                {
                    Data.Subchunk2ID += c;
                }
                if (Data.Subchunk2ID == "fact")
                {
                }
                else if (Data.Subchunk2ID == "data")
                {
                }
                else
                {
                    return "Format non conforme du fichier";
                }

                Data.Subchunk2Size = reader.ReadInt32();

                Buffer = reader.ReadBytes(Data.Subchunk2Size);

                reader.Close();

                return "Analyse du fichier réussi";
            }
        }
    }
}