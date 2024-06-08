using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.utils;
using SpoongePE.Core.NBT;
using System;
using YamlDotNet.Core.Tokens;

namespace SpoongePE.Core.Game.entity.impl
{
    public class EntityFallingSand : Entity
    {
        public int blockID;
        public int fallTime = 0;

        public EntityFallingSand(World var1) : base(var1)
        {
        }

        public EntityFallingSand(World var1, double var2, double var4, double var6, int var8) : base(var1)
        {
            this.blockID = var8;
            this.preventEntitySpawning = true;
            this.setSize(0.98F, 0.98F);
            this.yOffset = this.height / 2.0F;
            this.setPosition(var2, var4, var6);
            this.motionX = 0.0f;
            this.motionY = 0.0f;
            this.motionZ = 0.0f;
            this.prevPosX = (float)var2;
            this.prevPosY = (float)var4;
            this.prevPosZ = (float)var6;
        }

        protected new bool canTriggerWalking()
        {
            return false;
        }

        protected void entityInit()
        {
        }

        public new bool canBeCollidedWith()
        {
            return !this.isDead;
        }

        public new void onUpdate()
        {
            if (this.blockID == 0)
            {
                this.setEntityDead();
            }
            else
            {
                this.prevPosX = this.posX;
                this.prevPosY = this.posY;
                this.prevPosZ = this.posZ;
                ++this.fallTime;
                this.motionY -= 0.03999999910593033f;
                this.moveEntity(this.motionX, this.motionY, this.motionZ);
                this.motionX *= 0.9800000190734863f;
                this.motionY *= 0.9800000190734863f;
                this.motionZ *= 0.9800000190734863f;
                int var1 = MathHelper.floor_double(this.posX);
                int var2 = MathHelper.floor_double(this.posY);
                int var3 = MathHelper.floor_double(this.posZ);
                if (this.world.getBlockIDAt(var1, var2, var3) == this.blockID)
                {
                    this.world.placeBlockAndNotifyNearby(var1, var2, var3, 0);
                }

                if (this.onGround)
                {
                    this.motionX *= 0.699999988079071f;
                    this.motionZ *= 0.699999988079071f;
                    this.motionY *= -0.5f;
                    this.setEntityDead();
                    if ((!this.world.canBlockBePlacedAt(this.blockID, var1, var2, var3, true, 1) /*|| Block.sand.canFallBelow(this.worldObj, var1, var2 - 1, var3)*/ || !this.world.setBlockWithNotify(var1, var2, var3, this.blockID)))
                    {
                        this.dropItem(this.blockID, 1);
                    }
                }
                else if (this.fallTime > 100)
                {
                    this.dropItem(this.blockID, 1);
                    this.setEntityDead();
                }

            }
        }

        protected override void writeEntityToNBT(NbtCompound var1)
        {
            var1.Add(new NbtByte("Tile", (byte)this.blockID));
        }

        protected override void readEntityFromNBT(NbtCompound var1)
        {
            this.blockID = var1.Get<NbtByte>("Tile").ByteValue & 255;
        }

        public new float getShadowSize()
        {
            return 0.0F;
        }

        public World getWorld()
        {
            return this.world;
        }
    }
}