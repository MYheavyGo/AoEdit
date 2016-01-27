using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoEdit.Utils
{
    class Constants
    {
        public enum FORMATS : ushort
        {
            WAVE_FORMAT_PCM = 0x0001,
            WAVE_FORMAT_ADPCM = 0x0002,
            WAVE_FORMAT_IEEE_FLOAT = 0x0003,
            WAVE_FORMAT_VSELP = 0x0004,
            WAVE_FORMAT_IBM_CVSD = 0x0005,
            WAVE_FORMAT_ALAW = 0x0006,
            WAVE_FORMAT_MULAW = 0x0007,
            WAVE_FORMAT_DTS = 0x0008,
            WAVE_FORMAT_DRM = 0x0009,
            WAVE_FORMAT_EXTENSIBLE = 0xFFFE,
            WAVE_FORMAT_DEVELOPMENT= 0xFFFF
        }
    }
}
