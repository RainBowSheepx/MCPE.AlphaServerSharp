using SpoongePE.Core.Game.ItemBase;
using SpoongePE.Core.NBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.entity.impl
{
    public class EntityItem : Entity
    {
        public ItemStack item;
        private int field_803_e;
        public int age = 0;
        public int delayBeforeCanPickup;
        private int health = 5;
        public float hoverStart = (float)(new Random().NextDouble() * Math.PI * 2.0D);
        public EntityItem(World w) : base(w)
        {
            setSize(0.25F, 0.25F);
            yOffset = height / 2.0F;
        }
        public EntityItem(World paramfd, double paramDouble1, double paramDouble2, double paramDouble3, ItemStack paramiz) : base(w)
        {
            setSize(0.25F, 0.25F);
            yOffset = height / 2.0F;
            setPosition(paramDouble1, paramDouble2, paramDouble3);
            item = paramiz;
            rotationYaw = (float)(new Random().NextDouble() * 360.0D);
            motionX = (float)(new Random().NextDouble() * 0.2000000029802322D - 0.1000000014901161D);
            motionY = 0.2000000029802322f;
            motionZ = (float)(new Random().NextDouble() * 0.2000000029802322D - 0.1000000014901161D);
        }
        protected override void readEntityFromNBT(NbtCompound var1)
        {
            throw new NotImplementedException();
        }

        protected override void writeEntityToNBT(NbtCompound var1)
        {
            throw new NotImplementedException();
        }
    }
}
