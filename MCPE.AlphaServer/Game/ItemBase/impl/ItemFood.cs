using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpoongePE.Core.Game.player;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemFood : Item
    {
        private int healAmount;
        public ItemFood(int itemID, int var2) : base(itemID)
        {
            this.healAmount = var2;
            this.maxStackSize = 1;
        }

        public new ItemStack onItemRightClick(ItemStack var1, World var2, Player var3)
        {
            --var1.stackSize;
            var3.heal(this.healAmount);
            return var1;
        }

        public int getHealAmount() => this.healAmount;
        
    }
}
