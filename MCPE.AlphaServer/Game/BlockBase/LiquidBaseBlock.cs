using SpoongePE.Core.Game.material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.BlockBase
{
    public class LiquidBaseBlock : Block
    {
        public LiquidBaseBlock(int id, Material m) : base(id, m)
        {
            Block.shouldTick[id] = true;
        }
        public void updateLiquid(World world, int x, int y, int z)
        {
            if (world.getBlockIDAt(x, y, z) == this.blockID && this.material == Material.lava &&
            (world.getMaterial(x, y, z - 1) == Material.water || world.getMaterial(x, y, z + 1) == Material.water ||
            world.getMaterial(x - 1, y, z) == Material.water || world.getMaterial(x + 1, y, z) == Material.water ||
            world.getMaterial(x, y + 1, z) == Material.water))
            {

                int meta = world.getBlockMetaAt(x, y, z);
                if (meta == 0)
                {
                    world.placeBlockAndNotifyNearby(x, y, z, (byte)Block.obsidian.blockID);
                }
                else if (meta <= 4)
                {
                    world.placeBlockAndNotifyNearby(x, y, z, (byte)Block.cobblestone.blockID);
                }

            }
        }


        public override void onBlockAdded(World world, int x, int y, int z)
        {
            this.updateLiquid(world, x, y, z);
        }

        
        public override void onNeighborBlockChanged(World world, int x, int y, int z, int meta)
        {
            this.updateLiquid(world, x, y, z);
        }
    }
}
