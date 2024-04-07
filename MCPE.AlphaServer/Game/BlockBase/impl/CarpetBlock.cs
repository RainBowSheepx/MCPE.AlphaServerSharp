using SpoongePE.Core.Game.material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoongePE.Core.Game.BlockBase.impl
{
    public class CarpetBlock : SolidBlock
    {
        public CarpetBlock(int id, int color) : base(id, Material.cloth)
        {
        }
    }
}
