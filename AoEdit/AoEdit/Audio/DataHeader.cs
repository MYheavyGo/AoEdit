namespace AoEdit.Audio
{
    class DataHeader
    {
        public char[] FileTypeID { get; set; }
        public int FileLenght { get; set; }
        public char[] MediaTypedID { get; set; }
        public char[] ChunkIDFormat { get; set; }
        public int ChunkSizeFormat { get; set; }
        public short FormatTag { get; set; }
        public short Channels { get; set; }
        public int Frequency { get; set; }
        public int AverageBytesPerSec { get; set; }
        public short BlockAlign { get; set; }
        public short BitsPerSample { get; set; }
        public char[] ChunkIDData { get; set; }
        public int ChunkSizeData { get; set; }

        public ushort[] SizeOfBytes = new ushort[] { 4, 4, 4, 4, 4, 2, 2, 4, 4, 2, 2, 4, 4 };
    }
}
