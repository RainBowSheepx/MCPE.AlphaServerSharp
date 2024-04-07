using SpoongePE.Core.Game.BlockBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemAxe : ItemTool
    {
        private static Block[] blocksEffectiveAgainst = new Block[] { Block.planks, Block.bookshelf, Block.log, Block.chest, Block.woodenDoor, Block.woodSlab, Block.woodStairs, Block.doubleWoodSlab };
        public ItemAxe(int itemID, EnumToolMaterial var3) : base(itemID, 3, var3, blocksEffectiveAgainst)
        {
        }
    }
}
