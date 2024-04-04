using SpoongePE.Core.Game.material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpoongePE.Core.Utils;
using SpoongePE.Core.Game.utils;

namespace SpoongePE.Core.Game.BlockBase.impl
{
    public class StairsBlock : Block
    {
        public StairsBlock(int id, Material m) : base(id, m)
        {
            isSolid = false;
        }
        public override void onBlockPlacedByPlayer(World world, int x, int y, int z, int face, Player player)
        {
            switch (utils.Utils.getPlayerDirection(player))
            {
                case 0:
                    world.placeBlock(x, y, z, (byte)blockID, 2);
                    break;
                case 1:
                    world.placeBlock(x, y, z, (byte)blockID, 1);
                    break;
                case 2:
                    world.placeBlock(x, y, z, (byte)blockID, 3);
                    break;
                default:
                    world.placeBlock(x, y, z, (byte)blockID, 0);
                    break;

            }

        }
    }
}
