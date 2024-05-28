using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.ItemBase;
using SpoongePE.Core.Game.player;
using SpoongePE.Core.Game.utils;
using System.Collections.Generic;
using System;
using SpoongePE.Core.NBT;

namespace SpoongePE.Core.Game.entity.impl
{
    public class EntityArrow : Entity
    {
        private int xTile = -1;
        private int yTile = -1;
        private int zTile = -1;
        private int inTile = 0;
        private int inData = 0;
        private bool inGround = false;
        public bool doesArrowBelongToPlayer = false;
        public int arrowShake = 0;
        public EntityLiving owner;
        private int ticksInGround;
        private int ticksInAir = 0;

        public EntityArrow(World var1) : base(var1)
        {

            this.setSize(0.5F, 0.5F);
        }

        public EntityArrow(World var1, double var2, double var4, double var6) : base(var1)
        {

            this.setSize(0.5F, 0.5F);
            this.setPosition(var2, var4, var6);
            this.yOffset = 0.0F;
        }

        public EntityArrow(World var1, EntityLiving var2) : base(var1)
        {

            this.owner = var2;
            this.doesArrowBelongToPlayer = var2 is EntityPlayer;
            this.setSize(0.5F, 0.5F);
            this.setLocationAndAngles(var2.posX, var2.posY + (double)var2.getEyeHeight(), var2.posZ, var2.rotationYaw, var2.rotationPitch);
            this.posX -= (MathHelper.cos(this.rotationYaw / 180.0F * 3.1415927F) * 0.16F);
            this.posY -= 0.10000000149011612f;
            this.posZ -= (MathHelper.sin(this.rotationYaw / 180.0F * 3.1415927F) * 0.16F);
            this.setPosition(this.posX, this.posY, this.posZ);
            this.yOffset = 0.0F;
            this.motionX = (-MathHelper.sin(this.rotationYaw / 180.0F * 3.1415927F) * MathHelper.cos(this.rotationPitch / 180.0F * 3.1415927F));
            this.motionZ = (MathHelper.cos(this.rotationYaw / 180.0F * 3.1415927F) * MathHelper.cos(this.rotationPitch / 180.0F * 3.1415927F));
            this.motionY = (-MathHelper.sin(this.rotationPitch / 180.0F * 3.1415927F));
            this.setArrowHeading(this.motionX, this.motionY, this.motionZ, 1.5F, 1.0F);
        }

        protected void entityInit()
        {
        }

        public void setArrowHeading(double var1, double var3, double var5, float var7, float var8)
        {
            float var9 = MathHelper.sqrt_double(var1 * var1 + var3 * var3 + var5 * var5);
            var1 /= (double)var9;
            var3 /= (double)var9;
            var5 /= (double)var9;
            var1 += this.rand.NextDouble() * 0.007499999832361937D * (double)var8;
            var3 += this.rand.NextDouble() * 0.007499999832361937D * (double)var8;
            var5 += this.rand.NextDouble() * 0.007499999832361937D * (double)var8;
            var1 *= (double)var7;
            var3 *= (double)var7;
            var5 *= (double)var7;
            this.motionX = (float)var1;
            this.motionY = (float)var3;
            this.motionZ = (float)var5;
            float var10 = MathHelper.sqrt_double(var1 * var1 + var5 * var5);
            this.prevRotationYaw = this.rotationYaw = (float)(Math.Atan2(var1, var5) * 180.0D / 3.1415927410125732D);
            this.prevRotationPitch = this.rotationPitch = (float)(Math.Atan2(var3, (double)var10) * 180.0D / 3.1415927410125732D);
            this.ticksInGround = 0;
        }

        public void setVelocity(double var1, double var3, double var5)
        {
            this.motionX = (float)var1;
            this.motionY = (float)var3;
            this.motionZ = (float)var5;
            if (this.prevRotationPitch == 0.0F && this.prevRotationYaw == 0.0F)
            {
                float var7 = MathHelper.sqrt_double(var1 * var1 + var5 * var5);
                this.prevRotationYaw = this.rotationYaw = (float)(Math.Atan2(var1, var5) * 180.0D / 3.1415927410125732D);
                this.prevRotationPitch = this.rotationPitch = (float)(Math.Atan2(var3, (double)var7) * 180.0D / 3.1415927410125732D);
                this.prevRotationPitch = this.rotationPitch;
                this.prevRotationYaw = this.rotationYaw;
                this.setLocationAndAngles(this.posX, this.posY, this.posZ, this.rotationYaw, this.rotationPitch);
                this.ticksInGround = 0;
            }

        }

