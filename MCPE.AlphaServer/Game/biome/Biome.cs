using SpoongePE.Core.Game.biome.impl;
using SpoongePE.Core.Game.feature;
using SpoongePE.Core.Game.utils.random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.biome
{
    public class Biome
    {
        public static Biome[] biomes = new Biome[4096];
        public String name;
        public byte topBlock = (byte)Block.grass.blockID;
        public byte fillerBlock = (byte)Block.dirt.blockID; //TODO maybe stone??

        public static Biome desert = new FlatBiome("Desert");
        public static Biome rainForest = new Biome("RainForest");
        public static Biome swampland = new Biome("Swampland");
        public static Biome seasonalForest = new Biome("Seasonal Forest");
        public static Biome forest = new ForestBiome("Forest");
        public static Biome savanna = new FlatBiome("Savanna");
        public static Biome shrubland = new Biome("Shrubland");
        public static Biome taiga = new TaigaBiome("Taiga");
       
        public static Biome plains = new FlatBiome("Plains");
        public static Biome iceDesert = new FlatBiome("Ice Desert");
        public static Biome tundra = new FlatBiome("Tundra");
      


        public Biome(string name)
        {
            this.name = name;
            recalc();
        }


    public Feature getTreeFeature(BedrockRandom r)
        {
            r.nextInt(); //necessary for vanilla-like gen!
            return new TreeFeature();
        }

        public static Biome __getBiome(float temp, float rain)
        {
            rain *= temp;

            if (temp < 0.1f)
            {
                return Biome.tundra;
            }

            if (rain < 0.2f)
            {
                if (temp < 0.5f)
                {
                    return Biome.tundra;
                }
                if (temp < 0.95f)
                {
                    return Biome.savanna;
                }
                else
                {
                    return Biome.desert;
                }
            }

            if (rain > 0.5f && temp < 0.7f)
            {
                return Biome.swampland;
            }
            if (temp < 0.5f)
            {
                return Biome.taiga;
            }
            if (temp < 0.97f)
            {
                if (rain < 0.35f)
                {
                    return Biome.shrubland;
                }
                else
                {
                    return Biome.forest;
                }
            }
            if (rain < 0.45f)
            {
                return Biome.plains;
            }
            if (rain < 0.9f)
            {
                return Biome.seasonalForest;
            }
            return Biome.rainForest;

        }


        public static void recalc()
        {
            for (int i = 0; i < 64; ++i)
            {
                for (int j = 0; j < 64; ++j)
                {
                    Biome.biomes[i + (j * 64)] = Biome.__getBiome((float)i / 63f, (float)j / 63f);
                }
            }
        //    Biome.desert.topBlock = Biome.desert.fillerBlock = (byte)Block.sand.blockID;
       //     Biome.iceDesert.topBlock = Biome.iceDesert.fillerBlock = (byte)Block.sand.blockID;
        }



    public static Biome getBiome(float blockTemperature, float blockRainfall)
    {
        int i = (int)(blockTemperature * 63f);
        int j = (int)(blockRainfall * 63f);
        return Biome.biomes[i + j * 64];
    }
}
}
