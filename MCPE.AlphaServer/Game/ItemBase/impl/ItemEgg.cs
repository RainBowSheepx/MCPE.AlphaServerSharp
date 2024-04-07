using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemEgg : Item
    {
        public ItemEgg(int itemID) : base(itemID)
        {
            this.maxStackSize = 16;
        }
        // TODO onItemRightClick
    }
}
