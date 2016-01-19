using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoEdit
{
    /// <summary>
    /// Class for create or get the Chunk DATA for a WAV file
    /// 
    /// DATA (Size of the DATA and RAW Sound)
    /// - Subchunk2ID / 4
    /// - Subchunk2Size / 4
    /// - Data / *
    /// </summary>
    class WAVdata
    {
        // DATA
        public string Subchunk2ID { get; set; }
        public int Subchunk2Size { get; set; }
        public byte[] Data { get; set; }

        public WAVdata()
        {
            Subchunk2ID = "";
            Subchunk2Size = 0;
            Data = new byte[0];
        }

        public WAVdata(string id, int size, byte[] data)
        {
            Subchunk2ID = id;
            Subchunk2Size = size;
            Data = data;
        }
    }
}
