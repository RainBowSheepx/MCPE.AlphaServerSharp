using SpoongePE.Core.Game.feature;
using SpoongePE.Core.Game.utils.random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.biome.impl
{
    public class ForestBiome : Biome
    {
        public ForestBiome(string name) : base(name)
        {
        }

        public Feature getTreeFeature(BedrockRandom r)
        {
            if (r.nextInt(5) == 0)
            {
                return new BirchFeature();
            }
            else
            {
                r.nextInt();
                return new TreeFeature();
            }
        }
    }
}
