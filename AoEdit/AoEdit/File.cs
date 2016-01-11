using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoEdit
{
    class File
    {
        public string Path { get; set; }
        
        public byte[] OpenFile(string path)
        {
            byte[] buffer;
            Console.WriteLine(path);
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    int lenght = (int)fileStream.Length;
                    buffer = new byte[lenght];
                    int count;
                    int sum = 0;

                    while((count = fileStream.Read(buffer, sum, lenght - sum)) > 0)
                    {
                        sum += count;
                    }
                }
                finally
                {
                    fileStream.Close();
                }

                Console.WriteLine("Réussi");
                return buffer;
            }
        }
    }
}
