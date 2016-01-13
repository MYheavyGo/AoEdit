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
        public WAVRIFF Header { get; set; }

        WAVfmt fmt;
        public WAVfmt Fmt { get; set; }

        WAVdata data;
        public WAVdata Data { get; set; }

        WAVFile file;
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
