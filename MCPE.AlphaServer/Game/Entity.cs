using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.material;
using SpoongePE.Core.Game.utils;
using System;
using System.Collections.Generic;

namespace SpoongePE.Core.Game;

public abstract class Entity
{
    private static int LastEntityID = 1;

    public int EntityID;
    public Entity riddenByEntity;
    public Entity ridingEntity;
    public EntityData EntityData;
    public float prevPosX, prevPosY, prevPosZ, posX, posY, posZ, motionX, motionY, motionZ;
    public float rotationYaw, rotationPitch, prevRotationYaw, prevRotationPitch;
    public AxisAlignedBB boundingBox;

    public bool onGround, isCollidedHorizontally, isCollidedVertically, isCollided, beenAttacked, isInWeb, field_9293_aM, isDead;
    public float yOffset, ySize, width, height, prevDistanceWalkedModified, distanceWalkedModified, fallDistance, stepHeight;

    public double lastTickPosX, lastTickPosY, lastTickPosZ;
    public bool noClip;
    public float entityCollisionReduction;
    public int ticksExisted, fireResistance, fire;

    protected int maxAir;
    protected bool inWater;
    protected Random rand;
    public World world;
    public byte heartsLife;
    public int air;
    private bool isFirstUpdate;
    protected bool isImmuneToFire;
    public bool addedToChunk;
    public int chunkCoordX, chunkCoordY, chunkCoordZ, serverPosX, serverPosY, serverPosZ;
    private float nextStepDistance;

    public Entity(World w)
    {
        EntityID = LastEntityID++;

        EntityData = new EntityData();
        this.posX = 64;
        this.posY = 128;
        this.posZ = 64;
        this.rotationYaw = 0;
        this.rotationPitch = 0;
        this.heartsLife = 0;

        this.boundingBox = AxisAlignedBB.getBoundingBox(0.0D, 0.0D, 0.0D, 0.0D, 0.0D, 0.0D);
        this.onGround = false;
        this.isCollided = false;
        this.beenAttacked = false;
        this.field_9293_aM = true;
        this.isDead = false;
        this.yOffset = 0.0F;
        this.width = 0.6F;
        this.height = 1.8F;
        this.prevDistanceWalkedModified = 0.0F;
        this.distanceWalkedModified = 0.0F;
        this.fallDistance = 0.0F;
        this.nextStepDistance = 1;
        this.ySize = 0.0F;
        this.stepHeight = 0.0F;
        this.noClip = false;
        this.entityCollisionReduction = 0.0F;
        this.rand = new Random();
        this.ticksExisted = 0;
        this.fireResistance = 1;
        this.fire = 0;
        this.maxAir = 300;
        this.inWater = false;
        this.heartsLife = 0;
        this.air = 300;
        this.isFirstUpdate = true;
        this.isImmuneToFire = false;
        this.world = w;
        this.isImmuneToFire = false;
        this.addedToChunk = false;
        this.setPosition(0.0D, 0.0D, 0.0D);
        Define(EntityDataKey.Flags, EntityDataType.Byte);
        Define(EntityDataKey.Air, EntityDataType.Short);

    }
    public override bool Equals(object var1)
    {
        if (var1 is Entity)
        {
            return ((Entity)var1).EntityID == this.EntityID;
        }
        else
        {
            return false;
        }
    }
    public override int GetHashCode() => EntityID;
    public void setEntityDead()
    {
        this.isDead = true;
    }

    protected void setSize(float var1, float var2)
    {
        this.width = var1;
        this.height = var2;
    }

    protected void setRotation(float var1, float var2)
    {
        this.rotationYaw = var1 % 360.0F;
        this.rotationPitch = var2 % 360.0F;
    }
    public void setPosition(double var1, double var3, double var5)
    {
        this.posX = (float)var1;
        this.posY = (float)var3;
        this.posZ = (float)var5;
        float var7 = this.width / 2.0F;
        float var8 = this.height;
        this.boundingBox.setBounds(var1 - (double)var7, var3 - (double)this.yOffset + (double)this.ySize, var5 - (double)var7, var1 + (double)var7, var3 - (double)this.yOffset + (double)this.ySize + (double)var8, var5 + (double)var7);
    }

    public void setAngles(float var1, float var2)
    {
        float var3 = this.rotationPitch;
        float var4 = this.rotationYaw;
        this.rotationYaw = (float)((double)this.rotationYaw + (double)var1 * 0.15D);
        this.rotationPitch = (float)((double)this.rotationPitch - (double)var2 * 0.15D);
        if (this.rotationPitch < -90.0F)
        {
            this.rotationPitch = -90.0F;
        }

        if (this.rotationPitch > 90.0F)
        {
            this.rotationPitch = 90.0F;
        }

        this.prevRotationPitch += this.rotationPitch - var3;
        this.prevRotationYaw += this.rotationYaw - var4;
    }
    public void onUpdate()
    {
        this.onEntityUpdate();
    }

