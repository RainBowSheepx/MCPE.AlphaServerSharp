using MCPE.AlphaServer.Game.feature;
using MCPE.AlphaServer.Game.utils.random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPE.AlphaServer.Game.biome.impl
{
    public class TaigaBiome : Biome
    {
        public TaigaBiome(string name) : base(name)
        {
        }

        /** TODO
* _DWORD *__fastcall TaigaBiome::getTreeFeature(TaigaBiome *this, Random *a2)
{
unsigned int v2; // r0
_DWORD *result; // r0

v2 = Random::genrand_int32(a2);
if ( v2 == 3 * (v2 / 3) )
{
result = operator new(4u);
*result = &off_10E3A8; off_10E3A8      DCD _ZN11PineFeatureD1Ev+1 ; PineFeature::~PineFeature()
}
else
{
result = operator new(4u);
*result = &off_10E3C0; off_10E3C0      DCD _ZN13SpruceFeatureD1Ev+1 ; SpruceFeature::~SpruceFeature()
}
return result;
}
*/

        public Feature getTreeFeature(BedrockRandom r)
        {
            if (r.nextInt(3) == 0)
            {
                return new PineFeature();
            }
            else
            {
                return new SpruceFeature();
            }
        }
    }
}
