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
        public string Path { get; set; }
        public string FileName { get; set; }

        public byte[] OpenFile(string path)
        {
            return File.ReadAllBytes(path);
        }

        public void WriteFile(byte[] bytes)
        {
            File.WriteAllBytes(Path + FileName, bytes);
        }
    }
}
