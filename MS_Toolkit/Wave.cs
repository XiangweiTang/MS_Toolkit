using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MS_Toolkit
{
    class Wave
    {
        #region Const
        const string RIFF = "RIFF";
        const string WAVE = "WAVE";
        const string fmt_ = "fmt ";
        const string data = "data";
        const string list = "list";
        const string fact = "fact";
        static readonly Dictionary<int, string> AudioTypeDict = new Dictionary<int, string> { { 1, "PCM" }, { 3, "IEEE" }, { 6, "ALAW" }, { 7, "ULAW" }, { 0xfffe, "Extension" } };
        #endregion

        #region Chunks
        public Chunk ChunkRiff { get; private set; } = new Chunk();
        public Chunk ChunkFmt_ { get; private set; } = new Chunk();
        public Chunk ChunkData { get; private set; } = new Chunk();
        public Chunk ChunkList { get; private set; } = new Chunk();
        public Chunk ChunkFact { get; private set; } = new Chunk();
        public List<Chunk> Chunks { get; private set; } = new List<Chunk>();
        #endregion

        #region Parameters
        public short AudioTypeId { get; private set; } = 0;
        public string AudioTypeString { get; private set; } = string.Empty;
        public short Channel { get; private set; } = 0;
        public int SampleRate { get; private set; } = 0;
        public int ByteRate { get; private set; } = 0;
        public int BlockAlign { get;private set; }=0;
        public int BitsPerSample { get; private set; } = 0;
        public double AudioLength { get; private set; } = 0.0;

        public byte[] DataBytes { get; private set; } = new byte[0];
        #endregion
        
        private byte[] bytes;

        public Wave() { }

        public void DeepParse(string wavePath)
        {
            var bytes = File.ReadAllBytes(wavePath);
            DeepParse(bytes);
        }

        public void DeepParse(byte[] bytes)
        {
            ParseRiff(bytes, true);
            PostProcess();
            DataBytes = new byte[ChunkData.Length];
            Array.Copy(bytes, ChunkData.Offset + 8, DataBytes, 0, ChunkData.Length);
        }

        public void ShallowParse(string wavepath)
        {
            var bytes = Common.ReadBytes(wavepath, 100);
            ShallowParse(bytes);
        }

        public void ShallowParse(byte[] bytes)
        {
            ParseRiff(bytes, false);
            PostProcess();
        }

        private void ParseRiff(byte[] bytes, bool isDeep)
        {
            this.bytes = bytes;
            Sanity.Requires(bytes.Length >= 44, "File is less than 44 bytes.");
            Sanity.Requires(RIFF == Encoding.ASCII.GetString(bytes, 0, 4), $"File is not a {RIFF} file.");
            int length = BitConverter.ToInt32(bytes, 4);
            if (isDeep)
                Sanity.Requires(bytes.Length == length + 8, "Audio length mismatch in RIFF");
            Sanity.Requires(WAVE == Encoding.ASCII.GetString(bytes, 8, 4));
            ChunkRiff = new Chunk() { Name = "RIFF", Length = length, Offset = 0 };
            ParseChunk(isDeep, 12);
        }

        private void ParseChunk(bool isDeep, int offset)
        {
            if (offset + 8 < bytes.Length)
            {
                Sanity.Requires(!isDeep, $"Audio length mismatch in 0x{offset.ToString("x")}.");
                return;
            }
            string name = Encoding.ASCII.GetString(bytes, offset, 4);
            int length = BitConverter.ToInt32(bytes, offset + 4);
            Chunk currentChunk = new Chunk() { Name = name, Length = length, Offset = offset };
            switch (name)
            {
                case fmt_:
                    ChunkFmt_ = currentChunk;
                    break;
                case data:
                    ChunkData = currentChunk;
                    break;
                case list:
                    ChunkList = currentChunk;
                    break;
                case fact:
                    ChunkFact = currentChunk;
                    break;
                default:
                    Chunks.Add(currentChunk);
                    break;
            }
            if (offset + 8 + length == bytes.Length)
                return;
            if (offset + 8 + length > bytes.Length)
            {
                Sanity.Requires(!isDeep, $"Audio length mismatch in {name}.");
                return;
            }
            ParseChunk(isDeep, offset + 8 + length);
        }

        private void PostProcess()
        {
            Sanity.Requires(!string.IsNullOrWhiteSpace(ChunkFmt_.Name), "Missing format chunk.");
            Sanity.Requires(!string.IsNullOrWhiteSpace(ChunkData.Name), "Missing data chunk.");

            int offset = ChunkFmt_.Offset + 8;
            AudioTypeId = BitConverter.ToInt16(bytes, offset);
            Sanity.Requires(AudioTypeDict.ContainsKey(AudioTypeId), "Unsupported audio type.");
            AudioTypeString = AudioTypeDict[AudioTypeId];
            Channel = BitConverter.ToInt16(bytes, offset + 2);
            SampleRate = BitConverter.ToInt32(bytes, offset + 4);
            ByteRate = BitConverter.ToInt32(bytes, offset + 8);
            BlockAlign = BitConverter.ToInt16(bytes, offset + 12);
            BitsPerSample = BitConverter.ToInt16(bytes, offset + 14);
            AudioLength = 1.0 * ChunkData.Length / ByteRate;
        }

        public IEnumerable<int> OutputIntegers()
        {
            switch (BitsPerSample)
            {
                case 8:
                    return Output8BitsIntegers();
                case 16:
                    return Output16BitsIntegers();
                default:
                    Sanity.BeBetter(true, $"Unexpected bits per sample {BitsPerSample}.");
                    return new List<int>();
            }
        }

        private IEnumerable<int> Output8BitsIntegers()
        {
            for (int i = ChunkData.Offset + 8; i < ChunkData.Offset + 8 + ChunkData.Length; i++)
                yield return DataBytes[i];
        }

        private IEnumerable<int> Output16BitsIntegers()
        {
            for (int i = ChunkData.Offset + 8; i < ChunkData.Offset + 8 + ChunkData.Length; i += 2)
                yield return BitConverter.ToInt16(bytes, i);
        }

        public IEnumerable<double> OutputDouble()
        {
            switch (BitsPerSample)
            {
                case 8:
                    return Output8BitsDouble();
                case 16:
                    return Output16BitsDouble();
                default:
                    Sanity.BeBetter(true, $"Unexpected bits per sample {BitsPerSample}.");
                    return new List<double>();
            }
        }

        private IEnumerable<double> Output8BitsDouble()
        {
            for (int i = ChunkData.Offset + 8; i < ChunkData.Offset + 8 + ChunkData.Length; i++)
                yield return DataBytes[i] / 128.0;
        }

        private IEnumerable<double> Output16BitsDouble()
        {
            for (int i = ChunkData.Offset + 8; i < ChunkData.Offset + 8 + ChunkData.Length; i += 2)
                yield return BitConverter.ToInt16(DataBytes, i) / 32768.0;
        }
    }

    class Chunk
    {
        public string Name = string.Empty;
        public int Length = 0;
        public int Offset = 0;
    }
}
