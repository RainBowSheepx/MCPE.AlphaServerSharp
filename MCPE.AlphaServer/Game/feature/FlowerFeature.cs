using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.utils.random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.feature
{
    public class FlowerFeature : Feature
    {
        public byte blockID;
        public FlowerFeature(int id)
        {
            this.blockID = (byte)id;
        }
        public override bool place(World world, BedrockRandom rand, int x, int y, int z)
        {
            for (int i = 0; i < 64; i++)
            {
                int xPos = (x + rand.nextInt(8)) - rand.nextInt(8);
                int yPos = (y + rand.nextInt(4)) - rand.nextInt(4);
                int zPos = (z + rand.nextInt(8)) - rand.nextInt(8);
                if (world.isAirBlock(xPos, yPos, zPos) && Block.blocks[this.blockID].canSurvive(world, xPos, yPos, zPos))
                {
                    world.placeBlock(xPos, yPos, zPos, this.blockID);
                }
            }
            return true;
        }
    }
}
