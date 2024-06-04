using SpoongePE.Core.Game.material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.BlockBase.impl
{
    public class RailBlock : DecorationBlock
    {
        public RailBlock(int id, Material m) : base(id, m)
        {
        }

        public static bool isRailBlock(int var9)
        {
            throw new NotImplementedException();
        }

        public static bool isRailBlockAt(World world, int var1, int v, int var3)
        {
            throw new NotImplementedException();
        }

        internal bool getIsPowered()
        {
            throw new NotImplementedException();
        }
    }
}
