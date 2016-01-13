using System;
using System.Collections.Generic;
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

        public WAVFile File { get; set; }

        public byte[] Buffer { get; set; }

        public WAV()
        {
            File = new WAVFile();
            Header = new WAVRIFF();
            Fmt = new WAVfmt();
            Data = new WAVdata();
        }
    }
}
