using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemPickaxe : ItemTool
    {
        private static Block[] blocksEffectiveAgainst = new Block[] { Block.cobblestone, Block.cobbleStairs, Block.brickStairs, Block.netherBrickStairs, Block.quartzStairs, Block.sandstoneStairs, Block.stoneBrickStairs, Block.stone, Block.sandStone, Block.cobbleWall, Block.oreIron, Block.ironBlock, Block.oreCoal, Block.goldBlock, Block.oreGold, Block.oreDiamond, Block.diamondBlock, Block.ice, Block.netherrack, Block.lapisOre, Block.lapisBlock };
        
        public ItemPickaxe(int itemID, EnumToolMaterial var3) : base(itemID, 2, var3, blocksEffectiveAgainst) { }

        public new bool canHarvestBlock(Block var1)
        {
            if (var1 == Block.obsidian)
            {
                return this.toolMaterial.getHarvestLevel() == 3;
            }
            else if (var1 != Block.diamondBlock && var1 != Block.oreDiamond)
            {
                if (var1 != Block.goldBlock && var1 != Block.oreGold)
                {
                    if (var1 != Block.ironBlock && var1 != Block.oreIron)
                    {
                        if (var1 != Block.lapisBlock && var1 != Block.lapisOre)
                        {
                            if (var1.material == Material.stone)
                            {
                                return true;
                            }
                            else
                            {
                                return var1.material == Material.metal;
                            }
                        }
                        else
                        {
                            return this.toolMaterial.getHarvestLevel() >= 1;
                        }
                    }
                    else
                    {
                        return this.toolMaterial.getHarvestLevel() >= 1;
                    }
                }
                else
                {
                    return this.toolMaterial.getHarvestLevel() >= 2;
                }
            }
            else
            {
                return this.toolMaterial.getHarvestLevel() >= 2;
            }
        }
    }
}
