using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemSaddle : Item
    {
        public ItemSaddle(int itemID) : base(itemID)
        {
            this.maxStackSize = 1;
        }
        // TODO saddleEntity & hitEntity
    }
}
