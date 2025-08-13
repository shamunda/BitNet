using System;
using System.Linq;

namespace BitNet.Core
{
    public static class Kernels
    {
        private const int QK_I2 = 128;
        private const int QK_I2_S = 128;

        public static byte[] QuantizeI2S(float[] src)
        {
            var n = src.Length;
            var dst = new byte[n / 4 + sizeof(float)];

            // f32 -> q8
            var max = src.Max(Math.Abs);
            var i2_scale = max;

            var q8 = new byte[n];
            for (int i = 0; i < n; i++)
            {
                if (Math.Abs(src[i]) < 1e-6)
                {
                    q8[i] = 1;
                    continue;
                }
                q8[i] = src[i] * i2_scale > 0 ? (byte)2 : (byte)0;
            }

            // q8 -> 0, 1, 2
            //       |  |  |
            //      -1, 0, 1
            for (int i = 0; i < n / QK_I2; i++)
            {
                for (int j = 0; j < QK_I2; j++)
                {
                    int group_idx = j / 32;
                    int group_pos = j % 32;
                    byte temp = (byte)((q8[i * QK_I2 + j] & 0x03) << (6 - 2 * group_idx));
                    dst[i * 32 + group_pos] |= temp;
                }
            }

            var scale_bytes = BitConverter.GetBytes((float)i2_scale);
            Buffer.BlockCopy(scale_bytes, 0, dst, n / 4, sizeof(float));

            return dst;
        }

        public static float VecDotI2I8S(byte[] vx, byte[] vy)
        {
            int n = vy.Length;
            int nb = n / QK_I2_S;
            float sum = 0;

            for (int i = 0; i < nb; i++)
            {
                for (int j = 0; j < QK_I2_S; j++)
                {
                    int group_idx = j / 32;
                    int group_pos = j % 32;

                    // Unpack the 2-bit value
                    int val = (vx[i * 32 + group_pos] >> (6 - 2 * group_idx)) & 0x03;

                    // Convert to -1, 0, 1
                    if (val == 1) val = 0;
                    else if (val == 2) val = 1;
                    else val = -1;

                    sum += val * (sbyte)vy[i * QK_I2_S + j];
                }
            }

            return sum;
        }
    }
}
