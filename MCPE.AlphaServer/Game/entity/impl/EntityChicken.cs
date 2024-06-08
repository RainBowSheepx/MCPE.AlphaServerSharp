using SpoongePE.Core.Game.ItemBase;
using SpoongePE.Core.NBT;
using System;

namespace SpoongePE.Core.Game.entity.impl
{
    public class EntityChicken : EntityAnimal
    {
        public bool field_753_a = false;
        public float wingRotation = 0.0F;
        public float destPos = 0.0F;
        public float oFlapSpeed;
        public float oFlap;
        public float wingRotDelta = 1.0F;
        public int timeUntilNextEgg;

        public EntityChicken(World var1) : base(var1)
        {
            this.setSize(0.3F, 0.4F);
            this.health = 4;
            this.timeUntilNextEgg = this.rand.Next(6000) + 6000;
        }

        public new void onLivingUpdate()
        {
            base.onLivingUpdate();
            this.oFlap = this.wingRotation;
            this.oFlapSpeed = this.destPos;
            this.destPos = (float)((double)this.destPos + (double)(this.onGround ? -1 : 4) * 0.3D);
            if (this.destPos < 0.0F)
            {
                this.destPos = 0.0F;
            }

            if (this.destPos > 1.0F)
            {
                this.destPos = 1.0F;
            }

            if (!this.onGround && this.wingRotDelta < 1.0F)
            {
                this.wingRotDelta = 1.0F;
            }

            this.wingRotDelta = (float)((double)this.wingRotDelta * 0.9D);
            if (!this.onGround && this.motionY < 0.0D)
            {
                this.motionY *= 0.6f;
            }

            this.wingRotation += this.wingRotDelta * 2.0F;
            if (--this.timeUntilNextEgg <= 0)
            {
                this.dropItem(Item.egg.shiftedIndex, 1);
                this.timeUntilNextEgg = this.rand.Next(6000) + 6000;
            }

        }

        protected new void fall(float var1)
        {
        }

        protected override void writeEntityToNBT(NbtCompound var1)
        {
            base.writeEntityToNBT(var1);
        }

        protected override void readEntityFromNBT(NbtCompound var1)
        {
            base.readEntityFromNBT(var1);
        }

        protected new int getDropItemId()
        {
            return Item.feather.shiftedIndex;
        }
    }
}