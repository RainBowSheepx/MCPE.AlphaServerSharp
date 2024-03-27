using MCPE.AlphaServer.Game.material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPE.AlphaServer.Game.BlockBase
{
    public class PlantBlock : Block
    {
        public PlantBlock(int id, Material m) : base(id, m)
        {
            this.isSolid = false;
        }
        public void onNeighborBlockChanged(World world, int x, int y, int z, int meta)
        {

            if (!world.isBlockSolid(x, y - 1, z))
            { //TODO check not just solidness, but ids
                this.onBlockRemoved(world, x, y, z);
            }
        }
        public bool canSurvive(World world, int x, int y, int z)
        {
          
            bool res = world.canSeeSky(x, y, z);
            if (res)
            {
                int id = world.getBlockIDAt(x, y - 1, z);
                return id == Block.grass.blockID || id == Block.dirt.blockID || id == Block.farmland.blockID;
            }
            return res;
        }
    }
}
