using SpoongePE.Core.Game.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.utils
{
    public class Path
    {
        private PathPoint[] pathPoints = new PathPoint[1024];
        private int count = 0;
        public PathPoint AddPoint(PathPoint var1)
        {
            if (var1.index >= 0)
            {
                throw new InvalidOperationException("OW KNOWS!");
            }
            else
            {
                if (count == pathPoints.Length)
                {
                    PathPoint[] var2 = new PathPoint[count * 2];
                    Array.Copy(pathPoints, 0, var2, 0, count);
                    pathPoints = var2;
                }
                pathPoints[count] = var1;
                var1.index = count;
                SortBack(count++);
                return var1;
            }
        }
        public void ClearPath()
        {
            count = 0;
        }
        public PathPoint Dequeue()
        {
            PathPoint var1 = pathPoints[0];
            pathPoints[0] = pathPoints[--count];
            pathPoints[count] = null;
            if (count > 0)
            {
                SortForward(0);
            }
            var1.index = -1;
            return var1;
        }
        public void ChangeDistance(PathPoint var1, float var2)
        {
            float var3 = var1.distanceToTarget;
            var1.distanceToTarget = var2;
            if (var2 < var3)
            {
                SortBack(var1.index);
            }
            else
            {
                SortForward(var1.index);
            }
        }
        private void SortBack(int var1)
        {
            PathPoint var2 = pathPoints[var1];
            int var4;
            for (float var3 = var2.distanceToTarget; var1 > 0; var1 = var4)
            {
                var4 = var1 - 1 >> 1;
                PathPoint var5 = pathPoints[var4];
                if (var3 >= var5.distanceToTarget)
                {
                    break;
                }
                pathPoints[var1] = var5;
                var5.index = var1;
            }
            pathPoints[var1] = var2;
            var2.index = var1;
        }
        private void SortForward(int var1)
        {
            PathPoint var2 = pathPoints[var1];
            float var3 = var2.distanceToTarget;
            while (true)
            {
                int var4 = 1 + (var1 << 1);
                int var5 = var4 + 1;
                if (var4 >= count)
                {
                    break;
                }
                PathPoint var6 = pathPoints[var4];
                float var7 = var6.distanceToTarget;
                PathPoint var8;
                float var9;
                if (var5 >= count)
                {
                    var8 = null;
                    var9 = float.PositiveInfinity;
                }
                else
                {
                    var8 = pathPoints[var5];
                    var9 = var8.distanceToTarget;
                }
                if (var7 < var9)
                {
                    if (var7 >= var3)
                    {
                        break;
                    }
                    pathPoints[var1] = var6;
                    var6.index = var1;
                    var1 = var4;
                }
                else
                {
                    if (var9 >= var3)
                    {
                        break;
                    }
                    pathPoints[var1] = var8;
                    var8.index = var1;
                    var1 = var5;
                }
            }
            pathPoints[var1] = var2;
            var2.index = var1;
        }
        public bool IsPathEmpty()
        {
            return count == 0;
        }
    }
}
