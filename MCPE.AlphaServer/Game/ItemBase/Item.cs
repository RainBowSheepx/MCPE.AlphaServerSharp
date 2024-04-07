using SpoongePE.Core.Game.BlockBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase
{
    public class Item
    {
        // Source
        // net/minecraft/src/Item.java
        public static Item[] itemsList = new Item[512];

        public int itemID;
        public int shiftedIndex;
        protected int maxStackSize = 64;
        private int maxDamage = 0;
        protected bool hasSubtypes = false;
        private Item containerItem = null;
        public Item(int itemID)
        {
            this.itemID = itemID + 256;
            Item.itemsList[this.itemID] = this;
        }

        public bool onItemUse(ItemStack item, Player player, World world, int x, int y, int z, int side) => false;





        public Item setMaxStackSize(int var1)
        {
            this.maxStackSize = var1;
            return this;
        }



        public float getStrVsBlock(ItemStack var1, Block var2) => 1.0F;


        public ItemStack onItemRightClick(ItemStack var1, World var2, Player var3) => var1;


        public int getItemStackLimit() => this.maxStackSize;


        public int getPlacedBlockMetadata(int var1) => 0;


        public bool getHasSubtypes() => this.hasSubtypes;


        protected Item setHasSubtypes(bool var1)
        {
            this.hasSubtypes = var1;
            return this;
        }

        public int getMaxDamage() => this.maxDamage;

        protected Item setMaxDamage(int var1)
        {
            this.maxDamage = var1;
            return this;
        }

        public bool isDamagable() => this.maxDamage > 0 && !this.hasSubtypes;


        //  public bool hitEntity(ItemStack var1, EntityLiving var2, EntityLiving var3) => false;


        public bool onBlockDestroyed(ItemStack var1, int var2, int var3, int var4, int var5, Player var6) => false;


        public int getDamageVsEntity(Entity var1) => 1;


        public bool canHarvestBlock(Block var1) => false;


        //        public void saddleEntity(ItemStack var1, EntityLiving var2)



        public bool shouldRotateAroundWhenRendering() => false;

        public Item setContainerItem(Item var1)
        {
            if (this.maxStackSize > 1)
            {
                throw new ArgumentException("Max stack size must be 1 for items with crafting results");
            }
            else
            {
                this.containerItem = var1;
                return this;
            }
        }

        public Item getContainerItem() => this.containerItem;


        public bool hasContainerItem() => this.containerItem != null;


        public int getColorFromDamage(int var1) => 16777215;


        public void onUpdate(ItemStack var1, World var2, Entity var3, int var4, bool var5) { }


        public void onCreated(ItemStack var1, World var2, Player var3) { }



    }
}
