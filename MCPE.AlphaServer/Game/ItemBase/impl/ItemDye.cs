using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemDye : Item
    {
        public static string[] dyeColorNames = new string[] { "black", "red", "green", "brown", "blue", "purple", "cyan", "silver", "gray", "pink", "lime", "yellow", "lightBlue", "magenta", "orange", "white" };
        public ItemDye(int itemID) : base(itemID)
        {
            this.setHasSubtypes(true);
            this.setMaxDamage(0);
        }

        // TODO onItemUse
    }
}
