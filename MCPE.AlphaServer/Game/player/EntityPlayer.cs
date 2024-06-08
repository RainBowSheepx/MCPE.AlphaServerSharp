using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.entity;
using SpoongePE.Core.Game.entity.impl;
using SpoongePE.Core.Game.ItemBase;
using SpoongePE.Core.Game.material;
using SpoongePE.Core.Game.player.inventory;
using SpoongePE.Core.Game.utils;
using SpoongePE.Core.NBT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;

namespace SpoongePE.Core.Game.player
{
    public class EntityPlayer : EntityLiving
    {
        public InventoryPlayer inventory;
        public Container personalCraftingInventory;
        public Container currentCraftingInventory;
        public byte field_9152_am = 0;

        public float prevCameraYaw;
        public float cameraYaw;
        public bool isSwinging = false;
        public int swingProgressInt = 0;
        public string username;
        public int dimension; // for compability
        public double field_20066_r;
        public double field_20065_s;
        public double field_20064_t;
        public double field_20063_u;
        public double field_20062_v;
        public double field_20061_w;
        protected bool sleeping;
        public ChunkCoordinates bedChunkCoordinates;
        private int sleepTimer;
        public float field_22066_z;
        public float field_22067_A;
        private ChunkCoordinates playerSpawnCoordinate;
        private ChunkCoordinates startMinecartRidingCoordinate;


        private int damageRemainder = 0;


        public EntityPlayer(World var1) : base(var1)
        {

            //   this.personalCraftingInventory = new ContainerPlayer(this.inventory, !var1.singleplayerWorld);
            //   this.currentCraftingInventory = this.personalCraftingInventory;
            this.yOffset = 1.62F;
            ChunkCoordinates var2 = var1.getSpawnPoint();
            this.setLocationAndAngles((double)var2.posX + 0.5D, (double)(var2.posY + 1), (double)var2.posZ + 0.5D, 0.0F, 0.0F);
            this.health = 20;
            this.entityType = "humanoid";
            this.unused180 = 180.0F;
            this.fireResistance = 20;


        }
        public override void onUpdate()
        {
		return;
            if (this.isPlayerSleeping())
            {
                ++this.sleepTimer;
                if (this.sleepTimer > 100)
                {
                    this.sleepTimer = 100;
                }


                if (!this.isInBed())
                {
                    this.wakeUpPlayer(true, true, false);
                }
                else if (this.world.isDaytime())
                {
                    this.wakeUpPlayer(false, true, true);
                }

            }
            else if (this.sleepTimer > 0)
            {
                ++this.sleepTimer;
                if (this.sleepTimer >= 110)
                {
                    this.sleepTimer = 0;
                }
            }

            base.onUpdate();
            /*            if (this.craftingInventory != null && !this.craftingInventory.isUsableByPlayer(this))
                        {
                            this.closeScreen();
                            this.craftingInventory = this.inventorySlots;
                        }*/

            this.field_20066_r = this.field_20063_u;
            this.field_20065_s = this.field_20062_v;
            this.field_20064_t = this.field_20061_w;
            double var1 = this.posX - this.field_20063_u;
            double var3 = this.posY - this.field_20062_v;
            double var5 = this.posZ - this.field_20061_w;
            double var7 = 10.0D;
            if (var1 > var7)
            {
                this.field_20066_r = this.field_20063_u = this.posX;
            }

            if (var5 > var7)
            {
                this.field_20064_t = this.field_20061_w = this.posZ;
            }

            if (var3 > var7)
            {
                this.field_20065_s = this.field_20062_v = this.posY;
            }

            if (var1 < -var7)
            {
                this.field_20066_r = this.field_20063_u = this.posX;
            }

            if (var5 < -var7)
            {
                this.field_20064_t = this.field_20061_w = this.posZ;
            }

            if (var3 < -var7)
            {
                this.field_20065_s = this.field_20062_v = this.posY;
            }

            this.field_20063_u += var1 * 0.25D;
            this.field_20061_w += var5 * 0.25D;
            this.field_20062_v += var3 * 0.25D;

            if (this.ridingEntity == null)
            {
                this.startMinecartRidingCoordinate = null;
            }

        }

