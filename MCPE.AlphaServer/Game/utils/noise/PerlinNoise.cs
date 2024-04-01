using SpoongePE.Core.Game.utils.random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.utils.noise
{
    public class PerlinNoise
    {
        public ImprovedNoise[] noises;
        public int octavesAmount;
        public PerlinNoise(BedrockRandom bedrockRandom, int octaves)
        {
            this.octavesAmount = octaves;
            this.noises = new ImprovedNoise[octaves];
            for (int i = 0; i < octaves; ++i)
            {
                this.noises[i] = new ImprovedNoise(bedrockRandom);
            }
        }

        public float getValue(float x, float y)
        {
            float noise = 0;
            float scale = 1;

            for (int i = 0; i < this.octavesAmount; i++)
            {
                noise += this.noises[i].getValue(x * scale, y * scale) / scale;
                scale /= 2.0f;
            }

            return noise;
        }
        public float[] getRegion(float[] ad, int i, int j, int k, int l, float d, float d1, float d2)
        {
            return getRegion(ad, i, 10, j, k, 1, l, d, 1, d1);
        }
        public float[] getRegion(float[] noiseArray, float x, float y, float z, int width, int height, int depth, float scaleX, float scaleY, float scaleZ)
        {
            if (noiseArray == null)
            {
                noiseArray = new float[width * height * depth];
            }
            else
            {
               for(int i = 0; i < width * height * depth; ++i)
                {
                    noiseArray[i] = 0;
                }
            }

            float scale = 1;

            for (int i = 0; i < this.octavesAmount; i++)
            {
                this.noises[i].add(noiseArray, x, y, z, width, height, depth, scaleX * scale, scaleY * scale, scaleZ * scale, scale);
                scale /= 2.0f;
            }

            return noiseArray;
        }
    }
}
