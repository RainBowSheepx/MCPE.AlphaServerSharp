using SpoongePE.Core.Game.material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemDoor : Item
    {
        private Material doorMaterial;
        public ItemDoor(int itemID, Material var2) : base(itemID)
        {
            this.doorMaterial = var2;
            this.maxStackSize = 1;
        }
        // TODO onItemUse
    }
}
