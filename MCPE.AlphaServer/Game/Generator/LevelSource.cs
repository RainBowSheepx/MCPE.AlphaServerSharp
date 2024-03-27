using MCPE.AlphaServer.Game.biome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPE.AlphaServer.Game.Generator
{
    public interface LevelSource
    {
        public Chunk getChunk(int chunkX, int chunkZ);
        public void prepareHeights(int chunkX, int chunkZ, byte[,,] blockIDS, Biome[] biomes, float[] temperatures);
        public float[] getHeights(float[] heights, int chunkX, int chunkY, int chunkZ, int scaleX, int scaleY, int scaleZ);
        public void buildSurfaces(int chunkX, int chunkZ, byte[,,] blockIDS, Biome[] biomes);
        public void postProcess(int chunkX, int chunkZ);
    }
}