        protected new bool isMovementBlocked()
        {
            return this.health <= 0 || this.isPlayerSleeping();
        }

        public new void preparePlayerToSpawn()
        {
            this.yOffset = 1.62F;
            this.setSize(0.6F, 1.8F);
            base.preparePlayerToSpawn();
            this.health = 20;
            this.deathTime = 0;
        }


        public new void onLivingUpdate()
        {
            if (this.world.difficultySetting == 0 && this.health < 20 && this.ticksExisted % 20 * 12 == 0)
            {
                this.heal(1);
            }


            this.prevCameraYaw = this.cameraYaw;
            base.onLivingUpdate();
            float var1 = MathHelper.sqrt_double(this.motionX * this.motionX + this.motionZ * this.motionZ);
            float var2 = (float)Math.Atan(-this.motionY * 0.20000000298023224D) * 15.0F;
            if (var1 > 0.1F)
            {
                var1 = 0.1F;
            }

            if (!this.onGround || this.health <= 0)
            {
                var1 = 0.0F;
            }

            if (this.onGround || this.health <= 0)
            {
                var2 = 0.0F;
            }

            this.cameraYaw += (var1 - this.cameraYaw) * 0.4F;
            this.cameraPitch += (var2 - this.cameraPitch) * 0.8F;
            if (this.health > 0)
            {
                List<Entity> var3 = this.world.getEntitiesWithinAABBExcludingEntity(this, this.boundingBox.expand(1.0D, 0.0D, 1.0D));
                if (var3 != null)
                {
                    for (int var4 = 0; var4 < var3.Count; ++var4)
                    {
                        Entity var5 = var3[var4];
                        if (!var5.isDead)
                        {
                            this.collideWithPlayer(var5);
                        }
                    }
                }
            }

        }

        private void collideWithPlayer(Entity var1)
        {
            var1.onCollideWithPlayer(this);
        }
        public new void onDeath(Entity var1)
        {
            base.onDeath(var1);
            this.setSize(0.2F, 0.2F);
            this.setPosition(this.posX, this.posY, this.posZ);
            this.motionY = 0.10000000149011612f;
            if (this.username.Equals("Notch"))
            {
                this.dropPlayerItemWithRandomChoice(new ItemStack(Item.appleRed, 1), true);
            }

            this.inventory.dropAllItems();
            if (var1 != null)
            {
                this.motionX = (-MathHelper.cos((this.attackedAtYaw + this.rotationYaw) * 3.1415927F / 180.0F) * 0.1F);
                this.motionZ = (-MathHelper.sin((this.attackedAtYaw + this.rotationYaw) * 3.1415927F / 180.0F) * 0.1F);
            }
            else
            {
                this.motionX = this.motionZ = 0.0f;
            }

            this.yOffset = 0.1F;

        }

        public void dropCurrentItem()
        {
            this.dropPlayerItemWithRandomChoice(this.inventory.decrStackSize(this.inventory.currentItem, 1), false);
        }

        public void dropPlayerItemBySlot(int var1)
        {
            this.dropPlayerItemWithRandomChoice(this.inventory.decrStackSize(var1, 1), false);
        }

        public void dropPlayerItem(ItemStack var1)
        {
            this.dropPlayerItemWithRandomChoice(var1, false);
        }

