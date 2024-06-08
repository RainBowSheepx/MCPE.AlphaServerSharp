using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.ItemBase;
using SpoongePE.Core.Game.material;
using SpoongePE.Core.Game.player;
using SpoongePE.Core.Game.utils;
using SpoongePE.Core.NBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.entity
{
    public abstract class EntityLiving : Entity
    {
        public byte heartsHalvesLife = 20;
        public float field_9365_p;
        public float field_9363_r;
        public float renderYawOffset = 0.0F;
        public float prevRenderYawOffset = 0.0F;
        protected float field_9362_u;
        protected float field_9361_v;
        protected float field_9360_w;
        protected float field_9359_x;
        protected bool field_9358_y = true;
        //protected string texture = "/mob/char.png";
        protected bool field_9355_A = true;
        protected float unused180 = 0.0F;
        protected string entityType = null;
        protected float field_9349_D = 1.0F;
        protected int scoreValue = 0;
        protected float field_9345_F = 0.0F;

        public float prevSwingProgress;
        public float swingProgress;
        public int health = 10;
        public int prevHealth;
        private int livingSoundTime;
        public int hurtTime;
        public int maxHurtTime;
        public float attackedAtYaw = 0.0F;
        public int deathTime = 0;
        public int attackTime = 0;
        public float prevCameraPitch;
        public float cameraPitch;
        protected bool unused_flag = false;
        public int field_9326_T = -1;
        public float field_9325_U = (float)(new Random().NextDouble() * 0.8999999761581421D + 0.10000000149011612D);
        public float prevLegYaw;
        public float legYaw;
        public float legSwing;
        protected int newPosRotationIncrements;
        protected double newPosX;
        protected double newPosY;
        protected double newPosZ;
        protected double newRotationYaw;
        protected double newRotationPitch;
        float field_9348_ae = 0.0F;
        protected int field_9346_af = 0;
        protected int entityAge = 0;
        public float moveStrafing;
        public float moveForward;
        protected float randomYawVelocity;
        protected bool isJumping = false;
        protected float defaultPitch = 0.0F;
        protected float moveSpeed = 0.7F;
        private Entity currentTarget;
        protected int numTicksToChaseTarget = 0;
        public EntityLiving(World w) : base(w)
        {
            this.preventEntitySpawning = true;
            Random rnd = new Random();
            this.field_9363_r = (float)(rnd.NextDouble() + 1.0D) * 0.01F;
            this.setPosition(this.posX, this.posY, this.posZ);
            this.field_9365_p = (float)rnd.NextDouble() * 12398.0F;
            this.rotationYaw = (float)(rnd.NextDouble() * 3.1415927410125732D * 2.0D);
            this.stepHeight = 0.5F;
        }
        public bool canEntityBeSeen(Entity var1)
        {
            if (var1.getEntityString() != null)
            {
                return this.world.rayTraceBlocks(Vec3D.createVector(this.posX, this.posY + (double)this.getEyeHeight(), this.posZ), Vec3D.createVector(var1.posX, var1.posY + (double)var1.getEyeHeight() + 1.8, var1.posZ)) == null;
            }
            else
            {
                return this.world.rayTraceBlocks(Vec3D.createVector(this.posX, this.posY + (double)this.getEyeHeight(), this.posZ), Vec3D.createVector(var1.posX, var1.posY + (double)var1.getEyeHeight() + 0.06, var1.posZ)) == null;
            }
        }

        public new bool canBeCollidedWith() => !this.isDead;


        public new bool canBePushed() => !this.isDead;


        public new float getEyeHeight() => this.height * 0.85F;

        public new void onEntityUpdate()
        {
            this.prevSwingProgress = this.swingProgress;
            base.onEntityUpdate();


            if (this.isEntityAlive() && this.isEntityInsideOpaqueBlock())
            {
                this.attackEntityFrom((Entity)null, 1);
            }

            if (this.isImmuneToFire)
            {
                this.fire = 0;
            }
            if (this.isEntityAlive() && this.isInsideOfMaterial(Material.water) && !this.canBreatheUnderwater())
            {
                --this.air;
                if (this.air == -20)
                {
                    this.air = 0;



                    this.attackEntityFrom((Entity)null, 2);
                }

                this.fire = 0;
            }
            else
            {
                this.air = this.maxAir;
            }

            this.prevCameraPitch = this.cameraPitch;
            if (this.attackTime > 0)
            {
                --this.attackTime;
            }

            if (this.hurtTime > 0)
            {
                --this.hurtTime;
            }

            if (this.heartsLife > 0)
            {
                --this.heartsLife;
            }

            if (this.health <= 0)
            {
                ++this.deathTime;
                if (this.deathTime > 20)
                {
                    this.onEntityDeath();
                    this.setEntityDead();
                }
            }

            this.field_9359_x = this.field_9360_w;
            this.prevRenderYawOffset = this.renderYawOffset;
            this.prevRotationYaw = this.rotationYaw;
            this.prevRotationPitch = this.rotationPitch;
        }
        public bool canBreatheUnderwater() => false;


        public new void setPositionAndRotation2(double var1, double var3, double var5, float var7, float var8, int var9)
        {
            this.yOffset = 0.0F;
            this.newPosX = var1;
            this.newPosY = var3;
            this.newPosZ = var5;
            this.newRotationYaw = (double)var7;
            this.newRotationPitch = (double)var8;
            this.newPosRotationIncrements = var9;
        }
        public override void onUpdate()
        {
            base.onUpdate();
		
            this.onLivingUpdate();
	
            double var1 = this.posX - this.prevPosX;
            double var3 = this.posZ - this.prevPosZ;
            float var5 = MathHelper.sqrt_double(var1 * var1 + var3 * var3);
            float var6 = this.renderYawOffset;
            float var7 = 0.0F;
            this.field_9362_u = this.field_9361_v;
            float var8 = 0.0F;
            if (var5 > 0.05F)
            {
                var8 = 1.0F;
                var7 = var5 * 3.0F;
                var6 = (float)Math.Atan2(var3, var1) * 180.0F / 3.1415927F - 90.0F;
            }

            if (this.swingProgress > 0.0F)
            {
                var6 = this.rotationYaw;
            }

            if (!this.onGround)
            {
                var8 = 0.0F;
            }

            this.field_9361_v += (var8 - this.field_9361_v) * 0.3F;

            float var9;
            for (var9 = var6 - this.renderYawOffset; var9 < -180.0F; var9 += 360.0F)
            {
            }

            while (var9 >= 180.0F)
            {
                var9 -= 360.0F;
            }

            this.renderYawOffset += var9 * 0.3F;

            float var10;
            for (var10 = this.rotationYaw - this.renderYawOffset; var10 < -180.0F; var10 += 360.0F)
            {
            }

            while (var10 >= 180.0F)
            {
                var10 -= 360.0F;
            }

            bool var11 = var10 < -90.0F || var10 >= 90.0F;
            if (var10 < -75.0F)
            {
                var10 = -75.0F;
            }

            if (var10 >= 75.0F)
            {
                var10 = 75.0F;
            }

            this.renderYawOffset = this.rotationYaw - var10;
            if (var10 * var10 > 2500.0F)
            {
                this.renderYawOffset += var10 * 0.2F;
            }

            if (var11)
            {
                var7 *= -1.0F;
            }
	
            while (this.rotationYaw - this.prevRotationYaw < -180.0F)
            {
                this.prevRotationYaw -= 360.0F;
            }
	
            while (this.rotationYaw - this.prevRotationYaw >= 180.0F)
            {
                this.prevRotationYaw += 360.0F;
            }
	
            while (this.renderYawOffset - this.prevRenderYawOffset < -180.0F)
            {
                this.prevRenderYawOffset -= 360.0F;
            }
	
            while (this.renderYawOffset - this.prevRenderYawOffset >= 180.0F)
            {
                this.prevRenderYawOffset += 360.0F;
            }
	
            while (this.rotationPitch - this.prevRotationPitch < -180.0F)
            {
                this.prevRotationPitch -= 360.0F;
            }
	
            while (this.rotationPitch - this.prevRotationPitch >= 180.0F)
            {
                this.prevRotationPitch += 360.0F;
            }
	
            this.field_9360_w += var7;
        }
        protected new void setSize(float var1, float var2) => base.setSize(var1, var2);

        public void heal(int var1)
        {
            if (this.health > 0)
            {
                this.health += var1;
                if (this.health > 20)
                {
                    this.health = 20;
                }

                this.heartsLife = (byte)(this.heartsHalvesLife / 2);
            }
        }
        public new bool attackEntityFrom(Entity var1, int var2)
        {
            this.entityAge = 0;
            if (this.health <= 0)
            {
                return false;
            }
            else
            {
                this.legYaw = 1.5F;
                bool var3 = true;
                if ((float)this.heartsLife > (float)this.heartsHalvesLife / 2.0F)
                {
                    if (var2 <= this.field_9346_af)
                    {
                        return false;
                    }

                    this.damageEntity(var2 - this.field_9346_af);
                    this.field_9346_af = var2;
                    var3 = false;
                }
                else
                {
                    this.field_9346_af = var2;
                    this.prevHealth = this.health;
                    this.heartsLife = this.heartsHalvesLife;
                    this.damageEntity(var2);
                    this.hurtTime = this.maxHurtTime = 10;
                }
                Random rnd = new Random();
                this.attackedAtYaw = 0.0F;
                if (var3)
                {
                    this.world.setEntityState(this, (byte)2);
                    this.setBeenAttacked();
                    if (var1 != null)
                    {
                        double var4 = var1.posX - this.posX;

                        double var6;
                        for (var6 = var1.posZ - this.posZ; var4 * var4 + var6 * var6 < 1.0E-4D; var6 = (rnd.NextDouble() - rnd.NextDouble()) * 0.01D)
                        {
                            var4 = (-rnd.NextDouble()) * 0.01D;
                        }

                        this.attackedAtYaw = (float)(Math.Atan2(var6, var4) * 180.0D / 3.1415927410125732D) - this.rotationYaw;
                        this.knockBack(var1, var2, var4, var6);
                    }
                    else
                    {
                        this.attackedAtYaw = (float)((int)(rnd.NextDouble() * 2.0D) * 180);
                    }
                }

                if (this.health <= 0)
                {

                    this.onDeath(var1);
                }


                return true;
            }

        }
        protected void damageEntity(int var1) => this.health -= var1;

        public void knockBack(Entity var1, int var2, double var3, double var5)
        {
            float var7 = MathHelper.sqrt_double(var3 * var3 + var5 * var5);
            float var8 = 0.4F;
            this.motionX /= 2.0f;
            this.motionY /= 2.0f;
            this.motionZ /= 2.0f;
            this.motionX -= (float)var3 / var7 * var8;
            this.motionY += 0.4000000059604645f;
            this.motionZ -= (float)var5 / var7 * var8;
            if (this.motionY > 0.4000000059604645D)
            {
                this.motionY = 0.4000000059604645f;
            }

        }

        public void onDeath(Entity var1)
        {


            if (var1 != null)
            {
                var1.onKillEntity(this);
            }

            this.unused_flag = true;
            this.dropFewItems();


            this.world.setEntityState(this, (byte)3);
        }

        protected void dropFewItems()
        {
            int var1 = this.getDropItemId();
            if (var1 > 0)
            {
                int var2 = this.rand.Next(3);

                for (int var3 = 0; var3 < var2; ++var3)
                {
                    this.dropItem(var1, 1);
                }
            }

        }

        protected int getDropItemId() => 0;


        protected new void fall(float var1)
        {
            base.fall(var1);
            int var2 = (int)Math.Ceiling((double)(var1 - 3.0F));
            if (var2 > 0)
            {
                //this.attackEntityFrom((Entity)null, var2);
            }

        }
        public void moveEntityWithCustomSpeed(float groundSpeed, float airSpeed)
        {
            float var8 = 0.91F;
            if (this.onGround)
            {
                var8 = 0.54600006F;
                int var4 = this.world.getBlockIDAt(MathHelper.floor_double(this.posX), MathHelper.floor_double(this.boundingBox.minY) - 1, MathHelper.floor_double(this.posZ));
                if (var4 > 0)
                {
                    var8 = Block.blocks[var4].slipperiness * 0.91F;
                }
            }
            float var9 = 0.16277136F / (var8 * var8 * var8);
            this.moveFlying(this.moveStrafing, this.moveForward, this.onGround ? groundSpeed * var9 : airSpeed);
        }

        public void moveEntityWithHeading(float var1, float var2)
        {
            double var3;
            if (this.isInWater())
            {
                var3 = this.posY;
                this.moveFlying(var1, var2, 0.02F);
                this.moveEntity(this.motionX, this.motionY, this.motionZ);
                this.motionX *= 0.800000011920929f;
                this.motionY *= 0.800000011920929f;
                this.motionZ *= 0.800000011920929f;
                this.motionY -= 0.02f;
                if (this.isCollidedHorizontally && this.isOffsetPositionInLiquid(this.motionX, this.motionY + 0.6000000238418579D - this.posY + var3, this.motionZ))
                {
                    this.motionY = 0.30000001192092896f;
                }
            }
            else if (this.handleLavaMovement())
            {
                var3 = this.posY;
                this.moveFlying(var1, var2, 0.02F);
                this.moveEntity(this.motionX, this.motionY, this.motionZ);
                this.motionX *= 0.5f;
                this.motionY *= 0.5f;
                this.motionZ *= 0.5f;
                this.motionY -= 0.02f;
                if (this.isCollidedHorizontally && this.isOffsetPositionInLiquid(this.motionX, this.motionY + 0.6000000238418579D - this.posY + var3, this.motionZ))
                {
                    this.motionY = 0.30000001192092896f;
                }
            }
            else
            {
                float var8 = 0.91F;
                if (this.onGround)
                {
                    var8 = 0.54600006F;
                    int var4 = this.world.getBlockIDAt(MathHelper.floor_double(this.posX), MathHelper.floor_double(this.boundingBox.minY) - 1, MathHelper.floor_double(this.posZ));
                    if (var4 > 0)
                    {
                        var8 = Block.blocks[var4].slipperiness * 0.91F;
                    }
                }

                float var9 = 0.16277136F / (var8 * var8 * var8);
                this.moveFlying(var1, var2, this.onGround ? 0.1F * var9 : 0.02F);
                var8 = 0.91F;
                if (this.onGround)
                {
                    var8 = 0.54600006F;
                    int var5 = this.world.getBlockIDAt(MathHelper.floor_double(this.posX), MathHelper.floor_double(this.boundingBox.minY) - 1, MathHelper.floor_double(this.posZ));
                    if (var5 > 0)
                    {
                        var8 = Block.blocks[var5].slipperiness * 0.91F;
                    }
                }

                if (this.isOnLadder())
                {
                    float var10 = 0.15F;
                    if (this.motionX < (double)(-var10))
                    {
                        this.motionX = (-var10);
                    }

                    if (this.motionX > (double)var10)
                    {
                        this.motionX = var10;
                    }

                    if (this.motionZ < (double)(-var10))
                    {
                        this.motionZ = (-var10);
                    }

                    if (this.motionZ > (double)var10)
                    {
                        this.motionZ = var10;
                    }

                    this.fallDistance = 0.0F;
                    if (this.motionY < -0.15D)
                    {
                        this.motionY = -0.15f;
                    }

                    if (this.motionY < 0.0D)
                    {
                        this.motionY = 0.0f;
                    }
                }

                this.moveEntity(this.motionX, this.motionY, this.motionZ);
                if (this.isCollidedHorizontally && this.isOnLadder())
                {
                    this.motionY = 0.2f;
                }

                this.motionY -= 0.08f;

                this.motionY *= 0.9800000190734863f;
                this.motionX *= var8;
                this.motionZ *= var8;
            }

            this.prevLegYaw = this.legYaw;
            var3 = this.posX - this.prevPosX;
            double var11 = this.posZ - this.prevPosZ;
            float var7 = MathHelper.sqrt_double(var3 * var3 + var11 * var11) * 4.0F;
            if (var7 > 1.0F)
            {
                var7 = 1.0F;
            }

            this.legYaw += (var7 - this.legYaw) * 0.4F;
            this.legSwing += this.legYaw;
        }

        public bool isOnLadder()
        {
            int var1 = MathHelper.floor_double(this.posX);
            int var2 = MathHelper.floor_double(this.boundingBox.minY);
            int var3 = MathHelper.floor_double(this.posZ);
            return this.world.getBlockIDAt(var1, var2, var3) == Block.ladder.blockID;
        }
        public new bool isEntityAlive() => !this.isDead && this.health > 0;

        public void onLivingUpdate()
        {
            if (this.newPosRotationIncrements > 0)
            {
                double var1 = this.posX + (this.newPosX - this.posX) / (double)this.newPosRotationIncrements;
                double var3 = this.posY + (this.newPosY - this.posY) / (double)this.newPosRotationIncrements;
                double var5 = this.posZ + (this.newPosZ - this.posZ) / (double)this.newPosRotationIncrements;
		
                double var7;
                for (var7 = this.newRotationYaw - (double)this.rotationYaw; var7 < -180.0D; var7 += 360.0D)
                {
                }
				
                while (var7 >= 180.0D)
                {
                    var7 -= 360.0D;
                }
		
                this.rotationYaw = (float)((double)this.rotationYaw + var7 / (double)this.newPosRotationIncrements);
                this.rotationPitch = (float)((double)this.rotationPitch + (this.newRotationPitch - (double)this.rotationPitch) / (double)this.newPosRotationIncrements);
                --this.newPosRotationIncrements;
                this.setPosition(var1, var3, var5);
                this.setRotation(this.rotationYaw, this.rotationPitch);
                List<AxisAlignedBB> var9 = this.world.getCollidingBoundingBoxes(this, this.boundingBox.contract(0.03125D, 0.0D, 0.03125D));
                if (var9.Count > 0)
                {
                    double var10 = 0.0D;

                    for (int var12 = 0; var12 < var9.Count; ++var12)
                    {
                        AxisAlignedBB var13 = var9[var12];
                        if (var13.maxY > var10)
                        {
                            var10 = var13.maxY;
                        }
                    }

                    var3 += var10 - this.boundingBox.minY;
                    this.setPosition(var1, var3, var5);
                }
            }

            if (this.isMovementBlocked())
            {
                this.isJumping = false;
                this.moveStrafing = 0.0F;
                this.moveForward = 0.0F;
                this.randomYawVelocity = 0.0F;
            }

            this.updatePlayerActionState();


            bool var14 = this.isInWater();
            bool var2 = this.handleLavaMovement();
            if (this.isJumping)
            {
                if (var14)
                {
                    this.motionY += 0.03999999910593033f;
                }
                else if (var2)
                {
                    this.motionY += 0.03999999910593033f;
                }
                else if (this.onGround)
                {
                    this.jump();
                }
            }

            this.moveStrafing *= 0.98F;
            this.moveForward *= 0.98F;
            this.randomYawVelocity *= 0.9F;
            this.moveEntityWithHeading(this.moveStrafing, this.moveForward);
            List<Entity> var15 = this.world.getEntitiesWithinAABBExcludingEntity(this, this.boundingBox.expand(0.20000000298023224D, 0.0D, 0.20000000298023224D));
            if (var15 != null && var15.Count > 0)
            {
                for (int var4 = 0; var4 < var15.Count; ++var4)
                {
                    Entity var16 = var15[var4];
                    if (var16.canBePushed())
                    {
                        var16.applyEntityCollision(this);
                    }
                }
            }

        }

        protected virtual void updatePlayerActionState()
        {
            ++this.entityAge;
		
            EntityPlayer var1 = this.world.getClosestPlayerToEntity(this, -1.0D);
		
           // this.despawnEntity();
            this.moveStrafing = 0.0F;
            this.moveForward = 0.0F;
            float var2 = 8.0F;
		
            if (this.rand.NextSingle() < 0.02F)
            {
		
                var1 = this.world.getClosestPlayerToEntity(this, (double)var2);
                if (var1 != null)
                {
                    this.currentTarget = var1;
                    this.numTicksToChaseTarget = 10 + this.rand.Next(20);
                }
                else
                {
                    this.randomYawVelocity = (this.rand.NextSingle() - 0.5F) * 20.0F;
                }
		
            }
		
            if (this.currentTarget != null)
            {
                this.faceEntity(this.currentTarget, 10.0F, (float)this.getVerticalFaceSpeed());
                if (this.numTicksToChaseTarget-- <= 0 || this.currentTarget.isDead || this.currentTarget.getDistanceSqToEntity(this) > (double)(var2 * var2))
                {
                    this.currentTarget = null;
                }
            }
            else
            {
                if (this.rand.NextSingle() < 0.05F)
                {
                    this.randomYawVelocity = (this.rand.NextSingle() - 0.5F) * 20.0F;
                }

                this.rotationYaw += this.randomYawVelocity;
                this.rotationPitch = this.defaultPitch;
            }
		
            bool var3 = this.isInWater();
            bool var4 = this.handleLavaMovement();
            if (var3 || var4)
            {
                this.isJumping = this.rand.NextSingle() < 0.8F;
            }
		
        }
        protected bool isMovementBlocked() => this.health <= 0;


        protected void jump() => this.motionY = 0.41999998688697815f;




        protected int getVerticalFaceSpeed() => 40;


        private float updateRotation(float var1, float var2, float var3)
        {
            float var4;
            for (var4 = var2 - var1; var4 < -180.0F; var4 += 360.0F)
            {
            }

            while (var4 >= 180.0F)
            {
                var4 -= 360.0F;
            }

            if (var4 > var3)
            {
                var4 = var3;
            }

            if (var4 < -var3)
            {
                var4 = -var3;
            }

            return var1 + var4;
        }

        public void onEntityDeath()
        {
        }

        public bool getCanSpawnHere() => this.world.checkIfAABBIsClear(this.boundingBox) && this.world.getCollidingBoundingBoxes(this, this.boundingBox).Count == 0 && !this.world.getIsAnyLiquid(this.boundingBox);


        protected new void kill() => this.attackEntityFrom((Entity)null, 4);


        public Vec3D getPosition(float var1)
        {
            if (var1 == 1.0F)
            {
                return Vec3D.createVector(this.posX, this.posY, this.posZ);
            }
            else
            {
                double var2 = this.prevPosX + (this.posX - this.prevPosX) * (double)var1;
                double var4 = this.prevPosY + (this.posY - this.prevPosY) * (double)var1;
                double var6 = this.prevPosZ + (this.posZ - this.prevPosZ) * (double)var1;
                return Vec3D.createVector(var2, var4, var6);
            }
        }

        public new Vec3D getLookVec()
        {
            return this.getLook(1.0F);
        }

        public Vec3D getLook(float var1)
        {
            float var2;
            float var3;
            float var4;
            float var5;
            if (var1 == 1.0F)
            {
                var2 = MathHelper.cos(-this.rotationYaw * 0.017453292F - 3.1415927F);
                var3 = MathHelper.sin(-this.rotationYaw * 0.017453292F - 3.1415927F);
                var4 = -MathHelper.cos(-this.rotationPitch * 0.017453292F);
                var5 = MathHelper.sin(-this.rotationPitch * 0.017453292F);
                return Vec3D.createVector((double)(var3 * var4), (double)var5, (double)(var2 * var4));
            }
            else
            {
                var2 = this.prevRotationPitch + (this.rotationPitch - this.prevRotationPitch) * var1;
                var3 = this.prevRotationYaw + (this.rotationYaw - this.prevRotationYaw) * var1;
                var4 = MathHelper.cos(-var3 * 0.017453292F - 3.1415927F);
                var5 = MathHelper.sin(-var3 * 0.017453292F - 3.1415927F);
                float var6 = -MathHelper.cos(-var2 * 0.017453292F);
                float var7 = MathHelper.sin(-var2 * 0.017453292F);
                return Vec3D.createVector((double)(var5 * var6), (double)var7, (double)(var4 * var6));
            }
        }
        public MovingObjectPosition rayTrace(double var1, float var3)
        {
            Vec3D var4 = this.getPosition(var3);
            Vec3D var5 = this.getLook(var3);
            Vec3D var6 = var4.addVector(var5.xCoord * var1, var5.yCoord * var1, var5.zCoord * var1);
            return (MovingObjectPosition)this.world.rayTraceBlocks(var4, var6);
        }

        public int getMaxSpawnedInChunk() => 4;


        public ItemStack getHeldItem() => null;
        public new void handleHealthUpdate(byte var1)
        {
            if (var1 == 2)
            {
                this.legYaw = 1.5F;
                this.heartsLife = this.heartsHalvesLife;
                this.hurtTime = this.maxHurtTime = 10;
                this.attackedAtYaw = 0.0F;
                this.attackEntityFrom((Entity)null, 0);
            }
            else if (var1 == 3)
            {
                this.health = 0;
                this.onDeath((Entity)null);
            }
            else
            {
                base.handleHealthUpdate(var1);
            }

        }

        public bool isPlayerSleeping() => false;

        public void faceEntity(Entity var1, float var2, float var3)
        {
            double var4 = var1.posX - this.posX;
            double var8 = var1.posZ - this.posZ;
            double var6;
            if (var1 is EntityLiving)
            {
                EntityLiving var10 = (EntityLiving)var1;
                var6 = this.posY + (double)this.getEyeHeight() - (var10.posY + (double)var10.getEyeHeight());
            }
            else
            {
                var6 = (var1.boundingBox.minY + var1.boundingBox.maxY) / 2.0D - (this.posY + (double)this.getEyeHeight());
            }

            double var14 = (double)MathHelper.sqrt_double(var4 * var4 + var8 * var8);
            float var12 = (float)(Math.Atan2(var8, var4) * 180.0D / 3.1415927410125732D) - 90.0F;
            float var13 = (float)(-(Math.Atan2(var6, var14) * 180.0D / 3.1415927410125732D));
            this.rotationPitch = -this.updateRotation(this.rotationPitch, var13, var3);
            this.rotationYaw = this.updateRotation(this.rotationYaw, var12, var2);
        }

        protected override void readEntityFromNBT(NbtCompound var1)
        {
            this.health = var1.Get<NbtShort>("Health").ShortValue;
            if (!var1.TryGet<NbtShort>("Health", out _))
            {
                this.health = 10;
            }

            this.hurtTime = var1.Get<NbtShort>("HurtTime").ShortValue;
            this.deathTime = var1.Get<NbtShort>("DeathTime").ShortValue;
            this.attackTime = var1.Get<NbtShort>("AttackTime").ShortValue;
        }

        protected override void writeEntityToNBT(NbtCompound var1)
        {
            var1.Add(new NbtShort("Health", (short)this.health));
            var1.Add(new NbtShort("HurtTime", (short)this.hurtTime));
            var1.Add(new NbtShort("DeathTime", (short)this.deathTime));
            var1.Add(new NbtShort("AttackTime", (short)this.attackTime));
        }
    }
}
