using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Core;
using THREE.Materials;

namespace THREE.Objects
{
    public class LineLoop : Line
    {

        public LineLoop(Geometry geometry, Material material) : base(geometry,material)
        {
        }
    }
}
