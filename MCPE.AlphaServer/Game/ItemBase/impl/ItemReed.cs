using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemReed : Item
    {
        private int spawnID;
        public ItemReed(int itemID, Block var2) : base(itemID)
        {
            this.spawnID = var2.blockID;
        }

        public new bool onItemUse(ItemStack var1, Player var2, World var3, int var4, int var5, int var6, int var7)
        {
            if (var3.getBlockIDAt(var4, var5, var6) == Block.snow.blockID)
            {
                var7 = 0;
            }
            else
            {
                if (var7 == 0)
                {
                    --var5;
                }

                if (var7 == 1)
                {
                    ++var5;
                }

                if (var7 == 2)
                {
                    --var6;
                }

                if (var7 == 3)
                {
                    ++var6;
                }

                if (var7 == 4)
                {
                    --var4;
                }

                if (var7 == 5)
                {
                    ++var4;
                }
            }

            if (var1.stackSize == 0)
            {
                return false;
            }
            else
            {
                if (var3.mayPlace(this.spawnID, var4, var5, var6, false))
                {
                    Block var8 = Block.blocks[this.spawnID];
                    if (var3.setBlock(var4, var5, var6, this.spawnID))
                    {
                      //  Block.blocks[this.spawnID].onBlockPlaced(var3, var4, var5, var6, var7);
                        Block.blocks[this.spawnID].onBlockPlacedByPlayer(var3, var4, var5, var6, var7, var2);

                        --var1.stackSize;
                    }
                }

                return true;
            }
        }
    }
}
