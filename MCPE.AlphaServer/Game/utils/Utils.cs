using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.utils
{
    public class Utils
    {
        public static int getPlayerDirection(Player p)
        {
            return (int)((p.rotationYaw * 4.0 / 360) + 0.5) & 0x3;
        }

        public static int stringHash(String s)
        {
            int i = 0;
            for (int j = 0; j < s.Length; j++)
            {
                i = i * 31 + s[j];
            }

            return i;
        }
    }
}
