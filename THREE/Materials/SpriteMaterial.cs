using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Math;
namespace THREE.Materials
{
    public class SpriteMaterial : Material
    {

        //public bool SizeAttenuation = true;

        public SpriteMaterial()
        {
            this.type = "SpriteMaterial";

            this.Color = new Color().SetHex(0x000000);

            this.Transparent = true;

            this.Map = null;

            this.AlphaMap = null;

            this.Rotation = 0;

            this.SizeAttenuation = true;

            this.Transparent = true;

        }
    }
}
