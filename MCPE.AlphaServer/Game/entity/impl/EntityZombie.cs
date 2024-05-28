using SpoongePE.Core.Game.ItemBase;
using SpoongePE.Core.Game.utils;
using System;

namespace SpoongePE.Core.Game.entity.impl
{
    public class EntityZombie : EntityMob
    {
        public EntityZombie(World var1) : base(var1)
        {
            this.moveSpeed = 0.5F;
            this.attackStrength = 5;
        }

        public new void onLivingUpdate()
        {
            if (this.world.isDaytime())
            {
                float var1 = this.getEntityBrightness(1.0F);
                if (var1 > 0.5F && this.world.canBlockSeeTheSky(MathHelper.floor_double(this.posX), MathHelper.floor_double(this.posY), MathHelper.floor_double(this.posZ)) && this.rand.NextSingle() * 30.0F < (var1 - 0.4F) * 2.0F)
                {
                    this.fire = 300;
                }
            }

            base.onLivingUpdate();
        }

        protected new int getDropItemId()
        {
            return Item.feather.shiftedIndex;
        }
    }
}