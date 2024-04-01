using SpoongePE.Core.Game.material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpoongePE.Core.Utils;
using SpoongePE.Core.Game.utils;

namespace SpoongePE.Core.Game.BlockBase
{
    public class StairsBlock : Block
    {
        public StairsBlock(int id, Material m) : base(id, m)
        {
            this.isSolid = false;
        }
        public override void onBlockPlacedByPlayer(World world, int x, int y, int z, int face, Player player)
        {
            switch (utils.Utils.getPlayerDirection(player))
            {
                case 0:
                    world.placeBlock(x, y, z, (byte)this.blockID, (byte)2);
                    break;
                case 1:
                    world.placeBlock(x, y, z, (byte)this.blockID, (byte)1);
                    break;
                case 2:
                    world.placeBlock(x, y, z, (byte)this.blockID, (byte)3);
                    break;
                default:
                    world.placeBlock(x, y, z, (byte)this.blockID, (byte)0);
                    break;

            }

        }
    }
}
