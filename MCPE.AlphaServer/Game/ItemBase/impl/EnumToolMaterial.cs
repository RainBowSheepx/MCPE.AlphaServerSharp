using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class EnumToolMaterial
    {
        public static readonly EnumToolMaterial WOOD = new EnumToolMaterial(0, 59, 2.0F, 0);
        public static readonly EnumToolMaterial STONE = new EnumToolMaterial(1, 131, 4.0F, 1);
        public static readonly EnumToolMaterial IRON = new EnumToolMaterial(2, 250, 6.0F, 2);
        public static readonly EnumToolMaterial EMERALD = new EnumToolMaterial(3, 1561, 8.0F, 3);
        public static readonly EnumToolMaterial GOLD = new EnumToolMaterial(0, 32, 12.0F, 0);

        public static IEnumerable<EnumToolMaterial> Values
        {
            get
            {
                yield return WOOD;
                yield return STONE;
                yield return IRON;
                yield return EMERALD;
                yield return GOLD;
            }
        }

        public int harvestLevel { get; private set; }
        public int maxUses { get; private set; }
        public float efficiencyOnProperMaterial { get; private set; }

        public int damageVsEntity { get; private set; }

        EnumToolMaterial(int d, int i, float c, int k) =>
            (harvestLevel, maxUses, efficiencyOnProperMaterial, damageVsEntity) = (d, i, c, k);


        public int getMaxUses() => maxUses;


        public float getEfficiencyOnProperMaterial() => efficiencyOnProperMaterial;


        public int getDamageVsEntity() => damageVsEntity;


        public int getHarvestLevel() => harvestLevel;

    }
}
