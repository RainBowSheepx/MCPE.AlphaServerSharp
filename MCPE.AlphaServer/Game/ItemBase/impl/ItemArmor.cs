using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.ItemBase.impl
{
    public class ItemArmor : Item
    {
        private static int[] damageReduceAmountArray = new int[] { 3, 8, 6, 3 };
        private static int[] maxDamageArray = new int[] { 11, 16, 15, 13 };
        public int armorLevel;
        public int armorType;
        public int damageReduceAmount;
        public ItemArmor(int itemID, int var2, int var3, int var4) : base(itemID)
        {
            this.armorLevel = var2;
            this.armorType = var4;
          //  this.renderIndex = var3;
            this.damageReduceAmount = damageReduceAmountArray[var4];
            this.setMaxDamage(maxDamageArray[var4] * 3 << var2);
            this.maxStackSize = 1;
        }
    }
}
