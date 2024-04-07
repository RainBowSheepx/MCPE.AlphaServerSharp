using SpoongePE.Core.Game.BlockBase;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemTool : Item
    {
        private Block[] blocksEffectiveAgainst;
        private float efficiencyOnProperMaterial = 4.0F;
        private int damageVsEntity;
        protected EnumToolMaterial toolMaterial;

        public ItemTool(int itemID, int var2, EnumToolMaterial var3, Block[] var4) : base(itemID)
        {
            this.toolMaterial = var3;
            this.blocksEffectiveAgainst = var4;
            this.maxStackSize = 1;
            this.setMaxDamage(var3.getMaxUses());
            this.efficiencyOnProperMaterial = var3.getEfficiencyOnProperMaterial();
            this.damageVsEntity = var2 + var3.getDamageVsEntity();
        }

        public new float getStrVsBlock(ItemStack var1, Block var2)
        {
            for (int var3 = 0; var3 < this.blocksEffectiveAgainst.Length; ++var3)
            {
                if (this.blocksEffectiveAgainst[var3] == var2)
                {
                    return this.efficiencyOnProperMaterial;
                }
            }

            return 1.0F;
        }
        /*        public bool hitEntity(ItemStack var1, EntityLiving var2, EntityLiving var3)
        {
            var1.damageItem(2, var3);
            return true;
        }*/
        public new bool onBlockDestroyed(ItemStack var1, int var2, int var3, int var4, int var5, Player var6)
        {
            var1.damageItem(1, var6);
            return true;
        }

        public new int getDamageVsEntity(Entity var1) => this.damageVsEntity;


    }
}
