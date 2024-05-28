using SpoongePE.Core.Game.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.entity
{
    public class PathEntity
    {
        private readonly PathPoint[] points;
        public readonly int pathLength;
        private int pathIndex;

        public PathEntity(PathPoint[] var1)
        {
            this.points = var1;
            this.pathLength = var1.Length;
        }

        public void IncrementPathIndex() => this.pathIndex++;


        public bool IsFinished() => this.pathIndex >= this.points.Length;


        public PathPoint Func_22328_c() => this.pathLength > 0 ? this.points[this.pathLength - 1] : null;


        public Vec3D GetPosition(Entity var1)
        {
            double var2 = (double)this.points[this.pathIndex].xCoord + (double)((int)(var1.width + 1.0F)) * 0.5D;
            double var4 = (double)this.points[this.pathIndex].yCoord;
            double var6 = (double)this.points[this.pathIndex].zCoord + (double)((int)(var1.width + 1.0F)) * 0.5D;
            return Vec3D.createVector(var2, var4, var6);
        }
    }
}
