using SpoongePE.Core.Game.material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.BlockBase.impl
{
    public class PlantBlock : Block
    {
        public PlantBlock(int id, Material m) : base(id, m)
        {
            isSolid = false;
        }
        public override void onNeighborBlockChanged(World world, int x, int y, int z, int meta)
        {

            if (!world.isBlockSolid(x, y - 1, z))
            { //TODO check not just solidness, but ids
                onBlockRemoved(world, x, y, z);
            }
        }
        public override bool canSurvive(World world, int x, int y, int z)
        {

            bool res = world.canSeeSky(x, y, z);
            if (res)
            {
                int id = world.getBlockIDAt(x, y - 1, z);
                return id == grass.blockID || id == dirt.blockID || id == farmland.blockID;
            }
            return res;
        }
    }
}
