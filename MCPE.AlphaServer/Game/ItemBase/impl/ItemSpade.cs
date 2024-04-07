using SpoongePE.Core.Game.BlockBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemSpade : ItemTool
    {
        private static Block[] blocksEffectiveAgainst = new Block[] { Block.grass, Block.dirt, Block.sand, Block.gravel, Block.snowLayer, Block.snow, Block.clay, Block.farmland };
        public ItemSpade(int itemID, EnumToolMaterial var3) : base(itemID, 1, var3, blocksEffectiveAgainst) { }

        public new bool canHarvestBlock(Block var1)
        {
            if (var1 == Block.snowLayer)
            {
                return true;
            }
            else
            {
                return var1 == Block.snow;
            }
        }
    }
}
