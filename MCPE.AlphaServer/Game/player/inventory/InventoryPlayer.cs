using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.entity;
using SpoongePE.Core.Game.ItemBase;
using SpoongePE.Core.Game.ItemBase.impl;
using SpoongePE.Core.Game.player;
using SpoongePE.Core.NBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.player.inventory
{
    public class InventoryPlayer : IInventory
    {
        public ItemStack[] mainInventory = new ItemStack[36];
        public ItemStack[] armorInventory = new ItemStack[4];
        public int currentItem = 0;
        public EntityPlayer player;
        private ItemStack itemStack;
        public bool inventoryChanged = false;

        public InventoryPlayer(EntityPlayer var1)
        {
            this.player = var1;
        }

        public ItemStack getCurrentItem()
        {
            return this.currentItem < 9 && this.currentItem >= 0 ? this.mainInventory[this.currentItem] : null;
        }

        private int getInventorySlotContainItem(int var1)
        {
            for (int var2 = 0; var2 < this.mainInventory.Length; ++var2)
            {
                if (this.mainInventory[var2] != null && this.mainInventory[var2].itemID == var1)
                {
                    return var2;
                }
            }

            return -1;
        }

        private int storeItemStack(ItemStack var1)
        {
            for (int var2 = 0; var2 < this.mainInventory.Length; ++var2)
            {
                if (this.mainInventory[var2] != null && this.mainInventory[var2].itemID == var1.itemID && this.mainInventory[var2].isStackable() && this.mainInventory[var2].stackSize < this.mainInventory[var2].getMaxStackSize() && this.mainInventory[var2].stackSize < this.getInventoryStackLimit() && (!this.mainInventory[var2].getHasSubtypes() || this.mainInventory[var2].getItemDamage() == var1.getItemDamage()))
                {
                    return var2;
                }
            }

            return -1;
        }

        private int getFirstEmptyStack()
        {
            for (int var1 = 0; var1 < this.mainInventory.Length; ++var1)
            {
                if (this.mainInventory[var1] == null)
                {
                    return var1;
                }
            }

            return -1;
        }

        public void setCurrentItem(int var1, bool var2)
        {
            int var3 = this.getInventorySlotContainItem(var1);
            if (var3 >= 0 && var3 < 9)
            {
                this.currentItem = var3;
            }
        }

        public void changeCurrentItem(int var1)
        {
            if (var1 > 0)
            {
                var1 = 1;
            }

            if (var1 < 0)
            {
                var1 = -1;
            }

            for (this.currentItem -= var1; this.currentItem < 0; this.currentItem += 9)
            {
            }

            while (this.currentItem >= 9)
            {
                this.currentItem -= 9;
            }

        }

        private int storePartialItemStack(ItemStack var1)
        {
            int var2 = var1.itemID;
            int var3 = var1.stackSize;
            int var4 = this.storeItemStack(var1);
            if (var4 < 0)
            {
                var4 = this.getFirstEmptyStack();
            }

            if (var4 < 0)
            {
                return var3;
            }
            else
            {
                if (this.mainInventory[var4] == null)
                {
                    this.mainInventory[var4] = new ItemStack(var2, 0, var1.getItemDamage());
                }

                int var5 = var3;
                if (var3 > this.mainInventory[var4].getMaxStackSize() - this.mainInventory[var4].stackSize)
                {
                    var5 = this.mainInventory[var4].getMaxStackSize() - this.mainInventory[var4].stackSize;
                }

                if (var5 > this.getInventoryStackLimit() - this.mainInventory[var4].stackSize)
                {
                    var5 = this.getInventoryStackLimit() - this.mainInventory[var4].stackSize;
                }

                if (var5 == 0)
                {
                    return var3;
                }
                else
                {
                    var3 -= var5;
                    ItemStack var10000 = this.mainInventory[var4];
                    var10000.stackSize += (byte)var5;

                    return var3;
                }
            }
        }



        public bool consumeInventoryItem(int var1)
        {
            int var2 = this.getInventorySlotContainItem(var1);
            if (var2 < 0)
            {
                return false;
            }
            else
            {
                if (--this.mainInventory[var2].stackSize <= 0)
                {
                    this.mainInventory[var2] = null;
                }

                return true;
            }
        }

        public bool addItemStackToInventory(ItemStack var1)
        {
            int var2;
            if (var1.isItemDamaged())
            {
                var2 = this.getFirstEmptyStack();
                if (var2 >= 0)
                {
                    this.mainInventory[var2] = ItemStack.copyItemStack(var1);

                    var1.stackSize = 0;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                do
                {
                    var2 = var1.stackSize;
                    var1.stackSize = (byte)this.storePartialItemStack(var1);
                } while (var1.stackSize > 0 && var1.stackSize < var2);

                return var1.stackSize < var2;
            }
        }

        public ItemStack decrStackSize(int var1, int var2)
        {
            ItemStack[] var3 = this.mainInventory;
            if (var1 >= this.mainInventory.Length)
            {
                var3 = this.armorInventory;
                var1 -= this.mainInventory.Length;
            }

            if (var3[var1] != null)
            {
                ItemStack var4;
                if (var3[var1].stackSize <= var2)
                {
                    var4 = var3[var1];
                    var3[var1] = null;
                    return var4;
                }
                else
                {
                    var4 = var3[var1].splitStack(var2);
                    if (var3[var1].stackSize == 0)
                    {
                        var3[var1] = null;
                    }

                    return var4;
                }
            }
            else
            {
                return null;
            }
        }

        public void setInventorySlotContents(int var1, ItemStack var2)
        {
            ItemStack[] var3 = this.mainInventory;
            if (var1 >= var3.Length)
            {
                var1 -= var3.Length;
                var3 = this.armorInventory;
            }

            var3[var1] = var2;
        }

        public float getStrVsBlock(Block var1)
        {
            float var2 = 1.0F;
            if (this.mainInventory[this.currentItem] != null)
            {
                var2 *= this.mainInventory[this.currentItem].getStrVsBlock(var1);
            }

            return var2;
        }

        public NbtList writeToNBT(NbtList var1)
        {
            int var2;
            NbtCompound var3;
            for (var2 = 0; var2 < this.mainInventory.Length; ++var2)
            {
                if (this.mainInventory[var2] != null)
                {
                    var3 = new NbtCompound();
                    var3.Add(new NbtByte("Slot", (byte)var2));
                    this.mainInventory[var2].writeToNBT(var3);
                    var1.Add(var3);
                }
            }

            for (var2 = 0; var2 < this.armorInventory.Length; ++var2)
            {
                if (this.armorInventory[var2] != null)
                {
                    var3 = new NbtCompound();
                    var3.Add(new NbtByte("Slot", (byte)(var2 + 100)));
                    this.armorInventory[var2].writeToNBT(var3);
                    var1.Add(var3);
                }
            }

            return var1;
        }

        public void readFromNBT(NbtList var1)
        {
            this.mainInventory = new ItemStack[36];
            this.armorInventory = new ItemStack[4];

            for (int var2 = 0; var2 < var1.Count; ++var2)
            {
                NbtCompound var3 = var1.Get<NbtCompound>(var2);
                int var4 = var3.Get<NbtByte>("Slot").ByteValue & 255;
                ItemStack var5 = new ItemStack(var3);
                if (var5.getItem() != null)
                {
                    if (var4 >= 0 && var4 < this.mainInventory.Length)
                    {
                        this.mainInventory[var4] = var5;
                    }

                    if (var4 >= 100 && var4 < this.armorInventory.Length + 100)
                    {
                        this.armorInventory[var4 - 100] = var5;
                    }
                }
            }

        }

        public int getSizeInventory()
        {
            return this.mainInventory.Length + 4;
        }

        public ItemStack getStackInSlot(int var1)
        {
            ItemStack[] var2 = this.mainInventory;
            if (var1 >= var2.Length)
            {
                var1 -= var2.Length;
                var2 = this.armorInventory;
            }

            return var2[var1];
        }

        public string getInvName()
        {
            return "Inventory";
        }

        public int getInventoryStackLimit()
        {
            return 64;
        }

        public int getDamageVsEntity(Entity var1)
        {
            ItemStack var2 = this.getStackInSlot(this.currentItem);
            return var2 != null ? var2.getDamageVsEntity(var1) : 1;
        }

        public bool canHarvestBlock(Block var1)
        {
            if (var1.material.getIsHarvestable())
            {
                return true;
            }
            else
            {
                ItemStack var2 = this.getStackInSlot(this.currentItem);
                return var2 != null ? var2.canHarvestBlock(var1) : false;
            }
        }

        public ItemStack armorItemInSlot(int var1)
        {
            return this.armorInventory[var1];
        }

        public int getTotalArmorValue()
        {
            int var1 = 0;
            int var2 = 0;
            int var3 = 0;

            for (int var4 = 0; var4 < this.armorInventory.Length; ++var4)
            {
                if (this.armorInventory[var4] != null && this.armorInventory[var4].getItem() is ItemArmor)
                {
                    int var5 = this.armorInventory[var4].getMaxDamage();
                    int var6 = this.armorInventory[var4].getItemDamageForDisplay();
                    int var7 = var5 - var6;
                    var2 += var7;
                    var3 += var5;
                    int var8 = ((ItemArmor)this.armorInventory[var4].getItem()).damageReduceAmount;
                    var1 += var8;
                }
            }

            if (var3 == 0)
            {
                return 0;
            }
            else
            {
                return (var1 - 1) * var2 / var3 + 1;
            }
        }

        public void damageArmor(int var1)
        {
            for (int var2 = 0; var2 < this.armorInventory.Length; ++var2)
            {
                if (this.armorInventory[var2] != null && this.armorInventory[var2].getItem() is ItemArmor)
                {
                    this.armorInventory[var2].damageItem(var1, this.player);
                    if (this.armorInventory[var2].stackSize == 0)
                    {
                        this.armorInventory[var2].onItemDestroyedByUse(this.player);
                        this.armorInventory[var2] = null;
                    }
                }
            }

        }

        public void dropAllItems()
        {
            int var1;
            for (var1 = 0; var1 < this.mainInventory.Length; ++var1)
            {
                if (this.mainInventory[var1] != null)
                {
                    this.player.dropPlayerItemWithRandomChoice(this.mainInventory[var1], true);
                    this.mainInventory[var1] = null;
                }
            }

            for (var1 = 0; var1 < this.armorInventory.Length; ++var1)
            {
                if (this.armorInventory[var1] != null)
                {
                    this.player.dropPlayerItemWithRandomChoice(this.armorInventory[var1], true);
                    this.armorInventory[var1] = null;
                }
            }

        }

        public void onInventoryChanged()
        {
            this.inventoryChanged = true;
        }

        public void setItemStack(ItemStack var1)
        {
            this.itemStack = var1;
            this.player.onItemStackChanged(var1);
        }

        public ItemStack getItemStack()
        {
            return this.itemStack;
        }

        public bool canInteractWith(EntityPlayer var1)
        {
            if (this.player.isDead)
            {
                return false;
            }
            else
            {
                return var1.getDistanceSqToEntity(this.player) <= 64.0D;
            }
        }

        public bool hasItemStack(ItemStack var1)
        {
            int var2;
            for (var2 = 0; var2 < this.armorInventory.Length; ++var2)
            {
                if (this.armorInventory[var2] != null && this.armorInventory[var2].isStackEqual(var1))
                {
                    return true;
                }
            }

            for (var2 = 0; var2 < this.mainInventory.Length; ++var2)
            {
                if (this.mainInventory[var2] != null && this.mainInventory[var2].isStackEqual(var1))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
