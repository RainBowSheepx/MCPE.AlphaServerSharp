using SpoongePE.Core.Game.material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.BlockBase.impl
{
    public class LiquidStaticBlock : LiquidBaseBlock
    {
        public int tickrate;
        public LiquidStaticBlock(int id, Material m) : base(id, m)
        {
            isSolid = false;
            shouldTick[id] = m == Material.lava;
        }
        public void setDynamic(World world, int x, int y, int z)
        {
            int meta = world.getBlockMetaAt(x, y, z);
            world.editingBlocks = true;
            world.placeBlock(x, y, z, (byte)(blockID - 1), (byte)meta);
            world.addToTickNextTick(x, y, z, blockID - 1, tickrate);
            world.editingBlocks = false;
        }

        public override void onBlockAdded(World world, int x, int y, int z)
        {
            /**
             * if ( *((_DWORD *)this + 0x10) == Material::lava )
                return (LiquidTile *)LiquidTile::_trySpreadFire(this, a2, a3, a4, a5, a6);
              return this;
             */
            //TODO fire spread
        }

        public override void onNeighborBlockChanged(World world, int x, int y, int z, int meta)
        {
            base.onNeighborBlockChanged(world, x, y, z, meta);
            //   super.onNeighborBlockChanged(world, x, y, z, meta);
            if (blockID == world.getBlockIDAt(x, y, z)) setDynamic(world, x, y, z);
        }

    }
}
