using System;
using System.IO;

namespace SpoongePE.Core.Game;

using SpoongePE.Core.Game.Generator;
using SpoongePE.Core.Game.material;
using SpoongePE.Core.Game.utils;
using SpoongePE.Core.Game.utils.random;
using SpoongePE.Core.NBT;
using SpoongePE.Core.Network;
using SpoongePE.Core.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class World {
  //  private Chunk[,] _chunks;
    public NbtFile _levelDat;
    public NbtFile _entitiesDat;

    public string LevelName => name;
    public int Seed => worldSeed;
    public int SpawnX => spawnX;
    public int SpawnY => spawnY;
    public int SpawnZ => spawnZ;
    public long Time => worldTime;



    public Chunk this[int x, int z] => _chunks[x, z];


    public Dictionary<int, Player> players = new Dictionary<int, Player>();
    private int freeEID = 1;
    public int worldSeed = 0x256512;
    public BedrockRandom random;
    public Chunk[,] _chunks = new Chunk[16,16];
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
    public static World From(string folder) {
        NbtFile.BigEndianByDefault = false;

        var world = new World() {
            _chunks = new Chunk[16, 16],
            _levelDat = new NbtFile(),
            _entitiesDat = new NbtFile()
        };
        
        world._levelDat.LoadFromFileWithOffset(Path.Combine(folder, "level.dat"), 8);

        world._entitiesDat.LoadFromFileWithOffset(Path.Combine(folder, "entities.dat"), 12);
        
        using var chunksDat = File.OpenRead(Path.Combine(folder, "chunks.dat"));
        using var chunkReader = new BinaryReader(chunksDat);
   
        var chunkMetadata = Chunk.ReadMetadata(chunkReader);

        for (var xz = 0; xz < 16 * 16; xz++) {
            var x = xz % 16;
            var z = xz / 16;

            var offset = chunkMetadata[x, z];
            if (offset == 0)
                continue;

            chunksDat.Seek(offset, SeekOrigin.Begin);
            world._chunks[x, z] = Chunk.From(chunkReader);
        }

        var levelRootTag = world._levelDat.RootTag;
        
        world.name = levelRootTag["LevelName"].StringValue;
        world.worldSeed = (int)levelRootTag["RandomSeed"].LongValue;
        world.spawnX = levelRootTag["SpawnX"].IntValue;
        world.spawnY = levelRootTag["SpawnY"].IntValue;
        world.spawnZ = levelRootTag["SpawnZ"].IntValue;
        world.worldTime = (int)levelRootTag["Time"].LongValue;
        

        return world;
    }

    public void setInitialSpawn()
    {
        int spawnZ = 128, spawnX = 128;
        int spawnY = 127;
        while (true)
        {
            if(spawnY > 0 && this.isAirBlock(spawnX, spawnY, spawnZ))
            {
                --spawnY;
                continue;
            }
            int topBlock = this.getBlockIDAt(spawnX, spawnY, spawnZ);
            if(topBlock != Block.invisibleBedrock.blockID)
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

            if(spawnX > 3)
            {
                if(spawnX > 251) spawnX -= 32;
            }
            else
            {
                spawnX += 32;
            }

            if(spawnZ > 3)
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

/*
    public void SaveWorld(BinaryWriter writer)
    {
        for (var xz = 0; xz < 16 * 16; xz++)
        {
            var x = xz % 16;
            var z = xz / 16;

            var offset = chunkMetadata[x, z];
            if (offset == 0)
                continue;

            chunksDat.Seek(offset, SeekOrigin.Begin);
            world._chunks[x, z] = Chunk.From(chunkReader);
        }
    }*/
    public World() { }
    public void PrintLevelData() {
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
    public void PrintEntitiesData() {
        foreach (var data in _entitiesDat.RootTag)
            Console.WriteLine(data);
    }


    public World(int seed)
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
    }


    public World(string name, int seed)
    {
        this.name = name;
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
        Saver = new WorldSaver(this);
        if (Directory.Exists(name))
        {
            Saver.LoadAll();
        }
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
            this._chunks[x >> 4,z >> 4].BlockData[x & 0xf,z & 0xf,y] = 0;
            this._chunks[x >> 4,z >> 4].BlockMetadata[x & 0xf,z & 0xf,y] = 0;

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
            if (Block.blocks[id].GetType() == typeof(Block)) {
                Block.blocks[id].onNeighborBlockChanged(this, x, y, z, cid);
            }
        }
    }
    internal int getBlockIDAt(int x, int y, int z)
    {
        if (y > 127 || y < 0) return 0;
        if (x > 255 || z > 255 || z < 0 || x < 0) return Block.invisibleBedrock.blockID;
        return this._chunks[x >> 4,z >> 4].BlockData[x & 0xf,z & 0xf,y] & 0xff;
    }
    internal int getBlockMetaAt(int x, int y, int z)
    {
        if (x > 255 || y > 127 || z > 255 || y < 0 || z < 0 || x < 0) return 0;
        return this._chunks[x >> 4,z >> 4].BlockData[x & 0xf,z & 0xf,y];
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
    internal void placeBlockAndNotifyNearby(int x, int y, int z, byte id, byte meta = 0)
    {
        if (x < 256 && y < 128 && z < 256 && y >= 0 && x >= 0 && z >= 0)
        {
            Chunk c = this._chunks[x >> 4,z >> 4];
            c.setBlock(x & 0xf, y, z & 0xf, id, meta);

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
            c.setBlock(x & 0xf, y, z & 0xf, id, meta);

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
            return this._chunks[x >> 4,z >> 4].HeightMap[x & 0xf,z & 0xf];
        }
        return 0;
    }
    internal bool isAirBlock(int x, int y, int z)
    {
        if (x < 256 && y < 128 && z < 256 && y >= 0 && x >= 0 && z >= 0)
        {
            return this._chunks[x >> 4,z >> 4].BlockData[x & 0xf,z & 0xf,y] == 0;
        }
        return false;
    }

    internal int findTopSolidBlock(int x, int z)
    {
        if (x < 256 && z < 256 && x >= 0 && z >= 0)
        {

            byte[] idsY = new byte[128]; // = this.chunks[x >> 4,z >> 4].BlockData[x & 0xf,z & 0xf];
            for(int i = 0; i < 128; i++)
            {
                idsY[i] = this._chunks[x >> 4, z >> 4].BlockData[x & 0xf, z & 0xf,i];
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
                Chunk c = this._chunks[chunkX,chunkZ];
                int l1 = 0;
                do
                {
                    this.randInt1 = this.randInt1 * 3 + this.randInt2;
                    int xyz = this.randInt1 >>> 2;
                    int x = xyz & 0xf;
                    int z = xyz >>> 8 & 0xf;
                    int y = xyz >>> 16 & 0x7f;
                    int id = c.BlockData[x,z,y] & 0xff;
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

    internal int incrementAndGetNextFreeEID()
    {
        throw new NotImplementedException();
    }
}
