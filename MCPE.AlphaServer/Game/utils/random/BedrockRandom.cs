using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;

namespace SpoongePE.Core.Game.utils.random
{
    public class BedrockRandom
    {
        public static uint[] MAG = { 0, 0x9908b0df };
        public uint[] mt = new uint[625];
	    public uint index = 0;

        public BedrockRandom(int seed)
        {
            this.setSeed((uint)seed);
        }


        public BedrockRandom(uint seed)
        {
		    this.setSeed(seed);
        }
        public void setSeed(int seed)
        {
            this.setSeed((uint)seed);
        }
        public void setSeed(uint seed)
        {
		    this.mt[0] = seed;

            for (this.index = 1; this.index < 624; ++this.index){
			    this.mt[this.index] = 0x6c078965 * (this.mt[this.index - 1] >> 30 ^ this.mt[this.index - 1]) + this.index;
            }
        }

        public uint genRandInt()
        {
            uint y, kk;
            if (this.index >= 624 || this.index < 0){
                if (this.index >= 625 || this.index < 0) this.setSeed(4357);

                for (kk = 0; kk < 227; ++kk){
				    y = (this.mt[kk] & 0x80000000) | (this.mt[kk + 1] & 0x7fffffff);
				    this.mt[kk] = this.mt[kk + 397] ^ (y >> 1) ^ MAG[y & 0x1];
                }

                for (;kk < 623; ++kk){
				    y = (this.mt[kk] & 0x80000000) | (this.mt[kk + 1] & 0x7fffffff);
				    this.mt[kk] = this.mt[kk - 227] ^ (y >> 1) ^ MAG[y & 0x1];
                }
			
			    y = (this.mt[623] & 0x80000000) | (this.mt[0] & 0x7fffffff);
			    this.mt[623] = this.mt[396] ^ (y >> 1) ^ MAG[y & 0x1];
			    this.index = 0;
            }
		
		    y = this.mt[this.index++];
		    y ^= (y >> 11);
		    y ^= (y << 7) & 0x9d2c5680;
		    y ^= (y << 15) & 0xefc60000;
		    y ^= (y >> 18);
            return y;
        }

        public int nextInt()
        {
            return (int)(this.genRandInt() >> 1);
            
        }

        public int nextInt(int bound)
        {
            return (int)(this.genRandInt() % bound);
        }

        public float nextFloat()
        {
            return (float)this.genRandInt() / 0xffffffff;
        }
    }
}