    public void onEntityUpdate()
    {
        if (this.ridingEntity != null && this.ridingEntity.isDead)
        {
            this.ridingEntity = null;
        }

        ++this.ticksExisted;
        this.prevDistanceWalkedModified = this.distanceWalkedModified;
        this.prevPosX = this.posX;
        this.prevPosY = this.posY;
        this.prevPosZ = this.posZ;
        this.prevRotationPitch = this.rotationPitch;
        this.prevRotationYaw = this.rotationYaw;
        if (this.handleWaterMovement())
        {

            this.fallDistance = 0.0F;
            this.inWater = true;
            this.fire = 0;
        }
        else
        {
            this.inWater = false;
        }

        if (this.fire > 0)
        {
            if (this.isImmuneToFire)
            {
                this.fire -= 4;
                if (this.fire < 0)
                {
                    this.fire = 0;
                }
            }
            else
            {
                if (this.fire % 20 == 0)
                {
                    this.attackEntityFrom((Entity)null, 1);
                }

                --this.fire;
            }
        }

        if (this.handleLavaMovement())
        {
            this.setOnFireFromLava();
        }

        if (this.posY < -64.0D)
        {
            this.kill();
        }


        this.isFirstUpdate = false;
    }
    public void moveEntity(double var1, double var3, double var5)
    {
        if (this.noClip)
        {
            this.boundingBox.offset(var1, var3, var5);
            this.posX = (float)((this.boundingBox.minX + this.boundingBox.maxX) / 2.0D);
            this.posY = (float)(this.boundingBox.minY + (double)this.yOffset - (double)this.ySize);
            this.posZ = (float)((this.boundingBox.minZ + this.boundingBox.maxZ) / 2.0D);
        }
        else
        {
            this.ySize *= 0.4F;
            double var7 = this.posX;
            double var9 = this.posZ;
            if (this.isInWeb)
            {
                this.isInWeb = false;
                var1 *= 0.25D;
                var3 *= 0.05000000074505806D;
                var5 *= 0.25D;
                this.motionX = 0.0f;
                this.motionY = 0.0f;
                this.motionZ = 0.0f;
            }

            double var11 = var1;
            double var13 = var3;
            double var15 = var5;
            AxisAlignedBB var17 = this.boundingBox.copy();
            bool var18 = this.onGround;
            if (var18)
            {
                double var19;
                for (var19 = 0.05D; var1 != 0.0D && this.world.getCollidingBoundingBoxes(this, this.boundingBox.getOffsetBoundingBox(var1, -1.0D, 0.0D)).Count == 0; var11 = var1)
                {
                    if (var1 < var19 && var1 >= -var19)
                    {
                        var1 = 0.0D;
                    }
                    else if (var1 > 0.0D)
                    {
                        var1 -= var19;
                    }
                    else
                    {
                        var1 += var19;
                    }
                }

                for (; var5 != 0.0D && this.world.getCollidingBoundingBoxes(this, this.boundingBox.getOffsetBoundingBox(0.0D, -1.0D, var5)).Count == 0; var15 = var5)
                {
                    if (var5 < var19 && var5 >= -var19)
                    {
                        var5 = 0.0D;
                    }
                    else if (var5 > 0.0D)
                    {
                        var5 -= var19;
                    }
                    else
                    {
                        var5 += var19;
                    }
                }
            }

            List<AxisAlignedBB> var35 = this.world.getCollidingBoundingBoxes(this, this.boundingBox.addCoord(var1, var3, var5));

            for (int var20 = 0; var20 < var35.Count; ++var20)
            {
                var3 = ((AxisAlignedBB)var35[var20]).calculateYOffset(this.boundingBox, var3);
            }

            this.boundingBox.offset(0.0D, var3, 0.0D);
            if (!this.field_9293_aM && var13 != var3)
            {
                var5 = 0.0D;
                var3 = 0.0D;
                var1 = 0.0D;
            }

            bool var36 = this.onGround || var13 != var3 && var13 < 0.0D;

            int var21;
            for (var21 = 0; var21 < var35.Count; ++var21)
            {
                var1 = ((AxisAlignedBB)var35[var21]).calculateXOffset(this.boundingBox, var1);
            }

            this.boundingBox.offset(var1, 0.0D, 0.0D);
            if (!this.field_9293_aM && var11 != var1)
            {
                var5 = 0.0D;
                var3 = 0.0D;
                var1 = 0.0D;
            }

            for (var21 = 0; var21 < var35.Count; ++var21)
            {
                var5 = ((AxisAlignedBB)var35[var21]).calculateZOffset(this.boundingBox, var5);
            }

            this.boundingBox.offset(0.0D, 0.0D, var5);
            if (!this.field_9293_aM && var15 != var5)
            {
                var5 = 0.0D;
                var3 = 0.0D;
                var1 = 0.0D;
            }

            double var23;
            int var28;
            double var37;
            if (this.stepHeight > 0.0F && var36 && (var18 || this.ySize < 0.05F) && (var11 != var1 || var15 != var5))
            {
                var37 = var1;
                var23 = var3;
                double var25 = var5;
                var1 = var11;
                var3 = (double)this.stepHeight;
                var5 = var15;
                AxisAlignedBB var27 = this.boundingBox.copy();
                this.boundingBox.setBB(var17);
                var35 = this.world.getCollidingBoundingBoxes(this, this.boundingBox.addCoord(var11, var3, var15));

                for (var28 = 0; var28 < var35.Count; ++var28)
                {
                    var3 = ((AxisAlignedBB)var35[var28]).calculateYOffset(this.boundingBox, var3);
                }

                this.boundingBox.offset(0.0D, var3, 0.0D);
                if (!this.field_9293_aM && var13 != var3)
                {
                    var5 = 0.0D;
                    var3 = 0.0D;
                    var1 = 0.0D;
                }

                for (var28 = 0; var28 < var35.Count; ++var28)
                {
                    var1 = ((AxisAlignedBB)var35[var28]).calculateXOffset(this.boundingBox, var1);
                }

                this.boundingBox.offset(var1, 0.0D, 0.0D);
                if (!this.field_9293_aM && var11 != var1)
                {
                    var5 = 0.0D;
                    var3 = 0.0D;
                    var1 = 0.0D;
                }

                for (var28 = 0; var28 < var35.Count; ++var28)
                {
                    var5 = ((AxisAlignedBB)var35[var28]).calculateZOffset(this.boundingBox, var5);
                }

                this.boundingBox.offset(0.0D, 0.0D, var5);
                if (!this.field_9293_aM && var15 != var5)
                {
                    var5 = 0.0D;
                    var3 = 0.0D;
                    var1 = 0.0D;
                }

                if (!this.field_9293_aM && var13 != var3)
                {
                    var5 = 0.0D;
                    var3 = 0.0D;
                    var1 = 0.0D;
                }
                else
                {
                    var3 = (double)(-this.stepHeight);

                    for (var28 = 0; var28 < var35.Count; ++var28)
                    {
                        var3 = ((AxisAlignedBB)var35[var28]).calculateYOffset(this.boundingBox, var3);
                    }

                    this.boundingBox.offset(0.0D, var3, 0.0D);
                }

                if (var37 * var37 + var25 * var25 >= var1 * var1 + var5 * var5)
                {
                    var1 = var37;
                    var3 = var23;
                    var5 = var25;
                    this.boundingBox.setBB(var27);
                }
                else
                {
                    double var41 = this.boundingBox.minY - (double)((int)this.boundingBox.minY);
                    if (var41 > 0.0D)
                    {
                        this.ySize = (float)((double)this.ySize + var41 + 0.01D);
                    }
                }
            }

            this.posX = (float)((this.boundingBox.minX + this.boundingBox.maxX) / 2.0D);
            this.posY = (float)(this.boundingBox.minY + (double)this.yOffset - (double)this.ySize);
            this.posZ = (float)((this.boundingBox.minZ + this.boundingBox.maxZ) / 2.0D);
            this.isCollidedHorizontally = var11 != var1 || var15 != var5;
            this.isCollidedVertically = var13 != var3;
            this.onGround = var13 != var3 && var13 < 0.0D;
            this.isCollided = this.isCollidedHorizontally || this.isCollidedVertically;
            this.updateFallState(var3, this.onGround);
            if (var11 != var1)
            {
                this.motionX = 0.0f;
            }

            if (var13 != var3)
            {
                this.motionY = 0.0f;
            }

            if (var15 != var5)
            {
                this.motionZ = 0.0f;
            }

            var37 = this.posX - var7;
            var23 = this.posZ - var9;
            int var26;
            int var38;
            int var39;
            if (this.canTriggerWalking() && !var18 && this.ridingEntity == null)
            {
                this.distanceWalkedModified = (float)((double)this.distanceWalkedModified + (double)MathHelper.sqrt_double(var37 * var37 + var23 * var23) * 0.6D);
                var38 = MathHelper.floor_double(this.posX);
                var26 = MathHelper.floor_double(this.posY - 0.20000000298023224D - (double)this.yOffset);
                var39 = MathHelper.floor_double(this.posZ);
                var28 = this.world.getBlockIDAt(var38, var26, var39);
                if (this.world.getBlockIDAt(var38, var26 - 1, var39) == Block.fence.blockID)
                {
                    var28 = this.world.getBlockIDAt(var38, var26 - 1, var39);
                }

                if (this.distanceWalkedModified > (float)this.nextStepDistance && var28 > 0)
                {
                    ++this.nextStepDistance;

                    Block.blocks[var28].onEntityWalking(this.world, var38, var26, var39, this);
                }
            }

            var38 = MathHelper.floor_double(this.boundingBox.minX + 0.001D);
            var26 = MathHelper.floor_double(this.boundingBox.minY + 0.001D);
            var39 = MathHelper.floor_double(this.boundingBox.minZ + 0.001D);
            var28 = MathHelper.floor_double(this.boundingBox.maxX - 0.001D);
            int var40 = MathHelper.floor_double(this.boundingBox.maxY - 0.001D);
            int var30 = MathHelper.floor_double(this.boundingBox.maxZ - 0.001D);
            if (this.world.checkChunksExist(var38, var26, var39, var28, var40, var30))
            {
                for (int var31 = var38; var31 <= var28; ++var31)
                {
                    for (int var32 = var26; var32 <= var40; ++var32)
                    {
                        for (int var33 = var39; var33 <= var30; ++var33)
                        {
                            int var34 = this.world.getBlockMetaAt(var31, var32, var33);
                            if (var34 > 0)
                            {
                                Block.blocks[var34].onEntityCollidedWithBlock(this.world, var31, var32, var33, this);
                            }
                        }
                    }
                }
            }

            bool var42 = this.isWet();
            if (this.world.isBoundingBoxBurning(this.boundingBox.contract(0.001D, 0.001D, 0.001D)))
            {
                this.dealFireDamage(1);
                if (!var42)
                {
                    ++this.fire;
                    if (this.fire == 0)
                    {
                        this.fire = 300;
                    }
                }
            }
            else if (this.fire <= 0)
            {
                this.fire = -this.fireResistance;
            }

            if (var42 && this.fire > 0)
            {

                this.fire = -this.fireResistance;
            }

        }
    }

