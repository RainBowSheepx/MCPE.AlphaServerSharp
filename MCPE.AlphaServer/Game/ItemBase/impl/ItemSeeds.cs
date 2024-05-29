using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemSeeds : Item
    {
        private int blockType;
        public ItemSeeds(int itemID, int var2) : base(itemID)
        {
            this.blockType = var2;
        }

        public new bool onItemUse(ItemStack var1, Player var2, World var3, int var4, int var5, int var6, int var7)
        {
            if (var7 != 1)
            {
                return false;
            }
            else
            {
                int var8 = var3.getBlockIDAt(var4, var5, var6);
                if (var8 == Block.farmland.blockID && var3.isAirBlock(var4, var5 + 1, var6))
                {
                    var3.setBlock(var4, var5 + 1, var6, this.blockType);
                    --var1.stackSize;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
