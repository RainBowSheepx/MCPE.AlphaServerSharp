using SpoongePE.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.Generator
{
    public class NormalWorldGenerator
    {
        public static void generateChunks(World w)
        {
            for (int x = 0; x < 16; ++x)
            {
                Logger.Info("Generating " + x + ": [0-15] chunks");
                for (int z = 0; z < 16; ++z)
                {
                    w._chunks[x, z] = w.levelSource.getChunk(x, z);
                }
            }
            for (int x = 0; x < 16; ++x)
            {
                Logger.Info("Populating " + x + ": [0-15] chunks");
                for (int z = 0; z < 16; ++z)
                {
                    //TODO w.levelSource.postProcess(x, z);
                }
            }
            w.locationTable = new int[32, 32]; //TODO comp with vanilla
        }
    }
}
