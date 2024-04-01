using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.utils
{
    public class MathUtils
    {
        public static int ffloor(float f)
        {
            return (int)Math.Floor(f);
        }
        public static long ffloor(double f)
        {
            return (long)Math.Floor(f);
        }

        public static long fceil(double f)
        {
            return (long)Math.Ceiling(f);
        }

        public static int fceil(float f)
        {
            return (int)Math.Ceiling(f);
        }
    }
}
