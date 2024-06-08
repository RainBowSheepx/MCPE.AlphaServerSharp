using SpoongePE.Core.Game.ItemBase;
using SpoongePE.Core.Game.player;
using SpoongePE.Core.NBT;
using System;

namespace SpoongePE.Core.Game.entity.impl
{
    public class EntityPig : EntityAnimal
    {
        public EntityPig(World var1) : base(var1)
        {
            this.setSize(0.9F, 0.9F);
        }



        protected override void writeEntityToNBT(NbtCompound var1)
        {
            base.writeEntityToNBT(var1);
            var1.Add(new NbtByte("Saddle", this.getSaddled() ? (byte)1 : (byte)0));
        }

        protected override void readEntityFromNBT(NbtCompound var1)
        {
            base.readEntityFromNBT(var1);
            this.setSaddled(var1["Saddle"].ByteValue == 1 ? true : false);
        }




        public new bool interact(EntityPlayer var1)
        {
            if (!this.getSaddled() || this.riddenByEntity != null && this.riddenByEntity != var1)
            {
                return false;
            }
            else
            {
            //    var1.mountEntity(this);
                return true;
            }
        }

        protected new int getDropItemId()
        {
            return this.fire > 0 ? Item.porkCooked.shiftedIndex : Item.porkRaw.shiftedIndex;
        }

        public bool getSaddled()
        {
            return false; // (this.dataWatcher.getWatchableObjectByte(16) & 1) != 0;
        }

        public void setSaddled(bool var1)
        {
/*            if (var1)
            {
                this.dataWatcher.updateObject(16, (byte)1);
            }
            else
            {
                this.dataWatcher.updateObject(16, (byte)0);
            }*/

        }
    }
}