        public void dropPlayerItemWithRandomChoice(ItemStack var1, bool var2)
        {
            if (var1 != null)
            {
                EntityItem var3 = new EntityItem(this.world, this.posX, this.posY - 0.30000001192092896D + (double)this.getEyeHeight(), this.posZ, var1);
                var3.delayBeforeCanPickup = 40;
                float var4 = 0.1F;
                float var5;
                if (var2)
                {
                    var5 = this.rand.NextSingle() * 0.5F;
                    float var6 = this.rand.NextSingle() * 3.1415927F * 2.0F;
                    var3.motionX = (-MathHelper.sin(var6) * var5);
                    var3.motionZ = (MathHelper.cos(var6) * var5);
                    var3.motionY = 0.20000000298023224f;
                }
                else
                {
                    var4 = 0.3F;
                    var3.motionX = (-MathHelper.sin(this.rotationYaw / 180.0F * 3.1415927F) * MathHelper.cos(this.rotationPitch / 180.0F * 3.1415927F) * var4);
                    var3.motionZ = (MathHelper.cos(this.rotationYaw / 180.0F * 3.1415927F) * MathHelper.cos(this.rotationPitch / 180.0F * 3.1415927F) * var4);
                    var3.motionY = (-MathHelper.sin(this.rotationPitch / 180.0F * 3.1415927F) * var4 + 0.1F);
                    var4 = 0.02F;
                    var5 = this.rand.NextSingle() * 3.1415927F * 2.0F;
                    var4 *= this.rand.NextSingle();
                    var3.motionX += (float)Math.Cos(var5) * var4;
                    var3.motionY += ((this.rand.NextSingle() - this.rand.NextSingle()) * 0.1F);
                    var3.motionZ += (float)Math.Sin(var5) * var4;
                }

                this.joinEntityItemWithWorld(var3);

            }
        }
        protected void joinEntityItemWithWorld(EntityItem var1)
        {
            this.world.entityJoinedWorld(var1);
        }

        public float getCurrentPlayerStrVsBlock(Block var1)
        {
            float var2 = this.inventory.getStrVsBlock(var1);
            if (this.isInsideOfMaterial(Material.water))
            {
                var2 /= 5.0F;
            }

            if (!this.onGround)
            {
                var2 /= 5.0F;
            }

            return var2;
        }
        protected override void readEntityFromNBT(NbtCompound var1)
        {
            base.readEntityFromNBT(var1);
            NbtList var2 = var1.Get<NbtList>("Inventory");
            this.inventory.readFromNBT(var2);
            this.dimension = var1.Get<NbtInt>("Dimension").IntValue;
            this.sleeping = var1.Get<NbtByte>("Sleeping").ByteValue == 1 ? true : false;
            this.sleepTimer = var1.Get<NbtShort>("SleepTimer").ShortValue;
            if (this.sleeping)
            {
                this.bedChunkCoordinates = new ChunkCoordinates(MathHelper.floor_double(this.posX), MathHelper.floor_double(this.posY), MathHelper.floor_double(this.posZ));
                this.wakeUpPlayer(true, true, false);
            }
            NbtInt spawnX, spawnY, spawnZ;
            if (var1.TryGet("SpawnX", out spawnX) && var1.TryGet("SpawnY", out spawnY) && var1.TryGet("SpawnZ", out spawnZ))
            {
                this.playerSpawnCoordinate = new ChunkCoordinates(spawnX.IntValue, spawnY.IntValue, spawnZ.IntValue);
            }
        }

        protected override void writeEntityToNBT(NbtCompound var1)
        {
            base.writeEntityToNBT(var1);
            NbtList inv = new NbtList("Inventory");
            inventory.writeToNBT(inv);
            var1.Add(inv);
            var1.Add(new NbtInt("Dimension", this.dimension));
            var1.Add(new NbtByte("Sleeping", this.sleeping ? (byte)1 : (byte)0));
            var1.Add(new NbtShort("SleepTimer", (short)this.sleepTimer));
            if (this.playerSpawnCoordinate != null)
            {
                var1.Add(new NbtInt("SpawnX", this.playerSpawnCoordinate.posX));
                var1.Add(new NbtInt("SpawnY", this.playerSpawnCoordinate.posY));
                var1.Add(new NbtInt("SpawnZ", this.playerSpawnCoordinate.posZ));
            }
        }
        public bool canHarvestBlock(Block var1)
        {
            return this.inventory.canHarvestBlock(var1);
        }

