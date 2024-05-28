using SpoongePE.Core.Game.ItemBase;
using SpoongePE.Core.Game.player;
using SpoongePE.Core.NBT;
using System;

namespace SpoongePE.Core.Game.entity.impl
{
    public class EntityCow : EntityAnimal
    {
        public EntityCow(World var1) : base(var1)
        {
            this.setSize(0.9F, 1.3F);
        }

        public new void writeEntityToNBT(NbtCompound var1)
        {
            base.writeEntityToNBT(var1);
        }

        public new void readEntityFromNBT(NbtCompound var1)
        {
            base.readEntityFromNBT(var1);
        }

        protected new int getDropItemId()
        {
            return Item.leather.shiftedIndex;
        }

        public bool interact(EntityPlayer var1)
        {
            ItemStack var2 = var1.inventory.getCurrentItem();
            if (var2 != null && var2.itemID == Item.bucketEmpty.shiftedIndex)
            {
                var1.inventory.setInventorySlotContents(var1.inventory.currentItem, new ItemStack(Item.bucketMilk));
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}