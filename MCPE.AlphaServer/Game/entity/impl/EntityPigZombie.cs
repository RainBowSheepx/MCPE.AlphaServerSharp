using SpoongePE.Core.Game.ItemBase;
using System.Collections.Generic;
using System;
using SpoongePE.Core.NBT;
using SpoongePE.Core.Game.player;

namespace SpoongePE.Core.Game.entity.impl
{
    public class EntityPigZombie : EntityZombie
    {
        private int angerLevel = 0;
        private int randomSoundDelay = 0;
        private static ItemStack defaultHeldItem;

        public EntityPigZombie(World var1) : base(var1)
        {
            this.moveSpeed = 0.5F;
            this.attackStrength = 5;
            this.isImmuneToFire = true;
        }

        public new void onUpdate()
        {
            this.moveSpeed = this.playerToAttack != null ? 0.95F : 0.5F;
            base.onUpdate();
        }

        public new bool getCanSpawnHere()
        {
            return this.world.difficultySetting > 0 && this.world.checkIfAABBIsClear(this.boundingBox) && this.world.getCollidingBoundingBoxes(this, this.boundingBox).Count == 0 && !this.world.getIsAnyLiquid(this.boundingBox);
        }

        public new void writeEntityToNBT(NbtCompound var1)
        {
            base.writeEntityToNBT(var1);
            var1.Add(new NbtShort("Anger", (short)this.angerLevel));
        }

        public new void readEntityFromNBT(NbtCompound var1)
        {
            base.readEntityFromNBT(var1);
            this.angerLevel = var1["Anger"].ShortValue;
        }

        protected new Entity findPlayerToAttack()
        {
            return this.angerLevel == 0 ? null : base.findPlayerToAttack();
        }

        public new void onLivingUpdate()
        {
            base.onLivingUpdate();
        }

        public new bool attackEntityFrom(Entity var1, int var2)
        {
            if (var1 is EntityPlayer)
            {
                List<Entity> var3 = this.world.getEntitiesWithinAABBExcludingEntity(this, this.boundingBox.expand(32.0D, 32.0D, 32.0D));

                for (int var4 = 0; var4 < var3.Count; ++var4)
                {
                    Entity var5 = (Entity)var3[var4];
                    if (var5 is EntityPigZombie)
                    {
                        EntityPigZombie var6 = (EntityPigZombie)var5;
                        var6.becomeAngryAt(var1);
                    }
                }

                this.becomeAngryAt(var1);
            }

            return base.attackEntityFrom(var1, var2);
        }

        private void becomeAngryAt(Entity var1)
        {
            this.playerToAttack = var1;
            this.angerLevel = 400 + this.rand.Next(400);
            this.randomSoundDelay = this.rand.Next(40);
        }



        public new ItemStack getHeldItem()
        {
            return defaultHeldItem;
        }
        static EntityPigZombie()
        {
            defaultHeldItem = new ItemStack(Item.swordGold, 1);
        }
    }

}