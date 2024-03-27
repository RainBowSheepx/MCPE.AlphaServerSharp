using MCPE.AlphaServer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPE.AlphaServer.Game.Generator
{
    public class FlatWorldGenerator
    {
        public static void generateChunks(World w)
        {
            for (int x = 0; x < 16; ++x)
            {
                for (int z = 0; z < 16; ++z)
                {
                    Chunk c = new Chunk(x, z);
                    FlatWorldGenerator.generateChunk(c);
                    w._chunks[x,z] = c;
                }
                Logger.Warn("Generating " + x + ": [0-15] chunks");
            }

            w.locationTable = new int[32, 32]; //TODO comp with vanilla
        }

        public static void generateChunk(Chunk c)
        {
            Enumerable.Range(0, 128).ToList().ForEach(delegate (int xz)
            {
                int x = xz & 0xf;
                int z = xz >> 4;
                // Why would we need loops for flat world? // idk
                //       c.GetType();
         
                c.BlockData[x,z,3] = (byte)Block.grass.blockID;
              
                c.BlockMetadata[x,z,3] = (byte)0;
                c.BlockData[x,z,2] = (byte)Block.dirt.blockID;
                c.BlockMetadata[x,z,2] = 0;
                c.BlockData[x,z,1] = (byte)Block.dirt.blockID;
                c.BlockMetadata[x,z,1] = 0;
                c.BlockData[x,z,0] = (byte)Block.bedrock.blockID;
                c.BlockMetadata[x,z,0] = 0;
            });
         
        }
    }
}
