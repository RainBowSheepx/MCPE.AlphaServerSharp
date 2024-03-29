using SpoongePE.Core.Game.utils.random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.feature
{
    public abstract class Feature
    {
        public abstract bool place(World world, BedrockRandom rand, int x, int y, int z);
    }
}
