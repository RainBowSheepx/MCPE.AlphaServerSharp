using SpoongePE.Core.Game.utils;
using SpoongePE.Core.NBT;
using System;

namespace SpoongePE.Core.Game.entity.impl
{
    public class EntityTNTPrimed : Entity
    {
        public int fuse;

        public EntityTNTPrimed(World var1) : base(var1)
        {
            this.fuse = 0;
            this.preventEntitySpawning = true;
            this.setSize(0.98F, 0.98F);
            this.yOffset = this.height / 2.0F;
        }

        public EntityTNTPrimed(World var1, double var2, double var4, double var6) : this(var1)
        {
            this.setPosition(var2, var4, var6);
            float var8 = (float)(rand.NextDouble() * 3.1415927410125732D * 2.0D);
            this.motionX = (-MathHelper.sin(var8 * 3.1415927F / 180.0F) * 0.02F);
            this.motionY = 0.20000000298023224f;
            this.motionZ = (-MathHelper.cos(var8 * 3.1415927F / 180.0F) * 0.02F);
            this.fuse = 80;
            this.prevPosX = (float)var2;
            this.prevPosY = (float)var4;
            this.prevPosZ = (float)var6;
        }

        protected void entityInit()
        {
        }

        protected new bool canTriggerWalking()
        {
            return false;
        }

        public new bool canBeCollidedWith()
        {
            return !this.isDead;
        }

        public new void onUpdate()
        {
            this.prevPosX = this.posX;
            this.prevPosY = this.posY;
            this.prevPosZ = this.posZ;
            this.motionY -= 0.03999999910593033f;
            this.moveEntity(this.motionX, this.motionY, this.motionZ);
            this.motionX *= 0.9800000190734863f;
            this.motionY *= 0.9800000190734863f;
            this.motionZ *= 0.9800000190734863f;
            if (this.onGround)
            {
                this.motionX *= 0.699999988079071f;
                this.motionZ *= 0.699999988079071f;
                this.motionY *= -0.5f;
            }

            if (this.fuse-- <= 0)
            {
                    this.setEntityDead();
                    this.explode();

            }


        }

        private void explode()
        {
            float var1 = 4.0F;
            this.world.createExplosion((Entity)null, this.posX, this.posY, this.posZ, var1);
        }

        protected override void writeEntityToNBT(NbtCompound var1)
        {
            var1.Add(new NbtByte("Fuse", (byte)this.fuse));
        }

        protected  override void readEntityFromNBT(NbtCompound var1)
        {
            this.fuse = var1.Get<NbtByte>("Fuse").ByteValue;
        }

        public new float getShadowSize()
        {
            return 0.0F;
        }
    }
}