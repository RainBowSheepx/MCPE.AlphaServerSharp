using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.entity
{
    public class PathPoint
    {
        public readonly int xCoord;
        public readonly int yCoord;
        public readonly int zCoord;
        private readonly int hash;
        public int index = -1;
        public float totalPathDistance;
        public float distanceToNext;
        public float distanceToTarget;
        public PathPoint previous;
        public bool isFirst = false;

        public PathPoint(int var1, int var2, int var3)
        {
            xCoord = var1;
            yCoord = var2;
            zCoord = var3;
            hash = Func22329A(var1, var2, var3);
        }

        public static int Func22329A(int var0, int var1, int var2)
        {
            return var1 & 255 | (var0 & 32767) << 8 | (var2 & 32767) << 24 | (var0 < 0 ? int.MinValue : 0) | (var2 < 0 ? '\u8000' : 0);
        }

        public float DistanceTo(PathPoint var1)
        {
            float var2 = (float)(var1.xCoord - xCoord);
            float var3 = (float)(var1.yCoord - yCoord);
            float var4 = (float)(var1.zCoord - zCoord);
            return (float)Math.Sqrt(var2 * var2 + var3 * var3 + var4 * var4);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PathPoint))
            {
                return false;
            }
            PathPoint other = (PathPoint)obj;
            return hash == other.hash && xCoord == other.xCoord && yCoord == other.yCoord && zCoord == other.zCoord;
        }

        public override int GetHashCode()
        {
            return hash;
        }

        public bool IsAssigned()
        {
            return index >= 0;
        }

        public override string ToString()
        {
            return xCoord + ", " + yCoord + ", " + zCoord;
        }
    }
}
