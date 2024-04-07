using SpoongePE.Core.Game.BlockBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemMinecart : Item
    {
        public ItemMinecart(int itemID) : base(itemID)
        {
            this.maxStackSize = 1;
        }
        public new bool onItemUse(ItemStack var1, Player var2, World var3, int var4, int var5, int var6, int var7)
        {
            int var8 = var3.getBlockIDAt(var4, var5, var6);
            if (var8 == Block.rail.blockID || var8 == Block.poweredRail.blockID)
            {
                   // var3.entityJoinedWorld(new EntityMinecart(var3, (double)((float)var4 + 0.5F), (double)((float)var5 + 0.5F), (double)((float)var6 + 0.5F), this.minecartType));
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
