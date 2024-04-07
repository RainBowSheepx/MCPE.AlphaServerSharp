using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemSnowball : Item
    {
        public ItemSnowball(int itemID) : base(itemID)
        {
            this.maxStackSize = 16;
        }
        public new ItemStack onItemRightClick(ItemStack var1, World var2, Player var3)
        {
            --var1.stackSize;

                //var2.entityJoinedWorld(new EntitySnowball(var2, var3));
            

            return var1;
        }
    }
}
