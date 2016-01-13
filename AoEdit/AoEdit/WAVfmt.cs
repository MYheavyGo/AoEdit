using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoEdit
{
    /// <summary>
    /// Class for create or get the Chunk fmt for a WAV file
    /// 
    /// FMT (Sound data's format)
    ///     - Subchunk1ID / 4
    ///     - Subchunk1Size / 4
    ///     - AudioFormat / 2
    ///     - NumChannels / 2
    ///     - SampleRate / 4
    ///     - ByteRate / 4
    ///     - BlockAlign / 2
    ///     - BitsPerSample / 2
    /// </summary>
    class WAVfmt
    {
        // FMT
        public string Subchunk1ID { get; set; }
        public uint Subchunk1Size { get; set; }
        public ushort AudioFormat { get; set; }
        public ushort NumChannels { get; set; }
        public uint SampleRate { get; set; }
        public uint ByteRate { get; set; }
        public ushort BlockAlign { get; set; }
        public ushort BitsPerSample { get; set; }

        public WAVfmt()
        {
            Subchunk1ID = "fmt ";
            Subchunk1Size = 16;
            AudioFormat = 1;
            NumChannels = 2;
            SampleRate = 44100;
            BitsPerSample = 16;
            BlockAlign = (ushort)(NumChannels * (BitsPerSample / 8));
            ByteRate = SampleRate * BlockAlign;
        }
    }
}
