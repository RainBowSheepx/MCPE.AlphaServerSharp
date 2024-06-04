using SpoongePE.Core;
using SpoongePE.Core.Game.BlockBase;
using SpoongePE.Core.Game.entity;
using SpoongePE.Core.Game.utils;
using SpoongePE.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace SpoongePE.Core.Game;

public class Chunk
{
    private const int SectorSize = 0x1000;

    private byte[,,] _blockData = new byte[16, 16, 128];
    private byte[,,] _blockMetadata = new byte[16, 16, 128];
    private byte[,,] _blockLight = new byte[16, 16, 128];
    private byte[,,] _skyLight = new byte[16, 16, 128];
    private byte[,] _heightMap = new byte[16, 16];
    public ArrayList[] entities = new ArrayList[8];

    public byte[,,] BlockData => _blockData;
    public byte[,,] BlockMetadata => _blockMetadata;
    public byte[,,] BlockLight => _blockLight;
    public byte[,,] SkyLight => _skyLight;
    public byte[,] HeightMap => _heightMap;
    public int posX;
    public int posZ;
    public byte[,] updateMap = new byte[16, 16];

    private World world;
    private bool hasEntities = false;

    public Chunk(int x, int z, World world)
    {
        this.posX = x;
        this.posZ = z;
        this.world = world;
        for (int var4 = 0; var4 < this.entities.Length; ++var4)
        {
            this.entities[var4] = new ArrayList();
        }
    }
    public Chunk(byte[,,] blockData, int chunkX, int chunkZ, World world)
    {
        this.posX = chunkX;
        this.posZ = chunkZ;
        this._blockData = blockData;
        this.world = world;
        for (int var4 = 0; var4 < this.entities.Length; ++var4)
        {
            this.entities[var4] = new ArrayList();
        }
    }
    public Chunk()
    {

    }
/*    public void setBlockID(int x, int y, int z, byte id)
    {
        this.BlockData[x, z, y] = id;
        if (id != 0 && this.HeightMap[x, z] < y)
        {
            this.HeightMap[x, z] = (byte)y;
        }
    }*/

    public void setBlockMetadataRaw(int x, int y, int z, byte meta)
    {
        this.BlockMetadata[x, z, y] = meta;
    }

    public void setBlockRaw(int x, int y, int z, byte id, byte meta = 0)
    {
        this.BlockData[x, z, y] = id;
        this.BlockMetadata[x, z, y] = meta;
        if (id != 0 && this.HeightMap[x, z] < y)
        {
            this.HeightMap[x, z] = (byte)y;
        }
    }

    public bool setBlock(int x, int y, int z, byte id, byte meta)
    {

        int idBefore = this.getBlockID(x, y, z) & 0xff;
        if (idBefore == id)
        {
            if (this.getBlockMetadata(x, y, z) == meta) return false;
        }

        int worldX = this.posX * 16 + x;
        int worldZ = this.posZ * 16 + z;

        this._blockData[x, z, y] = (byte)id;

        if (idBefore > 0)
        {
            Block b = Block.blocks[idBefore];
            if (b != null)
            {
                b.onRemove(world, worldX, y, worldZ); //TODO Chunk::world
            }
            else
            {
                Logger.PWarn($"{worldX}-{y}-{worldZ} has unknown block ID({idBefore})!");
            }

            //Removal of TileEntities is also handled here, but they didnt exist until ~0.3
        }
        this.setBlockMetadataRaw(x, y, z, meta);

        //TODO light
        if (id != 0 && this.HeightMap[x, z] < y)
        {
            this.HeightMap[x, z] = (byte)y;
        }

        if (id > 0)
        {
            Block.blocks[id].onBlockAdded(world, worldX, y, worldZ); //TODO Chunk::world
        }

        //this.updateMap[x][z] |= 1 << (y >> 4);
        return true;
    }


    public void generateHeightMap()
    {
        for (int x = 0; x < 16; ++x)
        {
            for (int z = 0; z < 16; ++z)
            {
                byte l = 127;
                for (; l > 0 && (BlockData[x, z, l - 1] & 0xff) == 0; l--) ;

                HeightMap[x, z] = l;
            }
        }
    }

    public static int[,] ReadMetadata(BinaryReader reader)
    {
        var metadata = new int[16, 16];
        for (var offset = 0; offset < SectorSize; offset += 4)
        {
            var chunkMetadata = reader.ReadInt32();
            if (chunkMetadata == 0)
                continue;
            var x = (offset >> 2) % 32;
            var z = (offset >> 2) / 32;
            metadata[x, z] = (chunkMetadata >> 8) * SectorSize;
        }

        return metadata;
    }

