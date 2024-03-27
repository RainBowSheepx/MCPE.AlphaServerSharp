using MCPE.AlphaServer.Game.material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCPE.AlphaServer.Utils;
using MCPE.AlphaServer.Game.utils;

namespace MCPE.AlphaServer.Game.BlockBase
{
    public class StairsBlock : Block
    {
        public StairsBlock(int id, Material m) : base(id, m)
        {
            this.isSolid = false;
        }
        public void onBlockPlacedByPlayer(World world, int x, int y, int z, int face, Player player)
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
