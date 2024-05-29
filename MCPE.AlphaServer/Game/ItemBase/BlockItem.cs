using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.player;

namespace SpoongePE.Core.Game.ItemBase
{
    public class BlockItem : Item
    {
        public int blockID;

        public BlockItem(int id) : base(id)
        {
            this.blockID = id + 256;
        }

        public new bool useOn(ItemStack item, Player player, World world, int x, int y, int z, int side)
        {
            if (world.getBlockIDAt(x, y, z) == Block.snowLayer.blockID)
            {
                side = 0;
            }
            else
            {
                switch (side)
                {
                    case 0:
                        --y;
                        break;
                    case 1:
                        ++y;
                        break;
                    case 2:
                        --z;
                        break;
                    case 3:
                        ++z;
                        break;
                    case 4:
                        --x;
                        break;
                    case 5:
                        ++x;
                        break;
                }
            }

            if (item.stackSize == 0) return false;
            if (!world.mayPlace(this.blockID, x, y, z, false))
            {
                return false;
            }

            Block b = Block.blocks[this.blockID];

            if (!world.setBlock(x, y, z, this.blockID, 0, 3))
            {
                return true;
            }

            b.setPlacedOnFace(world, x, y, z, side);
            b.setPlacedBy(world, x, y, z, player);

            return true;
        }
    }
}
