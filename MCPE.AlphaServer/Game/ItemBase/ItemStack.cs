using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.entity;
using SpoongePE.Core.Game.player;
using SpoongePE.Core.NBT;
using System;
using System.Diagnostics;

namespace SpoongePE.Core.Game.ItemBase;

public class ItemStack // alias ItemInstance
{
    public int itemID;
    public byte stackSize;

    public int itemDamage;
    // Here's stuff like Tile* and Item*, we don't use those yet.

    public ItemStack() { }


    public ItemStack(Block var1) : this(var1, 1) { }


    public ItemStack(Block var1, int var2) : this(var1.blockID, (byte)var2, 0) { }


    public ItemStack(Block var1, int var2, int var3) : this(var1.blockID, (byte)var2, var3) { }


    public ItemStack(Item var1) : this(var1.shiftedIndex, 1, 0) { }


    public ItemStack(Item var1, int var2) : this(var1.shiftedIndex, (byte)var2, 0) { }


    public ItemStack(Item var1, int var2, int var3) : this(var1.shiftedIndex, (byte)var2, var3) { }

    public ItemStack(int itemID, byte itemCount, int itemMeta)
    {
        this.itemID = itemID;
        stackSize = itemCount;
        itemDamage = itemMeta;
    }
    public ItemStack(NbtCompound var1)
    {
        this.stackSize = 0;
        this.readFromNBT(var1);
    }
    public ItemStack splitStack(int var1)
    {
        this.stackSize -= (byte)var1;
        return new ItemStack(this.itemID, (byte)var1, this.itemDamage);
    }
    public Item getItem() => Item.itemsList[this.itemID];


    public bool useItem(Player player, World world, int x, int y, int z, int face) => getItem().onItemUse(this, player, world, x, y, z, face);

    public float getStrVsBlock(Block var1) => this.getItem().getStrVsBlock(this, var1);


    public ItemStack useItemRightClick(World var1, Player var2) => this.getItem().onItemRightClick(this, var1, var2);


    public NbtCompound writeToNBT(NbtCompound var1)
    {
        var1.Add(new NbtShort("id", (short)this.itemID));
        var1.Add(new NbtByte("Count", (byte)this.stackSize));
        var1.Add(new NbtShort("Damage", (short)this.itemDamage));
        return var1;
    }

    public void readFromNBT(NbtCompound var1)
    {
        this.itemID = var1["id"].ShortValue;
        this.stackSize = var1["Count"].ByteValue;
        this.itemDamage = var1["Damage"].ShortValue;
    }

    public int getMaxStackSize() => this.getItem().getItemStackLimit();


    public bool isStackable() => this.getMaxStackSize() > 1 && (!this.isItemStackDamageable() || !this.isItemDamaged());


    public bool isItemStackDamageable() => Item.itemsList[this.itemID].getMaxDamage() > 0;


    public bool getHasSubtypes() => Item.itemsList[this.itemID].getHasSubtypes();


    public bool isItemDamaged() => this.isItemStackDamageable() && this.itemDamage > 0;


    public int getItemDamageForDisplay() => this.itemDamage;


    public int getItemDamage() => this.itemDamage;


    public void setItemDamage(int var1)
    {
        this.itemDamage = var1;
    }

    public int getMaxDamage() => Item.itemsList[this.itemID].getMaxDamage();


    public void damageItem(int var1, Entity var2)
    {
        if (this.isItemStackDamageable())
        {
            this.itemDamage += var1;
            if (this.itemDamage > this.getMaxDamage())
            {

                --this.stackSize;
                if (this.stackSize < 0)
                {
                    this.stackSize = 0;
                }

                this.itemDamage = 0;
            }

        }
    }

    public void hitEntity(EntityLiving var1, EntityPlayer var2) => Item.itemsList[this.itemID].hitEntity(this, var1, var2);
    public void hitEntity(EntityLiving var1, Player var2) => Item.itemsList[this.itemID].hitEntity(this, var1, var2);

    public void onDestroyBlock(int var1, int var2, int var3, int var4, Player var5) => Item.itemsList[this.itemID].onBlockDestroyed(this, var1, var2, var3, var4, var5);


    public int getDamageVsEntity(Entity var1) => Item.itemsList[this.itemID].getDamageVsEntity(var1);


    public bool canHarvestBlock(Block var1) => Item.itemsList[this.itemID].canHarvestBlock(var1);


    public void onItemDestroyedByUse(Player var1)
    {
    }
    public void onItemDestroyedByUse(EntityPlayer var1)
    {
    }
    public void useItemOnEntity(EntityLiving var1) => Item.itemsList[this.itemID].saddleEntity(this, var1);


    public ItemStack copy()
    {
        return new ItemStack(this.itemID, this.stackSize, this.itemDamage);
    }

    public static bool areItemStacksEqual(ItemStack var0, ItemStack var1)
    {
        if (var0 == null && var1 == null)
        {
            return true;
        }
        else
        {
            return var0 != null && var1 != null ? var0.isItemStackEqual(var1) : false;
        }
    }

    private bool isItemStackEqual(ItemStack var1)
    {
        if (this.stackSize != var1.stackSize)
        {
            return false;
        }
        else if (this.itemID != var1.itemID)
        {
            return false;
        }
        else
        {
            return this.itemDamage == var1.itemDamage;
        }
    }

    public bool isItemEqual(ItemStack var1)
    {
        return this.itemID == var1.itemID && this.itemDamage == var1.itemDamage;
    }

    public static ItemStack copyItemStack(ItemStack var0) => var0 == null ? null : var0.copy();

    public void onCrafting(World var1, Player var2) => Item.itemsList[this.itemID].onCreated(this, var1, var2);

    public bool isStackEqual(ItemStack var1) => this.itemID == var1.itemID && this.stackSize == var1.stackSize && this.itemDamage == var1.itemDamage;



    public override string ToString() => $"ItemStack[id: {itemID} count: {stackSize} metadata: {itemDamage}]";


}
