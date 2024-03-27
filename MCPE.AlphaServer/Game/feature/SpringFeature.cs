using MCPE.AlphaServer.Game.utils.random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPE.AlphaServer.Game.feature
{
    public class SpringFeature : Feature
    {
        public byte blockID;
        public SpringFeature(int blockID)
        {
            this.blockID = (byte)blockID;
        }
        public override bool place(World world, BedrockRandom rand, int x, int y, int z)
        {
            if (world.getBlockIDAt(x, y + 1, z) != Block.stone.blockID)
            {
                return false;
            }
            if (world.getBlockIDAt(x, y - 1, z) != Block.stone.blockID)
            {
                return false;
            }
            if (world.getBlockIDAt(x, y, z) != 0 && world.getBlockIDAt(x, y, z) != Block.stone.blockID)
            {
                return false;
            }
            int l = 0;
            if (world.getBlockIDAt(x - 1, y, z) == Block.stone.blockID)
            {
                l++;
            }
            if (world.getBlockIDAt(x + 1, y, z) == Block.stone.blockID)
            {
                l++;
            }
            if (world.getBlockIDAt(x, y, z - 1) == Block.stone.blockID)
            {
                l++;
            }
            if (world.getBlockIDAt(x, y, z + 1) == Block.stone.blockID)
            {
                l++;
            }
            int i1 = 0;
            if (world.isAirBlock(x - 1, y, z))
            {
                i1++;
            }
            if (world.isAirBlock(x + 1, y, z))
            {
                i1++;
            }
            if (world.isAirBlock(x, y, z - 1))
            {
                i1++;
            }
            if (world.isAirBlock(x, y, z + 1))
            {
                i1++;
            }
            if (l == 3 && i1 == 1)
            {
                world.placeBlock(x, y, z, this.blockID);
                world.instantScheduledUpdate = true;
                Block.blocks[this.blockID].tick(world, x, y, z, rand);
                world.instantScheduledUpdate = false;
            }
            return true;
        }
    }
}