        public new void onUpdate()
        {
            base.onUpdate();
            if (this.prevRotationPitch == 0.0F && this.prevRotationYaw == 0.0F)
            {
                float var1 = MathHelper.sqrt_double(this.motionX * this.motionX + this.motionZ * this.motionZ);
                this.prevRotationYaw = this.rotationYaw = (float)(Math.Atan2(this.motionX, this.motionZ) * 180.0D / 3.1415927410125732D);
                this.prevRotationPitch = this.rotationPitch = (float)(Math.Atan2(this.motionY, (double)var1) * 180.0D / 3.1415927410125732D);
            }

            int var15 = this.world.getBlockIDAt(this.xTile, this.yTile, this.zTile);
            if (var15 > 0)
            {
                Block.blocks[var15].setBlockBoundsBasedOnState(this.world, this.xTile, this.yTile, this.zTile);
                AxisAlignedBB var2 = Block.blocks[var15].getCollisionBoundingBoxFromPool(this.world, this.xTile, this.yTile, this.zTile);
                if (var2 != null && var2.isVecInside(Vec3D.createVector(this.posX, this.posY, this.posZ)))
                {
                    this.inGround = true;
                }
            }

            if (this.arrowShake > 0)
            {
                --this.arrowShake;
            }

            if (this.inGround)
            {
                var15 = this.world.getBlockIDAt(this.xTile, this.yTile, this.zTile);
                int var18 = this.world.getBlockMetaAt(this.xTile, this.yTile, this.zTile);
                if (var15 == this.inTile && var18 == this.inData)
                {
                    ++this.ticksInGround;
                    if (this.ticksInGround == 1200)
                    {
                        this.setEntityDead();
                    }

                }
                else
                {
                    this.inGround = false;
                    this.motionX *= (this.rand.NextSingle() * 0.2F);
                    this.motionY *= (this.rand.NextSingle() * 0.2F);
                    this.motionZ *= (this.rand.NextSingle() * 0.2F);
                    this.ticksInGround = 0;
                    this.ticksInAir = 0;
                }
            }
            else
            {
                ++this.ticksInAir;
                Vec3D var16 = Vec3D.createVector(this.posX, this.posY, this.posZ);
                Vec3D var17 = Vec3D.createVector(this.posX + this.motionX, this.posY + this.motionY, this.posZ + this.motionZ);
                MovingObjectPosition var3 = this.world.rayTraceBlocks_do_do(var16, var17, false, true);
                var16 = Vec3D.createVector(this.posX, this.posY, this.posZ);
                var17 = Vec3D.createVector(this.posX + this.motionX, this.posY + this.motionY, this.posZ + this.motionZ);
                if (var3 != null)
                {
                    var17 = Vec3D.createVector(var3.hitVec.xCoord, var3.hitVec.yCoord, var3.hitVec.zCoord);
                }

                Entity var4 = null;
                List<Entity> var5 = this.world.getEntitiesWithinAABBExcludingEntity(this, this.boundingBox.addCoord(this.motionX, this.motionY, this.motionZ).expand(1.0D, 1.0D, 1.0D));
                double var6 = 0.0D;

                float var10;
                for (int var8 = 0; var8 < var5.Count; ++var8)
                {
                    Entity var9 = var5[var8];
                    if (var9.canBeCollidedWith() && (var9 != this.owner || this.ticksInAir >= 5))
                    {
                        var10 = 0.3F;
                        AxisAlignedBB var11 = var9.boundingBox.expand((double)var10, (double)var10, (double)var10);
                        MovingObjectPosition var12 = var11.calculateIntercept(var16, var17);
                        if (var12 != null)
                        {
                            double var13 = var16.distanceTo(var12.hitVec);
                            if (var13 < var6 || var6 == 0.0D)
                            {
                                var4 = var9;
                                var6 = var13;
                            }
                        }
                    }
                }

                if (var4 != null)
                {
                    var3 = new MovingObjectPosition(var4);
                }

                float var19;
                if (var3 != null)
                {
                    if (var3.entityHit != null)
                    {
                        if (var3.entityHit.attackEntityFrom(this.owner, 4))
                        {

                            this.setEntityDead();
                        }
                        else
                        {
                            this.motionX *= -0.10000000149011612f;
                            this.motionY *= -0.10000000149011612f;
                            this.motionZ *= -0.10000000149011612f;
                            this.rotationYaw += 180.0F;
                            this.prevRotationYaw += 180.0F;
                            this.ticksInAir = 0;
                        }
                    }
                    else
                    {
                        this.xTile = var3.blockX;
                        this.yTile = var3.blockY;
                        this.zTile = var3.blockZ;
                        this.inTile = this.world.getBlockIDAt(this.xTile, this.yTile, this.zTile);
                        this.inData = this.world.getBlockMetaAt(this.xTile, this.yTile, this.zTile);
                        this.motionX = ((float)(var3.hitVec.xCoord - this.posX));
                        this.motionY = ((float)(var3.hitVec.yCoord - this.posY));
                        this.motionZ = ((float)(var3.hitVec.zCoord - this.posZ));
                        var19 = MathHelper.sqrt_double(this.motionX * this.motionX + this.motionY * this.motionY + this.motionZ * this.motionZ);
                        this.posX -= this.motionX / var19 * 0.05000000074505806f;
                        this.posY -= this.motionY / var19 * 0.05000000074505806f;
                        this.posZ -= this.motionZ / var19 * 0.05000000074505806f;

                        this.inGround = true;
                        this.arrowShake = 7;
                    }
                }

                this.posX += this.motionX;
                this.posY += this.motionY;
                this.posZ += this.motionZ;
                var19 = MathHelper.sqrt_double(this.motionX * this.motionX + this.motionZ * this.motionZ);
                this.rotationYaw = (float)(Math.Atan2(this.motionX, this.motionZ) * 180.0D / 3.1415927410125732D);

                for (this.rotationPitch = (float)(Math.Atan2(this.motionY, (double)var19) * 180.0D / 3.1415927410125732D); this.rotationPitch - this.prevRotationPitch < -180.0F; this.prevRotationPitch -= 360.0F)
                {
                }

                while (this.rotationPitch - this.prevRotationPitch >= 180.0F)
                {
                    this.prevRotationPitch += 360.0F;
                }

                while (this.rotationYaw - this.prevRotationYaw < -180.0F)
                {
                    this.prevRotationYaw -= 360.0F;
                }

                while (this.rotationYaw - this.prevRotationYaw >= 180.0F)
                {
                    this.prevRotationYaw += 360.0F;
                }

                this.rotationPitch = this.prevRotationPitch + (this.rotationPitch - this.prevRotationPitch) * 0.2F;
                this.rotationYaw = this.prevRotationYaw + (this.rotationYaw - this.prevRotationYaw) * 0.2F;
                float var20 = 0.99F;
                var10 = 0.03F;
                if (this.isInWater())
                {


                    var20 = 0.8F;
                }

                this.motionX *= var20;
                this.motionY *= var20;
                this.motionZ *= var20;
                this.motionY -= var10;
                this.setPosition(this.posX, this.posY, this.posZ);
            }
        }

        protected override void writeEntityToNBT(NbtCompound var1)
        {
            // TODO
        }

        protected override void readEntityFromNBT(NbtCompound var1)
        {
            // TODO:
        }

        public new void onCollideWithPlayer(EntityPlayer var1)
        {

                if (this.inGround && this.doesArrowBelongToPlayer && this.arrowShake <= 0 && var1.inventory.addItemStackToInventory(new ItemStack(Item.arrow, 1)))
                {
      
                    var1.onItemPickup(this, 1);
                    this.setEntityDead();
                }

            
        }

        public new float getShadowSize()
        {
            return 0.0F;
        }


    }
}