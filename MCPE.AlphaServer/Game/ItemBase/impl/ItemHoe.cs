using SpoongePE.Core.Game.BlockBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemHoe : Item
    {
        public ItemHoe(int itemID, EnumToolMaterial var2) : base(itemID)
        {
            this.maxStackSize = 1;
            this.setMaxDamage(var2.getMaxUses());
        }

        public new bool onItemUse(ItemStack var1, Player var2, World var3, int var4, int var5, int var6, int var7)
        {
            int var8 = var3.getBlockIDAt(var4, var5, var6);
            int var9 = var3.getBlockIDAt(var4, var5 + 1, var6);
            if ((var7 == 0 || var9 != 0 || var8 != Block.grass.blockID) && var8 != Block.dirt.blockID)
            {
                return false;
            }
            else
            {
                Block var10 = Block.farmland;

                var3.setBlock(var4, var5, var6, var10.blockID);
                var1.damageItem(1, var2);
                return true;

            }
        }
    }
}
