using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.utils
{
    public class ChunkCoordinates : IComparable<ChunkCoordinates>
    {
        public int posX;
        public int posY;
        public int posZ;

        public ChunkCoordinates()
        {
        }

        public ChunkCoordinates(int var1, int var2, int var3)
        {
            this.posX = var1;
            this.posY = var2;
            this.posZ = var3;
        }

        public ChunkCoordinates(ChunkCoordinates var1)
        {
            this.posX = var1.posX;
            this.posY = var1.posY;
            this.posZ = var1.posZ;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChunkCoordinates))
            {
                return false;
            }
            else
            {
                ChunkCoordinates other = (ChunkCoordinates)obj;
                return this.posX == other.posX && this.posY == other.posY && this.posZ == other.posZ;
            }
        }

        public override int GetHashCode()
        {
            return this.posX + (this.posZ << 8) + (this.posY << 16);
        }

        public int CompareTo(ChunkCoordinates other)
        {
            return this.posY == other.posY ? (this.posZ == other.posZ ? this.posX - other.posX : this.posZ - other.posZ) : this.posY - other.posY;
        }

        public double GetSqDistanceTo(int var1, int var2, int var3)
        {
            int var4 = this.posX - var1;
            int var5 = this.posY - var2;
            int var6 = this.posZ - var3;
            return Math.Sqrt(var4 * var4 + var5 * var5 + var6 * var6);
        }
    }
}
