using AoEdit.Audio;
using System.IO;
using System.Linq;

namespace AoEdit
{
    class WAVFile
    {
        //Ouvre le fichier selectionné et crée un objet WAV
        public static WAV OpenFile(string path)
        {
            //File.WriteAllBytes(path + ".hex", File.ReadAllBytes(path));

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            return new WAV(path, fs);
        }

        //Ecris dans un fichier depuis un WAV
        public static void WriteFile(string path, DataHeader Header, byte[] Buffer)
        {
            FileStream fileStream = new FileStream(path, FileMode.Create);

            BinaryWriter writer = new BinaryWriter(fileStream);

            //Ecrire le chunk de base
            writer.Write(Header.FileTypeID);
            writer.Write(Header.FileLenght);
            writer.Write(Header.MediaTypedID);

            //Ecrire le chunck du format
            writer.Write(Header.ChunkIDFormat);
            writer.Write(Header.ChunkSizeFormat);
            writer.Write(Header.FormatTag);
            writer.Write(Header.Channels);
            writer.Write(Header.Frequency);
            writer.Write(Header.AverageBytesPerSec);
            writer.Write(Header.BlockAlign);
            writer.Write(Header.BitsPerSample);

            //Ecrire le chunck de données
            writer.Write(Header.ChunkIDData);
            writer.Write(Header.ChunkSizeData);
            foreach(byte dataPoint in Buffer)
            {
                writer.Write(dataPoint);
            }

            //Fermeture des streams
            writer.Close();
            fileStream.Close();
        }
    }
}
