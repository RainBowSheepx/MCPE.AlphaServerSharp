using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    internal class ItemFlintAndSteel : Item
    {
        public ItemFlintAndSteel(int itemID) : base(itemID)
        {
            this.maxStackSize = 1;
            this.setMaxDamage(64);
        }

        public new bool onItemUse(ItemStack var1, Player var2, World var3, int var4, int var5, int var6, int var7)
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

            int var8 = var3.getBlockIDAt(var4, var5, var6);
            if (var8 == 0)
            {
                var3.setBlock(var4, var5, var6, Block.fire.blockID);
            }

            var1.damageItem(1, var2);
            return true;
        }
    }
}
