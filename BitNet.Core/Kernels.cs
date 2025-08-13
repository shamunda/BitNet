namespace BitNet.Core;

public class Kernels
{
    // C# implementation of the ggml-bitnet-mad.cpp kernels
    // Note: These are direct ports and may not be optimized for C#.
    // They are intended to be functionally correct first.

    public static void QuantizeI2S(float[] src, sbyte[] dst, int n)
    {
        for (int i = 0; i < n; i++)
        {
            float v = src[i];
            dst[i] = (sbyte)(v > 0 ? 1 : -1);
        }
    }

    public static int VecDotI2I8S(int n, sbyte[] sx, sbyte[] sy)
    {
        int res = 0;
        for (int i = 0; i < n; i++)
        {
            res += sx[i] * sy[i];
        }
        return res;
    }
}