        public void onItemPickup(Entity var1, int var2)
        {
        }

        public new float getEyeHeight()
        {
            return 0.12F;
        }

        protected void resetHeight()
        {
            this.yOffset = 1.62F;
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
                if (this.isPlayerSleeping())
                {
                    this.wakeUpPlayer(true, true, false);
                }

                if (var1 is EntityMob || var1 is EntityArrow)
                {
                    if (this.world.difficultySetting == 0)
                    {
                        var2 = 0;
                    }

                    if (this.world.difficultySetting == 1)
                    {
                        var2 = var2 / 3 + 1;
                    }

                    if (this.world.difficultySetting == 3)
                    {
                        var2 = var2 * 3 / 2;
                    }
                }

                if (var2 == 0)
                {
                    return false;
                }
                else
                {
                    Object var3 = var1;
                    if (var1 is EntityArrow && ((EntityArrow)var1).owner != null)
                    {
                        var3 = ((EntityArrow)var1).owner;
                    }




                    return base.attackEntityFrom(var1, var2);
                }
            }
        }

        protected bool isPVPEnabled()
        {
            return false;
        }
        protected new void damageEntity(int var1)
        {
            /*int var2 = 25 - this.inventory.getTotalArmorValue();
            int var3 = var1 * var2 + this.damageRemainder;
            this.inventory.damageArmor(var1);
            var1 = var3 / 25;
            this.damageRemainder = var3 % 25;
            base.damageE1ntity(var1);*/
        }

        public void useCurrentItemOnEntity(Entity var1)
        {
            if (!var1.interact(this))
            {
                ItemStack var2 = this.getCurrentEquippedItem();
                if (var2 != null && var1 is EntityLiving)
                {
                    var2.useItemOnEntity((EntityLiving)var1);
                    if (var2.stackSize <= 0)
                    {
                        var2.onItemDestroyedByUse(this);
                        this.destroyCurrentEquippedItem();
                    }
                }

            }
        }
        public ItemStack getCurrentEquippedItem()
        {
            return this.inventory.getCurrentItem();
        }

        public void destroyCurrentEquippedItem()
        {
            this.inventory.setInventorySlotContents(this.inventory.currentItem, (ItemStack)null);
        }
        public new double getYOffset()
        {
            return (double)(this.yOffset - 0.5F);
        }
        public void attackTargetEntityWithCurrentItem(Entity var1)
        {
            int var2 = this.inventory.getDamageVsEntity(var1);
            if (var2 > 0)
            {
                if (this.motionY < 0.0D)
                {
                    ++var2;
                }

                var1.attackEntityFrom(this, var2);
                ItemStack var3 = this.getCurrentEquippedItem();
                if (var3 != null && var1 is EntityLiving)
                {
                    var3.hitEntity((EntityLiving)var1, this);
                    if (var3.stackSize <= 0)
                    {
                        var3.onItemDestroyedByUse(this);
                        this.destroyCurrentEquippedItem();
                    }
                }
            }

        }
        public void respawnPlayer()
        {
        }



        public void onItemStackChanged(ItemStack var1)
        {
        }
        public new void setEntityDead()
        {
            base.setEntityDead();
            /*            this.inventorySlots.onCraftGuiClosed(this);
                        if (this.craftingInventory != null)
                        {
                            this.craftingInventory.onCraftGuiClosed(this);
                        }*/

        }

