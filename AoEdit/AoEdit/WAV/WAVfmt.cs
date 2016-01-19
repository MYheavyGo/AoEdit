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
        public int Subchunk1Size { get; set; }
        public int AudioFormat { get; set; }
        public int NumChannels { get; set; }
        public int SampleRate { get; set; }
        public int ByteRate { get; set; }
        public int BlockAlign { get; set; }
        public int BitsPerSample { get; set; }

        private string[] nameAudioFormat = new string[] { "", "PCM", "", "", "", "", "A-Law", "U-Law",
        "", "", "", "", "", ""};

        public WAVfmt()
        {
            Subchunk1ID = "";
            Subchunk1Size = 0;
            AudioFormat = 0;
            NumChannels = 0;
            SampleRate = 0;
            BitsPerSample = 0;
            BlockAlign = (ushort)(NumChannels * (BitsPerSample / 8));
            ByteRate = SampleRate * BlockAlign;
        }

        public string getNameAudioFormat()
        {
            if (AudioFormat >= 0 & AudioFormat < nameAudioFormat.Length)
            {
                return nameAudioFormat[AudioFormat];
            }
            return "INCONNU";
        }
    }
}
