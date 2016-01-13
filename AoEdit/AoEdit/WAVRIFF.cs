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
        public string ChunkID;
        public uint ChunkSize;
        public string Format;

        public byte[] Buffer { get; set; }
        public string Log { get; set; }
        public List<NUMBER> HeaderDATA { get; set; }
        public int[] SizeDelimiter { get; set; }
        int sizeHeader = 44;

        public WAVRIFF()
        {
            ChunkID = "RIFF";
            ChunkSize = 0;
            Format = "WAVE";
        }

        public WAVRIFF(byte[] buffer)
        {
            Buffer = buffer;
            SizeDelimiter = new int[] { 4, 4, 4, 4, 4, 2, 2, 4, 4, 2, 2, 4, 4 };
        }
    }
}
