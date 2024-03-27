using MCPE.AlphaServer.Game.material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPE.AlphaServer.Game.BlockBase
{
    public class DoorBlock : Block
    {
        public DoorBlock(int id, Material m) : base(id, m)
        {
            this.isSolid = false;
        }
    }
}
