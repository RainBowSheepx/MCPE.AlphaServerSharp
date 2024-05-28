using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.utils
{
    public enum EnumSkyBlock
    {
        Sky = 15,
        Block = 0
    }

    public class EnumSkyBlockValues
    {
        public int defaultLightValue;

        private EnumSkyBlockValues(int value)
        {
            defaultLightValue = value;
        }

        public static EnumSkyBlockValues Sky = new EnumSkyBlockValues(15);
        public static EnumSkyBlockValues Block = new EnumSkyBlockValues(0);
    }
}
