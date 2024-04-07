using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.material;
using SpoongePE.Core.Game.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemBucket : Item
    {
        private int isFull;

        public ItemBucket(int itemID, int var2) : base(itemID)
        {
            this.maxStackSize = 1;
            this.isFull = var2;
        }
        // TODO: onItemRightClick
    }
}
