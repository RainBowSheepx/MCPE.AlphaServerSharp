using SpoongePE.Core.Game.material;
using SpoongePE.Core.Game.utils.random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.feature
{
    public class ReedsFeature : Feature
    {
        public override bool place(World world, BedrockRandom rand, int x, int y, int z)
        {
            for (int l = 0; l < 20; ++l)
            {
                int blockX = (int)(x + rand.nextInt(4)) - rand.nextInt(4);
                int blockZ = (int)(z + rand.nextInt(4)) - rand.nextInt(4);
                if (world.isAirBlock(blockX, y, blockZ) && (world.getMaterial(blockX - 1, y - 1, blockZ) == Material.water || world.getMaterial(blockX + 1, y - 1, blockZ) == Material.water || world.getMaterial(blockX, y - 1, blockZ - 1) == Material.water || world.getMaterial(blockX, y - 1, blockZ + 1) == Material.water))
                {
                    int nextInt3 = 2 + rand.nextInt(rand.nextInt(3) + 1);
                    for (int i2 = 0; i2 < nextInt3; i2++)
                    {
                        if (Block.reeds.canSurvive(world, blockX, y + i2, blockZ))
                        { //TODO a bit non-vanilla, make vanilla
                            world.placeBlock(blockX, y + i2, blockZ, (byte)Block.reeds.blockID);
                        }
                    }
                }
            }
            return true;
        }
    }
}
