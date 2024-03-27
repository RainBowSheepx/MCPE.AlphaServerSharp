using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MCPE.AlphaServer.Game.utils
{
    public class MathUtils
    {
        public static int ffloor(float f)
        {
            byte[] bytes = BitConverter.GetBytes(f - (int)f);
            return ((int)f) + (BitConverter.ToInt32(bytes,0) >> 31);
        }
        public static long ffloor(double f)
        {
            byte[] bytes = BitConverter.GetBytes(f - (long)f);
            return ((long)f) + (BitConverter.ToInt64(bytes,0) >> 63);
        }

        public static long fceil(double f)
        {
            byte[] bytes = BitConverter.GetBytes(((long)f) - f);
            return ((long)f) - (BitConverter.ToInt64(bytes, 0) >> 63);
        }

        public static int fceil(float f)
        {
            byte[] bytes = BitConverter.GetBytes(((int)f) - f);
            return ((int)f) - (BitConverter.ToInt32(bytes, 0) >> 31);
        }
    }
}
