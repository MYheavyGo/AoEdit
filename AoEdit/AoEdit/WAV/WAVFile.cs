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

        public Stream OpenFile(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }

        public void WriteFile(byte[] bytes)
        {
            File.WriteAllBytes(Path + FileName, bytes);
        }
    }
}
