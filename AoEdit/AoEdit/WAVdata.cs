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
        public uint Subchunk2Size { get; set; }
        public ushort[] Data { get; set; }

        public WAVdata()
        {
            Subchunk2ID = "data";
            Subchunk2Size = 0;
            Data = new ushort[0];
        }
    }
}