    protected void updateFallState(double var1, bool var3)
    {
        if (var3)
        {
            if (this.fallDistance > 0.0F)
            {
                this.fall(this.fallDistance);
                this.fallDistance = 0.0F;
            }
        }
        else if (var1 < 0.0D)
        {
            this.fallDistance = (float)((double)this.fallDistance - var1);
        }

    }
    protected void fall(float var1)
    {
        if (this.riddenByEntity != null)
        {
            this.riddenByEntity.fall(var1);
        }
    }
    protected void setBeenAttacked()
    {
        this.beenAttacked = true;
    }
    public bool attackEntityFrom(Entity var1, int var2)
    {
        this.setBeenAttacked();
        return false;
    }
    protected void setOnFireFromLava()
    {
        if (!this.isImmuneToFire)
        {
            this.attackEntityFrom((Entity)null, 4);
            this.fire = 600;
        }

    }
    protected void kill()
    {
        this.setEntityDead();
    }
    public bool handleWaterMovement() => this.world.handleMaterialAcceleration(this.boundingBox.expand(0.0D, -0.4000000059604645D, 0.0D).contract(0.001D, 0.001D, 0.001D), Material.water, this);
    public bool handleLavaMovement() => this.world.isMaterialInBB(this.boundingBox.expand(-0.10000000149011612D, -0.4000000059604645D, -0.10000000149011612D), Material.lava);
    public bool isWet()
    {
        return this.inWater;
    }

    public bool isInWater()
    {
        return this.inWater;
    }
    protected void dealFireDamage(int var1)
    {
        if (!this.isImmuneToFire)
        {
            this.attackEntityFrom((Entity)null, var1);
        }

    }
    protected bool canTriggerWalking() => true;


    public void Define(EntityDataKey id, EntityDataType dataType) => EntityData.Define(id, dataType);
    public void Set(EntityDataKey id, object value) => EntityData.Set(id, value);
    public T Get<T>(EntityDataKey id) => EntityData.Get<T>(id);

    public AxisAlignedBB getBoundingBox() => null;

    internal AxisAlignedBB getCollisionBox(Entity entity) => null;
}
