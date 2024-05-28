using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.entity;
using SpoongePE.Core.Game.material;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.utils
{
    public class Pathfinder
    {
        private World _worldMap;
        private Path _path;
        private MCHash _pointMap;
        private PathPoint[] _pathOptions;
        public Pathfinder(World worldMap)
        {
            _worldMap = worldMap;
            _path = new Path();
            _pointMap = new MCHash();
            _pathOptions = new PathPoint[32];
        }
        public PathEntity CreateEntityPathTo(Entity entity, Entity target, float distance)
        {
            return CreateEntityPathTo(entity, target.posX, target.boundingBox.minY, target.posZ, distance);
        }
        public PathEntity CreateEntityPathTo(Entity entity, int x, int y, int z, float distance)
        {
            return CreateEntityPathTo(entity, (double)(x + 0.5F), (double)(y + 0.5F), (double)(z + 0.5F), distance);
        }
        private PathEntity CreateEntityPathTo(Entity entity, double x, double y, double z, float distance)
        {
            _path.ClearPath();
            _pointMap.ClearMap();
      
            PathPoint start = OpenPoint(MathHelper.floor_double(entity.boundingBox.minX), MathHelper.floor_double(entity.boundingBox.minY), MathHelper.floor_double(entity.boundingBox.minZ));
            PathPoint end = OpenPoint(MathHelper.floor_double(x - (double)(entity.width / 2.0F)), MathHelper.floor_double(y), MathHelper.floor_double(z - (double)(entity.width / 2.0F)));
            PathPoint size = new PathPoint(MathHelper.floor_float(entity.width + 1.0F), MathHelper.floor_float(entity.height + 1.0F), MathHelper.floor_float(entity.width + 1.0F));
            PathEntity path = AddToPath(entity, start, end, size, distance);
            return path;
        }
        private PathEntity AddToPath(Entity entity, PathPoint start, PathPoint end, PathPoint size, float distance)
        {
            start.totalPathDistance = 0.0F;
            start.distanceToNext = start.DistanceTo(end);
            start.distanceToTarget = start.distanceToNext;
            _path.ClearPath();
            _path.AddPoint(start);
            PathPoint current = start;
            while (!_path.IsPathEmpty())
            {
                PathPoint point = _path.Dequeue();
                if (point.Equals(end))
                {
                    return CreateEntityPath(start, end);
                }
                if (point.DistanceTo(end) < current.DistanceTo(end))
                {
                    current = point;
                }
                point.isFirst = true;
                int optionsCount = FindPathOptions(entity, point, size, end, distance);
                for (int i = 0; i < optionsCount; i++)
                {
                    PathPoint option = _pathOptions[i];
                    float totalDistance = point.totalPathDistance + point.DistanceTo(option);
                    if (!option.IsAssigned() || totalDistance < option.totalPathDistance)
                    {
                        option.previous = point;
                        option.totalPathDistance = totalDistance;
                        option.distanceToNext = option.DistanceTo(end);
                        if (option.IsAssigned())
                        {
                            _path.ChangeDistance(option, option.totalPathDistance + option.distanceToNext);
                        }
                        else
                        {
                            option.distanceToTarget = option.totalPathDistance + option.distanceToNext;
                            _path.AddPoint(option);
                        }
                    }
                }
            }
            if (current == start)
            {
                return null;
            }
            else
            {
                return CreateEntityPath(start, current);
            }
        }
        private int FindPathOptions(Entity entity, PathPoint point, PathPoint size, PathPoint end, float distance)
        {
            int optionsCount = 0;
            byte verticalOffset = 0;
            if (GetVerticalOffset(entity, point.xCoord, point.yCoord + 1, point.zCoord, size) == 1)
            {
                verticalOffset = 1;
            }
            PathPoint safePoint1 = GetSafePoint(entity, point.xCoord, point.yCoord, point.zCoord + 1, size, verticalOffset);
            PathPoint safePoint2 = GetSafePoint(entity, point.xCoord - 1, point.yCoord, point.zCoord, size, verticalOffset);
            PathPoint safePoint3 = GetSafePoint(entity, point.xCoord + 1, point.yCoord, point.zCoord, size, verticalOffset);
            PathPoint safePoint4 = GetSafePoint(entity, point.xCoord, point.yCoord, point.zCoord - 1, size, verticalOffset);
            if (safePoint1 != null && !safePoint1.isFirst && safePoint1.DistanceTo(end) < distance)
            {
                _pathOptions[optionsCount++] = safePoint1;
            }
            if (safePoint2 != null && !safePoint2.isFirst && safePoint2.DistanceTo(end) < distance)
            {
                _pathOptions[optionsCount++] = safePoint2;
            }
            if (safePoint3 != null && !safePoint3.isFirst && safePoint3.DistanceTo(end) < distance)
            {
                _pathOptions[optionsCount++] = safePoint3;
            }
            if (safePoint4 != null && !safePoint4.isFirst && safePoint4.DistanceTo(end) < distance)
            {
                _pathOptions[optionsCount++] = safePoint4;
            }
            return optionsCount;
        }
        private PathPoint GetSafePoint(Entity entity, int x, int y, int z, PathPoint size, int verticalOffset)
        {
            PathPoint safePoint = null;
            if (GetVerticalOffset(entity, x, y, z, size) == 1)
            {
                safePoint = OpenPoint(x, y, z);
            }
            if (safePoint == null && verticalOffset > 0 && GetVerticalOffset(entity, x, y + verticalOffset, z, size) == 1)
            {
                safePoint = OpenPoint(x, y + verticalOffset, z);
                y += verticalOffset;
            }
            if (safePoint != null)
            {
                int yOffset = 0;
                int offset = 0;
                while (y > 0)
                {
                    offset = GetVerticalOffset(entity, x, y - 1, z, size);
                    if (offset != 1)
                    {
                        break;
                    }
                    yOffset++;
                    if (yOffset >= 4)
                    {
                        return null;
                    }
                    y--;
                    if (y > 0)
                    {
                        safePoint = OpenPoint(x, y, z);
                    }
                }
                if (offset == -2)
                {
                    return null;
                }
            }
            return safePoint;
        }
        private int GetVerticalOffset(Entity entity, int x, int y, int z, PathPoint size)
        {
            for (int i = x; i < x + size.xCoord; i++)
            {
                for (int j = y; j < y + size.yCoord; j++)
                {
                    for (int k = z; k < z + size.zCoord; k++)
                    {
                        int blockId = _worldMap.getBlockIDAt(i, j, k);
                        if (blockId > 0)
                        {
                            if (blockId != Block.ironDoor.blockID && blockId != Block.woodenDoor.blockID)
                            {
                                Material material = Block.blocks[blockId].material;
                                if (material.getIsLiquid())
                                {
                                    return 0;
                                }
                                if (material == Material.water)
                                {
                                    return -1;
                                }
                                if (material == Material.lava)
                                {
                                    return -2;
                                }
                            }
                            else
                            {
                                int metadata = _worldMap.getBlockMetaAt(i, j, k);
  /*                              if (!BlockDoor.Func_27036_e(metadata))
                                {
                                    return 0;
                                }*/
                            }
                        }
                    }
                }
            }
            return 1;
        }
        private PathPoint OpenPoint(int x, int y, int z)
        {
            int key = PathPoint.Func22329A(x, y, z);
            PathPoint point = (PathPoint)_pointMap.Lookup(key);
            if (point == null)
            {
                point = new PathPoint(x, y, z);
                _pointMap.AddKey(key, point);
            }
            return point;
        }
        private PathEntity CreateEntityPath(PathPoint start, PathPoint end)
        {
            int length = 1;
            PathPoint point;
            for (point = end; point.previous != null; point = point.previous)
            {
                length++;
            }
            PathPoint[] pathPoints = new PathPoint[length];
            point = end;
            length--;
            for (pathPoints[length] = end; point.previous != null; pathPoints[length] = point)
            {
                point = point.previous;
                length--;
            }
            return new PathEntity(pathPoints);
        }
    }
}
