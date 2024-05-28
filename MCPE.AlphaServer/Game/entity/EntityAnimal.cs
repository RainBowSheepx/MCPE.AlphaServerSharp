using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.utils;
using SpoongePE.Core.NBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.entity
{
    public abstract class EntityAnimal : EntityCreature
    {
        public EntityAnimal(World var1) : base(var1)
        {
           
        }

        protected float getBlockPathWeight(int var1, int var2, int var3)
        {
            return this.world.getBlockIDAt(var1, var2 - 1, var3) == Block.grass.blockID ? 10.0F : this.world.getLightBrightness(var1, var2, var3) - 0.5F;
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
            return this.world.getBlockIDAt(var1, var2 - 1, var3) == Block.grass.blockID && this.world.getFullBlockLightValue(var1, var2, var3) > 8 && base.getCanSpawnHere();
        }
    }
}
