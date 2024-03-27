using MCPE.AlphaServer.Game.material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPE.AlphaServer.Game.BlockBase
{
    public class LiquidStaticBlock : LiquidBaseBlock
    {
        public int tickrate;
        public LiquidStaticBlock(int id, Material m) : base(id, m)
        {
            this.isSolid = false;
            Block.shouldTick[id] = (m == Material.lava);
        }
        public void setDynamic(World world, int x, int y, int z)
        {
            int meta = world.getBlockMetaAt(x, y, z);
            world.editingBlocks = true;
            world.placeBlock(x, y, z, (byte)(this.blockID - 1), (byte)meta);
            world.addToTickNextTick(x, y, z, this.blockID - 1, this.tickrate);
            world.editingBlocks = false;
        }
        
        public void onBlockAdded(World world, int x, int y, int z)
        {
            /**
             * if ( *((_DWORD *)this + 0x10) == Material::lava )
                return (LiquidTile *)LiquidTile::_trySpreadFire(this, a2, a3, a4, a5, a6);
              return this;
             */
            //TODO fire spread
        }
       
    public void onNeighborBlockChanged(World world, int x, int y, int z, int meta)
        {
            base.onNeighborBlockChanged(world, x, y, z, meta);
         //   super.onNeighborBlockChanged(world, x, y, z, meta);
            if (this.blockID == world.getBlockIDAt(x, y, z)) this.setDynamic(world, x, y, z);
        }

    }
}
