using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemSign : Item
    {
        public ItemSign(int itemID) : base(itemID)
        {
            this.maxStackSize = 1;
        }

        public new bool onItemUse(ItemStack var1, Player var2, World var3, int var4, int var5, int var6, int var7)
        {
            if (var7 == 0)
            {
                return false;
            }
            else if (!var3.getMaterial(var4, var5, var6).isSolid)
            {
                return false;
            }
            else
            {
                if (var7 == 1)
                {
                    ++var5;
                }

                if (var7 == 2)
                {
                    --var6;
                }

                if (var7 == 3)
                {
                    ++var6;
                }

                if (var7 == 4)
                {
                    --var4;
                }

                if (var7 == 5)
                {
                    ++var4;
                }

                if (!Block.signPost.mayPlace(var3, var4, var5, var6))
                {
                    return false;
                }
                else
                {
                    if (var7 == 1)
                    {
                        var3.setBlock(var4, var5, var6, Block.signPost.blockID, MathHelper.floor_double((double)((var2.yaw + 180.0F) * 16.0F / 360.0F) + 0.5D) & 15);
                    }
                    else
                    {
                        var3.setBlock(var4, var5, var6, Block.wallSign.blockID, var7);
                    }

                    --var1.stackSize;

                    return true;
                }
            }
        }
    }
}
