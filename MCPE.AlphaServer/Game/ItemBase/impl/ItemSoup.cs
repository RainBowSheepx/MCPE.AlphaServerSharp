using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    internal class ItemSoup : ItemFood
    {
        public ItemSoup(int itemID, int var2) : base(itemID, var2, false)
        {
        }
        public new ItemStack onItemRightClick(ItemStack var1, World var2, Player var3)
        {
            base.onItemRightClick(var1, var2, var3);
            //  return new ItemStack(Item.bowlEmpty);
            return null;
        }
    }
}
