namespace BitNet.Core
{
    public enum GgmlType
    {
        F32,
        F16,
        Q4_0,
        Q4_1,
        Q5_0,
        Q5_1,
        Q8_0,
        Q8_1,
        Q2_K,
        Q3_K,
        Q4_K,
        Q5_K,
        Q6_K,
        Q8_K,
        IQ2_XXS,
        IQ2_XS,
        IQ3_XXS,
        IQ1_S,
        IQ4_NL,
        IQ3_S,
        IQ2_S,
        IQ4_XS,
        I8,
        I16,
        I32,
        I64,
        F64,
        IQ1_M,
        BF16,
        TL1,
        TL2,
    }

    public class GgmlTensor
    {
        public GgmlType Type { get; set; }
        public int NDims { get; set; }
        public long[] Ne { get; set; } = new long[0];
        public byte[] Data { get; set; } = new byte[0];
        public string Backend { get; set; } = "CPU";
    }

    public class BitnetTensorExtra
    {
        public int LutScalesSize { get; set; }
        public int BK { get; set; }
        public int NTileNum { get; set; }
        public byte[] QWeights { get; set; } = new byte[0];
        public float[] Scales { get; set; } = new float[0];
    }

    public class GgufTensorInfo
    {
        public string Name { get; set; } = "";
        public uint[] Dims { get; set; } = new uint[0];
        public GgmlType Type { get; set; }
        public ulong Offset { get; set; }
    }
}
