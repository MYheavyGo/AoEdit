using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoEdit
{
    class WAV
    {
        WAVRIFF header;
        public byte[] Buffer { get; set; }

        public WAV() { }
        public WAV(byte[] buffer)
        {
            Buffer = buffer;
            header = new WAVRIFF();
        }
    }
}
