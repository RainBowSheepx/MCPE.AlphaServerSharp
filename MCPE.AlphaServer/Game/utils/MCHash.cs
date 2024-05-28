using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.utils
{
    public class MCHash
    {
        private MCHashEntry[] slots = new MCHashEntry[16];
        private int count;
        private int threshold = 12;
        private float growFactor = 12.0f / 16.0f;
        private volatile int versionStamp;
        private static int ComputeHash(int var0)
        {
            var0 ^= var0 >> 20 ^ var0 >> 12;
            return var0 ^ var0 >> 7 ^ var0 >> 4;
        }
        private static int GetSlotIndex(int var0, int var1)
        {
            return var0 & var1 - 1;
        }
        public object Lookup(int var1)
        {
            int var2 = ComputeHash(var1);
            for (MCHashEntry var3 = this.slots[GetSlotIndex(var2, this.slots.Length)]; var3 != null; var3 = var3.NextEntry)
            {
                if (var3.HashEntry == var1) return var3.ValueEntry;
            }
            return null;
        }
        public bool ContainsItem(int var1)
        {
            return this.LookupEntry(var1) != null;
        }
        private MCHashEntry LookupEntry(int var1)
        {
            int var2 = ComputeHash(var1);
            for (MCHashEntry var3 = this.slots[GetSlotIndex(var2, this.slots.Length)]; var3 != null; var3 = var3.NextEntry)
            {
                if (var3.HashEntry == var1) return var3;
            }
            return null;
        }
        public void AddKey(int var1, object var2)
        {
            int var3 = ComputeHash(var1);
            int var4 = GetSlotIndex(var3, this.slots.Length);
            for (MCHashEntry var5 = this.slots[var4]; var5 != null; var5 = var5.NextEntry)
            {
                if (var5.HashEntry == var1) var5.ValueEntry = var2;
            }
            ++this.versionStamp;
            this.Insert(var3, var1, var2, var4);
        }
        private void Grow(int var1)
        {
            MCHashEntry[] var2 = this.slots;
            int var3 = var2.Length;
            if (var3 == 1073741824) this.threshold = int.MaxValue;
            else
            {
                MCHashEntry[] var4 = new MCHashEntry[var1];
                this.CopyTo(var4);
                this.slots = var4;
                this.threshold = (int)((float)var1 * this.growFactor);
            }
        }
        private void CopyTo(MCHashEntry[] var1)
        {
            MCHashEntry[] var2 = this.slots;
            int var3 = var1.Length;
            for (int var4 = 0; var4 < var2.Length; ++var4)
            {
                MCHashEntry var5 = var2[var4];
                if (var5 != null)
                {
                    var2[var4] = null;
                    MCHashEntry var6;
                    do
                    {
                        var6 = var5.NextEntry;
                        int var7 = GetSlotIndex(var5.SlotHash, var3);
                        var5.NextEntry = var1[var7];
                        var1[var7] = var5;
                        var5 = var6;
                    } while (var6 != null);
                }
            }
        }
        public object RemoveObject(int var1)
        {
            MCHashEntry var2 = this.RemoveEntry(var1);
            return var2 == null ? null : var2.ValueEntry;
        }
        private MCHashEntry RemoveEntry(int var1)
        {
            int var2 = ComputeHash(var1);
            int var3 = GetSlotIndex(var2, this.slots.Length);
            MCHashEntry var4 = this.slots[var3];
            MCHashEntry var5;
            MCHashEntry var6;
            for (var5 = var4; var5 != null; var5 = var6)
            {
                var6 = var5.NextEntry;
                if (var5.HashEntry == var1)
                {
                    ++this.versionStamp;
                    --this.count;
                    if (var4 == var5) this.slots[var3] = var6;
                    else var4.NextEntry = var6;
                    return var5;
                }
                var4 = var5;
            }
            return var5;
        }
        public void ClearMap()
        {
            ++this.versionStamp;
            MCHashEntry[] var1 = this.slots;
            for (int var2 = 0; var2 < var1.Length; ++var2)
            {
                var1[var2] = null;
            }
            this.count = 0;
        }
        private void Insert(int var1, int var2, object var3, int var4)
        {
            MCHashEntry var5 = this.slots[var4];
            this.slots[var4] = new MCHashEntry(var1, var2, var3, var5);
            if (this.count++ >= this.threshold) this.Grow(2 * this.slots.Length);
        }
        public static int GetHash(int var0)
        {
            return ComputeHash(var0);
        }
    }
}
