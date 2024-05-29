using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.BlockBase.impl;
using SpoongePE.Core.Game.player;
using SpoongePE.Core.Game.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemBed : Item
    {
        public ItemBed(int itemID) : base(itemID)
        {
        }

        public new bool onItemUse(ItemStack var1, Player var2, World var3, int var4, int var5, int var6, int var7)
        {
            if (var7 != 1)
            {
                return false;
            }
            else
            {
                ++var5;
                BedBlock var8 = Block.bedBlock;
                int var9 = MathHelper.floor_double((double)(var2.rotationYaw * 4.0F / 360.0F) + 0.5D) & 3;
                short var10 = 0;
                short var11 = 0;
                if (var9 == 0)
                {
                    var11 = 1;
                }

                if (var9 == 1)
                {
                    var10 = -1;
                }

                if (var9 == 2)
                {
                    var11 = -1;
                }

                if (var9 == 3)
                {
                    var10 = 1;
                }

                if (var3.isAirBlock(var4, var5, var6) && var3.isAirBlock(var4 + var10, var5, var6 + var11) && var3.isBlockSolid(var4, var5 - 1, var6) && var3.isBlockSolid(var4 + var10, var5 - 1, var6 + var11))
                {
                    var3.setBlock(var4, var5, var6, var8.blockID, var9);
                    var3.setBlock(var4 + var10, var5, var6 + var11, var8.blockID, var9 + 8);
                    --var1.stackSize;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
