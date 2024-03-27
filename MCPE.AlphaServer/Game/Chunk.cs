using System;
using System.Diagnostics;
using System.IO;

namespace MCPE.AlphaServer.Game;

public class Chunk {
    private const int SectorSize = 0x1000;

    private byte[,,] _blockData = new byte[16, 16, 128];
    private byte[,,] _blockMetadata = new byte[16,16, 128];
    private byte[,,] _blockLight = new byte[16, 16, 128];
    private byte[,,] _skyLight = new byte[16, 16, 128];
    private byte[,] _heightMap = new byte[16, 16];


    public byte[,,] BlockData => _blockData;
    public byte[,,] BlockMetadata => _blockMetadata;
    public byte[,,] BlockLight => _blockLight;
    public byte[,,] SkyLight => _skyLight;
    public byte[,] HeightMap => _heightMap;
    public int posX;
    public int posZ;
    public byte[,] updateMap = new byte[16, 16];

    public Chunk(int x, int z)
    {
        this.posX = x;
        this.posZ = z;
    }
    public Chunk(byte[,,] blockData, int chunkX, int chunkZ)
    {
        this.posX = chunkX;
        this.posZ = chunkZ;
        this._blockData = blockData;
    }
    public Chunk()
    {

    }
    public void setBlockID(int x, int y, int z, byte id)
    {
        this.BlockData[x,z,y] = id;
        if (id != 0 && this.HeightMap[x, z] < y)
        {
            this.HeightMap[x, z] = (byte)y;
        }
    }

    public void setBlockMetadata(int x, int y, int z, byte meta)
    {
        this.BlockMetadata[x, z, y] = meta;
    }

    public void setBlock(int x, int y, int z, byte id, byte meta = 0)
    {
        this.BlockData[x,z,y] = id;
        this.BlockMetadata[x,z,y] = meta;
        if (id != 0 && this.HeightMap[x, z] < y)
        {
            this.HeightMap[x, z] = (byte)y;
        }
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

    public static int[,] ReadMetadata(BinaryReader reader) {
        var metadata = new int[16, 16];
        for (var offset = 0; offset < SectorSize; offset += 4) {
            var chunkMetadata = reader.ReadInt32();
            if (chunkMetadata == 0)
                continue;

            var x = (offset >> 2) % 32;
            var z = (offset >> 2) / 32;

            metadata[x, z] = (chunkMetadata >> 8) * SectorSize;
        }

        return metadata;
    }

    private static void DecompressBlockMetadata(byte[] buffer, Array destination) {
        var outputBuffer = new byte[32768];
        for (var offset = 0; offset < outputBuffer.Length / 2; offset += 2) {
            var inputByte = buffer[offset / 2];
            outputBuffer[offset] = (byte)(inputByte & 0x0F);
            outputBuffer[offset + 1] = (byte)(inputByte >> 4);
        }

        Buffer.BlockCopy(outputBuffer, 0, destination, 0, outputBuffer.Length);
    }

    public static Chunk From(BinaryReader reader) {
        Debug.Assert(reader.ReadInt32() == 82180);

        var chunkBuffer = reader.ReadBytes(82176);

        var chunk = new Chunk {
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
}
