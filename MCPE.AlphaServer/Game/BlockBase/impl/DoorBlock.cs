using SpoongePE.Core.Game.material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.BlockBase.impl
{
    public class DoorBlock : Block
    {
        public DoorBlock(int id, Material m) : base(id, m)
        {
            isSolid = false;
        }
    }
}
