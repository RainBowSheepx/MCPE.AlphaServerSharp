using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.BlockBase.impl;
using SpoongePE.Core.Game.entity.impl;
using SpoongePE.Core.Game.ItemBase;
using SpoongePE.Core.Game.material;
using SpoongePE.Core.Game.player;
using SpoongePE.Core.Game.utils;
using SpoongePE.Core.NBT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpoongePE.Core.Game.entity;

public abstract class Entity
{
    private static int LastEntityID = 1;

    public int EntityID;
    public bool preventEntitySpawning;
    public Entity riddenByEntity; // not used
    public Entity ridingEntity; // not used
    public EntityData EntityData;
    public float prevPosX, prevPosY, prevPosZ, posX, posY, posZ, motionX, motionY, motionZ;
    public float rotationYaw, rotationPitch, prevRotationYaw, prevRotationPitch;
    public AxisAlignedBB boundingBox;

    public bool onGround, isCollidedHorizontally, isCollidedVertically, isCollided, beenAttacked, isInWeb, field_9293_aM, isDead;
    public float yOffset, ySize, width, height, prevDistanceWalkedModified, distanceWalkedModified, fallDistance, stepHeight;

    public float lastTickPosX, lastTickPosY, lastTickPosZ;
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
    public float entityBrightness;
    public Entity(World w)
    {
        EntityID = LastEntityID++;

        EntityData = new EntityData();
        posX = 64;
        posY = 128;
        posZ = 64;
        rotationYaw = 0;
        rotationPitch = 0;
        heartsLife = 0;
        preventEntitySpawning = false;
        boundingBox = AxisAlignedBB.getBoundingBox(0.0D, 0.0D, 0.0D, 0.0D, 0.0D, 0.0D);
        onGround = false;
        isCollided = false;
        beenAttacked = false;
        field_9293_aM = true;
        isDead = false;
        yOffset = 0.0F;
        width = 0.6F;
        height = 1.8F;
        prevDistanceWalkedModified = 0.0F;
        distanceWalkedModified = 0.0F;
        fallDistance = 0.0F;
        nextStepDistance = 1;
        ySize = 0.0F;
        stepHeight = 0.0F;
        noClip = false;
        entityCollisionReduction = 0.0F;
        rand = new Random();
        ticksExisted = 0;
        fireResistance = 1;
        fire = 0;
        maxAir = 300;
        inWater = false;
        heartsLife = 0;
        air = 300;
        isFirstUpdate = true;
        isImmuneToFire = false;
        world = w;
        isImmuneToFire = false;
        addedToChunk = false;
        entityBrightness = 0.0F;
        setPosition(0.0D, 0.0D, 0.0D);
        Define(EntityDataKey.Flags, EntityDataType.Byte);
        Define(EntityDataKey.Air, EntityDataType.Short);

    }
    public override bool Equals(object var1)
    {
        if (var1 is Entity)
        {
            return ((Entity)var1).EntityID == EntityID;
        }
        else
        {
            return false;
        }
    }
    public override int GetHashCode() => EntityID;
    public void setEntityDead()
    {
        isDead = true;
    }

    protected void setSize(float var1, float var2)
    {
        width = var1;
        height = var2;
    }

    protected void setRotation(float var1, float var2)
    {
        rotationYaw = var1 % 360.0F;
        rotationPitch = var2 % 360.0F;
    }
    public void setPosition(double var1, double var3, double var5)
    {
        posX = (float)var1;
        posY = (float)var3;
        posZ = (float)var5;
        float var7 = width / 2.0F;
        float var8 = height;
        boundingBox.setBounds(var1 - (double)var7, var3 - yOffset + ySize, var5 - (double)var7, var1 + (double)var7, var3 - yOffset + ySize + (double)var8, var5 + (double)var7);
    }

