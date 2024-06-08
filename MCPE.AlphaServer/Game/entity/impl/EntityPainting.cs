using SpoongePE.Core.Game.ItemBase;
using SpoongePE.Core.Game.material;
using SpoongePE.Core.Game.utils;
using SpoongePE.Core.NBT;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SpoongePE.Core.Game.entity.impl
{
    public class EntityPainting : Entity
    {
        private int field_452_ad;
        public int direction;
        public int xPosition;
        public int yPosition;
        public int zPosition;
        public EnumArt art;
        public EntityPainting(World var1) : base(var1)
        {
            this.field_452_ad = 0;
            this.direction = 0;
            this.yOffset = 0.0F;
            this.setSize(0.5F, 0.5F);
        }

        public EntityPainting(World var1, int var2, int var3, int var4, int var5) : this(var1)
        {
            this.xPosition = var2;
            this.yPosition = var3;
            this.zPosition = var4;
            List<EnumArt> var6 = new List<EnumArt>();
            EnumArt[] var7 = (EnumArt[])Enum.GetValues(typeof(EnumArt));

            foreach (EnumArt var10 in var7)
            {
                this.art = var10;
                this.func_179_a(var5);
                if (this.onValidSurface())
                {
                    var6.Add(var10);
                }
            }

            if (var6.Count > 0)
            {
                this.art = var6[new Random().Next(var6.Count)];
            }

            this.func_179_a(var5);
        }

        public void func_179_a(int var1)
        {
            this.direction = var1;
            this.prevRotationYaw = this.rotationYaw = (float)(var1 * 90);
            float var2 = (float)this.art.sizeX;
            float var3 = (float)this.art.sizeY;
            float var4 = (float)this.art.sizeX;

            if (var1 != 0 && var1 != 2)
            {
                var2 = 0.5F;
            }
            else
            {
                var4 = 0.5F;
            }

            var2 /= 32.0F;
            var3 /= 32.0F;
            var4 /= 32.0F;

            float var5 = (float)this.xPosition + 0.5F;
            float var6 = (float)this.yPosition + 0.5F;
            float var7 = (float)this.zPosition + 0.5F;
            float var8 = 9.0F / 16.0F;

            if (var1 == 0)
            {
                var7 -= var8;
            }

            if (var1 == 1)
            {
                var5 -= var8;
            }

            if (var1 == 2)
            {
                var7 += var8;
            }

            if (var1 == 3)
            {
                var5 += var8;
            }

            if (var1 == 0)
            {
                var5 -= this.func_180_c(this.art.sizeX);
            }

            if (var1 == 1)
            {
                var7 += this.func_180_c(this.art.sizeX);
            }

            if (var1 == 2)
            {
                var5 += this.func_180_c(this.art.sizeX);
            }

            if (var1 == 3)
            {
                var7 -= this.func_180_c(this.art.sizeX);
            }

            var6 += this.func_180_c(this.art.sizeY);

            this.setPosition((double)var5, (double)var6, (double)var7);

            float var9 = -(0.1F / 16.0F);

            this.boundingBox.setBounds((double)(var5 - var2 - var9),
                                       (double)(var6 - var3 - var9),
                                       (double)(var7 - var4 - var9),
                                       (double)(var5 + var2 + var9),
                                       (double)(var6 + var3 + var9),
                                       (double)(var7 + var4 + var9));
        }
        private float func_180_c(int var1)
        {
            return var1 == 32 ? 0.5F : (var1 == 64 ? 0.5F : 0.0F);
        }

        public new void onUpdate()
        {
            if (++this.field_452_ad == 100)
            {
                this.field_452_ad = 0;

                if (!this.onValidSurface())
                {
                    this.setEntityDead();
                    this.world.entityJoinedWorld(new EntityItem(this.world, this.posX, this.posY, this.posZ, new ItemStack(Item.painting)));
                }
            }
        }
        public bool onValidSurface()
        {
            if (this.world.getCollidingBoundingBoxes(this, this.boundingBox).Count > 0)
            {
                return false;
            }
            else
            {
                int var1 = this.art.sizeX / 16;
                int var2 = this.art.sizeY / 16;
                int var3 = this.xPosition;
                int var4 = this.yPosition;
                int var5 = this.zPosition;

                if (this.direction == 0)
                {
                    var3 = MathHelper.floor_double(this.posX - (double)((float)this.art.sizeX / 32.0F));
                }

                if (this.direction == 1)
                {
                    var5 = MathHelper.floor_double(this.posZ - (double)((float)this.art.sizeX / 32.0F));
                }

                if (this.direction == 2)
                {
                    var3 = MathHelper.floor_double(this.posX - (double)((float)this.art.sizeX / 32.0F));
                }

                if (this.direction == 3)
                {
                    var5 = MathHelper.floor_double(this.posZ - (double)((float)this.art.sizeX / 32.0F));
                }

                var4 = MathHelper.floor_double(this.posY - (double)((float)this.art.sizeY / 32.0F));

                for (int var6 = 0; var6 < var1; ++var6)
                {
                    for (int var7 = 0; var7 < var2; ++var7)
                    {
                        Material var8;
                        if (this.direction != 0 && this.direction != 2)
                        {
                            var8 = this.world.getBlockMaterial(this.xPosition, var4 + var7, var5 + var6);
                        }
                        else
                        {
                            var8 = this.world.getBlockMaterial(var3 + var6, var4 + var7, this.zPosition);
                        }

                        if (!var8.IsSolid())
                        {
                            return false;
                        }
                    }
                }

                List<Entity> var9 = this.world.getEntitiesWithinAABBExcludingEntity(this, this.boundingBox);

                foreach (Entity entity in var9)
                {
                    if (entity is EntityPainting)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
        public new bool canBeCollidedWith()
        {
            return true;
        }

        public new bool attackEntityFrom(Entity var1, int var2)
        {
            if (!this.isDead)
            {
                this.setEntityDead();
                this.setBeenAttacked();
                this.world.entityJoinedWorld(new EntityItem(this.world, this.posX, this.posY, this.posZ, new ItemStack(Item.painting)));
            }

            return true;
        }

        protected override void writeEntityToNBT(NbtCompound var1)
        {
            var1.Add(new NbtByte("Dir", (byte)this.direction));
            var1.Add(new NbtString("Motive", this.art.title));
            var1.Add(new NbtInt("TileX", this.xPosition));
            var1.Add(new NbtInt( "TileY", this.yPosition));
            var1.Add(new NbtInt("TileZ", this.zPosition));
        }

        protected override void readEntityFromNBT(NbtCompound var1)
        {
            this.direction = var1.Get<NbtByte>("Dir").ByteValue;
            this.xPosition = var1.Get<NbtInt>("TileX").IntValue;
            this.yPosition = var1.Get<NbtInt>("TileY").IntValue;
            this.zPosition = var1.Get<NbtInt>("TileZ").IntValue;
            string var2 = var1.Get<NbtString>("Motive").StringValue;
            EnumArt[] var3 = (EnumArt[])Enum.GetValues(typeof(EnumArt));

            foreach (EnumArt art in var3)
            {
                if (art.title.Equals(var2))
                {
                    this.art = art;
                }
            }

            if (this.art == null)
            {
                this.art = EnumArt.Kebab;
            }

            this.func_179_a(this.direction);
        }

        public new void moveEntity(double var1, double var3, double var5)
        {
            if (var1 * var1 + var3 * var3 + var5 * var5 > 0.0D)
            {
                this.setEntityDead();
                this.world.entityJoinedWorld(new EntityItem(this.world, this.posX, this.posY, this.posZ, new ItemStack(Item.painting)));
            }
        }

        public new void addVelocity(double var1, double var3, double var5)
        {
            if (var1 * var1 + var3 * var3 + var5 * var5 > 0.0D)
            {
                this.setEntityDead();
                this.world.entityJoinedWorld(new EntityItem(this.world, this.posX, this.posY, this.posZ, new ItemStack(Item.painting)));
            }
        }
    }

    public class EnumArt
    {
        public static readonly EnumArt Kebab = new EnumArt("Kebab", 1, 1); // size = n*16
        public static readonly EnumArt Aztec = new EnumArt("Aztec", 1, 1);
        public static readonly EnumArt Alban = new EnumArt("Alban", 1, 1);
        public static readonly EnumArt Aztec2 = new EnumArt("Aztec2", 1, 1);
        public static readonly EnumArt Bomb = new EnumArt("Bomb", 1, 1);
        public static readonly EnumArt Plant = new EnumArt("Plant", 1, 1);
        public static readonly EnumArt Wasteland = new EnumArt("Wasteland", 1, 1);
        public static readonly EnumArt Wanderer = new EnumArt("Wanderer", 1, 2);
        public static readonly EnumArt Graham = new EnumArt("Graham", 1, 2);
        public static readonly EnumArt Pool = new EnumArt("Pool", 2, 1);
        public static readonly EnumArt Courbet = new EnumArt("Courbet", 2, 1);
        public static readonly EnumArt Sunset = new EnumArt("Sunset", 2, 1);
        public static readonly EnumArt Sea = new EnumArt("Sea", 2, 1);
        public static readonly EnumArt Creebet = new EnumArt("Creebet", 2, 1);
        public static readonly EnumArt Match = new EnumArt("Match", 2, 2);
        public static readonly EnumArt Bust = new EnumArt("Bust", 2, 2);
        public static readonly EnumArt Stage = new EnumArt("Stage", 2, 2);
        public static readonly EnumArt Void = new EnumArt("Void", 2, 2);
        public static readonly EnumArt SkullAndRoses = new EnumArt("SkullAndRoses", 2, 2);
        public static readonly EnumArt Fighters = new EnumArt("Fighters", 4, 2);
        public static readonly EnumArt Skeleton = new EnumArt("Skeleton", 4, 3);
        public static readonly EnumArt DonkeyKong = new EnumArt("DonkeyKong", 4, 3);
        public static readonly EnumArt Pointer = new EnumArt("Pointer", 4, 4);
        public static readonly EnumArt Pigscene = new EnumArt("Pigscene", 4, 4);
        public static readonly EnumArt BurningSkull = new EnumArt("Flaming Skull", 4, 4); // Flaming Skull = BurningSkull



        public string title;
        public int sizeX;
        public int sizeY;

        private EnumArt(string title, int sizeX, int sizeY)
        {
            this.title = title;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
        }
    }
}