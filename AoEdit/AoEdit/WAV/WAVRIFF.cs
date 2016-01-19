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
        public string ChunkID { get; set; }
        public int ChunkSize { get; set; }
        public string Format { get; set; }

        public WAVRIFF()
        {
            ChunkID = "";
            ChunkSize = 0;
            Format = "";
        }
    }
}
