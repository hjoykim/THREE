using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THREE.Math;
namespace THREE.Materials
{
    public class ShadowMaterial : Material
    {
        public ShadowMaterial()
        {
            this.type = "ShadowMaterial";

            this.Transparent = true;

            this.Color = new Color().SetHex(0x000000);
        }
    }
}
