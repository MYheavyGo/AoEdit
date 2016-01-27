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
            //File.WriteAllBytes(path + ".hex", File.ReadAllBytes(path));
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }

        public void WriteFile(short[] bytes, wavfile header)
        {
            FileStream fileStream = new FileStream(Path + FileName, FileMode.Create);

            BinaryWriter writer = new BinaryWriter(fileStream);

            writer.Write(header.id.ToArray());
            writer.Write(header.totalLenght);
            writer.Write(header.wavefmt.ToArray());
            writer.Write(header.format);
            writer.Write(header.pcm);
            writer.Write(header.channels);
            writer.Write(header.frequency);
            writer.Write(header.bytesPerSecond);
            writer.Write(header.bytesByCapture);
            writer.Write(header.bitsPerSample);
            writer.Write(header.data.ToArray());
            writer.Write(header.bytesInData);
            foreach(short dataPoint in bytes)
            {
                writer.Write(dataPoint);
            }

            writer.Close();
            fileStream.Close();
        }
    }
}
