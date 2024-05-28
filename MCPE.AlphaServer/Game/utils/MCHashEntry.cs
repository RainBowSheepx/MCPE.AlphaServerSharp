namespace SpoongePE.Core.Game.utils
{
    public class MCHashEntry
    {
        public readonly int HashEntry;
        public object ValueEntry;
        public MCHashEntry NextEntry;
        public readonly int SlotHash;
        public MCHashEntry(int var1, int var2, object var3, MCHashEntry var4)
        {
            ValueEntry = var3;
            NextEntry = var4;
            HashEntry = var2;
            SlotHash = var1;
        }
        public int GetHash()
        {
            return HashEntry;
        }
        public object GetValue()
        {
            return ValueEntry;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is MCHashEntry))
            {
                return false;
            }
            MCHashEntry other = (MCHashEntry)obj;
            int hash = HashEntry;
            int otherHash = other.HashEntry;
            if (hash == otherHash || (hash != 0 && hash.Equals(otherHash)))
            {
                object value = ValueEntry;
                object otherValue = other.ValueEntry;
                return value == otherValue || (value != null && value.Equals(otherValue));
            }
            return false;
        }
        public override int GetHashCode()
        {
            return MCHash.GetHash(HashEntry);
        }
        public override string ToString()
        {
            return $"{HashEntry}={ValueEntry}";
        }
    }
}