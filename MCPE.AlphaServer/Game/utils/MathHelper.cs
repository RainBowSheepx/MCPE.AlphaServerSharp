using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.utils
{
    public class MathHelper
    {
        private static float[] SIN_TABLE = new float[65536];

        public MathHelper()
        {
            for (int var0 = 0; var0 < 65536; ++var0)
            {
                SIN_TABLE[var0] = (float)Math.Sin((double)var0 * 3.141592653589793D * 2.0D / 65536.0D);
            }
        }
        public static float sin(float var0)
        {
            return SIN_TABLE[(int)(var0 * 10430.378F) & '\uffff'];
        }

        public static float cos(float var0)
        {
            return SIN_TABLE[(int)(var0 * 10430.378F + 16384.0F) & '\uffff'];
        }

        public static float sqrt_float(float var0)
        {
            return (float)Math.Sqrt((double)var0);
        }

        public static float sqrt_double(double var0)
        {
            return (float)Math.Sqrt(var0);
        }

        public static int floor_float(float var0)
        {
            int var1 = (int)var0;
            return var0 < (float)var1 ? var1 - 1 : var1;
        }

        public static int floor_double(double var0)
        {
            int var2 = (int)var0;
            return var0 < (double)var2 ? var2 - 1 : var2;
        }

        public static int clamp_int(int p_76125_0_, int p_76125_1_, int p_76125_2_)
        {
            return p_76125_0_ < p_76125_1_ ? p_76125_1_ : (p_76125_0_ > p_76125_2_ ? p_76125_2_ : p_76125_0_);
        }

        public static float abs(float var0)
        {
            return var0 >= 0.0F ? var0 : -var0;
        }

        public static double abs_max(double var0, double var2)
        {
            if (var0 < 0.0D)
            {
                var0 = -var0;
            }

            if (var2 < 0.0D)
            {
                var2 = -var2;
            }

            return var0 > var2 ? var0 : var2;
        }

        public static int bucketInt(int var0, int var1)
        {
            return var0 < 0 ? -((-var0 - 1) / var1) - 1 : var0 / var1;
        }

        public static bool stringNullOrLengthZero(string var0)
        {
            return var0 == null || var0.Length == 0;
        }
    }
}
