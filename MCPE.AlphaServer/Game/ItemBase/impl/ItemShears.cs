using SpoongePE.Core.Game.BlockBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemShears : Item
    {
        public ItemShears(int itemID) : base(itemID)
        {
            this.setMaxStackSize(1);
            this.setMaxDamage(238);
        }

        public new bool onBlockDestroyed(ItemStack var1, int var2, int var3, int var4, int var5, Player var6)
        {
            if (var2 == Block.leaves.blockID || var2 == Block.cobweb.blockID)
            {
                var1.damageItem(1, var6);
            }

            return base.onBlockDestroyed(var1, var2, var3, var4, var5, var6);
        }

        public new bool canHarvestBlock(Block var1)
        {
            return var1.blockID == Block.cobweb.blockID;
        }

        public new float getStrVsBlock(ItemStack var1, Block var2)
        {
            if (var2.blockID != Block.cobweb.blockID && var2.blockID != Block.leaves.blockID)
            {
                return var2.blockID == Block.wool.blockID ? 5.0F : base.getStrVsBlock(var1, var2);
            }
            else
            {
                return 15.0F;
            }
        }
    }
}