    private static void DecompressBlockMetadata(byte[] buffer, Array destination)
    {
        var outputBuffer = new byte[32768];
        for (var offset = 0; offset < outputBuffer.Length / 2; offset += 2)
        {
            var inputByte = buffer[offset / 2];
            outputBuffer[offset] = (byte)(inputByte & 0x0F);
            outputBuffer[offset + 1] = (byte)(inputByte >> 4);
        }

        Buffer.BlockCopy(outputBuffer, 0, destination, 0, outputBuffer.Length);
    }
    public static byte[] CompressBlockMetadata(Array source)
    {
        byte[] buffer = new byte[16384];
        var inputBuffer = new byte[source.Length];
        Buffer.BlockCopy(source, 0, inputBuffer, 0, source.Length);

        for (var offset = 0; offset < inputBuffer.Length; offset += 2)
        {
            var outputByte = (byte)(inputBuffer[offset] | (inputBuffer[offset + 1] << 4));
            buffer[offset / 2] = outputByte;
        }
        return buffer;
    }


    public static Chunk From(BinaryReader reader)
    {
        Debug.Assert(reader.ReadInt32() == 82180);

        var chunkBuffer = reader.ReadBytes(82176);

        var chunk = new Chunk
        {
            _blockData = new byte[16, 16, 128],
            _blockMetadata = new byte[16, 16, 128],
            _blockLight = new byte[16, 16, 128],
            _skyLight = new byte[16, 16, 128]
        };

        const int sliceSize = 16 * 128 * 16;

        Buffer.BlockCopy(chunkBuffer, 0, chunk._blockData, 0, sliceSize);

        DecompressBlockMetadata(chunkBuffer.AsSpan(sliceSize, sliceSize / 2).ToArray(), chunk._blockMetadata);
        DecompressBlockMetadata(chunkBuffer.AsSpan(sliceSize + sliceSize / 2, sliceSize / 2).ToArray(),
            chunk._skyLight
        );
        DecompressBlockMetadata(chunkBuffer.AsSpan(sliceSize + sliceSize, sliceSize / 2).ToArray(), chunk._blockLight);

        return chunk;
    }



    // Garbage for compability
    // --https://github.com/GameHerobrine/Minecraft013-Server/blob/main/src/net/skidcode/gh/server/world/chunk/Chunk.java
    public int getBlockID(int x, int y, int z)
    {
        // TODO: Add more checks
        return this.BlockData[x, z, y];
    }
    public int getBlockMetadata(int x, int y, int z)
    {
        if (y > 127 || y < 0) return 0;// TODO: Add more checks
        /* int index = x << 11 | z << 7 | y;*/
        return BlockMetadata[x, z, y];
    }

    public int getSkylight(int x, int y, int z)
    {
        if (y > 127 || y < 0) return 0; // TODO: Add more checks
     /*   int index = x << 11 | z << 7 | y;*/
        return SkyLight[x, z, y];
    }

    public int getBlockLight(int x, int y, int z)
    {
        if (y > 127 || y < 0) return 0; // TODO: Add more checks
        /*  int index = x << 11 | z << 7 | y;*/
        return BlockLight[x, z, y];
    }
    public bool canBlockSeeTheSky(int var1, int var2, int var3)
    {
        return var2 >= (this.HeightMap[var1, var3] & 255);
    }
    public void getEntitiesWithinAABBForEntity(Entity var1, AxisAlignedBB var2, List<Entity> var3)
    {
        int var4 = MathHelper.floor_double((var2.minY - 2.0D) / 16.0D);
        int var5 = MathHelper.floor_double((var2.maxY + 2.0D) / 16.0D);
        if (var4 < 0)
        {
            var4 = 0;
        }

        if (var5 >= this.entities.Length)
        {
            var5 = this.entities.Length - 1;
        }

        for (int var6 = var4; var6 <= var5; ++var6)
        {
            ArrayList var7 = this.entities[var6];

            for (int var8 = 0; var8 < var7.Count; ++var8)
            {
                Entity var9 = (Entity)var7[var8];
                if (var9 != var1 && var9.boundingBox.intersectsWith(var2))
                {
                    var3.Add(var9);
                }
            }
        }
    }

    internal void addEntity(Entity var1)
    {
        this.hasEntities = true;
        int var2 = MathHelper.floor_double(var1.posX / 16.0D);
        int var3 = MathHelper.floor_double(var1.posZ / 16.0D);
        if (var2 != this.posX || var3 != this.posZ)
        {
            Console.WriteLine("Wrong location! " + var1);
            Console.WriteLine(new StackTrace().ToString());
        }

        int var4 = MathHelper.floor_double(var1.posY / 16.0D);
        if (var4 < 0)
        {
            var4 = 0;
        }

        if (var4 >= this.entities.Length)
        {
            var4 = this.entities.Length - 1;
        }

        var1.addedToChunk = true;
        var1.chunkCoordX = this.posX;
        var1.chunkCoordY = var4;
        var1.chunkCoordZ = this.posZ;
        this.entities[var4].Add(var1);
    }
    public void removeEntity(Entity var1)
    {
        this.removeEntityAtIndex(var1, var1.chunkCoordY);
    }

    public void removeEntityAtIndex(Entity var1, int var2)
    {
        if (var2 < 0)
        {
            var2 = 0;
        }

        if (var2 >= this.entities.Length)
        {
            var2 = this.entities.Length - 1;
        }

        this.entities[var2].Remove(var1);
    }
}
