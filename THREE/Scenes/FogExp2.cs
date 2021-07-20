using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THREE.Scenes
{
    public class FogExp2 : Fog
    {

        public float Density;

        public FogExp2(Color color, float? density = null)
        {
            this.Name = "";
            this.Density = density != null ? density.Value :(float)0.00025;
        }
    }
}
