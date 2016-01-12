using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoEdit
{
    class WAVRIFF
    {
        /// <summary>
        /// Class for create or get the Chunk RIFF for a WAV file
        /// 
        /// Header
        /// - ChunkID / 4
        /// - ChunkSize / 4
        /// - Format / 4
        /// </summary>
           
        // Header of WAV
        public byte[] ChunkID;
        public uint ChunkSize;
        public byte[] Format;

        // FMT
        public byte[] Subchunk1ID { get; set; }
        public uint Subchunk1Size { get; set; }
        public ushort AudioFormat { get; set; }
        public ushort NumChannels { get; set; }
        public uint SampleRate { get; set; }
        public uint ByteRate { get; set; }
        public ushort BlockAlign { get; set; }
        public ushort BitsPerSample { get; set; }

        // DATA
        public byte[] Subchunk2ID { get; set; }
        public ushort Subchunk2Size { get; set; }

        public byte[] Buffer { get; set; }
        public string Log { get; set; }
        public List<NUMBER> HeaderDATA { get; set; }
        public int[] SizeDelimiter { get; set; }
        int sizeHeader = 44;

        public WAVRIFF() { }
        public WAVRIFF(byte[] buffer)
        {
            Buffer = buffer;
            SizeDelimiter = new int[] { 4, 4, 4, 4, 4, 2, 2, 4, 4, 2, 2, 4, 4 };
        }
    }
}
