using SpoongePE.Core.Game.material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.BlockBase.impl
{
    public class ChestBlock : SolidBlock
    {
        public ChestBlock(int id, Material m, int meta = 0) : base(id, m, meta)
        {
        }
    }
}
