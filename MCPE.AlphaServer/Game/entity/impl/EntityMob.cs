using SpoongePE.Core.Game.player;
using SpoongePE.Core.Game.utils;
using SpoongePE.Core.NBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.entity.impl
{
    public class EntityMob : EntityCreature
    {
        protected int attackStrength = 2;

        public EntityMob(World var1) : base(var1)
        {
            this.health = 20;
        }

        public new void onLivingUpdate()
        {
            float var1 = this.getEntityBrightness(1.0F);
            if (var1 > 0.5F)
            {
                this.entityAge += 2;
            }

            base.onLivingUpdate();
        }

  

        public new void onUpdate()
        {
            base.onUpdate();
            if (this.world.difficultySetting == 0)
            {
                this.setEntityDead();
            }

        }

        protected new Entity findPlayerToAttack()
        {
            EntityPlayer var1 = this.world.getClosestPlayerToEntity(this, 16.0D);
            return var1 != null && this.canEntityBeSeen(var1) ? var1 : null;
        }

        public new bool attackEntityFrom(Entity var1, int var2)
        {
            if (base.attackEntityFrom(var1, var2))
            {
                if (this.riddenByEntity != var1 && this.ridingEntity != var1)
                {
                    if (var1 != this)
                    {
                        this.playerToAttack = var1;
                    }

                    return true;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        protected new void attackEntity(Entity var1, float var2)
        {
            if (this.attackTime <= 0 && var2 < 2.0F && var1.boundingBox.maxY > this.boundingBox.minY && var1.boundingBox.minY < this.boundingBox.maxY)
            {
                this.attackTime = 20;
                var1.attackEntityFrom(this, this.attackStrength);
            }

        }

        protected new float getBlockPathWeight(int var1, int var2, int var3)
        {
            return 0.5F - this.world.getLightBrightness(var1, var2, var3);
        }

        public new void writeEntityToNBT(NbtCompound var1)
        {
            base.writeEntityToNBT(var1);
        }

        public new void readEntityFromNBT(NbtCompound var1)
        {
            base.readEntityFromNBT(var1);
        }

        public new bool getCanSpawnHere()
        {
            int var1 = MathHelper.floor_double(this.posX);
            int var2 = MathHelper.floor_double(this.boundingBox.minY);
            int var3 = MathHelper.floor_double(this.posZ);
            if (this.world.getSavedLightValue(EnumSkyBlock.Sky, var1, var2, var3) > this.rand.Next(32))
            {
                return false;
            }
            else
            {
                int var4 = this.world.getBlockLightValue(var1, var2, var3);

                return var4 <= this.rand.Next(8) && base.getCanSpawnHere();
            }
        }
    }
}
