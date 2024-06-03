using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.ItemBase;
using SpoongePE.Core.Game.player;
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
        public EntityItem(World paramfd, double paramDouble1, double paramDouble2, double paramDouble3, ItemStack paramiz) : base(paramfd)
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
            this.health = var1.Get<NbtShort>("Health").ShortValue & 255;
            this.age = var1.Get<NbtShort>("Age").ShortValue;
            NbtCompound localnu = var1.Get<NbtCompound>("Item");
            this.item = new ItemStack(localnu);
        }

        protected override void writeEntityToNBT(NbtCompound var1)
        {
            var1.Add(new NbtShort("Health", (short)this.health));
            var1.Add(new NbtShort("Age", (short)this.age));
            NbtCompound itemTag = new NbtCompound("Item");
            item.writeToNBT(itemTag);
            var1.Add(itemTag);
        }

        public new void onCollideWithPlayer(EntityPlayer paramgs)
        {
                int i = this.item.stackSize;
                if (this.delayBeforeCanPickup == 0 && paramgs.inventory.addItemStackToInventory(this.item))
                {
                    paramgs.onItemPickup(this, i);
                    if (this.item.stackSize <= 0)
                    {
                        this.setEntityDead();
                    }
                }
        }
    }
}
