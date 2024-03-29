using SpoongePE.Core.Game.utils.random;
using SpoongePE.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.feature
{
    public class CactusFeature : Feature
    {
        public override bool place(World world, BedrockRandom rand, int x, int y, int z)
        {
            for (int l = 0; l < 10; l++)
            {
                int i1 = (x + rand.nextInt(8)) - rand.nextInt(8);
                int j1 = (y + rand.nextInt(4)) - rand.nextInt(4);
                int k1 = (z + rand.nextInt(8)) - rand.nextInt(8);
                if (!world.isAirBlock(i1, j1, k1))
                {
                    continue;
                }
                int l1 = 1 + rand.nextInt(rand.nextInt(3) + 1);
                for (int i2 = 0; i2 < l1; i2++)
                {
                    if (Block.cactus.canSurvive(world, i1, j1 + i2, k1))
                    {
                        world.placeBlock(i1, j1 + i2, k1, (byte)Block.cactus.blockID);
                        Logger.Info("placing cactus on" + i1 + ":" + (j1 + i2) + ":" + k1);
                    }
                }

            }

            return true;
        }
    }
}