    public void setAngles(float var1, float var2)
    {
        float var3 = rotationPitch;
        float var4 = rotationYaw;
        rotationYaw = (float)(rotationYaw + (double)var1 * 0.15D);
        rotationPitch = (float)(rotationPitch - (double)var2 * 0.15D);
        if (rotationPitch < -90.0F)
        {
            rotationPitch = -90.0F;
        }

        if (rotationPitch > 90.0F)
        {
            rotationPitch = 90.0F;
        }

        prevRotationPitch += rotationPitch - var3;
        prevRotationYaw += rotationYaw - var4;
    }
    public void onUpdate()
    {
        onEntityUpdate();
    }

    public void onEntityUpdate()
    {
        if (ridingEntity != null && ridingEntity.isDead)
        {
            ridingEntity = null;
        }

        ++ticksExisted;
        prevDistanceWalkedModified = distanceWalkedModified;
        prevPosX = posX;
        prevPosY = posY;
        prevPosZ = posZ;
        prevRotationPitch = rotationPitch;
        prevRotationYaw = rotationYaw;
        if (handleWaterMovement())
        {

            fallDistance = 0.0F;
            inWater = true;
            fire = 0;
        }
        else
        {
            inWater = false;
        }

        if (fire > 0)
        {
            if (isImmuneToFire)
            {
                fire -= 4;
                if (fire < 0)
                {
                    fire = 0;
                }
            }
            else
            {
                if (fire % 20 == 0)
                {
                    attackEntityFrom(null, 1);
                }

                --fire;
            }
        }

        if (handleLavaMovement())
        {
            setOnFireFromLava();
        }

        if (posY < -64.0D)
        {
            kill();
        }


        isFirstUpdate = false;
    }
    public void moveEntity(double var1, double var3, double var5)
    {
        if (noClip)
        {
            boundingBox.offset(var1, var3, var5);
            posX = (float)((boundingBox.minX + boundingBox.maxX) / 2.0D);
            posY = (float)(boundingBox.minY + yOffset - ySize);
            posZ = (float)((boundingBox.minZ + boundingBox.maxZ) / 2.0D);
        }
        else
        {
            ySize *= 0.4F;
            double var7 = posX;
            double var9 = posZ;
            if (isInWeb)
            {
                isInWeb = false;
                var1 *= 0.25D;
                var3 *= 0.05000000074505806D;
                var5 *= 0.25D;
                motionX = 0.0f;
                motionY = 0.0f;
                motionZ = 0.0f;
            }

            double var11 = var1;
            double var13 = var3;
            double var15 = var5;
            AxisAlignedBB var17 = boundingBox.copy();
            bool var18 = onGround;
            if (var18)
            {
                double var19;
                for (var19 = 0.05D; var1 != 0.0D && world.getCollidingBoundingBoxes(this, boundingBox.getOffsetBoundingBox(var1, -1.0D, 0.0D)).Count == 0; var11 = var1)
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

                for (; var5 != 0.0D && world.getCollidingBoundingBoxes(this, boundingBox.getOffsetBoundingBox(0.0D, -1.0D, var5)).Count == 0; var15 = var5)
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

            List<AxisAlignedBB> var35 = world.getCollidingBoundingBoxes(this, boundingBox.addCoord(var1, var3, var5));

            for (int var20 = 0; var20 < var35.Count; ++var20)
            {
                var3 = var35[var20].calculateYOffset(boundingBox, var3);
            }

            boundingBox.offset(0.0D, var3, 0.0D);
            if (!field_9293_aM && var13 != var3)
            {
                var5 = 0.0D;
                var3 = 0.0D;
                var1 = 0.0D;
            }

            bool var36 = onGround || var13 != var3 && var13 < 0.0D;

            int var21;
            for (var21 = 0; var21 < var35.Count; ++var21)
            {
                var1 = var35[var21].calculateXOffset(boundingBox, var1);
            }

            boundingBox.offset(var1, 0.0D, 0.0D);
            if (!field_9293_aM && var11 != var1)
            {
                var5 = 0.0D;
                var3 = 0.0D;
                var1 = 0.0D;
            }

            for (var21 = 0; var21 < var35.Count; ++var21)
            {
                var5 = var35[var21].calculateZOffset(boundingBox, var5);
            }

            boundingBox.offset(0.0D, 0.0D, var5);
            if (!field_9293_aM && var15 != var5)
            {
                var5 = 0.0D;
                var3 = 0.0D;
                var1 = 0.0D;
            }

            double var23;
            int var28;
            double var37;
            if (stepHeight > 0.0F && var36 && (var18 || ySize < 0.05F) && (var11 != var1 || var15 != var5))
            {
                var37 = var1;
                var23 = var3;
                double var25 = var5;
                var1 = var11;
                var3 = stepHeight;
                var5 = var15;
                AxisAlignedBB var27 = boundingBox.copy();
                boundingBox.setBB(var17);
                var35 = world.getCollidingBoundingBoxes(this, boundingBox.addCoord(var11, var3, var15));

                for (var28 = 0; var28 < var35.Count; ++var28)
                {
                    var3 = var35[var28].calculateYOffset(boundingBox, var3);
                }

                boundingBox.offset(0.0D, var3, 0.0D);
                if (!field_9293_aM && var13 != var3)
                {
                    var5 = 0.0D;
                    var3 = 0.0D;
                    var1 = 0.0D;
                }

                for (var28 = 0; var28 < var35.Count; ++var28)
                {
                    var1 = var35[var28].calculateXOffset(boundingBox, var1);
                }

                boundingBox.offset(var1, 0.0D, 0.0D);
                if (!field_9293_aM && var11 != var1)
                {
                    var5 = 0.0D;
                    var3 = 0.0D;
                    var1 = 0.0D;
                }

                for (var28 = 0; var28 < var35.Count; ++var28)
                {
                    var5 = var35[var28].calculateZOffset(boundingBox, var5);
                }

                boundingBox.offset(0.0D, 0.0D, var5);
                if (!field_9293_aM && var15 != var5)
                {
                    var5 = 0.0D;
                    var3 = 0.0D;
                    var1 = 0.0D;
                }

                if (!field_9293_aM && var13 != var3)
                {
                    var5 = 0.0D;
                    var3 = 0.0D;
                    var1 = 0.0D;
                }
                else
                {
                    var3 = (double)-stepHeight;

                    for (var28 = 0; var28 < var35.Count; ++var28)
                    {
                        var3 = var35[var28].calculateYOffset(boundingBox, var3);
                    }

                    boundingBox.offset(0.0D, var3, 0.0D);
                }

                if (var37 * var37 + var25 * var25 >= var1 * var1 + var5 * var5)
                {
                    var1 = var37;
                    var3 = var23;
                    var5 = var25;
                    boundingBox.setBB(var27);
                }
                else
                {
                    double var41 = boundingBox.minY - (int)boundingBox.minY;
                    if (var41 > 0.0D)
                    {
                        ySize = (float)(ySize + var41 + 0.01D);
                    }
                }
            }

            posX = (float)((boundingBox.minX + boundingBox.maxX) / 2.0D);
            posY = (float)(boundingBox.minY + yOffset - ySize);
            posZ = (float)((boundingBox.minZ + boundingBox.maxZ) / 2.0D);
            isCollidedHorizontally = var11 != var1 || var15 != var5;
            isCollidedVertically = var13 != var3;
            onGround = var13 != var3 && var13 < 0.0D;
            isCollided = isCollidedHorizontally || isCollidedVertically;
            updateFallState(var3, onGround);
            if (var11 != var1)
            {
                motionX = 0.0f;
            }

            if (var13 != var3)
            {
                motionY = 0.0f;
            }

            if (var15 != var5)
            {
                motionZ = 0.0f;
            }

            var37 = posX - var7;
            var23 = posZ - var9;
            int var26;
            int var38;
            int var39;
            if (canTriggerWalking() && !var18 && ridingEntity == null)
            {
                distanceWalkedModified = (float)(distanceWalkedModified + (double)MathHelper.sqrt_double(var37 * var37 + var23 * var23) * 0.6D);
                var38 = MathHelper.floor_double(posX);
                var26 = MathHelper.floor_double(posY - 0.20000000298023224D - yOffset);
                var39 = MathHelper.floor_double(posZ);
                var28 = world.getBlockIDAt(var38, var26, var39);
                if (world.getBlockIDAt(var38, var26 - 1, var39) == Block.fence.blockID)
                {
                    var28 = world.getBlockIDAt(var38, var26 - 1, var39);
                }

                if (distanceWalkedModified > nextStepDistance && var28 > 0)
                {
                    ++nextStepDistance;

                    Block.blocks[var28].onEntityWalking(world, var38, var26, var39, this);
                }
            }

            var38 = MathHelper.floor_double(boundingBox.minX + 0.001D);
            var26 = MathHelper.floor_double(boundingBox.minY + 0.001D);
            var39 = MathHelper.floor_double(boundingBox.minZ + 0.001D);
            var28 = MathHelper.floor_double(boundingBox.maxX - 0.001D);
            int var40 = MathHelper.floor_double(boundingBox.maxY - 0.001D);
            int var30 = MathHelper.floor_double(boundingBox.maxZ - 0.001D);
            if (world.checkChunksExist(var38, var26, var39, var28, var40, var30))
            {
                for (int var31 = var38; var31 <= var28; ++var31)
                {
                    for (int var32 = var26; var32 <= var40; ++var32)
                    {
                        for (int var33 = var39; var33 <= var30; ++var33)
                        {
                            int var34 = world.getBlockMetaAt(var31, var32, var33);
                            if (var34 > 0)
                            {
                                Block.blocks[var34].onEntityCollidedWithBlock(world, var31, var32, var33, this);
                            }
                        }
                    }
                }
            }

            bool var42 = isWet();
            if (world.isBoundingBoxBurning(boundingBox.contract(0.001D, 0.001D, 0.001D)))
            {
                dealFireDamage(1);
                if (!var42)
                {
                    ++fire;
                    if (fire == 0)
                    {
                        fire = 300;
                    }
                }
            }
            else if (fire <= 0)
            {
                fire = -fireResistance;
            }

            if (var42 && fire > 0)
            {

                fire = -fireResistance;
            }

        }
    }

    protected void updateFallState(double var1, bool var3)
    {
        if (var3)
        {
            if (fallDistance > 0.0F)
            {
                fall(fallDistance);
                fallDistance = 0.0F;
            }
        }
        else if (var1 < 0.0D)
        {
            fallDistance = (float)(fallDistance - var1);
        }

    }
    protected void fall(float var1)
    {
        if (riddenByEntity != null)
        {
            riddenByEntity.fall(var1);
        }
    }
    protected void setBeenAttacked()
    {
        beenAttacked = true;
    }
    public bool attackEntityFrom(Entity var1, int var2)
    {
        setBeenAttacked();
        return false;
    }
    protected void setOnFireFromLava()
    {
        if (!isImmuneToFire)
        {
            attackEntityFrom(null, 4);
            fire = 600;
        }

    }
    protected void kill()
    {
        setEntityDead();
    }
    public bool handleWaterMovement() => world.handleMaterialAcceleration(boundingBox.expand(0.0D, -0.4000000059604645D, 0.0D).contract(0.001D, 0.001D, 0.001D), Material.water, this);
    public bool handleLavaMovement() => world.isMaterialInBB(boundingBox.expand(-0.10000000149011612D, -0.4000000059604645D, -0.10000000149011612D), Material.lava);
    public bool isWet()
    {
        return inWater;
    }

    public bool isInWater()
    {
        return inWater;
    }
    protected void dealFireDamage(int var1)
    {
        if (!isImmuneToFire)
        {
            attackEntityFrom(null, var1);
        }

    }
    protected bool canTriggerWalking() => true;
    public bool isInsideOfMaterial(Material var1)
    {
        double var2 = posY + (double)getEyeHeight();
        int var4 = MathHelper.floor_double(posX);
        int var5 = MathHelper.floor_float(MathHelper.floor_double(var2));
        int var6 = MathHelper.floor_double(posZ);
        int var7 = world.getBlockIDAt(var4, var5, var6);
        if (var7 != 0 && Block.blocks[var7].material == var1)
        {
            float var8 = LiquidBaseBlock.getPercentAir(world.getBlockMetaAt(var4, var5, var6)) - 0.11111111F;
            float var9 = var5 + 1 - var8;
            return var2 < (double)var9;
        }
        else
        {
            return false;
        }
    }
    public void moveFlying(float var1, float var2, float var3)
    {
        float var4 = MathHelper.sqrt_float(var1 * var1 + var2 * var2);
        if (var4 >= 0.01F)
        {
            if (var4 < 1.0F)
            {
                var4 = 1.0F;
            }

            var4 = var3 / var4;
            var1 *= var4;
            var2 *= var4;
            float var5 = MathHelper.sin(rotationYaw * 3.1415927F / 180.0F);
            float var6 = MathHelper.cos(rotationYaw * 3.1415927F / 180.0F);
            motionX += var1 * var6 - var2 * var5;
            motionZ += var2 * var6 + var1 * var5;
        }
    }
    public void setPositionAndRotation(double var1, double var3, double var5, float var7, float var8)
    {
        prevPosX = posX = (float)var1;
        prevPosY = posY = (float)var3;
        prevPosZ = posZ = (float)var5;
        prevRotationYaw = rotationYaw = var7;
        prevRotationPitch = rotationPitch = var8;
        ySize = 0.0F;
        double var9 = (double)(prevRotationYaw - var7);
        if (var9 < -180.0D)
        {
            prevRotationYaw += 360.0F;
        }

        if (var9 >= 180.0D)
        {
            prevRotationYaw -= 360.0F;
        }

        setPosition(posX, posY, posZ);
        setRotation(var7, var8);
    }
    public float getDistanceToEntity(Entity var1)
    {
        float var2 = (float)(posX - var1.posX);
        float var3 = (float)(posY - var1.posY);
        float var4 = (float)(posZ - var1.posZ);
        return MathHelper.sqrt_float(var2 * var2 + var3 * var3 + var4 * var4);
    }
    public double getDistanceSq(double var1, double var3, double var5)
    {
        double var7 = posX - var1;
        double var9 = posY - var3;
        double var11 = posZ - var5;
        return var7 * var7 + var9 * var9 + var11 * var11;
    }

    public double getDistance(double var1, double var3, double var5)
    {
        double var7 = posX - var1;
        double var9 = posY - var3;
        double var11 = posZ - var5;
        return (double)MathHelper.sqrt_double(var7 * var7 + var9 * var9 + var11 * var11);
    }

    public double getDistanceSqToEntity(Entity var1)
    {
        double var2 = posX - var1.posX;
        double var4 = posY - var1.posY;
        double var6 = posZ - var1.posZ;
        return var2 * var2 + var4 * var4 + var6 * var6;
    }

    public void addVelocity(double var1, double var3, double var5)
    {
        motionX += (float)var1;
        motionY += (float)var3;
        motionZ += (float)var5;
    }


    public bool canBeCollidedWith()
    {
        return false;
    }

    public bool canBePushed()
    {
        return false;
    }


    public bool addEntityID(NbtCompound var1)
    {
        string var2 = getEntityString();
        if (!isDead && var2 != null)
        {
            var1.Add(new NbtString("id", var2));
            writeToNBT(var1);
            return true;
        }
        else
        {
            return false;
        }
    }
    public void writeToNBT(NbtCompound var1)
    {
        var1.Add(this.newFloatNBTList("Pos", this.posX, this.posY + this.ySize, this.posZ));
        var1.Add(this.newFloatNBTList("Motion", this.motionX, this.motionY, this.motionZ));
        var1.Add(this.newFloatNBTList("Rotation", this.rotationYaw, this.rotationPitch));
        var1.Add(new NbtFloat("FallDistance", this.fallDistance));
        var1.Add(new NbtShort("Fire", (short)this.fire));
        var1.Add(new NbtShort("Air", (short)this.air));
        var1.Add(new NbtByte("OnGround", this.onGround == true ? (byte)1 : (byte)0));
        this.writeEntityToNBT(var1);
    }

    public void readFromNBT(NbtCompound var1)
    {
        NbtList var2 = (NbtList)var1["Pos"];
        NbtList var3 = (NbtList)var1["Motion"];
        NbtList var4 = (NbtList)var1["Rotation"];
        motionX = var3.Get<NbtFloat>(0).FloatValue;
        motionY = var3.Get<NbtFloat>(1).FloatValue;
        motionZ = var3.Get<NbtFloat>(2).FloatValue;

        if (Math.Abs(motionX) > 10.0)
            motionX = 0.0f;

        if (Math.Abs(motionY) > 10.0)
            motionY = 0.0f;

        if (Math.Abs(motionZ) > 10.0)
            motionZ = 0.0f;

        prevPosX = lastTickPosX = posX = var2.Get<NbtFloat>(0).FloatValue;
        prevPosY = lastTickPosY = posY = var2.Get<NbtFloat>(1).FloatValue;
        prevPosZ = lastTickPosZ = posZ = var2.Get<NbtFloat>(2).FloatValue;

        prevRotationYaw = rotationYaw = var4.Get<NbtFloat>(0).FloatValue;
        prevRotationPitch = rotationPitch = var4.Get<NbtFloat>(1).FloatValue;

        fallDistance = var1.Get<NbtFloat>("FallDistance").FloatValue;
        fire = var1.Get<NbtShort>("Fire").ShortValue;
        air = var1.Get<NbtShort>("Air").ShortValue;
        onGround = var1.Get<NbtByte>("OnGround").ByteValue == 1 ? true : false;

        setPosition(posX, posY, posZ);
        setRotation(rotationYaw, rotationPitch);

        readEntityFromNBT(var1);
    }

    protected NbtList newDoubleNBTList(string tagName, params double[] var1)
    {
        NbtList var2 = new NbtList(tagName);

        foreach (double var6 in var1)
        {
            var2.Add(new NbtDouble(var6));
        }

        return var2;
    }

    protected NbtList newFloatNBTList(string tagName, params float[] var1)
    {
        NbtList var2 = new NbtList();

        foreach (float var in var1)
        {
            var2.Add(new NbtFloat(var));
        }

        return var2;
    }
    public EntityItem dropItem(int var1, int var2) => dropItemWithOffset(var1, var2, 0.0F);

    public EntityItem dropItemWithOffset(int var1, int var2, float var3) => entityDropItem(new ItemStack(var1, (byte)var2, 0), var3);


    public EntityItem entityDropItem(ItemStack var1, float var2)
    {
        EntityItem var3 = new EntityItem(world, posX, posY + (double)var2, posZ, var1);
        var3.delayBeforeCanPickup = 10;
        world.entityJoinedWorld(var3);
        return var3;
    }

    public string getEntityString() => EntityList.GetEntityString(this);


    public bool isEntityAlive() => !isDead;


    public bool isEntityInsideOpaqueBlock()
    {
        for (int var1 = 0; var1 < 8; ++var1)
        {
            float var2 = ((var1 >> 0) % 2 - 0.5F) * width * 0.9F;
            float var3 = ((var1 >> 1) % 2 - 0.5F) * 0.1F;
            float var4 = ((var1 >> 2) % 2 - 0.5F) * width * 0.9F;
            int var5 = MathHelper.floor_double(posX + (double)var2);
            int var6 = MathHelper.floor_double(posY + (double)getEyeHeight() + (double)var3);
            int var7 = MathHelper.floor_double(posZ + (double)var4);
            if (world.isBlockNormalCube(var5, var6, var7))
            {
                return true;
            }
        }

        return false;
    }




    public double getYOffset() => yOffset;

    public void setPositionAndRotation2(double var1, double var3, double var5, float var7, float var8, int var9)
    {
        setPosition(var1, var3, var5);
        setRotation(var7, var8);
        List<AxisAlignedBB> var10 = world.getCollidingBoundingBoxes(this, boundingBox.contract(0.03125D, 0.0D, 0.03125D));
        if (var10.Count > 0)
        {
            double var11 = 0.0D;

            for (int var13 = 0; var13 < var10.Count; ++var13)
            {
                AxisAlignedBB var14 = var10[var13];
                if (var14.maxY > var11)
                {
                    var11 = var14.maxY;
                }
            }

            var3 += var11 - boundingBox.minY;
            setPosition(var1, var3, var5);
        }

    }

    public float getCollisionBorderSize() => 0.1F;


    public Vec3D getLookVec() => null;

    public void setVelocity(float var1, float var3, float var5)
    {
        motionX = var1;
        motionY = var3;
        motionZ = var5;
    }
    public bool isBurning() => fire > 0;  //|| this.getEntityFlag(0);

    protected abstract void readEntityFromNBT(NbtCompound var1);

    protected abstract void writeEntityToNBT(NbtCompound var1);

    public float getShadowSize() => height / 2.0F;

    public float getEyeHeight() => 0.0F;

    protected bool pushOutOfBlocks(double var1, double var3, double var5)
    {
        int var7 = MathHelper.floor_double(var1);
        int var8 = MathHelper.floor_double(var3);
        int var9 = MathHelper.floor_double(var5);
        double var10 = var1 - var7;
        double var12 = var3 - var8;
        double var14 = var5 - var9;
        if (world.isBlockNormalCube(var7, var8, var9))
        {
            bool var16 = !world.isBlockNormalCube(var7 - 1, var8, var9);
            bool var17 = !world.isBlockNormalCube(var7 + 1, var8, var9);
            bool var18 = !world.isBlockNormalCube(var7, var8 - 1, var9);
            bool var19 = !world.isBlockNormalCube(var7, var8 + 1, var9);
            bool var20 = !world.isBlockNormalCube(var7, var8, var9 - 1);
            bool var21 = !world.isBlockNormalCube(var7, var8, var9 + 1);
            int var22 = -1;
            double var23 = 9999.0D;
            if (var16 && var10 < var23)
            {
                var23 = var10;
                var22 = 0;
            }

            if (var17 && 1.0D - var10 < var23)
            {
                var23 = 1.0D - var10;
                var22 = 1;
            }

            if (var18 && var12 < var23)
            {
                var23 = var12;
                var22 = 2;
            }

            if (var19 && 1.0D - var12 < var23)
            {
                var23 = 1.0D - var12;
                var22 = 3;
            }

            if (var20 && var14 < var23)
            {
                var23 = var14;
                var22 = 4;
            }

            if (var21 && 1.0D - var14 < var23)
            {
                var23 = 1.0D - var14;
                var22 = 5;
            }

            float var25 = rand.NextSingle() * 0.2F + 0.1F;
            if (var22 == 0)
            {
                motionX = -var25;
            }

            if (var22 == 1)
            {
                motionX = var25;
            }

            if (var22 == 2)
            {
                motionY = -var25;
            }

            if (var22 == 3)
            {
                motionY = var25;
            }

            if (var22 == 4)
            {
                motionZ = -var25;
            }

            if (var22 == 5)
            {
                motionZ = var25;
            }
        }

        return false;
    }

    public void Define(EntityDataKey id, EntityDataType dataType) => EntityData.Define(id, dataType);
    public void Set(EntityDataKey id, object value) => EntityData.Set(id, value);
    public T Get<T>(EntityDataKey id) => EntityData.Get<T>(id);

    public AxisAlignedBB getBoundingBox() => null;

    internal AxisAlignedBB getCollisionBox(Entity entity) => null;

    public void onKillEntity(EntityLiving entityLiving)
    {

    }

    internal void applyEntityCollision(Entity var1)
    {
        if (var1.riddenByEntity != this && var1.ridingEntity != this)
        {
            double var2 = var1.posX - this.posX;
            double var4 = var1.posZ - this.posZ;
            double var6 = MathHelper.abs_max(var2, var4);
            if (var6 >= 0.009999999776482582D)
            {
                var6 = (double)MathHelper.sqrt_double(var6);
                var2 /= var6;
                var4 /= var6;
                double var8 = 1.0D / var6;
                if (var8 > 1.0D)
                {
                    var8 = 1.0D;
                }

                var2 *= var8;
                var4 *= var8;
                var2 *= 0.05000000074505806D;
                var4 *= 0.05000000074505806D;
                var2 *= (double)(1.0F - this.entityCollisionReduction);
                var4 *= (double)(1.0F - this.entityCollisionReduction);
                this.addVelocity(-var2, 0.0D, -var4);
                var1.addVelocity(var2, 0.0D, var4);
            }

        }
    }

    public void handleHealthUpdate(byte var1)
    {
    }


    public bool isOffsetPositionInLiquid(double var1, double var3, double var5)
    {
        AxisAlignedBB var7 = this.boundingBox.getOffsetBoundingBox(var1, var3, var5);
        List<AxisAlignedBB> var8 = this.world.getCollidingBoundingBoxes(this, var7);
        if (var8.Count > 0)
        {
            return false;
        }
        else
        {
            return !this.world.getIsAnyLiquid(var7);
        }
    }

    public float getEntityBrightness(float var1)
    {
        int var2 = MathHelper.floor_double(this.posX);
        double var3 = (this.boundingBox.maxY - this.boundingBox.minY) * 0.66D;
        int var5 = MathHelper.floor_double(this.posY - (double)this.yOffset + var3);
        int var6 = MathHelper.floor_double(this.posZ);
        if (this.world.checkChunksExist(MathHelper.floor_double(this.boundingBox.minX), MathHelper.floor_double(this.boundingBox.minY), MathHelper.floor_double(this.boundingBox.minZ), MathHelper.floor_double(this.boundingBox.maxX), MathHelper.floor_double(this.boundingBox.maxY), MathHelper.floor_double(this.boundingBox.maxZ)))
        {
            float var7 = this.world.getLightBrightness(var2, var5, var6);
            if (var7 < this.entityBrightness)
            {
                var7 = this.entityBrightness;
            }

            return var7;
        }
        else
        {
            return this.entityBrightness;
        }
    }
    public void setLocationAndAngles(double var1, double var3, double var5, float var7, float var8)
    {
        this.lastTickPosX = this.prevPosX = this.posX = (float)var1;
        this.lastTickPosY = this.prevPosY = this.posY = (float)var3 + this.yOffset;
        this.lastTickPosZ = this.prevPosZ = this.posZ = (float)var5;
        this.rotationYaw = var7;
        this.rotationPitch = var8;
        this.setPosition(this.posX, this.posY, this.posZ);
    }
    protected void preparePlayerToSpawn()
    {
        if (this.world != null)
        {
            while (this.posY > 0.0D)
            {
                this.setPosition(this.posX, this.posY, this.posZ);
                if (this.world.getCollidingBoundingBoxes(this, this.boundingBox).Count == 0)
                {
                    break;
                }

                ++this.posY;
            }

            this.motionX = this.motionY = this.motionZ = 0.0f;
            this.rotationPitch = 0.0F;
        }
    }
    public void onCollideWithPlayer(EntityPlayer var1)
    {
    }
    public bool interact(EntityPlayer var1) => false;

    public ItemStack[] getInventory()
    {
        return null;
    }

    internal void mountEntity(EntityMinecart entityMinecart)
    {
        throw new NotImplementedException();
    }
}