        public new bool isEntityInsideOpaqueBlock()
        {
            return !this.sleeping && base.isEntityInsideOpaqueBlock();
        }
        public EnumStatus sleepInBedAt(int var1, int var2, int var3)
        {


            if (!this.isPlayerSleeping() && this.isEntityAlive())
            {


                if (this.world.isDaytime())
                {
                    return EnumStatus.NOT_POSSIBLE_NOW;
                }

                if (Math.Abs(this.posX - (double)var1) <= 3.0D && Math.Abs(this.posY - (double)var2) <= 2.0D && Math.Abs(this.posZ - (double)var3) <= 3.0D)
                {
                    goto label11;
                }

                return EnumStatus.TOO_FAR_AWAY;
            }

            return EnumStatus.OTHER_PROBLEM;

        label11:

            this.setSize(0.2F, 0.2F);
            this.yOffset = 0.2F;
            if (this.world.blockExists(var1, var2, var3))
            {
                int var4 = this.world.getBlockMetaAt(var1, var2, var3);

                float var6 = 0.5F;
                float var7 = 0.5F;



                this.setPosition((double)((float)var1 + var6), (double)((float)var2 + 0.9375F), (double)((float)var3 + var7));
            }
            else
            {
                this.setPosition((double)((float)var1 + 0.5F), (double)((float)var2 + 0.9375F), (double)((float)var3 + 0.5F));
            }

            this.sleeping = true;
            this.sleepTimer = 0;
            this.bedChunkCoordinates = new ChunkCoordinates(var1, var2, var3);
            this.motionX = this.motionZ = this.motionY = 0.0f;

            this.world.updateAllPlayersSleepingFlag();


            return EnumStatus.OK;
        }
        public void wakeUpPlayer(bool var1, bool var2, bool var3)
        {
            this.setSize(0.6F, 1.8F);
            this.resetHeight();
            ChunkCoordinates var4 = this.bedChunkCoordinates;
            ChunkCoordinates var5 = this.bedChunkCoordinates;
            if (var4 != null && this.world.getBlockIDAt(var4.posX, var4.posY, var4.posZ) == Block.bedBlock.blockID)
            {
                // BlockBed.setBedOccupied(this.worldObj, var4.x, var4.y, var4.z, false);
                /*                var5 = BlockBed.getNearestEmptyChunkCoordinates(this.worldObj, var4.x, var4.y, var4.z, 0);
                                if (var5 == null)
                                {
                                    var5 = new ChunkCoordinates(var4.x, var4.y + 1, var4.z);
                                }

                                this.setPosition((double)((float)var5.x + 0.5F), (double)((float)var5.y + this.yOffset + 0.1F), (double)((float)var5.z + 0.5F));*/
            }

            this.sleeping = false;

            this.world.updateAllPlayersSleepingFlag();


            if (var1)
            {
                this.sleepTimer = 0;
            }
            else
            {
                this.sleepTimer = 100;
            }

            if (var3)
            {
                this.setPlayerSpawnCoordinate(this.bedChunkCoordinates);
            }

        }
        public new bool isPlayerSleeping()
        {
            return this.sleeping;
        }

        public bool isPlayerFullyAsleep()
        {
            return this.sleeping && this.sleepTimer >= 100;
        }

        public int getSleepTimer()
        {
            return this.sleepTimer;
        }
        private bool isInBed()
        {
            return this.world.getBlockIDAt(this.bedChunkCoordinates.posX, this.bedChunkCoordinates.posY, this.bedChunkCoordinates.posZ) == Block.bedBlock.blockID;
        }
        public ChunkCoordinates getPlayerSpawnCoordinate()
        {
            return this.playerSpawnCoordinate;
        }
        public void setPlayerSpawnCoordinate(ChunkCoordinates var1)
        {
            if (var1 != null)
            {
                this.playerSpawnCoordinate = new ChunkCoordinates(var1);
            }
            else
            {
                this.playerSpawnCoordinate = null;
            }

        }

        internal void displayGUIChest(EntityMinecart entityMinecart)
        {
            throw new NotImplementedException();
        }
    }


    public enum EnumStatus
    {
        OK,
        NOT_POSSIBLE_HERE,
        NOT_POSSIBLE_NOW,
        TOO_FAR_AWAY,
        OTHER_PROBLEM
    }

}
