using SpoongePE.Core.Game.ItemBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.player.inventory
{
    public interface IInventory
    {
        int getSizeInventory();

        ItemStack getStackInSlot(int var1);

        ItemStack decrStackSize(int var1, int var2);

        void setInventorySlotContents(int var1, ItemStack var2);

        String getInvName();

        int getInventoryStackLimit();

        void onInventoryChanged();

        bool canInteractWith(EntityPlayer var1);
    }
}
