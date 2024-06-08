using SpoongePE.Core.Game.material;
using SpoongePE.Core.Game.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.BlockBase.impl
{
    public class RailBlock : DecorationBlock
    {
        public RailBlock(int id, Material m) : base(id, m)
        {
        }

        public static bool isRailBlock(int var9) => var9 == rail.blockID || var9 == poweredRail.blockID;

        public static bool isRailBlockAt(World world, int var1, int var2, int var3)
        {
            int var4 = world.getBlockIDAt(var1, var2, var3);
            return var4 == rail.blockID || var4 == poweredRail.blockID;
        }

        internal bool getIsPowered() => blockID == poweredRail.blockID;

        public new AxisAlignedBB getCollisionBoundingBoxFromPool(World var1, int var2, int var3, int var4)
        {
            return null;
        }

        public new bool isOpaqueCube()
        {
            return false;
        }
        public new bool renderAsNormalBlock()
        {
            return false;
        }
    }
}
