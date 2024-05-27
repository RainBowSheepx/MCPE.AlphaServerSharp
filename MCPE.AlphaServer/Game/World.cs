using System;
using System.IO;

namespace SpoongePE.Core.Game;

using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.BlockBase.impl;
using SpoongePE.Core.Game.Generator;
using SpoongePE.Core.Game.material;
using SpoongePE.Core.Game.utils;
using SpoongePE.Core.Game.utils.random;
using SpoongePE.Core.NBT;
using SpoongePE.Core.Network;
using SpoongePE.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class World
{
    //  private Chunk[,] _chunks;
    public NbtFile _levelDat;
    public NbtFile _entitiesDat;

    public string LevelName => name;
    public int Seed => worldSeed;
    public int SpawnX => spawnX;
    public int SpawnY => spawnY;
    public int SpawnZ => spawnZ;
    public long Time => worldTime;

    public Chunk this[int x, int z] => _chunks[x, z]; // Why we don't use it?

    public Dictionary<int, Player> players = new Dictionary<int, Player>();
    private int freeEID = 1;
    public int worldSeed = 0x256512;
    public BedrockRandom random;
    public Chunk[,] _chunks = new Chunk[16, 16];
    public bool instantScheduledUpdate = false, editingBlocks = false;
    public int spawnX, spawnY, spawnZ;
    public string name = "unknown";
    public long worldTime = 0;
    public int saveTime = 0;
    public int[,] locationTable;
    public int unknown5 = 0;
    public BiomeSource biomeSource;
    public LevelSource levelSource;
    public int randInt1, randInt2;
    public WorldSaver Saver;
    public SortedSet<TickNextTickData> scheduledTickTreeSet;
    public HashSet<TickNextTickData> scheduledTickSet;

    public World(int seed, string name = "")
    {

        this.worldSeed = seed;
        this.random = new BedrockRandom(seed);
        this.biomeSource = new BiomeSource(this);
        this.levelSource = new RandomLevelSource(this, seed); //TODO API
        this.scheduledTickTreeSet = new SortedSet<TickNextTickData>();
        this.scheduledTickSet = new HashSet<TickNextTickData>();
        this.randInt1 = 0x283AE83; //it is static in 0.1
        this.randInt2 = 0x3C6EF35F;
        _levelDat = new NbtFile();
        _entitiesDat = new NbtFile();
        if (name.Length < 1) return;
        this.name = name;
        Saver = new WorldSaver(this);
        if (Directory.Exists(Path.Combine("worlds", name)))
        {
            Saver.LoadAll();
        }
    }
    public void PrintLevelData()
    {
        foreach (var data in _levelDat.RootTag)
            Console.WriteLine(data);
    }
    public void addPlayer(Player player)
    {
        this.players.Add(player.EntityID, player);
    }
    public void removePlayer(int eid)
    {
        this.players.Remove(eid);
        /*  RemoveEntityPacket pk = new RemoveEntityPacket();
          pk.EntityId = eid;
          foreach (Player p in this.players.Values)
          {
              p.Send(pk);
          }*/
    }
    public void PrintEntitiesData()
    {
        foreach (var data in _entitiesDat.RootTag)
            Console.WriteLine(data);
    }
    public void setInitialSpawn()
    {
        int spawnZ = 128, spawnX = 128;
        int spawnY = 127;
        while (true)
        {
            if (spawnY > 0 && this.isAirBlock(spawnX, spawnY, spawnZ))
            {
                --spawnY;
                continue;
            }
            int topBlock = this.getBlockIDAt(spawnX, spawnY, spawnZ);
            if (topBlock != Block.invisibleBedrock.blockID)
            {
                Block b = Block.blocks[topBlock];
                if (b != null && b.isSolid)
                {
                    spawnY += 1;
                    break;
                }
            }


            spawnY = 127;
            spawnX += random.nextInt(32) - random.nextInt(32);
            spawnZ += random.nextInt(32) - random.nextInt(32);

            if (spawnX > 3)
            {
                if (spawnX > 251) spawnX -= 32;
            }
            else
            {
                spawnX += 32;
            }

            if (spawnZ > 3)
            {
                if (spawnZ > 251) spawnZ -= 32;
            }
            else
            {
                spawnZ += 32;
            }
        }

        this.spawnX = spawnX;
        this.spawnZ = spawnZ;
        this.spawnY = spawnY;
        Logger.Debug($"Placed spawn on {spawnX} {spawnY} {spawnZ}");
    }

    public bool setBlock(int x, int y, int z, int id, int meta = 0, int flags = 3)
    {
        Chunk c = this._chunks[x >> 4, z >> 4];
        bool s = c.setBlock(x & 0xf, y, z & 0xf, (byte)id, (byte)meta);
        if (s)
        {
            if ((flags & 1) != 0)
            { //update neighbors
                this.notifyNearby(x, y, z, id);
            }

            if ((flags & 2) != 0)
            { //update using level listeners
                this.sendBlockPlace(x, y, z, (byte)c.getBlockID(x & 0xf, y, z & 0xf), (byte)meta); //TODO check
            }
        }
        return s;

    }

    public void notifyNearby(int x, int y, int z, int cid)
    {
        this.notifyNeighbor(x - 1, y, z, cid);
        this.notifyNeighbor(x + 1, y, z, cid);
        this.notifyNeighbor(x, y - 1, z, cid);
        this.notifyNeighbor(x, y + 1, z, cid);
        this.notifyNeighbor(x, y, z - 1, cid);
        this.notifyNeighbor(x, y, z + 1, cid);
    }
    internal void removeBlock(int x, int y, int z)
    {
        if (x < 256 && y < 128 && z < 256 && y >= 0 && x >= 0 && z >= 0)
        {
            this._chunks[x >> 4, z >> 4].BlockData[x & 0xf, z & 0xf, y] = 0;
            this._chunks[x >> 4, z >> 4].BlockMetadata[x & 0xf, z & 0xf, y] = 0;

            this.notifyNearby(x, y, z, 0);
            sendBlockPlace(x, y, z, 0);
        }
    }
    public void notifyNeighbor(int x, int y, int z, int cid)
    {
        if (!editingBlocks)
        {
            int id = this.getBlockIDAt(x, y, z);
            if (id == 0) return;
            if (Block.blocks[id].GetType() == typeof(Block))
            {
                Block.blocks[id].onNeighborBlockChanged(this, x, y, z, cid);
            }
        }
    }
    internal int getBlockIDAt(int x, int y, int z)
    {
        if (y > 127 || y < 0) return 0;
        if (x > 255 || z > 255 || z < 0 || x < 0) return Block.invisibleBedrock.blockID;
        return this._chunks[x >> 4, z >> 4].BlockData[x & 0xf, z & 0xf, y] & 0xff;
    }
    internal int getBlockMetaAt(int x, int y, int z)
    {
        if (x > 255 || y > 127 || z > 255 || y < 0 || z < 0 || x < 0) return 0;
        return this._chunks[x >> 4, z >> 4].BlockData[x & 0xf, z & 0xf, y];
    }
    public void sendBlockPlace(int x, int y, int z, byte id, byte meta = 0)
    {
        UpdateBlockPacket pk = new UpdateBlockPacket();
        pk.X = x;
        pk.Y = (byte)y;
        pk.Z = z;
        pk.Block = id;
        pk.Meta = meta;

        broadcastPacket(pk);
    }
    internal void placeBlockAndNotifyNearby(int x, int y, int z, byte id, byte meta = 0) // duplicate setBlock
    {
        if (x < 256 && y < 128 && z < 256 && y >= 0 && x >= 0 && z >= 0)
        {
            Chunk c = this._chunks[x >> 4, z >> 4];
            c.setBlockRaw(x & 0xf, y, z & 0xf, id, meta);

            if (id > 0) Block.blocks[id].onBlockAdded(this, x, y, z);

            this.notifyNearby(x, y, z, id);
            this.sendBlockPlace(x, y, z, id, meta);
        }
    }


    internal void placeBlock(int x, int y, int z, byte id, byte meta = 0)
    {
        if (x < 256 && y < 128 && z < 256 && y >= 0 && x >= 0 && z >= 0)
        {
            Chunk c = this._chunks[x >> 4, z >> 4];
            c.setBlockRaw(x & 0xf, y, z & 0xf, id, meta);

            if (id > 0) Block.blocks[id].onBlockAdded(this, x, y, z);

            this.sendBlockPlace(x, y, z, id, meta);
        }
    }
    internal bool isBlockSolid(int x, int y, int z)
    {
        Block b = Block.blocks[this.getBlockIDAt(x, y, z)];
        return (b.GetType() == typeof(Block) && b.material.isSolid);
    }
    internal void broadcastPacket(UpdateBlockPacket pk)
    {
        foreach (Player pl in this.players.Values)
        {
            pl.Send(pk);
        }
    }








    internal void addToTickNextTick(int x, int y, int z, int id, int delay)
    {
        TickNextTickData tick = new TickNextTickData(x, y, z, id);
        if (this.instantScheduledUpdate)
        {
            if (this.hasChunksAt(tick.posX - 8, tick.posY - 8, tick.posZ - 8, tick.posX + 8, tick.posY + 8, tick.posZ + 8))
            {
                int worldID = this.getBlockIDAt(x, y, z);
                if (worldID == tick.blockID && worldID > 0)
                {
                    Block.blocks[worldID].tick(this, x, y, z, this.random);
                }
            }
        }
        else if (this.hasChunksAt(x - 8, y - 8, z - 8, x + 8, y + 8, z + 8))
        {
            if (id > 0) tick.scheduledTime = delay + this.worldTime;
            if (!scheduledTickSet.Contains(tick))
            {
                scheduledTickSet.Add(tick);
                scheduledTickTreeSet.Add(tick);
            }
        }
    }

    private bool hasChunksAt(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
    {
        if (minY > -1 && minY < 128)
        {
            for (int chunkX = minX >> 4; chunkX <= maxX >> 4; ++chunkX)
            {
                for (int chunkZ = minZ >> 4; chunkZ <= maxZ >> 4; ++chunkZ)
                {
                    if (chunkX < 0 || chunkZ < 0 || chunkX > 15 || chunkZ > 15 || this._chunks[chunkX, chunkZ] == null) return false;
                }
            }
            return true;
        }
        return false;
    }
    internal Material getMaterial(int x, int y, int z)
    {
        int blockID = this.getBlockIDAt(x, y, z);
        Block b = Block.blocks[blockID];

        if (b != null)
        {
            return b.material;
        }
        return Material.air;
    }


    internal int getHeightValue(int x, int z)
    {
        if (x < 256 && z < 256 && x >= 0 && z >= 0)
        {
            return this._chunks[x >> 4, z >> 4].HeightMap[x & 0xf, z & 0xf];
        }
        return 0;
    }
    internal bool isAirBlock(int x, int y, int z)
    {
        if (x < 256 && y < 128 && z < 256 && y >= 0 && x >= 0 && z >= 0)
        {
            return this._chunks[x >> 4, z >> 4].BlockData[x & 0xf, z & 0xf, y] == 0;
        }
        return false;
    }

    internal int findTopSolidBlock(int x, int z)
    {
        if (x < 256 && z < 256 && x >= 0 && z >= 0)
        {

            byte[] idsY = new byte[128]; // = this.chunks[x >> 4,z >> 4].BlockData[x & 0xf,z & 0xf];
            for (int i = 0; i < 128; i++)
            {
                idsY[i] = this._chunks[x >> 4, z >> 4].BlockData[x & 0xf, z & 0xf, i];
            }
            int k = 127;
            byte id;

            do
            {
                id = (byte)(idsY[k] & 0xff);
                Material m = id == 0 ? Material.air : Block.blocks[id].material;
                if (m.isSolid || m.isLiquid) return k + 1;
            } while (k-- > 0); //TODO why did i change it?
        }
        return -1;
    }
    public async Task tick()
    {

        /*Timer*/
        this.worldTime++;
        //Normal Ticking: Water/Lava
        int ticksAmount = scheduledTickTreeSet.Count;
        if (ticksAmount > 1000) ticksAmount = 1000;
        for (int i = 0; i < ticksAmount; ++i)
        {
            TickNextTickData tick = scheduledTickTreeSet.First();
            if (tick.scheduledTime > this.worldTime)
            {
                break;
            }
            scheduledTickTreeSet.Remove(tick);
            scheduledTickSet.Remove(tick);
            if (this.hasChunksAt(tick.posX - 8, tick.posY - 8, tick.posY - 8, tick.posX + 8, tick.posY + 8, tick.posY + 8))
            {
                int id = this.getBlockIDAt(tick.posX, tick.posY, tick.posZ);
                if (id > 0 && id == tick.blockID)
                {
                    // Async moment
                    await Task.Run(() => Block.blocks[id].tick(this, tick.posX, tick.posY, tick.posZ, random));
                }
            }
        }

        //Random Ticking

        for (int chunkX = 0; chunkX < 16; ++chunkX)
        {
            for (int chunkZ = 0; chunkZ < 16; ++chunkZ)
            {
                Chunk c = this._chunks[chunkX, chunkZ];
                int l1 = 0;
                do
                {
                    this.randInt1 = this.randInt1 * 3 + this.randInt2;
                    int xyz = this.randInt1 >>> 2;
                    int x = xyz & 0xf;
                    int z = xyz >>> 8 & 0xf;
                    int y = xyz >>> 16 & 0x7f;
                    int id = c.BlockData[x, z, y] & 0xff;
                    if (Block.shouldTick[id])
                    {
                        await Task.Run(() => Block.blocks[id].tick(this, x + (c.posX << 4), y, z + (c.posZ << 4), random));
                    }
                } while (++l1 <= 80);
            }
        }

    }
    internal bool canSeeSky(int x, int y, int z)
    {
        if (x < 256 && y < 128 && z < 256 && y >= 0 && x >= 0 && z >= 0)
        {
            return y >= this._chunks[x >> 4, z >> 4].HeightMap[x & 0xf, z & 0xf];
        }
        return false;
    }
    public bool mayPlace(int blockID, int x, int y, int z, bool tgl)
    {

        int blockAt = this.getBlockIDAt(x, y, z);
        //TODO aabbs checks // no
        if (blockAt == Block.waterStill.blockID || blockAt == Block.waterFlowing.blockID || blockAt == Block.lavaStill.blockID || blockAt == Block.lavaFlowing.blockID || blockAt == Block.fire.blockID || blockAt == Block.snowLayer.blockID)
        {
            return true;
        }

        return blockID > 0 && Block.blocks[blockAt] == null && Block.blocks[blockID].mayPlace(this, x, y, z);
    }
    internal int incrementAndGetNextFreeEID()
    {
        throw new NotImplementedException();
    }

    internal void entityJoinedWorld(Entity ent)
    {
        throw new NotImplementedException();
    }
    public bool blockExists(int var1, int var2, int var3) => var2 >= 0 && var2 < 128 ? this.chunkExists(var1 >> 4, var3 >> 4) : false;

    private bool chunkExists(int var1, int var2)
    {
        // maybe outofbounds
        return this._chunks[var1, var2] != null;
    }
    private List<AxisAlignedBB> collidingBoundingBoxes = new List<AxisAlignedBB>();
    private List<Entity> field_1012_M = new List<Entity>();
    public List<AxisAlignedBB> getCollidingBoundingBoxes(Entity var1, AxisAlignedBB var2)
    {
        this.collidingBoundingBoxes.Clear();
        int var3 = MathHelper.floor_double(var2.minX);
        int var4 = MathHelper.floor_double(var2.maxX + 1.0D);
        int var5 = MathHelper.floor_double(var2.minY);
        int var6 = MathHelper.floor_double(var2.maxY + 1.0D);
        int var7 = MathHelper.floor_double(var2.minZ);
        int var8 = MathHelper.floor_double(var2.maxZ + 1.0D);

        for (int var9 = var3; var9 < var4; ++var9)
        {
            for (int var10 = var7; var10 < var8; ++var10)
            {
                if (this.blockExists(var9, 64, var10))
                {
                    for (int var11 = var5 - 1; var11 < var6; ++var11)
                    {
                        Block var12 = Block.blocks[this.getBlockIDAt(var9, var11, var10)];
                        if (var12 != null)
                        {
                            var12.getCollidingBoundingBoxes(this, var9, var11, var10, var2, this.collidingBoundingBoxes);
                        }
                    }
                }
            }
        }

        double var14 = 0.25D;
        List<Entity> var15 = this.getEntitiesWithinAABBExcludingEntity(var1, var2.expand(var14, var14, var14));

        for (int var16 = 0; var16 < var15.Count(); ++var16)
        {
            AxisAlignedBB var13 = ((Entity)var15[var16]).getBoundingBox();
            if (var13 != null && var13.intersectsWith(var2))
            {
                this.collidingBoundingBoxes.Add(var13);
            }

            var13 = var1.getCollisionBox((Entity)var15[var16]);
            if (var13 != null && var13.intersectsWith(var2))
            {
                this.collidingBoundingBoxes.Add(var13);
            }
        }

        return this.collidingBoundingBoxes;
    }
    public List<Entity> getEntitiesWithinAABBExcludingEntity(Entity var1, AxisAlignedBB var2)
    {
        this.field_1012_M.Clear();
        int var3 = MathHelper.floor_double((var2.minX - 2.0D) / 16.0D);
        int var4 = MathHelper.floor_double((var2.maxX + 2.0D) / 16.0D);
        int var5 = MathHelper.floor_double((var2.minZ - 2.0D) / 16.0D);
        int var6 = MathHelper.floor_double((var2.maxZ + 2.0D) / 16.0D);

        for (int var7 = var3; var7 <= var4; ++var7)
        {
            for (int var8 = var5; var8 <= var6; ++var8)
            {
                if (this.chunkExists(var7, var8))
                {
                    this._chunks[var7, var8].getEntitiesWithinAABBForEntity(var1, var2, this.field_1012_M);
                }
            }
        }

        return this.field_1012_M;
    }

    public bool handleMaterialAcceleration(AxisAlignedBB var1, Material var2, Entity var3)
    {
        int var4 = MathHelper.floor_double(var1.minX);
        int var5 = MathHelper.floor_double(var1.maxX + 1.0D);
        int var6 = MathHelper.floor_double(var1.minY);
        int var7 = MathHelper.floor_double(var1.maxY + 1.0D);
        int var8 = MathHelper.floor_double(var1.minZ);
        int var9 = MathHelper.floor_double(var1.maxZ + 1.0D);

        bool var10 = false;
        Vec3D var11 = Vec3D.createVector(0.0D, 0.0D, 0.0D);

        for (int var12 = var4; var12 < var5; ++var12)
        {
            for (int var13 = var6; var13 < var7; ++var13)
            {
                for (int var14 = var8; var14 < var9; ++var14)
                {
                    Block var15 = Block.blocks[this.getBlockIDAt(var12, var13, var14)];
                    if (var15 != null && var15.material == var2)
                    {
                        double var16 = (double)((float)(var13 + 1) - LiquidBaseBlock.getPercentAir(this.getBlockMetaAt(var12, var13, var14)));
                        if ((double)var7 >= var16)
                        {
                            var10 = true;
                            var15.velocityToAddToEntity(this, var12, var13, var14, var3, var11);
                        }
                    }
                }
            }
        }

        if (var11.lengthVector() > 0.0D)
        {
            var11 = var11.normalize();
            double var18 = 0.014D;
            var3.motionX += (float)(var11.xCoord * var18);
            var3.motionY += (float)(var11.yCoord * var18);
            var3.motionZ += (float)(var11.zCoord * var18);
        }

        return var10;

    }
    public bool isMaterialInBB(AxisAlignedBB var1, Material var2)
    {
        int var3 = MathHelper.floor_double(var1.minX);
        int var4 = MathHelper.floor_double(var1.maxX + 1.0D);
        int var5 = MathHelper.floor_double(var1.minY);
        int var6 = MathHelper.floor_double(var1.maxY + 1.0D);
        int var7 = MathHelper.floor_double(var1.minZ);
        int var8 = MathHelper.floor_double(var1.maxZ + 1.0D);

        for (int var9 = var3; var9 < var4; ++var9)
        {
            for (int var10 = var5; var10 < var6; ++var10)
            {
                for (int var11 = var7; var11 < var8; ++var11)
                {
                    Block var12 = Block.blocks[this.getBlockIDAt(var9, var10, var11)];
                    if (var12 != null && var12.material == var2)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool isAABBInMaterial(AxisAlignedBB var1, Material var2)
    {
        int var3 = MathHelper.floor_double(var1.minX);
        int var4 = MathHelper.floor_double(var1.maxX + 1.0D);
        int var5 = MathHelper.floor_double(var1.minY);
        int var6 = MathHelper.floor_double(var1.maxY + 1.0D);
        int var7 = MathHelper.floor_double(var1.minZ);
        int var8 = MathHelper.floor_double(var1.maxZ + 1.0D);

        for (int var9 = var3; var9 < var4; ++var9)
        {
            for (int var10 = var5; var10 < var6; ++var10)
            {
                for (int var11 = var7; var11 < var8; ++var11)
                {
                    Block var12 = Block.blocks[this.getBlockIDAt(var9, var10, var11)];
                    if (var12 != null && var12.material == var2)
                    {
                        int var13 = this.getBlockMetaAt(var9, var10, var11);
                        double var14 = (double)(var10 + 1);
                        if (var13 < 8)
                        {
                            var14 = (double)(var10 + 1) - (double)var13 / 8.0D;
                        }

                        if (var14 >= var1.minY)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public bool checkChunksExist(int var38, int var26, int var39, int var28, int var40, int var30)
    {
        return true;
    }

    internal bool isBoundingBoxBurning(AxisAlignedBB var1)
    {
        int var2 = MathHelper.floor_double(var1.minX);
        int var3 = MathHelper.floor_double(var1.maxX + 1.0D);
        int var4 = MathHelper.floor_double(var1.minY);
        int var5 = MathHelper.floor_double(var1.maxY + 1.0D);
        int var6 = MathHelper.floor_double(var1.minZ);
        int var7 = MathHelper.floor_double(var1.maxZ + 1.0D);
        if (this.checkChunksExist(var2, var4, var6, var3, var5, var7))
        {
            for (int var8 = var2; var8 < var3; ++var8)
            {
                for (int var9 = var4; var9 < var5; ++var9)
                {
                    for (int var10 = var6; var10 < var7; ++var10)
                    {
                        int var11 = this.getBlockIDAt(var8, var9, var10);
                        if (var11 == Block.fire.blockID || var11 == Block.lavaFlowing.blockID || var11 == Block.lavaStill.blockID)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    internal bool isBlockNormalCube(int var5, int var6, int var7)
    {
        throw new NotImplementedException();
    }
}
