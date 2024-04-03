using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase
{
    public class Item
    {
        public static Item[] items = new Item[512];

        public int itemID;

        public Item(int itemID)
        {
            this.itemID = itemID + 256;
            Item.items[this.itemID] = this;
        }

        public bool useOn(ItemInstance item, Player player, World world, int x, int y, int z, int side)
        {
            return false;
        }

    }
}
