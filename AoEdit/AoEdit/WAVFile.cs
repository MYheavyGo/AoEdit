using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoEdit
{
    class WAVFile
    {        
        public byte[] OpenFile(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
}
