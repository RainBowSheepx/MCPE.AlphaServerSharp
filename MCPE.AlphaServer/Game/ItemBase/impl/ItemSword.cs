using SpoongePE.Core.Game.BlockBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemSword : Item
    {
        private int weaponDamage;
        public ItemSword(int itemID, EnumToolMaterial var2) : base(itemID)
        {
            this.maxStackSize = 1;
            this.setMaxDamage(var2.getMaxUses());
            this.weaponDamage = 4 + var2.getDamageVsEntity() * 2;
        }

        public new float getStrVsBlock(ItemStack var1, Block var2)
        {
            return var2.blockID == Block.cobweb.blockID ? 15.0F : 1.5F;
        }

/*        public bool hitEntity(ItemStack var1, EntityLiving var2, EntityLiving var3)
        {
            var1.damageItem(1, var3);
            return true;
        }*/

        public new bool onBlockDestroyed(ItemStack var1, int var2, int var3, int var4, int var5, Player var6)
        {
            var1.damageItem(2, var6);
            return true;
        }

        public new int getDamageVsEntity(Entity var1) => this.weaponDamage;
        
        public new bool canHarvestBlock(Block var1) => var1.blockID == Block.cobweb.blockID;
        
    }
}
